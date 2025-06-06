﻿using System;
using System.Windows.Forms;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using SAM.Core.Grasshopper.Properties;

namespace SAM.Core.Grasshopper
{
    public abstract class GH_SAMEnumComponent<T> : GH_SAMComponent where T : Enum
    {
        private T value;

        public GH_SAMEnumComponent(string name, string nickname, string description, string category, string subCategory)
            : base(name, nickname, description, category, subCategory)
        {

        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(typeof(T).Name, value.GetHashCode());
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            int @int = -1;
            if (reader.TryGetInt32(typeof(T).Name, ref @int) && @int != -1)
            {
                try
                {
                    value = (T)Enum.ToObject(typeof(T), @int);
                }
                catch
                {

                }
            }

            return base.Read(reader);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            foreach (Enum @enum in Enum.GetValues(typeof(T)))
                Menu_AppendItem(menu, @enum.ToString(), Menu_Changed, true, @enum.Equals(value)).Tag = @enum;

            base.AppendAdditionalComponentMenuItems(menu);
        }

        private void Menu_Changed(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is T)
            {
                value = (T)item.Tag;
                ExpireSolution(true);
            }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager inputParamManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager outputParamManager)
        {
            outputParamManager.AddParameter(new GooObjectParam() { Name = typeof(T).Name, NickName = typeof(T).Name, Description = typeof(T).Name, Access = GH_ParamAccess.item});
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="dataAccess">
        /// The DA object is used to retrieve from inputs and store in outputs.
        /// </param>
        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            dataAccess.SetData(0, new GooObject(value));
        }

    }
}
