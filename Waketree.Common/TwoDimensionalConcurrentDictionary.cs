using System.Collections.Concurrent;

namespace Waketree.Common
{
    public class TwoDimensionalConcurrentDictionary<K1, K2, V>
    {
        private ConcurrentDictionary<K1, ConcurrentDictionary<K2, V>> dict;
        private ConcurrentDictionary<K1, ConcurrentDictionary<K2, object>> testAndSetLocks;

        public TwoDimensionalConcurrentDictionary()
        {
            this.dict = new ConcurrentDictionary<K1, ConcurrentDictionary<K2, V>>();
            this.testAndSetLocks = new ConcurrentDictionary<K1, ConcurrentDictionary<K2, object>>();
        }

        public bool ContainsKey(K1 k1, K2 k2)
        {
            if (!this.dict.TryGetValue(k1, out var secondDict))
            {
                return false;
            }
            return secondDict.ContainsKey(k2);
        }

        public bool ContainsKey(K1 k1)
        {
            return this.dict.ContainsKey(k1);
        }

        public void AddOrUpdate(K1 k1, K2 k2, V v)
        {
            if (!this.dict.TryGetValue(k1, out var secondDict))
            {
                secondDict = new ConcurrentDictionary<K2, V>();
                this.dict.TryAdd(k1, secondDict);
            }
            secondDict[k2] = v;
        }

        public void AddOrUpdate(Tuple<K1, K2> k, V v)
        {
            this.AddOrUpdate(k.Item1, k.Item2, v);
        }

        /// <summary>
        /// Tests the (k1, k2) value for equality w/ expectedVal, and if equal - then sets the value to newVal
        ///     NOTE: if the key doesnt exist yet, will always create and return true
        /// </summary>
        /// <param name="k1">key 1</param>
        /// <param name="k2">key 2</param>
        /// <param name="newVal">the new val to set it equal to</param>
        /// <param name="expectedVal">the value to test for equality</param>
        /// <returns>true if the value equaled the expectedVal and the value was changed to the newVal</returns>
        public bool TestAndSet(K1 k1, K2 k2, V newVal, V expectedVal)
        {
            object? testAndSetLock;
            bool newLockDict = false;
            bool newLock = false;
            if (!this.testAndSetLocks.TryGetValue(k1, out var testAndSetLocksDict2))
            {
                testAndSetLock = new object();
                newLockDict = true;
                newLock = true;
            }
            else
            {
                if (!testAndSetLocksDict2.TryGetValue(k2, out testAndSetLock))
                {
                    testAndSetLock = new object();
                    newLock = true;
                }
            }

            lock (testAndSetLock)
            {

                // create new lock dictionary / lock entries
                if (newLockDict)
                {
                    this.testAndSetLocks.TryAdd(k1, new ConcurrentDictionary<K2, object>());
                }
                if (newLock)
                {
                    this.testAndSetLocks.TryGetValue(k1, out testAndSetLocksDict2);
                    testAndSetLocksDict2[k2] = testAndSetLock;
                }

                // actual test and set
                if (!this.dict.TryGetValue(k1, out var secondDict))
                {
                    secondDict = new ConcurrentDictionary<K2, V>();
                    this.dict.TryAdd(k1, secondDict);
                }
                if (secondDict.TryGetValue(k2, out var compareVal))
                {
                    if (compareVal.Equals(expectedVal))
                    {
                        secondDict[k2] = newVal;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    secondDict[k2] = newVal;
                    return true;
                }
            }    
        }

        public bool TestAndSet(Tuple<K1, K2> k, V newVal, V expectedVal)
        {
            return this.TestAndSet(k.Item1, k.Item2, newVal, expectedVal);
        }

        public V? Get(K1 k1, K2 k2)
        {
            if (!this.dict.TryGetValue(k1, out var secondDict))
            {
                return default(V);
            }
            if (!secondDict.TryGetValue(k2, out var v))
            {
                return default(V);
            }
            return v;
        }

        public V? Get(Tuple<K1, K2> k)
        {
            return this.Get(k.Item1, k.Item2);
        }

        public ConcurrentDictionary<K2, V>? Get(K1 k1)
        {
            if (!this.dict.TryGetValue(k1, out var secondDict))
            {
                return null;
            }
            return secondDict;
        }

        public void Delete(K1 k1, K2 k2)
        {

            // value deletion
            if (this.dict.TryGetValue(k1, out var secondDict))
            {
                secondDict.TryRemove(k2, out _);
                if (secondDict.Count == 0)
                {
                    this.dict.TryRemove(k1, out _);
                }
            }

            // test and set lock deletion
            if (this.testAndSetLocks.TryGetValue(k1, out var secondLockDict))
            {
                secondLockDict.TryRemove(k2, out _);
                if (secondLockDict.Count == 0)
                {
                    this.dict.TryRemove(k1, out _);
                }
            }
        }

        public void Delete(Tuple<K1, K2> k)
        {
            this.Delete(k.Item1, k.Item2);
        }

        public void Delete(K1 k1)
        {
            this.dict.TryRemove(k1, out _);

            this.testAndSetLocks.TryRemove(k1, out _);
        }

        public IEnumerable<K2>? this[K1 k1]
        {
            get
            {
                if (!this.dict.TryGetValue(k1, out var secondDict))
                {
                    return null;
                }
                return secondDict.Keys;
            }
        }

        public V? this[K1 k1, K2 k2]
        {
            get
            {
                return this.Get(k1, k2);
            }
            set
            {
                this.AddOrUpdate(k1, k2, value);
            }
        }

        public IEnumerable<Tuple<K1,K2>> AllKeyCombos
        {
            get
            {
                var retVal = new List<Tuple<K1,K2>>();
                foreach(var key in this.dict.Keys)
                {
                    foreach(var key2 in this.dict[key].Keys)
                    {
                        retVal.Add(new Tuple<K1, K2>(key, key2));
                    }
                }
                return retVal;
            }
        }

        public IEnumerable<V> AllValues
        {
            get
            {
                var valList = new List<V>();
                foreach (var key in this.AllKeyCombos)
                {
                    var val = this.Get(key);
                    if (val != null)
                    {
                        valList.Add(val);   
                    }
                }
                return valList;
            }
        }
    }
}
