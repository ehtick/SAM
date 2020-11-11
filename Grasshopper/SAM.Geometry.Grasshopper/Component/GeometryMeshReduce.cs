﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SAM.Core.Grasshopper;
using SAM.Geometry.Grasshopper.Properties;
using System;
using System.Collections.Generic;

namespace SAM.Geometry.Grasshopper
{
    public class GeometryMeshReduce : GH_SAMComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("50f45468-60e1-4a11-9c2c-0e2b01f72952");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Geometry;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public GeometryMeshReduce()
          : base("Geometry.MeshReduce", "Geometry.MeshReduce",
              "Reduce Rhino Mesh",
              "SAM", "Geometry")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddMeshParameter("_mesh", "_mesh", "Rhino Mesh", GH_ParamAccess.item);
            inputParamManager.AddBooleanParameter("_distortion_", "_distortion_", "Allow Distortion", GH_ParamAccess.item, true);
            inputParamManager.AddIntegerParameter("_count", "_count", "Desired Polygon Count", GH_ParamAccess.item);
            inputParamManager.AddIntegerParameter("_accuracy", "_accuracy", "Accuracy", GH_ParamAccess.item, 10);
            inputParamManager.AddBooleanParameter("_normalizedSize", "_normalizedSize", "Normalized Size", GH_ParamAccess.item, true);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddMeshParameter("Mesh", "Mesh", "Mesh", GH_ParamAccess.item);
            //outputParamManager.AddParameter( string, "Normal", "Normal", "Normal", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            Mesh mesh = null;
            if (!dataAccess.GetData(0, ref mesh))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            bool allowDistortion = true;
            dataAccess.GetData(1, ref allowDistortion);

            int deisredPolygonCount = 0;
            dataAccess.GetData(2, ref deisredPolygonCount);

            int accuracy = 0;
            dataAccess.GetData(3, ref accuracy);

            bool normalizedSize = true;
            dataAccess.GetData(1, ref normalizedSize);

            Mesh result = mesh.DuplicateMesh();
            Modify.Reduce(result, allowDistortion, deisredPolygonCount, accuracy, normalizedSize);

            dataAccess.SetData(0, result);
        }
    }
}