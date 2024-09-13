using Waketree.Common;
using Waketree.Common.Models;
using Waketree.Service.Models;
using Waketree.Service.Singletons;

namespace Waketree.Service
{
    public static class RESTCalls
    {
        public static WaketreeProcess? GetProcessFromSupervisor(string processID)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<WaketreeProcess>(
                string.Format(WaketreeConstants.URLSupervisorGetProcess,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort,
                    processID)).Result;
            return (response.Item1) ? response.Item3 : null;
        }

        public static object? GetMemory(string ipAddress, string processID, string memoryName)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<ComplexSerializableObject?>(
                string.Format(WaketreeConstants.URLServiceGetMemory,
                    ipAddress,
                    Config.GetConfig().ServiceRESTPort,
                    processID,
                    memoryName)).Result;
            return (response.Item1) ? response.Item3?.DeserializeObject() : null;
        }

        public static string GetMemoryLocation(string processID, string memoryName)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<WaketreeMemory>(
                string.Format(WaketreeConstants.URLSupervisorGetMemory,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort,
                    processID,
                    memoryName)).Result;
            return ((response.Item1) && (response.Item3 != null)) ? 
                response.Item3.MemoryLocation : 
                string.Empty;
        }

        public static bool RegisterMemory(string processID, string memoryName)
        {
            var response = RESTUtilities.PostCallRestServiceAsync<MemoryRegisterResponse, WaketreeMemory>(
                string.Format(WaketreeConstants.URLSupervisorRegisterMemory,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort),
                new WaketreeMemory()
                {
                    ProcessID = processID,
                    MemoryName = memoryName,
                    MemoryLocation = Utils.GetPrimaryIP4()
                }).Result;
            return ((response.Item1) && (response.Item3 != null)) ?
                response.Item3.Success :
                false;
        }

        public static Tuple<bool, bool> AddOrUpdateMemory(string ipAddress, string processID, string memoryName, object val)
        {
            var valSerialize = new ComplexSerializableObject(val);
            var response = RESTUtilities.PostCallRestServiceAsync<MemoryUpdateResponse, WaketreeMemoryValue>(
                string.Format(WaketreeConstants.URLServiceAddOrUpdate,
                    ipAddress,
                    Config.GetConfig().ServiceRESTPort),
                new WaketreeMemoryValue()
                {
                    MemoryLocation = ipAddress,
                    ProcessID = processID,
                    MemoryName = memoryName,
                    MemoryValue = valSerialize
                }).Result;
            var retVal = (response == null) ?
                new Tuple<bool, bool>(false, false) :
                (response.Item3 == null) ?
                    new Tuple<bool, bool>(response.Item1, false) :
                    new Tuple<bool, bool>(response.Item1, response.Item3.Success);
            return retVal;
        }

        public static bool MemoryExists(string processID)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<bool>(
                string.Format(WaketreeConstants.URLSupervisorMemoryExistsProcess,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort,
                    processID)).Result;
            return (response.Item1) ?
                response.Item3 :
                false;
        }

        public static bool MemoryExists(string processID, string memoryName)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<bool>(
                string.Format(WaketreeConstants.URLSupervisorMemoryExists,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort,
                    processID,
                    memoryName)).Result;
            return (response.Item1) ?
                response.Item3 :
                false;
        }

        public static bool DeleteMemory(string ipAddress, string processID, string memoryName)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<MemoryDeleteResponse>(
                string.Format(WaketreeConstants.URLServiceMemoryDelete,
                    ipAddress,
                    Config.GetConfig().ServiceRESTPort,
                    processID,
                    memoryName)).Result;
            return ((response.Item1) && (response.Item3 != null)) ?
                response.Item3.Success :
                false;
        }

        public static bool DeregisterMemory(string processID, string memoryName)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<MemoryDeregisterResponse>(
                string.Format(WaketreeConstants.URLSupervisorMemoryDeregister,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort,
                    processID,
                    memoryName)).Result;
            return ((response.Item1) && (response.Item3 != null)) ?
                response.Item3.Success :
                false;
        }

        public static Tuple<bool, bool> TestAndSet(string ipAddress, string processID, string memoryName, object newVal, object expectedVal)
        {
            var newValSerialize = new ComplexSerializableObject(newVal);
            var expectedValSerialize = new ComplexSerializableObject(expectedVal);
            var response = RESTUtilities.PostCallRestServiceAsync<TestAndSetResponse, WaketreeMemoryTestAndSet>(
                string.Format(WaketreeConstants.URLServiceTestAndSet,
                    ipAddress,
                    Config.GetConfig().ServiceRESTPort),
                new WaketreeMemoryTestAndSet()
                {
                    MemoryLocation = ipAddress,
                    ProcessID = processID,
                    MemoryName = memoryName,
                    MemoryValue = newValSerialize,
                    MemoryExpectedValue = expectedValSerialize
                }).Result;
            var retVal = (response == null) ?
                new Tuple<bool, bool>(false, false) :
                (response.Item3 == null) ?
                    new Tuple<bool, bool>(response.Item1, false) :
                    new Tuple<bool, bool>(response.Item1, response.Item3.Success);
            return retVal;
        }

        public static bool CreateThread (ThreadCreateArgument arg)
        {
            var response = RESTUtilities.PostCallRestServiceAsync<ThreadCreateResponse, ThreadCreateArgument>(
                string.Format(WaketreeConstants.URLSupervisorCreateThread,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort),
                    arg).Result;
            return ((response.Item1) && (response.Item3 != null)) ?
                response.Item3.Success :
                false;
        }

        public static bool ThreadEnded(string threadID)
        {
            var response = RESTUtilities.GetCallRestServiceAsync<ThreadEndedResponse>(
                string.Format(WaketreeConstants.URLSupervisorThreadEnded,
                    State.GetState().SupervisorIP,
                    Config.GetConfig().SupervisorRESTPort,
                    threadID)).Result;
            return ((response.Item1) && (response.Item3 != null)) ?
                response.Item3.Success :
                false;
        }
    }
}
