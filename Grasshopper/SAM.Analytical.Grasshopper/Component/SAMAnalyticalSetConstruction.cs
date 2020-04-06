﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

using SAM.Analytical.Grasshopper.Properties;

namespace SAM.Analytical.Grasshopper
{
    public class SAMAnalyticalSetConstruction : GH_Component
    {
        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("5803c0d0-3504-4aac-8c6f-d050e360da70");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Resources.SAM_Small;

        /// <summary>
        /// Initializes a new instance of the SAM_point3D class.
        /// </summary>
        public SAMAnalyticalSetConstruction()
          : base("SAMAnalytical.SetConstruction", "SAMAnalytical.SetConstruction",
              "Rename Construction",
              "SAM", "Analytical")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
            inputParamManager.AddParameter(new GooPanelParam(), "_panels", "_construction", "SAM Analytical Panels", GH_ParamAccess.list);
            inputParamManager.AddParameter(new GooConstructionParam(), "_constructions", "_constructions", "SAM Analytical Contructions", GH_ParamAccess.list);
            inputParamManager.AddTextParameter("_csv", "_csv", "csv", GH_ParamAccess.item);
            inputParamManager.AddTextParameter("_sourceColumn", "_sourceColumn", "Source Column Name", GH_ParamAccess.item);
            inputParamManager.AddTextParameter("_destinationColumn", "_destinationColumn", "Destination Column Name", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooConstructionParam(), "Construction", "Construction", "SAM Analytical Construction", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            List<Panel> panels = new List<Panel>();
            if (!dataAccess.GetDataList(0, panels))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Construction> constructions = new List<Construction>();
            if (!dataAccess.GetDataList(1, constructions))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string csvOrPath = null;
            if (!dataAccess.GetData(2, ref csvOrPath) || string.IsNullOrWhiteSpace(csvOrPath))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string sourceColumn = null;
            if (!dataAccess.GetData(3, ref sourceColumn) || string.IsNullOrWhiteSpace(sourceColumn))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            string destinationColumn = null;
            if (!dataAccess.GetData(4, ref destinationColumn) || string.IsNullOrWhiteSpace(destinationColumn))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            Core.DelimitedFileTable delimitedFileTable = null;
            if (Core.Query.ValidFilePath(csvOrPath))
            {
                delimitedFileTable = new Core.DelimitedFileTable(new Core.DelimitedFileReader(Core.DelimitedFileType.Csv, csvOrPath));
            }
            else
            {
                string[] lines = csvOrPath.Split('\n');
                delimitedFileTable = new Core.DelimitedFileTable(new Core.DelimitedFileReader(Core.DelimitedFileType.Csv, lines));
            }

            if(delimitedFileTable == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            int index_Source = delimitedFileTable.GetIndex(sourceColumn);
            if(index_Source == -1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            int index_Destination = delimitedFileTable.GetIndex(destinationColumn);
            if (index_Destination == -1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid data");
                return;
            }

            List<Panel> result = new List<Panel>();
            foreach(Panel panel in panels)
            {
                Construction construction = panel?.Construction;
                if (construction == null)
                    continue;
                
                string name = construction.Name;
                if (name == null)
                {
                    //result.Add(construction);
                    continue;
                }

                string name_destination = null;
                for(int i =0; i < delimitedFileTable.Count; i++)
                {
                    string name_source = null;
                    if (!delimitedFileTable.TryGetValue(i, index_Source, out name_source))
                        continue;

                    if (!name.Equals(name_source))
                        continue;

                    if (!delimitedFileTable.TryGetValue(i, index_Destination, out name_destination))
                    {
                        name_destination = null;
                        continue;
                    }

                    break;
                }

                if(name_destination == null)
                {
                    //result.Add(construction);
                    continue;
                }
                else
                {
                    Construction construction_New = constructions.Find(x => x.Name == name_destination);
                    if (construction_New == null)
                        continue;
                    
                    result.Add(new Panel(panel, construction_New));
                }
            }

            dataAccess.SetDataList(0, result.ConvertAll(x => new GooPanel(x)));
        }
    }
}