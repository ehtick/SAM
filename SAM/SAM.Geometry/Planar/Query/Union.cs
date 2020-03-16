﻿using System.Collections.Generic;

namespace SAM.Geometry.Planar
{
    public static partial class Query
    {
        public static List<Polygon2D> Union(this Polygon2D polygon2D_1, Polygon2D polygon2D_2)
        {
            if (polygon2D_1 == null || polygon2D_2 == null)
                return null;

            List<Polygon2D> result = new List<Polygon2D>();

            List<Polygon2D> polygon2Ds = new PointGraph2D(new List<Polygon2D>() { polygon2D_1, polygon2D_2 }, true).GetPolygon2Ds();
            if (polygon2Ds == null || polygon2Ds.Count == 0)
                return result;

            BoundingBox2D boundingBox2D_1 = polygon2D_1.GetBoundingBox();
            BoundingBox2D boundingBox2D_2 = polygon2D_1.GetBoundingBox();

            foreach (Polygon2D polygon2D in polygon2Ds)
            {
                Point2D point2D = polygon2D.GetInternalPoint2D();

                if (!boundingBox2D_1.Inside(point2D) || boundingBox2D_2.Inside(point2D))
                    continue;

                if (polygon2D_1.Inside(point2D) && polygon2D_2.Inside(point2D))
                    result.Add(polygon2D);
            }

            return result;
        }
    }
}
