﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using SAM.Analytical.Grasshopper.Properties;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SAM.Analytical.Grasshopper
{
    public class GooAnalyticalModel : GooJSAMObject<AnalyticalModel>, IGH_PreviewData, IGH_BakeAwareData
    {
        public GooAnalyticalModel()
            : base()
        {
        }

        public GooAnalyticalModel(AnalyticalModel analyticalModel)
            : base(analyticalModel)
        {
        }

        public BoundingBox ClippingBox
        {
            get
            {
                if (Value?.AdjacencyCluster == null)
                    return BoundingBox.Unset;

                return new GooAdjacencyCluster(Value.AdjacencyCluster).ClippingBox;
            }
        }

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
        {
            obj_guid = Guid.Empty;
            
            if (Value?.AdjacencyCluster == null)
                return false;

            return new GooAdjacencyCluster(Value.AdjacencyCluster).BakeGeometry(doc, att, out obj_guid);
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            if (Value?.AdjacencyCluster == null)
                return;

            new GooAdjacencyCluster(Value.AdjacencyCluster).DrawViewportMeshes(args);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value?.AdjacencyCluster == null)
                return;

            new GooAdjacencyCluster(Value.AdjacencyCluster).DrawViewportWires(args);
        }

        public override IGH_Goo Duplicate()
        {
            return new GooAnalyticalModel(Value);
        }

        public override bool CastFrom(object source)
        {
            if (source is AnalyticalModel)
            {
                Value = (AnalyticalModel)source;
                return true;
            }

            if (typeof(IGH_Goo).IsAssignableFrom(source.GetType()))
            {
                try
                {
                    source = (source as dynamic).Value;
                }
                catch
                {
                }

                if (source is AnalyticalModel)
                {
                    Value = (AnalyticalModel)source;
                    return true;
                }
            }

            return base.CastFrom(source);
        }

        public override bool CastTo<Y>(ref Y target)
        {
            if (Value == null)
                return false;

            if (typeof(Y).IsAssignableFrom(Value.GetType()))
            {
                target = (Y)(object)Value;
                return true;
            }
            
            if (typeof(Y).IsAssignableFrom(typeof(GH_Mesh)))
            {
                target = (Y)(object)Value.ToGrasshopper_Mesh();
                return true;
            }

            //if (typeof(Y).IsAssignableFrom(typeof(GH_Brep)))
            //{
            //    List<Geometry.Spatial.Shell> shells = Value.GetShells();
            //    if (shells != null)
            //    {
            //        Brep brep = Brep.MergeBreps(shells.ConvertAll(x => x.ToRhino()), Core.Tolerance.MacroDistance);
            //        if (brep != null)
            //        {
            //            target = (Y)(object)new GH_Brep(brep);
            //            return true;
            //        }
            //    }
            //}

            return base.CastTo(ref target);
        }
    }

    public class GooAnalyticalModelParam : GH_PersistentParam<GooAnalyticalModel>, IGH_PreviewObject, IGH_BakeAwareObject
    {
        public override Guid ComponentGuid => new Guid("01466a73-e3f3-495d-b794-bd322c9edfa0");

                protected override System.Drawing.Bitmap Icon => Core.Convert.ToBitmap(Resources.SAM_Small);

        public bool Hidden { get; set; }

        public bool IsPreviewCapable => !VolatileData.IsEmpty;

        public BoundingBox ClippingBox => Preview_ComputeClippingBox();

        public bool IsBakeCapable => true;

        public GooAnalyticalModelParam()
            : base(typeof(AnalyticalModel).Name, typeof(AnalyticalModel).Name, typeof(AnalyticalModel).FullName.Replace(".", " "), "Params", "SAM")
        {
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GooAnalyticalModel> values)
        {
            throw new NotImplementedException();
        }

        protected override GH_GetterResult Prompt_Singular(ref GooAnalyticalModel value)
        {
            throw new NotImplementedException();
        }

        public void DrawViewportWires(IGH_PreviewArgs args) => Preview_DrawWires(args);

        public void DrawViewportMeshes(IGH_PreviewArgs args) => Preview_DrawMeshes(args);

        public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids)
        {
            BakeGeometry(doc, doc.CreateDefaultAttributes(), obj_ids);
        }

        public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids)
        {
            foreach (var value in VolatileData.AllData(true))
            {
                Guid uuid = default;
                (value as IGH_BakeAwareData)?.BakeGeometry(doc, att, out uuid);
                obj_ids.Add(uuid);
            }
        }

        public void BakeGeometry_ByPanelType(RhinoDoc doc)
        {
            Modify.BakeGeometry_ByPanelType(doc, VolatileData, true, Core.Tolerance.Distance);
        }

        public void BakeGeometry_ByDischargeCoefficient(RhinoDoc doc)
        {
            Modify.BakeGeometry_ByDischargeCoefficient(doc, VolatileData);
        }

        public void BakeGeometry_ByConstruction(RhinoDoc doc)
        {
            Modify.BakeGeometry_ByConstruction(doc, VolatileData, true, Core.Tolerance.Distance);
        }

        public void BakeGeometry_ByBoundaryType(RhinoDoc doc)
        {
            Modify.BakeGeometry_ByBoundaryType(doc, VolatileData, true, Core.Tolerance.Distance);
        }

        public override void AppendAdditionalMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Bake By Type", Menu_BakeByPanelType, Core.Convert.ToBitmap(Resources.SAM3), VolatileData.AllData(true).Any());
            Menu_AppendItem(menu, "Bake By Construction", Menu_BakeByConstruction, Core.Convert.ToBitmap(Resources.SAM3), VolatileData.AllData(true).Any());
            Menu_AppendItem(menu, "Bake By BoundaryType", Menu_BakeByBoundaryType, Core.Convert.ToBitmap(Resources.SAM3), VolatileData.AllData(true).Any());
            Menu_AppendItem(menu, "Bake By Discharge Coefficient", Menu_BakeByDischargeCoefficient, Core.Convert.ToBitmap(Resources.SAM3), VolatileData.AllData(true).Any());
            Menu_AppendItem(menu, "Save As...", Menu_SaveAs, Core.Convert.ToBitmap(Resources.SAM3), VolatileData.AllData(true).Any());
            
            if(System.IO.File.Exists(Query.AnalyticalUIPath()))
            {
                Menu_AppendItem(menu, "Open in UI", Menu_OpenInUI, Core.Convert.ToBitmap(Resources.SAM3), VolatileData.AllData(true).Any());
            }

            base.AppendAdditionalMenuItems(menu);
        }

        private void Menu_BakeByPanelType(object sender, EventArgs e)
        {
            BakeGeometry_ByPanelType(RhinoDoc.ActiveDoc);
        }

        private void Menu_BakeByDischargeCoefficient(object sender, EventArgs e)
        {
            BakeGeometry_ByDischargeCoefficient(RhinoDoc.ActiveDoc);
        }

        private void Menu_BakeByConstruction(object sender, EventArgs e)
        {
            BakeGeometry_ByConstruction(RhinoDoc.ActiveDoc);
        }

        private void Menu_BakeByBoundaryType(object sender, EventArgs e)
        {
            BakeGeometry_ByBoundaryType(RhinoDoc.ActiveDoc);
        }

        private void Menu_SaveAs(object sender, EventArgs e)
        {
            Core.Grasshopper.Query.SaveAs(VolatileData);
        }

        private void Menu_OpenInUI(object sender, EventArgs e)
        {
            Process process = Convert.ToUI(VolatileData);
        }
    }
}