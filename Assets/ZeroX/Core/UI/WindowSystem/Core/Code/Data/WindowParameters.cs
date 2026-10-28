using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroX.WindowSystem
{
    public class WindowParameters : IEnumerable<object>
    {
        private readonly Dictionary<string, object> dictParameter = new Dictionary<string, object>();
        
        
        
        //Const
        public const string EMPTY_PARAM_STRING = "";
        
        
        //Property
        public int Count => dictParameter.Count;
        public object this[string key] => dictParameter[key];
        
        
        
        public object Get(string key)
        {
            if (dictParameter.TryGetValue(key, out object value))
                return value;

            return null;
        }
        
        public string GetString(string key)
        {
            if (dictParameter.TryGetValue(key, out object value) == false)
                return EMPTY_PARAM_STRING;
            
            if(value == null)
                return EMPTY_PARAM_STRING;

            if (value is string valueS)
                return valueS;
                
            Debug.LogErrorFormat("Invalid cast '{0}'", key);
            return EMPTY_PARAM_STRING;
        }
        
        public bool Add(string key, object value)
        {
            if (dictParameter.TryAdd(key, value))
            {
                return true;
            }
            
            Debug.LogErrorFormat("Key '{0}' already exists", key);
            return false;
        }

        public void Set(string key, object value)
        {
            dictParameter[key] = value;
        }

        public bool Remove(string key)
        {
            return dictParameter.Remove(key);
        }

        public void Clear()
        {
            dictParameter.Clear();
        }

        public IEnumerator<object> GetEnumerator()
        {
            foreach (var value in dictParameter.Values)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var value in dictParameter.Values)
            {
                yield return value;
            }
        }
    }
}