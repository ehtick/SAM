﻿using Grasshopper.Kernel;
using SAM.Analytical.Grasshopper.Properties;
using SAM.Core.Grasshopper;
using System;
using System.Collections.Generic;

namespace SAM.Analytical.Grasshopper
{
    [Obsolete("Obsolete since 2021.11.24")]
    public class SAMAddMissingMaterials : GH_SAMVariableOutputParameterComponent
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("38319282-f569-40ba-86cd-e556bf1c4674");

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
        public SAMAddMissingMaterials()
          : base("SAMAnalytical.AddMissingMaterials", "SAMAnalytical.AddMissingMaterials",
              "Add Missing Materials",
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

                GooMaterialLibraryParam gooMaterialLibraryParam = new GooMaterialLibraryParam { Name = "_materialLibrary_", NickName = "_materialLibrary_", Description = "SAM Core Material Library", Access = GH_ParamAccess.item };
                gooMaterialLibraryParam.PersistentData.Append(new GooMaterialLibrary(Analytical.Query.DefaultMaterialLibrary()));
                result.Add(new GH_SAMParam(gooMaterialLibraryParam, ParamVisibility.Binding));

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
                result.Add(new GH_SAMParam(new GooMaterialParam() { Name = "materials", NickName = "materials", Description = "SAM Core Materials", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
                result.Add(new GH_SAMParam(new global::Grasshopper.Kernel.Parameters.Param_String() { Name = "missingMaterialNames", NickName = "missingMaterialNames", Description = "Missing Material Names. This Materials could not be found in MaterialLibrary and are still missing", Access = GH_ParamAccess.list }, ParamVisibility.Voluntary));
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

            Core.MaterialLibrary materialLibrary = null;
            index = Params.IndexOfInputParam("_materialLibrary_");
            if (index == -1 || !dataAccess.GetData(index, ref materialLibrary) || materialLibrary == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            buildingModel = new BuildingModel(buildingModel);

            List<Core.IMaterial> materials = buildingModel.AddMissingMaterials(materialLibrary, out List<string>  missingMaterialNames);

            index = Params.IndexOfOutputParam("buildingModel");
            if (index != -1)
                dataAccess.SetData(index, new GooBuildingModel(buildingModel));

            index = Params.IndexOfOutputParam("materials");
            if (index != -1)
                dataAccess.SetDataList(index, materials?.ConvertAll(x => new GooMaterial(x)));

            index = Params.IndexOfOutputParam("missingMaterialNames");
            if (index != -1)
                dataAccess.SetDataList(index, missingMaterialNames);
        }
    }
}