﻿using SAM.Core;
using System;

namespace SAM.Analytical
{
    public static partial class Query
    {
        [Obsolete]
        public static double ExternalEmissivity(this OpaqueMaterial opaqueMaterial)
        {
            if (opaqueMaterial == null)
                return double.NaN;

            return opaqueMaterial.GetValue<double>(OpaqueMaterialParameter.ExternalEmissivity);

            double result = double.NaN;
            if (!Core.Query.TryGetValue(opaqueMaterial, ParameterName_ExternalEmissivity(), out result))
                return double.NaN;

            return result;
        }

        [Obsolete]
        public static double ExternalEmissivity(this TransparentMaterial transparentMaterial)
        {
            if (transparentMaterial == null)
                return double.NaN;

            return transparentMaterial.GetValue<double>(TransparentMaterialParameter.ExternalEmissivity);

            double result = double.NaN;
            if (!Core.Query.TryGetValue(transparentMaterial, ParameterName_ExternalEmissivity(), out result))
                return double.NaN;

            return result;
        }
    }
}