﻿using SAM.Core;
using System.Collections.Generic;

namespace SAM.Analytical
{
    public static partial class Query
    {
        public static Dictionary<IPartition, IMaterial> InternalMaterialDictionary(this ArchitecturalModel architecturalModel, Space space, double silverSpacing = Tolerance.MacroDistance, double tolerance = Tolerance.Distance)
        {
            Dictionary<IPartition, Architectural.MaterialLayer> dictionary = InternalMaterialLayerDictionary(architecturalModel, space, silverSpacing, tolerance);
            if (dictionary == null || dictionary.Count == 0)
                return null;

            Dictionary<IPartition, IMaterial> result = new Dictionary<IPartition, IMaterial>();
            foreach (KeyValuePair<IPartition, Architectural.MaterialLayer> keyValuePair in dictionary)
            {
                result[keyValuePair.Key] = architecturalModel.GetMaterial(keyValuePair.Value);
            }

            return result;
        }
    }
}