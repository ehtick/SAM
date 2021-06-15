﻿using Newtonsoft.Json.Linq;

namespace SAM.Architectural
{
    public abstract class WallType : HostBuildingElementType
    {
        public WallType(WallType wallType)
            : base(wallType)
        {

        }

        public WallType(JObject jObject)
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
