﻿using SAM.Core;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Analytical
{
    public static partial class Modify
    {
        public static InternalCondition MapInternalCondition(this Space space, InternalConditionLibrary internalConditionLibrary, TextMap textMap)
        {
            if (space == null || internalConditionLibrary == null || textMap == null)
                return null;

            string name = space.Name;
            if (string.IsNullOrWhiteSpace(name))
                return null;

            HashSet<string> names_InternalCondition = textMap.GetSortedKeys(name);
            if (names_InternalCondition == null || names_InternalCondition.Count == 0)
                return null;

            List<InternalCondition> internalConditions = internalConditionLibrary.GetInternalConditions(names_InternalCondition.First());
            if (internalConditions == null || internalConditions.Count == 0)
                return null;

            InternalCondition internalCondition = internalConditions[0];
            if (internalCondition == null)
                return null;

            space.InternalCondition = internalCondition;
            return internalCondition;
        }
    }
}