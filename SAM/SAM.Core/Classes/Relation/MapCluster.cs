﻿using Newtonsoft.Json.Linq;
using SAM.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Core
{
    public class MapCluster : SAMObject, IJSAMObject
    {
        private Dictionary<string, Type> dictionary;

        private List<Tuple<string, string, string, string>> tuples;

        public MapCluster(MapCluster mapCluster)
            :base(mapCluster)
        {
            tuples = mapCluster.tuples.ConvertAll(x => new Tuple<string, string, string, string>(x.Item1, x.Item2, x.Item3, x.Item4));
        }
        
        public MapCluster()
        {
            tuples = new List<Tuple<string, string, string, string>>();
        }

        public MapCluster(JObject jObject)
        {
            FromJObject(jObject);
        }

        public bool Add(Type type_1, Type type_2, string name_1, string name_2)
        {
            if (type_1 == null || type_2 == null || string.IsNullOrEmpty(name_1) || string.IsNullOrEmpty(name_2))
                return false;

            bool result = Add(GetId(type_1), GetId(type_2), name_1, name_2);
            if(result)
            {
                AddType(type_1);
                AddType(type_2);
            }
            return result;
        }

        public List<Type> Add(Enum @enum, Type type, string name)
        {
            if (type == null || string.IsNullOrEmpty(name))
                return null;

            AssociatedTypes associatedTypes = AssociatedTypes.Get(@enum.GetType());
            if (associatedTypes == null)
                return null;

            Type[] types = associatedTypes.Types;
            if (types == null || types.Length == 0)
                return null;

            string name_Temp = @enum.ToString();
            ParameterProperties parameterProperties = ParameterProperties.Get(@enum);
            if(parameterProperties != null)
                name_Temp = parameterProperties.Name;

            if (name_Temp == null)
                return null;

            List<Type> result = new List<Type>();
            foreach (Type type_Temp in types)
                if (Add(type_Temp, type, name_Temp, name))
                    result.Add(type_Temp);

            return result;
        }
        
        public bool Add(string id_1, string id_2, string name_1, string name_2)
        {
            if (string.IsNullOrEmpty(id_1) || string.IsNullOrEmpty(id_2) || string.IsNullOrEmpty(name_1) || string.IsNullOrEmpty(name_2))
                return false;

            List<Tuple<string, string, string, string>> tuples_Temp = tuples.FindAll(x => x.Item1.Equals(id_1));
            if (tuples_Temp.Count == 0)
            {
                tuples.Add(new Tuple<string, string, string, string>(id_1, id_2, name_1, name_2));
                return true;
            }

            tuples_Temp = tuples_Temp.FindAll(x => x.Item2.Equals(id_2));
            if (tuples_Temp.Count == 0)
            {
                tuples.Add(new Tuple<string, string, string, string>(id_1, id_2, name_1, name_2));
                return true;
            }

            tuples_Temp = tuples_Temp.FindAll(x => x.Item3.Equals(name_1));
            if (tuples_Temp.Count == 0)
            {
                tuples.Add(new Tuple<string, string, string, string>(id_1, id_2, name_1, name_2));
                return true;
            }

            tuples_Temp = tuples_Temp.FindAll(x => x.Item4.Equals(name_2));
            if (tuples_Temp.Count == 0)
            {
                tuples.Add(new Tuple<string, string, string, string>(id_1, id_2, name_1, name_2));
                return true;
            }

            return false;
        }

        public string GetName(string id_1, string id_2, string name_1)
        {
            if (string.IsNullOrEmpty(id_1) || string.IsNullOrEmpty(id_2) || string.IsNullOrEmpty(name_1))
                return null;

            return GetNames(id_1, id_2, name_1)?.FirstOrDefault();

            //return tuples.Find(x => x.Item1.Equals(id_1) && x.Item2.Equals(id_2) && x.Item3.Equals(name_1))?.Item4;
        }

        public string GetName(Type type_1, Type type_2, string name_1)
        {
            if (type_1 == null || type_2 == null || string.IsNullOrEmpty(name_1))
                return null;

            return GetName(GetId(type_1), GetId(type_2), name_1);
        }

        public List<string> GetNames(Type type_1, Type type_2)
        {
            if (type_1 == null || type_2 == null)
                return null;

            return GetNames(GetId(type_1), GetId(type_2));
        }

        public List<string> GetNames(string id_1, string id_2)
        {
            return GetNames(id_1, id_2, null);

            //return tuples.FindAll(x => x.Item1.Equals(id_1) && x.Item2.Equals(id_2))?.ConvertAll(x => x.Item3);
        }

        public override JObject ToJObject()
        {
            JObject jObject = base.ToJObject();
            if (jObject == null)
                return null;

            JArray jArray_Map = new JArray();
            foreach (Tuple<string, string, string, string> tuple in tuples)
            {
                JArray jArray = new JArray();
                jArray.Add(tuple.Item1);
                jArray.Add(tuple.Item2);
                jArray.Add(tuple.Item3);
                jArray.Add(tuple.Item4);
                jArray_Map.Add(jArray);
            }

            jObject.Add("Map", jArray_Map);

            return jObject;
        }

        public override bool FromJObject(JObject jObject)
        {
            if (!base.FromJObject(jObject))
                return false;

            tuples = new List<Tuple<string, string, string, string>>();

            JArray jArray_Map = jObject.Value<JArray>("Map");
            if(jArray_Map != null)
            {
                
                foreach(JArray jArray in jArray_Map)
                {
                    if (jArray.Count < 4)
                        continue;

                    Tuple<string, string, string, string> tuple = new Tuple<string, string, string, string>(jArray[0].Value<string>(), jArray[1].Value<string>(), jArray[2].Value<string>(), jArray[3].Value<string>());

                    if (tuple != null)
                        tuples.Add(tuple);
                }

 
            }

            return true;
        }

        private List<string> GetNames(string id_1, string id_2, string name_1)
        {
            if (string.IsNullOrWhiteSpace(id_1) || string.IsNullOrWhiteSpace(id_2))
                return null;

            Type type_1 = GetType(id_1, false);
            Type type_2 = GetType(id_2, false);

            List<string> result = new List<string>();
            foreach (Tuple<string, string, string, string> tuple in tuples)
            {
                bool valid;

                valid = false;
                if (type_1 != null)
                {
                    Type type_1_Tuple = GetType(tuple.Item1);
                    if (type_1_Tuple != null)
                        valid = type_1.Equals(type_1_Tuple) || type_1_Tuple.IsAssignableFrom(type_1);
                }

                if (!valid)
                    valid = tuple.Item1.Equals(id_1);

                if (!valid)
                    continue;

                valid = false;
                if (type_2 != null)
                {
                    Type type_2_Tuple = GetType(tuple.Item2);
                    if (type_2_Tuple != null)
                        valid = type_2.Equals(type_2_Tuple) || type_2_Tuple.IsAssignableFrom(type_2);
                }

                if (!valid)
                    valid = tuple.Item2.Equals(id_2);

                if (!valid)
                    continue;

                if(name_1 != null)
                {
                    if(tuple.Item3.Equals(name_1))
                    {
                        result.Add(tuple.Item4);
                        return result;
                    }

                    continue;
                }
                else
                {
                    result.Add(tuple.Item3);
                }
            }

            return result;
        }

        private bool AddType(Type type)
        {
            string id = GetId(type);
            if (string.IsNullOrEmpty(id) || !id.StartsWith("::"))
                return false;

            if (dictionary == null)
                dictionary = new Dictionary<string, Type>();

            dictionary[id] = type;
            return true;
        }

        private Type GetType(string id, bool includeInDictionary = true)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (!id.StartsWith("::"))
                return null;

            Type result = null;
            if (dictionary != null)
                if (!dictionary.TryGetValue(id, out result))
                    result = null;

            if (result != null)
                return result;

            result = Type.GetType(id.Substring(2), false);

            if(includeInDictionary)
            {
                if (dictionary == null)
                    dictionary = new Dictionary<string, Type>();

                dictionary[id] = result;
            }

            return result;
        }

        private static string GetId(Type type)
        {
            string fullName = Query.FullTypeName(type);
            if (string.IsNullOrWhiteSpace(fullName))
                fullName = type.FullName;

            return string.Format("::{0}", fullName);
        }
    }
}
