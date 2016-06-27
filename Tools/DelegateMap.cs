/*
 * Author: Brent Kuzmanich
 * Project: DelegateMap
 * Comments:
 * A simple delegate-based approach to object mapping.
 */
using System;
using System.Collections.Generic;

namespace DelegateMap
{
    public class DelegateMap
    {
        //singleton instance
        private static DelegateMap instance = null;        
        private Dictionary<string, Delegate> mappings;       

        private DelegateMap()
        {
            //initialize dict of mappings
            mappings = new Dictionary<string, Delegate>();
        }

        public static DelegateMap Instance
        {
            get
            {
                if (instance == null)
                    instance = new DelegateMap();
                return instance;
            }
        }

        //AddMapping: create a new dictionary entry in mappings
        public void AddMapping(string key, Delegate func)
        {
            //add new mapping to dict
            mappings.Add(key, func);
        }

        //Map: return  new instance of specified out-object based on in-object
        public TargetType Map<SourceType, TargetType>(string mapKey, SourceType src)
        {
            return (TargetType)mappings[mapKey].DynamicInvoke(src);
        }
        
        //Override of Map: Maps between 2 initialized objects w/ optional return int
        public void Map<SourceType, TargetType, error>(string mapKey, SourceType src, TargetType targ)
        {
            mappings[mapKey].DynamicInvoke(src, targ);            
        }

    }
}
