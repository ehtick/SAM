﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Geometry.Spatial
{
    public class Extrusion : SAMGeometry, IBoundable3D
    {
        private Face3D face;
        private Vector3D vector;

        public Extrusion(Face3D face, double height)
        {
            this.face = new Face3D(face);
            vector = new Vector3D(0, 0, height);
        }

        public Extrusion(Face3D face, Vector3D vector)
        {
            this.face = face;
            this.vector = vector;
        }

        public Extrusion(Extrusion extrusion)
        {
            face = new Face3D(extrusion.face);
            vector = new Vector3D(extrusion.vector);
        }

        public Extrusion(JObject jObject)
            : base(jObject)
        {

        }

        public BoundingBox3D GetBoundingBox(double offset = 0)
        {
            throw new NotImplementedException();
        }

        public ISAMGeometry3D GetMoved(Vector3D vector3D)
        {
            return new Extrusion((Face3D)face.GetMoved(vector3D), (Vector3D)vector.Clone());
        }

        public Vector3D Vector
        {
            get
            {
                return new Vector3D(vector);
            }
        }

        public override ISAMGeometry Clone()
        {
            return new Extrusion(this);
        }

        public override bool FromJObject(JObject jObject)
        {
            face = new Face3D(jObject.Value<JObject>("Face"));
            vector = new Vector3D(jObject.Value<JObject>("Vector"));
            return true;
        }

        public override JObject ToJObject()
        {
            JObject jObject = base.ToJObject();
            if (jObject == null)
                return null;

            jObject.Add("Face", face.ToJObject());
            jObject.Add("Vector", vector.ToJObject());

            return jObject;
        }
    }
}
