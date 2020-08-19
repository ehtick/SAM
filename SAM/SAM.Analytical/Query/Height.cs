﻿using SAM.Geometry.Planar;
using SAM.Geometry.Spatial;

namespace SAM.Analytical
{
    public static partial class Query
    {
        public static double Height(this PlanarBoundary3D planarBoundary3D)
        {
            //TODO: Find better way to determine Height

            IClosed2D closed2D = planarBoundary3D?.Edge2DLoop?.GetClosed2D();
            if (closed2D == null)
                return double.NaN;

            ISegmentable2D segmentable2D = closed2D as ISegmentable2D;
            if (segmentable2D == null)
                throw new System.NotImplementedException();

            Rectangle2D rectangle2D = Geometry.Planar.Create.Rectangle2D(segmentable2D.GetPoints());
            if (rectangle2D == null)
                return double.NaN;

            Plane plane = Plane.WorldXY;

            Vector3D vector3D_WidthDirection = plane.Convert(rectangle2D.WidthDirection);
            Vector3D vector3D_HeightDirection = plane.Convert(rectangle2D.HeightDirection);

            Vector3D axisY = plane.AxisY;

            double result = rectangle2D.Height;
            if (axisY.SmallestAngle(vector3D_WidthDirection) < axisY.SmallestAngle(vector3D_HeightDirection))
                result = rectangle2D.Width;

            return result;


            //Method 2
            //IClosedPlanar3D closedPlanar3D = Plane.WorldXY.Convert(closed2D);
            //if (closedPlanar3D == null)
            //    return double.NaN;

            //BoundingBox3D boundingBox3D = closedPlanar3D.GetBoundingBox();
            //if(boundingBox3D == null)
            //    return double.NaN;

            //return boundingBox3D.Max.Y - boundingBox3D.Min.Y;


            //Method 1
            //ISegmentable2D segmentable2D = closed2D as ISegmentable2D;

            //if (segmentable2D == null)
            //    throw new System.NotImplementedException();

            //Rectangle2D rectangle2D = Geometry.Planar.Create.Rectangle2D(segmentable2D.GetPoints());
            //if (rectangle2D == null)
            //    return double.NaN;

            //return rectangle2D.Height;
        }
    }
}