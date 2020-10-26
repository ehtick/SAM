﻿using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;

namespace SAM.Analytical.Grasshopper
{
    public static partial class Modify
    {
        public static bool BakeGeometry(this Panel panel, RhinoDoc rhinoDoc, ObjectAttributes objectAttributes, out Guid guid, bool cutApertures = false, double tolerance = Core.Tolerance.Distance)
        {
            guid = Guid.Empty;

            if (panel == null || rhinoDoc == null || objectAttributes == null)
                return false;

            Brep brep = panel.ToRhino(cutApertures, tolerance);
            if (brep == null)
                return Geometry.Grasshopper.Modify.BakeGeometry(panel.GetFace3D(), rhinoDoc, objectAttributes, out guid);

            guid = rhinoDoc.Objects.AddBrep(brep);
            return true;
        }

        public static bool BakeGeometry(this Aperture aperture, RhinoDoc rhinoDoc, ObjectAttributes objectAttributes, out Guid guid)
        {
            guid = Guid.Empty;

            if (aperture == null || rhinoDoc == null || objectAttributes == null)
                return false;

            return Geometry.Grasshopper.Modify.BakeGeometry(aperture.GetFace3D(), rhinoDoc, objectAttributes, out guid);
        }
    }
}