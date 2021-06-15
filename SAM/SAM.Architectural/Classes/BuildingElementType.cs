﻿using Newtonsoft.Json.Linq;

using SAM.Core;
using System.Collections.Generic;

namespace SAM.Architectural
{
    public abstract class BuildingElementType : SAMType
    {
        public BuildingElementType(BuildingElementType buildingElementType)
            : base(buildingElementType)
        {

        }

        public BuildingElementType(JObject jObject)
            : base(jObject)
        {

        }

        public override bool FromJObject(JObject jObject)
        {
            if (!base.FromJObject(jObject))
                return false;

            return true;
        }

        public override JObject ToJObject()
        {
            JObject jObject = base.ToJObject();

            if (jObject == null)
                return jObject;

            return jObject;
        }

    }
}
