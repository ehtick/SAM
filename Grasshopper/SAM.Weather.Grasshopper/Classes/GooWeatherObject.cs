﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SAM.Weather.Grasshopper.Properties;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace SAM.Weather.Grasshopper
{
    public class GooWeatherObject : GooJSAMObject<IWeatherObject>
    {
        public GooWeatherObject()
            : base()
        {
        }

        public GooWeatherObject(IWeatherObject weatherObject)
            : base(weatherObject)
        {
        }

        public override IGH_Goo Duplicate()
        {
            return new GooWeatherObject(Value);
        }
    }

    public class GooWeatherObjectParam : GH_PersistentParam<GooWeatherObject>, IGH_PreviewObject
    {
        public override Guid ComponentGuid => new Guid("b96713e0-5cde-4774-a061-0f3490d3d0a5");

                protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        bool IGH_PreviewObject.Hidden { get; set; }

        bool IGH_PreviewObject.IsPreviewCapable => !VolatileData.IsEmpty;

        BoundingBox IGH_PreviewObject.ClippingBox => Preview_ComputeClippingBox();

        void IGH_PreviewObject.DrawViewportMeshes(IGH_PreviewArgs args) => Preview_DrawMeshes(args);

        void IGH_PreviewObject.DrawViewportWires(IGH_PreviewArgs args) => Preview_DrawWires(args);

        public GooWeatherObjectParam()
            : base(typeof(IWeatherObject).Name, typeof(IWeatherObject).Name, typeof(IWeatherObject).FullName.Replace(".", " "), "Params", "SAM")
        {
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GooWeatherObject> values)
        {
            throw new NotImplementedException();
        }

        protected override GH_GetterResult Prompt_Singular(ref GooWeatherObject value)
        {
            throw new NotImplementedException();
        }

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(menu, "Save As...", Menu_SaveAs, VolatileData.AllData(true).Any());

            //Menu_AppendSeparator(menu);

            base.AppendAdditionalMenuItems(menu);
        }

        private void Menu_SaveAs(object sender, EventArgs e)
        {
            Core.Grasshopper.Query.SaveAs(VolatileData);
        }
    }
}