﻿using NetTopologySuite.Geometries;
using NetTopologySuite.Simplify;
using SAM.Core;

namespace SAM.Geometry.Planar
{
    public static partial class Query
    {
        public static Polygon SimplifyByNTS_TopologyPreservingSimplifier(this Polygon polygon, double tolerance = Tolerance.Distance)
        {
            if (polygon == null)
                return null;

            Polygon result = TopologyPreservingSimplifier.Simplify(polygon, tolerance) as Polygon;
            if (result == null)
                return polygon;

            return result;
        }

        public static Polygon2D SimplifyByNTS_TopologyPreservingSimplifier(this Polygon2D polygon2D, double tolerance = Tolerance.Distance)
        {
            if (polygon2D == null)
                return null;

            LinearRing linearRing = ((IClosed2D)polygon2D).ToNTS(tolerance);

            linearRing = TopologyPreservingSimplifier.Simplify(linearRing, tolerance) as LinearRing;
            if (linearRing == null)
                return null;

            return linearRing.ToSAM();
        }

        public static Face2D SimplifyByNTS_TopologyPreservingSimplifier(this Face2D face2D, double tolerance = Tolerance.Distance)
        {
            if (face2D == null)
                return null;

            Polygon polygon = face2D.ToNTS(tolerance);

            polygon = TopologyPreservingSimplifier.Simplify(polygon, tolerance) as Polygon;

            return polygon?.ToSAM();
        }
    }
}