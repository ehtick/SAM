﻿using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SAM.Analytical.Grasshopper.Properties;
using SAM.Geometry.Grasshopper;
using SAM.Geometry.Spatial;

namespace SAM.Analytical.Grasshopper
{
    public class SAMAnalyticalAddAperture : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("56dcc3a4-a1ad-4568-858d-fe9fb6ad32db");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalAddAperture()
          : base("SAMAnalytical.AddAperture", "SAMAnalytical.AddAperture",
              "Add Aperture to SAM Analytical Panel",
              "SAM", "Analytical")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddGenericParameter("_geometry", "geometry", "Geometry", GH_ParamAccess.list);
            inputParamManager.AddParameter(new GooPanelParam(), "_panel;", "_panel", "SAM Analytical Panel", GH_ParamAccess.item);
            inputParamManager.AddParameter(new GooApertureConstructionParam(), "_apertureConstruction;", "_apertureConstruction", "SAM Analytical Aperture Construction", GH_ParamAccess.item);
            inputParamManager.AddNumberParameter("maxDistance_", "maxDistance", "Maximal Distance", GH_ParamAccess.item, 0.1);
            inputParamManager.AddBooleanParameter("_trimGeometry_", "trimGeometry", "Trim Aperture Geometry", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooPanelParam(), "Panels", "Panels", "SAM Analytical Panels", GH_ParamAccess.list);
            outputParamManager.AddParameter(new GooApertureParam(), "Apertures", "Apertures", "SAM Analytical Apertures", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            List<object> objects = new List<object>();
            if (!dataAccess.GetDataList(0, objects))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<ISAMGeometry3D> geometry3Ds = new List<ISAMGeometry3D>();
            foreach (object @object in objects)
            {
                List<ISAMGeometry3D> geometry3Ds_Temp = null;
                if (@object is IGH_GeometricGoo)
                {
                    geometry3Ds_Temp = ((IGH_GeometricGoo)@object).ToSAM(true).Cast<ISAMGeometry3D>().ToList();
                }
                else if (@object is GH_ObjectWrapper)
                {
                    GH_ObjectWrapper objectWrapper_Temp = ((GH_ObjectWrapper)@object);
                    if (objectWrapper_Temp.Value is ISAMGeometry3D)
                        geometry3Ds_Temp = new List<ISAMGeometry3D>() { (ISAMGeometry3D)objectWrapper_Temp.Value };
                    else if(objectWrapper_Temp.Value is Geometry.Planar.ISAMGeometry2D)
                        geometry3Ds_Temp = new List<ISAMGeometry3D>() { Plane.Base.Convert(objectWrapper_Temp.Value as dynamic) };
                }
                else if (@object is IGH_Goo)
                {
                    Geometry.ISAMGeometry sAMGeometry = (@object as dynamic).Value as Geometry.ISAMGeometry;
                    if (sAMGeometry is ISAMGeometry3D)
                        geometry3Ds_Temp = new List<ISAMGeometry3D>() { (ISAMGeometry3D)sAMGeometry };
                    else if (sAMGeometry is Geometry.Planar.ISAMGeometry2D)
                        geometry3Ds_Temp = new List<ISAMGeometry3D>() { Plane.Base.Convert(sAMGeometry as dynamic) };
                }

                if (geometry3Ds_Temp != null && geometry3Ds_Temp.Count > 0)
                    geometry3Ds.AddRange(geometry3Ds_Temp);
            }


            List<IClosedPlanar3D> closedPlanar3Ds = Geometry.Spatial.Query.ClosedPlanar3Ds(geometry3Ds);
            if (closedPlanar3Ds == null || closedPlanar3Ds.Count() == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            bool trimGeometry = true;
            if (!dataAccess.GetData(4, ref trimGeometry))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Panel panel = null;
            if (!dataAccess.GetData(1, ref panel))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            panel = new Panel(panel);

            ApertureConstruction apertureConstruction = null;
            if (!dataAccess.GetData(2, ref apertureConstruction))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            double maxDistance = Geometry.Tolerance.MacroDistance;
            dataAccess.GetData(3, ref maxDistance);

            List<Aperture> apertures = new List<Aperture>();
            foreach (IClosedPlanar3D closedPlanar3D in closedPlanar3Ds)
            {
                Aperture aperture = panel.AddAperture(apertureConstruction, closedPlanar3D, trimGeometry, maxDistance);
                if (aperture != null)
                    apertures.Add(aperture);
            }

            dataAccess.SetData(0, new GooPanel(panel));
            dataAccess.SetDataList(1, apertures.ConvertAll(x => new GooAperture(x)));
        }
    }
}