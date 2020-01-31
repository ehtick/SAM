﻿using System;
using System.Collections.Generic;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Geometry.Grasshopper;
using SAM.Core.Grasshopper;


namespace SAM.Analytical.Grasshopper
{
    public class GooPlanarBoundary3D : GooSAMObject<PlanarBoundary3D>, IGH_PreviewData, IGH_BakeAwareData
    {
        public GooPlanarBoundary3D()
            : base()
        {

        }

        public GooPlanarBoundary3D(PlanarBoundary3D planarBoundary3D)
            : base(planarBoundary3D)
        {

        }

        public BoundingBox ClippingBox
        {
            get
            {
                if (Value == null)
                    return BoundingBox.Empty;

                return Geometry.Grasshopper.Convert.ToRhino(Value.GetBoundingBox());
            }
        }

        public override IGH_Goo Duplicate()
        {
            return new GooPlanarBoundary3D(Value);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            PlanarBoundary3D planarBoundary3D = Value;
            if (planarBoundary3D == null)
                return;

            Dictionary<Edge3DLoop, System.Drawing.Color> aDictionary = new Dictionary<Edge3DLoop, System.Drawing.Color>();

            //Assign Color for Edges
            aDictionary[planarBoundary3D.GetEdge3DLoop()] = System.Drawing.Color.DarkRed;

            
            IEnumerable<Edge3DLoop> edge3DLoops = planarBoundary3D.GetInternalEdge3DLoops();
            if (edge3DLoops != null)
            {
                foreach (Edge3DLoop edge3DLoop in edge3DLoops)
                    aDictionary[edge3DLoop] = System.Drawing.Color.BlueViolet;
            }

            foreach (KeyValuePair<Edge3DLoop, System.Drawing.Color> keyValuePair in aDictionary)
            {
                List<Edge3D> edge3Ds = keyValuePair.Key.Edge3Ds;
                if (edge3Ds == null || edge3Ds.Count == 0)
                    continue;

                List<Point3d> point3ds = edge3Ds.ConvertAll(x => x.Curve3D.GetStart().ToRhino());
                if (point3ds.Count == 0)
                    continue;

                point3ds.Add(point3ds[0]);

                args.Pipeline.DrawPolyline(point3ds, keyValuePair.Value);
            }
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            Brep brep = Value.ToRhino();
            if (brep != null)
                args.Pipeline.DrawBrepShaded(brep, args.Material);
        }

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
        {
            GeometryBase geometryBase = Value.ToRhino();
            if(geometryBase == null)
            {
                obj_guid = Guid.Empty;
                return false;
            }

            obj_guid = doc.Objects.Add(geometryBase);
            return true;
        }
    }

    public class GooPlanarBoundary3DParam : GH_PersistentParam<GooPlanarBoundary3D>
    {
        public override Guid ComponentGuid => new Guid("3b944b3c-bc94-46cc-aea3-b74385e138dc");

        public GooPlanarBoundary3DParam()
            : base(typeof(PlanarBoundary3D).Name, typeof(PlanarBoundary3D).Name, typeof(PlanarBoundary3D).FullName.Replace(".", " "), "SAM", "Parameters")
        {
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GooPlanarBoundary3D> values)
        {
            throw new NotImplementedException();
        }

        protected override GH_GetterResult Prompt_Singular(ref GooPlanarBoundary3D value)
        {
            throw new NotImplementedException();
        }
    }
}
