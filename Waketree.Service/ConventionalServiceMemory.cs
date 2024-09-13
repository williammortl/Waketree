using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Net.Http.Headers;
using Waketree.Common;
using Waketree.Common.Interfaces;
using Waketree.Service.Models;

namespace Waketree.Service
{
    public class ConventionalServiceMemory : IServiceMemory
    {
        private enum MemoryLocation
        {
            DoesntExist,
            Local,
            Cluster
        }

        private TwoDimensionalConcurrentDictionary<string, string, object> localMemory;
        private TwoDimensionalConcurrentDictionary<string, string, string> locationCache;


        public ConventionalServiceMemory()
        {
            this.localMemory = new TwoDimensionalConcurrentDictionary<string, string, object>();
            this.locationCache = new TwoDimensionalConcurrentDictionary<string, string, string>();
        }

        public IEnumerable<string> this[string processID] 
        { 
            get
            {
                if (this.localMemory.ContainsKey(processID))
                {
                    return this.localMemory.Get(processID).Keys.AsEnumerable<string>();
                }
                else
                {
                    return new List<string>();
                }
            }
        }


        public IEnumerable<Tuple<string, string>> LocalMemoryInfo
        {
            get
            {
                return this.localMemory.AllKeyCombos;
            }
        }

        public IEnumerable<string> LocalMemoriesByProcess(string processID)
        {
            var memoryNames = this.localMemory.Get(processID);
            return (memoryNames != null) ?
                memoryNames.Keys :
                Enumerable.Empty<string>();
        }

        public bool ContainsMemory(string processID)
        {

            // is local?
            if (this.localMemory.ContainsKey(processID))
            {
                return true;
            }

            // is cached?
            if (this.locationCache.ContainsKey(processID))
            {
                return true;
            }

            // does supervisor know?
            if (RESTCalls.MemoryExists(processID))
            {
                return true;
            }

            return false;
        }

        public bool ContainsMemory(string processID, string memoryName)
        {

            // is local?
            if (this.localMemory.ContainsKey(processID, memoryName))
            {
                return true;
            }

            // is cached?
            if (this.locationCache.ContainsKey(processID, memoryName))
            {
                return true;
            }

            // does supervisor know?
            if (RESTCalls.MemoryExists(processID, memoryName))
            {
                return true;
            }

            return false;
        }

        public void AddOrUpdateMemory(Tuple<string, string> memory, object? mem)
        {
            this.AddOrUpdateMemory(memory.Item1, memory.Item2, mem, true);
        }

        public void AddOrUpdateMemory(string processID, string memoryName, object? mem)
        {
            this.AddOrUpdateMemory(processID, memoryName, mem, true);
        }

        private void AddOrUpdateMemory(string processID, string memoryName, object? mem, bool recurse)
        {
            var locationData = this.GetVariableLocation(processID, memoryName);
            switch (locationData.Item1)
            {
                case MemoryLocation.Local:
                    {
                        if (mem != null)
                        {
                            this.localMemory.AddOrUpdate(processID, memoryName, mem);
                        }
                        else
                        {
                            this.localMemory.Delete(processID, memoryName);
                            RESTCalls.DeregisterMemory(processID, memoryName);
                        }
                        break;
                    }
                case MemoryLocation.Cluster:
                    {
                        if ((locationData.Item2 == null) || (locationData.Item2 == string.Empty))
                        {
                            throw new InvalidDataException("Cannot add or update memory without a valid ip address!");
                        }
                        if (mem != null)
                        {
                            var retVal = RESTCalls.AddOrUpdateMemory(locationData.Item2,
                                processID,
                                memoryName,
                                mem);
                            if (recurse && (!retVal.Item1))
                            {
                                // call looks like it failed, clear cache if needed and try again
                                this.locationCache.Delete(processID, memoryName);
                                this.AddOrUpdateMemory(processID, memoryName, mem, false);
                            }
                        }
                        else
                        {
                            this.DeleteMemory(processID, memoryName);
                        }
                        break;
                    }
                case MemoryLocation.DoesntExist:
                    {
                        if (mem != null)
                        {
                            RESTCalls.RegisterMemory(processID, memoryName);
                            this.localMemory.AddOrUpdate(processID, memoryName, mem);
                        }
                        break;
                    }
            }
        }

        public void AddOrUpdateMemory(Tuple<string, string> memory, WaketreeMemoryValue mem)
        {
            this.AddOrUpdateMemory(memory.Item1, memory.Item2, mem);
        }

        public object? GetMemory(Tuple<string, string> memory)
        {
            return this.GetMemory(memory.Item1, memory.Item2, true);
        }

        public object? GetMemory(string processID, string memoryName)
        {
            return this.GetMemory(processID, memoryName, true);
        }

