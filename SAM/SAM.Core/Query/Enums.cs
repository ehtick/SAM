﻿using SAM.Core.Attributes;
using System;
using System.Collections.Generic;

namespace SAM.Core
{
    public static partial class Query
    {
        public static List<Enum> Enums(Type type, string value, bool notPublic = false)
        {
            if (type == null || string.IsNullOrEmpty(value))
                return null;

            Dictionary<Type, AssociatedTypes> dictionary = ParameterTypesDictionary(null, true, notPublic);
            if (dictionary == null)
                return null;

            List<Enum> result = new List<Enum>();
            foreach (KeyValuePair<Type, AssociatedTypes> keyValuePair in dictionary)
            {
                if (!keyValuePair.Value.IsValid(type))
                    continue;

                foreach (Enum @enum in Enum.GetValues(keyValuePair.Key))
                {
                    if (@enum.ToString().Equals(value))
                    {
                        result.Add(@enum);
                        continue;
                    }

                    ParameterProperties parameterProperties = ParameterProperties.Get(@enum);
                    if (parameterProperties == null)
                        continue;

                    string name = parameterProperties.Name;
                    if (string.IsNullOrEmpty(name))
                        continue;

                    if (name.Equals(value))
                        result.Add(@enum);
                }
            }

            return result;
        }
    }
}