﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using SAM.Analytical.Grasshopper.Properties;
using SAM.Core;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper
{
    public class SAMAnalyticalExtendPanels : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("97e5ae37-fedf-4f2b-9650-b551da66de6d");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.1";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalExtendPanels()
          : base("SAMAnalytical.ExtendPanels", "SAMAnalytical.ExtendPanels",
              "Extend Panels",
              "SAM", "Analytical01")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override GH_SAMParam[] Inputs
        {
            get
            {
                List<GH_SAMParam> result = new List<GH_SAMParam>();

                GooPanelParam panelParam = new GooPanelParam() { Name = "_panels", NickName = "_panels", Description = "SAM Analytical Panels", Access = GH_ParamAccess.list };
                panelParam.DataMapping = GH_DataMapping.Flatten;
                result.Add(new GH_SAMParam(panelParam, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_GenericObject genericObject = new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "_elevation", NickName = "elevation", Description = "Elevation", Access = GH_ParamAccess.item };
                result.Add(new GH_SAMParam(genericObject, ParamVisibility.Binding));

                global::Grasshopper.Kernel.Parameters.Param_Number paramNumber;

                paramNumber = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "maxDistance_", NickName = "maxDistance_", Description = "Max Distance", Access = GH_ParamAccess.item };
                paramNumber.SetPersistentData(0.2);
                result.Add(new GH_SAMParam(paramNumber, ParamVisibility.Binding));

                paramNumber = new global::Grasshopper.Kernel.Parameters.Param_Number() { Name = "tolerance_", NickName = "tolerance_", Description = "Tolerance", Access = GH_ParamAccess.item };
                paramNumber.SetPersistentData(Tolerance.Distance);
                result.Add(new GH_SAMParam(paramNumber, ParamVisibility.Voluntary));

                return result.ToArray();
            }
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override GH_SAMParam[] Outputs
        {
            get
            {
                List<GH_SAMParam> result = new List<GH_SAMParam>();
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "panels", NickName = "panels", Description = "SAM Analytical Panels", Access = GH_ParamAccess.list }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPanelParam() { Name = "extendedPanels", NickName = "extendedPanels", Description = "SAM Analytical Panels", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_GenericObject() { Name = "segment3Ds", NickName = "segment3Ds", Description = "SAM Geometry Segment3Ds", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                return result.ToArray();
            }
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            int index = -1;

            index = Params.IndexOfInputParam("_panels");
            List<Panel> panels = new List<Panel>();
            if(index == -1 || !dataAccess.GetDataList(index, panels) || panels == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid Data");
                return;
            }

            index = Params.IndexOfInputParam("_elevation");
            GH_ObjectWrapper objectWrapper_Elevation = null;
            if (index == -1 || !dataAccess.GetData(index, ref objectWrapper_Elevation) || objectWrapper_Elevation == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid Data");
                return;
            }

            double elevation = double.NaN;

            object @object = objectWrapper_Elevation.Value;
            if (@object is IGH_Goo)
            {
                @object = (@object as dynamic).Value;
            }

            if (@object is double)
            {
                elevation = (double)@object;
            }
            else if (@object is string)
            {
                if (double.TryParse((string)@object, out double elevation_Temp))
                    elevation = elevation_Temp;
            }
            else if (@object is Architectural.Level)
            {
                elevation = ((Architectural.Level)@object).Elevation;
            }

            index = Params.IndexOfInputParam("maxDistance_");
            double maxDistance = 0.2;
            if (index != -1)
                dataAccess.GetData(index, ref maxDistance);

            if (double.IsNaN(maxDistance))
                maxDistance = 0.2;

            index = Params.IndexOfInputParam("tolerance_");
            double tolerance = Tolerance.Distance;
            if (index != -1)
                dataAccess.GetData(index, ref tolerance);

            if (double.IsNaN(tolerance))
                tolerance = Tolerance.Distance;

            panels = panels?.ConvertAll(x => Create.Panel(x));

            Analytical.Modify.Extend(panels, elevation, maxDistance, out List<Panel> panels_Extended, out List<Geometry.Spatial.Segment3D> segment3Ds, Tolerance.MacroDistance, Tolerance.Angle, tolerance);

            index = Params.IndexOfOutputParam("panels");
            if (index != -1)
                dataAccess.SetDataList(index, panels?.ConvertAll(x => new GooPanel(x)));

            index = Params.IndexOfOutputParam("extendedPanels");
            if (index != -1)
                dataAccess.SetDataList(index, panels_Extended?.ConvertAll(x => new GooPanel(x)));

            index = Params.IndexOfOutputParam("segment3Ds");
            if (index != -1)
                dataAccess.SetDataList(index, segment3Ds);

        }
    }
}