        private object? GetMemory(string processID, string memoryName, bool recurse)
        {
            var locationData = this.GetVariableLocation(processID, memoryName);
            switch (locationData.Item1)
            {
                case MemoryLocation.Local:
                    {
                        return this.localMemory.Get(processID, memoryName);
                    }
                case MemoryLocation.Cluster:
                    {
                        if ((locationData.Item2 == null) || (locationData.Item2 == string.Empty))
                        {
                            throw new InvalidDataException("Cannot get memory without a valid ip address!");
                        }
                        var retVal = RESTCalls.GetMemory(locationData.Item2, processID, memoryName);
                        if (recurse && (retVal == null)) 
                        {
                            // call looks like it failed, clear cache if needed and try again
                            this.locationCache.Delete(processID, memoryName);
                            retVal = this.GetMemory(processID, memoryName, false);
                        }
                        return retVal;
                    }
            }
            return null;
        }

        public void DeleteMemory(Tuple<string, string> memory)
        {
            this.DeleteMemory(memory.Item1, memory.Item2, true);
        }

        public void DeleteMemory(string processID, string memoryName)
        {
            this.DeleteMemory(processID, memoryName, true);
        }

        private void DeleteMemory(string processID, string memoryName, bool recurse)
        {
            var locationData = this.GetVariableLocation(processID, memoryName);
            switch (locationData.Item1)
            {
                case MemoryLocation.Local:
                    {
                        this.localMemory.Delete(processID, memoryName);
                        RESTCalls.DeregisterMemory(processID, memoryName);
                        break;
                    }
                case MemoryLocation.Cluster:
                    {
                        if ((locationData.Item2 == null) || (locationData.Item2 == string.Empty))
                        {
                            throw new InvalidDataException("Cannot delete memory without a valid ip address!");
                        }
                        var retVal = RESTCalls.DeleteMemory(locationData.Item2, processID, memoryName);
                        if (recurse && (!retVal))
                        {
                            // call looks like it failed, clear cache if needed and try again
                            this.locationCache.Delete(processID, memoryName);
                            this.DeleteMemory(processID, memoryName, false);
                        }
                        break;
                    }
            }
        }

        public void DeleteLocalMemoryByProcessID(string processID)
        {
            var memoryNameDict = this.localMemory.Get(processID);
            if (memoryNameDict != null)
            {
                var memoryNames = memoryNameDict.Keys.ToArray();
                foreach (var memoryName in memoryNames)
                {
                    this.localMemory.Delete(processID, memoryName);
                }

                this.localMemory.Delete(processID);
            }
        }

        private Tuple<MemoryLocation, string?> GetVariableLocation(string processID, string name)
        {

            // is this variable located in this node's memory?
            if (this.localMemory.ContainsKey(processID, name.ToString()))
            {
                return new Tuple<MemoryLocation, string?>(MemoryLocation.Local, null);
            }

            // is this variable in the cache already?
            var ipAddress = this.locationCache.Get(processID, name);
            if (ipAddress != null)
            {
                return new Tuple<MemoryLocation, string?>(MemoryLocation.Cluster, ipAddress);
            }

            // does the supervisor know about the memory? if yes, cache
            ipAddress = RESTCalls.GetMemoryLocation(processID, name);
            if (ipAddress != string.Empty)
            {
                this.locationCache.AddOrUpdate(processID, name, ipAddress);
                return new Tuple<MemoryLocation, string?>(MemoryLocation.Cluster, ipAddress);
            }

            // finally, doesnt exist anywhere
            return new Tuple<MemoryLocation, string?>(MemoryLocation.DoesntExist, null);
        }

        public bool TestAndSet(Tuple<string, string> memory, object? newVal, object? expectedVal)
        {
            return this.TestAndSet(memory.Item1, memory.Item2, newVal, expectedVal, true);
        }

        public bool TestAndSet(string processID, string memoryName, object? newVal, object? expectedVal)
        {
            return this.TestAndSet(processID, memoryName, newVal, expectedVal, true);
        }

        private bool TestAndSet(string processID, string memoryName, object? newVal, object? expectedVal, bool recurse)
        {
            if ((newVal == null) || (expectedVal == null))
            {
                return false;
            }

            var locationData = this.GetVariableLocation(processID, memoryName);
            switch (locationData.Item1)
            {
                case MemoryLocation.Local:
                    {
                        return this.localMemory.TestAndSet(processID, memoryName, newVal, expectedVal);
                    }
                case MemoryLocation.Cluster:
                    {
                        if ((locationData.Item2 == null) || (locationData.Item2 == string.Empty))
                        {
                            throw new InvalidDataException("Cannot test and set memory without a valid ip address!");
                        }
                        var restRet = RESTCalls.TestAndSet(
                            locationData.Item2,
                            processID,
                            memoryName,
                            newVal,
                            expectedVal);
                        var retVal = restRet.Item2;
                        if (recurse && (!restRet.Item1))
                        {
                            // call looks like it failed, clear cache if needed and try again
                            this.locationCache.Delete(processID, memoryName);
                            retVal = this.TestAndSet(processID, memoryName, newVal, expectedVal, false);
                        }
                        return retVal;
                    }
                case MemoryLocation.DoesntExist:
                    {
                        RESTCalls.RegisterMemory(processID, memoryName);
                        var retVal = this.localMemory.TestAndSet(processID, memoryName, newVal, expectedVal);
                        return retVal;
                    }
            }
            return false;
        }
    }
}
