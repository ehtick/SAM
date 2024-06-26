﻿using System.Collections.Generic;

namespace SAM.Analytical
{
    public static partial class Query
    {
        public static Geometry.Spatial.Shell Shell(this AdjacencyCluster adjacencyCluster, ISpace space)
        {
            if (adjacencyCluster == null || space == null)
            {
                return null;
            }

            List<IPanel> panels = adjacencyCluster.GetRelatedObjects<IPanel>(space);
            if (panels == null || panels.Count == 0)
            {
                return null;
            }

            return new Geometry.Spatial.Shell(panels.ConvertAll(x => x.Face3D));
        }

        public static Geometry.Spatial.Shell Shell(this AdjacencyCluster adjacencyCluster, System.Guid spaceGuid)
        {
            if (adjacencyCluster == null || spaceGuid == System.Guid.Empty)
            {
                return null;
            }

            Space space = adjacencyCluster.GetObject<Space>(spaceGuid);
            if(space == null)
            {
                return null;
            }

            return Shell(adjacencyCluster, space);
        }
    }
}