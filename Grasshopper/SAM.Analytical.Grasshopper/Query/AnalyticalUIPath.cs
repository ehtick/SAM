﻿namespace SAM.Analytical.Grasshopper
{
    public static partial class Query
    {
        public static string AnalyticalUIPath()
        {
            string fileName = "SAM Analytical.exe";

            string path = System.IO.Path.Combine(Core.Query.ExecutingAssemblyDirectory(), fileName);
            if(!System.IO.Path.Exists(path))
            {
                string path_Temp = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Core.Query.ExecutingAssemblyDirectory()), fileName);
                if (System.IO.Path.Exists(path_Temp))
                {
                    path = path_Temp;
                }
            }

            if (!System.IO.Path.Exists(path))
            {
                string path_Temp = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "SAM", fileName);
                if (System.IO.Path.Exists(path_Temp))
                {
                    path = path_Temp;
                }
            }

            return path;
        }
    }
}