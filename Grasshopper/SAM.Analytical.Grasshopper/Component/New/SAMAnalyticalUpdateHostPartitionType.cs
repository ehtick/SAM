﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Properties;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper
{
    [Obsolete("Obsolete since 2021.11.24")]
    public class SAMAnalyticalUpdateHostPartitionType : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("d57f6a6d-14f3-4b1a-a220-9188e00f4eb5");

        /// <summary>
        /// The latest version of this component
        /// </summary>
        public override string LatestComponentVersion => "1.0.0";

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
                protected override System.Drawing.Bitmap Icon => Core.Convert.ToBitmap(Resources.SAM_Small);

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalUpdateHostPartitionType()
          : base("SAMAnalytical.UpdateHostPartitionType", "SAMAnalytical.UpdateHostPartitionType",
              "Related Objects in BuildingModel",
              "SAM", "Analytical")
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
                result.Add(new GH_SAMParam(new GooBuildingModelParam() { Name = "_buildingModel", NickName = "_buildingModel", Description = "SAM Architectural BuildingModel", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPartitionParam() { Name = "partitions_", NickName = "partitions_", Description = "SAM Architectural Partitions", Access = GH_ParamAccess.list, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooWallTypeParam() { Name = "curtainWallType_", NickName = "curtainWallType_", Description = "Curtain WallType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooWallTypeParam() { Name = "internalWallType_", NickName = "internalWallType_", Description = "Internal WallType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooWallTypeParam() { Name = "externalWallType_", NickName = "externalWallType_", Description = "External WallType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooWallTypeParam() { Name = "undergroundWallType_", NickName = "undergroundWallType_", Description = "Underground WallType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooFloorTypeParam() { Name = "internalFloorType_", NickName = "internalFloorType_", Description = "Internal FloorType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooFloorTypeParam() { Name = "externalFloorType_", NickName = "externalFloorType_", Description = "External FloorType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooFloorTypeParam() { Name = "onGradeFloorType_", NickName = "onGradeFloorType_", Description = "On Grade FloorType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooFloorTypeParam() { Name = "undergroundFloorType_", NickName = "undergroundFloorType_", Description = "Underground FloorType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooFloorTypeParam() { Name = "undergroundCeilingFloorType_", NickName = "undergroundCeilingFloorType_", Description = "Underground Ceiling FloorType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new GooRoofTypeParam() { Name = "roofType_", NickName = "roofType_", Description = "RoofType", Access = GH_ParamAccess.item, Optional = true }, ParamVisibility.Voluntary));
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
                result.Add(new GH_SAMParam(new GooBuildingModelParam() { Name = "buildingModel", NickName = "buildingModel", Description = "SAM Architectural BuildingModel", Access = GH_ParamAccess.item }, ParamVisibility.Binding));
                result.Add(new GH_SAMParam(new GooPartitionParam() { Name = "partitions", NickName = "partitions", Description = "SAM Architectural Partitions", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                return result.ToArray();
            }
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            int index = -1;

            BuildingModel buildingModel = null;
            index = Params.IndexOfInputParam("_buildingModel");
            if (index == -1 || !dataAccess.GetData(index, ref buildingModel) || buildingModel == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<IPartition> partitions = null;
            index = Params.IndexOfInputParam("partitions_");
            if (index != -1)
            {
                List<IPartition> partitions_Temp = new List<IPartition>();
                if(dataAccess.GetDataList(index, partitions_Temp) && partitions_Temp != null && partitions_Temp.Count != 0)
                {
                    partitions = partitions_Temp;
                }
            }

            WallType curtainWallType = null;
            index = Params.IndexOfInputParam("curtainWallType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref curtainWallType);
            }

            WallType internalWallType = null;
            index = Params.IndexOfInputParam("internalWallType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref internalWallType);
            }

            WallType externalWallType = null;
            index = Params.IndexOfInputParam("externalWallType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref externalWallType);
            }

            WallType undergroundWallType = null;
            index = Params.IndexOfInputParam("undergroundWallType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref undergroundWallType);
            }

            FloorType internalFloorType = null;
            index = Params.IndexOfInputParam("internalFloorType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref internalFloorType);
            }

            FloorType externalFloorType = null;
            index = Params.IndexOfInputParam("externalFloorType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref externalFloorType);
            }

            FloorType onGradeFloorType = null;
            index = Params.IndexOfInputParam("onGradeFloorType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref onGradeFloorType);
            }

            FloorType undergroundFloorType = null;
            index = Params.IndexOfInputParam("undergroundFloorType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref undergroundFloorType);
            }

            FloorType undergroundCeilingFloorType = null;
            index = Params.IndexOfInputParam("undergroundCeilingFloorType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref undergroundCeilingFloorType);
            }

            RoofType roofType = null;
            index = Params.IndexOfInputParam("roofType_");
            if (index != -1)
            {
                dataAccess.GetData(index, ref roofType);
            }

            buildingModel = new BuildingModel(buildingModel);

            partitions = buildingModel.UpdateHostPartitionType(
                partitions, 
                curtainWallType, 
                internalWallType, 
                externalWallType,
                undergroundWallType,
                internalFloorType,
                externalFloorType,
                onGradeFloorType,
                undergroundFloorType,
                undergroundCeilingFloorType,
                roofType
                );

            index = Params.IndexOfOutputParam("buildingModel");
            if (index != -1)
                dataAccess.SetData(index, new GooBuildingModel(buildingModel));

            index = Params.IndexOfOutputParam("partitions");
            if (index != -1)
                dataAccess.SetDataList(index, partitions?.ConvertAll(x => new GooPartition(x)));
        }
    }
}