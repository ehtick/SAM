﻿using SAM.Geometry.Planar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAM.Geometry.Spatial
{
    public static partial class Create
    {
        public static List<Shell> Shells(this IEnumerable<ISegmentable2D> segmentable2Ds, double elevation_Min, double elevation_Max, double tolerance = Core.Tolerance.Distance)
        {
            if (segmentable2Ds == null || double.IsNaN(elevation_Min) || double.IsNaN(elevation_Max))
                return null;

            Plane plane_Min = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Min)) as Plane;
            Plane plane_Max = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Max)) as Plane;

            Plane plane_Min_Flipped = new Plane(plane_Min);
            plane_Min_Flipped.FlipZ();

            List<Polygon2D> polygon2Ds = Planar.Create.Polygon2Ds(segmentable2Ds, tolerance);
            if (polygon2Ds == null)
                return null;

            List<Shell> result = new List<Shell>();

            if (polygon2Ds.Count == 0)
                return result;

            for (int i = 0; i < polygon2Ds.Count; i++)
            {
                Polygon2D polygon2D = polygon2Ds[i]?.SimplifyBySAM_Angle();
                if (polygon2D == null)
                    polygon2D = polygon2Ds[i];

                List<Segment2D> segment2Ds = polygon2D.GetSegments();
                if (segment2Ds == null || segment2Ds.Count < 3)
                    continue;

                segment2Ds = Planar.Query.Snap(segment2Ds, true);


                List<Face3D> face3Ds = new List<Face3D>();
                foreach (Segment2D segment2D in segment2Ds)
                {
                    Segment3D segment3D_Min = plane_Min.Convert(segment2D);
                    Segment3D segment3D_Max = plane_Max.Convert(segment2D);

                    Polygon3D polygon3D = new Polygon3D(new Point3D[] { segment3D_Max[0], segment3D_Max[1], segment3D_Min[1], segment3D_Min[0] });

                    face3Ds.Add(new Face3D(polygon3D));
                }

                Polygon3D polygon3D_Min = plane_Min.Convert(polygon2D);
                polygon3D_Min = plane_Min_Flipped.Convert(plane_Min_Flipped.Convert(polygon3D_Min));
                if (polygon3D_Min != null)
                    face3Ds.Add(new Face3D(polygon3D_Min));

                Polygon3D polygon3D_Max = plane_Max.Convert(polygon2D);
                if (polygon3D_Max != null)
                    face3Ds.Add(new Face3D(polygon3D_Max));

                Shell shell = new Shell(face3Ds);
                result.Add(shell);
            }

            return result;
        }

        public static List<Shell> Shells_Depreciated(this IEnumerable<Face3D> face3Ds, double snapTolerance = Core.Tolerance.MacroDistance, double tolerance = Core.Tolerance.Distance)
        {
            if (face3Ds == null)
                return null;

            List<Tuple<double, List<Face2D>>> tuples = new List<Tuple<double, List<Face2D>>>();
            foreach (Face3D face3D in face3Ds)
            {
                BoundingBox3D boundingBox3D = face3D?.GetBoundingBox();
                if (boundingBox3D == null)
                    continue;

                double elevation_Min = boundingBox3D.Min.Z;
                Tuple<double, List<Face2D>> tuple = tuples.Find(x => System.Math.Abs(x.Item1 - elevation_Min) < tolerance);
                if (tuple == null)
                {
                    tuple = new Tuple<double, List<Face2D>>(elevation_Min, new List<Face2D>());
                    tuples.Add(tuple);
                }

                Plane plane = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Min)) as Plane;
                Face2D face2D = plane.Convert(plane.Project(face3D));

                tuple.Item2.Add(face2D);
            }

            List<Shell> result = new List<Shell>();

            if (tuples == null || tuples.Count == 0)
                return result;

            tuples.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            Dictionary<double, List<Tuple<Point2D, Face2D>>> dictionary_Split = new Dictionary<double, List<Tuple<Point2D, Face2D>>>();
            for (int i = 0; i < tuples.Count - 1; i++)
            {
                Tuple<double, List<Face2D>> Tuple_Bottom = tuples[i];
                Tuple<double, List<Face2D>> Tuple_Top = tuples[i + 1];

                List<Face2D> face2Ds_All_Temp = new List<Face2D>();

                if (tuples[i].Item2 != null)
                    face2Ds_All_Temp.AddRange(tuples[i].Item2);

                if (tuples[i + 1].Item2 != null)
                    face2Ds_All_Temp.AddRange(tuples[i + 1].Item2);

                List<Segment2D> segment2Ds = new List<Segment2D>();
                foreach (Face2D face2D in face2Ds_All_Temp)
                {
                    foreach (IClosed2D closed2D in face2D.Edge2Ds)
                    {
                        ISegmentable2D segmentable2D = closed2D as ISegmentable2D;
                        if (segmentable2D == null)
                            segmentable2D = closed2D as ISegmentable2D;

                        if (segmentable2D == null)
                            continue;

                        segment2Ds.AddRange(segmentable2D.GetSegments());
                    }
                }

                if (segment2Ds == null || segment2Ds.Count < 3)
                    continue;

                //segment2Ds = segment2Ds.ConvertAll(x => Planar.Query.Extend(x, snapTolerance, true, true));
                segment2Ds = Planar.Query.Snap(segment2Ds, true, snapTolerance);

                List<Polygon2D> polygon2Ds = Planar.Create.Polygon2Ds(segment2Ds, tolerance);
                face2Ds_All_Temp = Planar.Create.Face2Ds(polygon2Ds, true);
                if (face2Ds_All_Temp != null || face2Ds_All_Temp.Count > 0)
                {
                    dictionary_Split[Tuple_Top.Item1] = face2Ds_All_Temp.ConvertAll(x => new Tuple<Point2D, Face2D>(x?.GetInternalPoint2D(), x));
                    dictionary_Split[Tuple_Top.Item1].RemoveAll(x => x.Item1 == null || x.Item2 == null);
                }
            }

            for (int i = 0; i < tuples.Count - 1; i++)
            {
                double elevation_Bottom = tuples[i].Item1;
                double elevation_Top = tuples[i + 1].Item1;

                Plane plane_Top = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Top)) as Plane;
                Plane plane_Bottom = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Bottom)) as Plane;
                Plane plane_Bottom_Flipped = new Plane(plane_Bottom);
                plane_Bottom_Flipped.FlipZ();

                List<Face2D> face2Ds = tuples[i].Item2;
                foreach (Face2D face2D in face2Ds)
                {
                    List<Face3D> face3Ds_Shell = new List<Face3D>();

                    List<Segment2D> segment2Ds = new List<Segment2D>();
                    foreach (IClosed2D closed2D in face2D.Edge2Ds)
                    {
                        ISegmentable2D segmentable2D = closed2D as ISegmentable2D;
                        if (segmentable2D == null)
                            segmentable2D = closed2D as ISegmentable2D;

                        if (segmentable2D == null)
                            continue;

                        segment2Ds.AddRange(segmentable2D.GetSegments());
                    }

                    if (segment2Ds == null || segment2Ds.Count < 3)
                        continue;

                    segment2Ds = Planar.Query.Snap(segment2Ds, true, snapTolerance);

                    foreach (Segment2D segment2D in segment2Ds)
                    {
                        Segment3D segment3D_Top = plane_Top.Convert(segment2D);
                        Segment3D segment3D_Bottom = plane_Bottom.Convert(segment2D);

                        //Polygon3D polygon3D = Polygon3D(new Point3D[] { segment3D_Bottom[0], segment3D_Bottom[1], segment3D_Top[1], segment3D_Top[0] }, tolerance);
                        Polygon3D polygon3D = Polygon3D(new Point3D[] { segment3D_Top[0], segment3D_Top[1], segment3D_Bottom[1], segment3D_Bottom[0] }, tolerance);
                        //Polygon3D polygon3D = Polygon3D(new Point3D[] { segment3D_Top[0], segment3D_Top[1], segment3D_Bottom[1], segment3D_Bottom[0] }, tolerance);
                        //Polygon3D polygon3D = new Polygon3D(new Point3D[] { segment3D_Top[0], segment3D_Top[1], segment3D_Bottom[1], segment3D_Bottom[0] });
                        if (polygon3D == null)
                            continue;

                        Face3D face3D = new Face3D(polygon3D);
                        face3Ds_Shell.Add(face3D);
                    }

                    //Faces Bottom
                    List<Face2D> face2Ds_Bottom = new List<Face2D>() { face2D };
                    if (dictionary_Split.Count > 0)
                    {
                        if (dictionary_Split.TryGetValue(elevation_Bottom, out List<Tuple<Point2D, Face2D>> tuples_Split))
                        {
                            BoundingBox2D boundingBox2D = face2D.GetBoundingBox();
                            if (boundingBox2D != null)
                            {
                                List<Face2D> face2Ds_Bottom_Temp = new List<Face2D>();
                                foreach (Tuple<Point2D, Face2D> tuple in tuples_Split)
                                {
                                    if (!boundingBox2D.InRange(tuple.Item1, tolerance))
                                        continue;

                                    if (!face2D.InRange(tuple.Item1))
                                        continue;

                                    face2Ds_Bottom_Temp.Add(tuple.Item2);
                                }

                                if (face2Ds_Bottom_Temp != null && face2Ds_Bottom_Temp.Count > 0)
                                    face2Ds_Bottom = face2Ds_Bottom_Temp;
                            }

                        }
                    }

                    foreach (Face2D face2D_Bottom in face2Ds_Bottom)
                    {
                        Face3D face3D_Bottom = plane_Top.Convert(face2D_Bottom);
                        if (face3D_Bottom == null)
                            continue;

                        Face2D face2D_Bottom_Flipped = plane_Bottom_Flipped.Convert(face3D_Bottom);
                        face3D_Bottom = plane_Bottom_Flipped.Convert(face2D_Bottom_Flipped);

                        face3Ds_Shell.Add(face3D_Bottom);
                    }


                    //Faces Top
                    List<Face2D> face2Ds_Top = new List<Face2D>() { face2D };
                    if (dictionary_Split.Count > 0)
                    {
                        if (dictionary_Split.TryGetValue(elevation_Top, out List<Tuple<Point2D, Face2D>> tuples_Split))
                        {
                            BoundingBox2D boundingBox2D = face2D.GetBoundingBox();
                            if (boundingBox2D != null)
                            {
                                List<Face2D> face2Ds_Top_Temp = new List<Face2D>();
                                foreach (Tuple<Point2D, Face2D> tuple in tuples_Split)
                                {
                                    if (!boundingBox2D.InRange(tuple.Item1, tolerance))
                                        continue;

                                    if (!face2D.InRange(tuple.Item1))
                                        continue;

                                    face2Ds_Top_Temp.Add(tuple.Item2);
                                }

                                if (face2Ds_Top_Temp != null && face2Ds_Top_Temp.Count > 0)
                                    face2Ds_Top = face2Ds_Top_Temp;
                            }

                        }
                    }

                    foreach (Face2D face2D_Top in face2Ds_Top)
                    {
                        Face3D face3D_Top = plane_Top.Convert(face2D_Top);
                        if (face3D_Top == null)
                            continue;
                        face3Ds_Shell.Add(face3D_Top);
                    }

                    Shell shell = new Shell(face3Ds_Shell);
                    result.Add(shell);
                }
            }

            return result;
        }

        public static List<Shell> Shells(this IEnumerable<Face3D> face3Ds, double tolerance = Core.Tolerance.Distance)
        {
            if (face3Ds == null)
                return null;

            List<Face2D> face2Ds_All = new List<Face2D>();
            List<Tuple<double, List<Tuple<Face2D, BoundingBox2D>>>> tuples = new List<Tuple<double, List<Tuple<Face2D, BoundingBox2D>>>>();
            foreach (Face3D face3D in face3Ds)
            {
                BoundingBox3D boundingBox3D = face3D?.GetBoundingBox();
                if (boundingBox3D == null)
                    continue;

                double elevation_Min = boundingBox3D.Min.Z;
                Tuple<double, List<Tuple<Face2D, BoundingBox2D>>> tuple = tuples.Find(x => System.Math.Abs(x.Item1 - elevation_Min) < tolerance);
                if (tuple == null)
                {
                    tuple = new Tuple<double, List<Tuple<Face2D, BoundingBox2D>>>(elevation_Min, new List<Tuple<Face2D, BoundingBox2D>>());
                    tuples.Add(tuple);
                }

                Plane plane = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Min)) as Plane;
                Face2D face2D = plane.Convert(plane.Project(face3D));

                tuple.Item2.Add(new Tuple<Face2D, BoundingBox2D>(face2D, face2D.GetBoundingBox()));
                face2Ds_All.Add(face2D);
            }

            face2Ds_All = Planar.Query.Split(face2Ds_All);
            List<Point2D> point2Ds = face2Ds_All.ConvertAll(x => x.GetInternalPoint2D());

            List<Shell> result = new List<Shell>();

            if (tuples == null || tuples.Count == 0)
                return result;

            tuples.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            for (int i = 0; i < tuples.Count - 1; i++)
            {
                Tuple<double, List<Tuple<Face2D, BoundingBox2D>>> tuple_Bottom = tuples[i];
                foreach (Tuple<Face2D, BoundingBox2D> tuple_Bottom_Face2D in tuple_Bottom.Item2)
                {
                    List<Point2D> point2Ds_Temp = point2Ds.FindAll(x => tuple_Bottom_Face2D.Item2.Inside(x, tolerance));
                    if (point2Ds_Temp == null || point2Ds_Temp.Count == 0)
                        continue;

                    Face2D face2D_Bottom = tuple_Bottom_Face2D.Item1;
                    point2Ds_Temp.RemoveAll(x => !face2D_Bottom.Inside(x, tolerance));
                    if (point2Ds_Temp == null || point2Ds_Temp.Count == 0)
                        continue;

                    List<Tuple<double, Face2D>> face2Ds_Top = new List<Tuple<double, Face2D>>();
                    foreach (Point2D point2D_Temp in point2Ds_Temp)
                    {
                        bool found = false;

                        for (int j = i + 1; j < tuples.Count; j++)
                        {
                            Tuple<double, List<Tuple<Face2D, BoundingBox2D>>> tuple_Top = tuples[j];
                            foreach (Tuple<Face2D, BoundingBox2D> tuple_Top_Face2D in tuple_Top.Item2)
                            {
                                if (!tuple_Top_Face2D.Item2.Inside(point2D_Temp, tolerance))
                                    continue;

                                Face2D face2D_Top = tuple_Top_Face2D.Item1;

                                if (!face2D_Top.Inside(point2D_Temp, tolerance))
                                    continue;

                                found = true;
                                if (face2Ds_Top.Find(x => x.Item2 == face2D_Top) == null)
                                    face2Ds_Top.Add(new Tuple<double, Face2D>(tuple_Top.Item1, face2D_Top));

                                break;
                            }

                            if (found)
                                break;
                        }
                    }

                    if (face2Ds_Top == null || face2Ds_Top.Count == 0)
                        continue;

                    List<Face2D> face2Ds_All_Temp = new List<Face2D>(face2Ds_Top.ConvertAll(x => x.Item2));
                    face2Ds_All_Temp.Add(face2D_Bottom);

                    face2Ds_All_Temp = Planar.Query.Split(face2Ds_All_Temp);
                    face2Ds_All_Temp?.RemoveAll(x => !face2D_Bottom.Inside(x.InternalPoint2D(), tolerance));

                    if (face2Ds_All_Temp == null || face2Ds_All_Temp.Count == 0)
                        continue;

                    List<Face3D> face3Ds_Shell = new List<Face3D>();
                    foreach(Face2D face2D_Temp in face2Ds_All_Temp)
                    {
                        Point2D point2D = face2D_Temp.GetInternalPoint2D();
                        if (point2D == null)
                            continue;

                        double elevation_Top = face2Ds_Top.ConvertAll(x => x.Item1).Max();
                        int index = face2Ds_Top.FindIndex(x => x.Item2.Inside(point2D));
                        if (index != -1)
                            elevation_Top = face2Ds_Top[index].Item1;

                        double elevation_Bottom = tuple_Bottom.Item1;
                         
                        Plane plane_Top = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Top)) as Plane;
                        Plane plane_Bottom = Spatial.Plane.WorldXY.GetMoved(new Vector3D(0, 0, elevation_Bottom)) as Plane;
                        Plane plane_Bottom_Flipped = new Plane(plane_Bottom);
                        plane_Bottom_Flipped.FlipZ();

                        List<Segment2D> segment2Ds = new List<Segment2D>();
                        foreach (IClosed2D closed2D in face2D_Temp.Edge2Ds)
                        {
                            ISegmentable2D segmentable2D = closed2D as ISegmentable2D;
                            if (segmentable2D == null)
                                segmentable2D = closed2D as ISegmentable2D;

                            if (segmentable2D == null)
                                continue;

                            segment2Ds.AddRange(segmentable2D.GetSegments());
                        }

                        if (segment2Ds == null || segment2Ds.Count < 3)
                            continue;

                        foreach (Segment2D segment2D in segment2Ds)
                        {
                            Segment3D segment3D_Top = plane_Top.Convert(segment2D);
                            Segment3D segment3D_Bottom = plane_Bottom.Convert(segment2D);

                            Polygon3D polygon3D = Polygon3D(new Point3D[] { segment3D_Top[0], segment3D_Top[1], segment3D_Bottom[1], segment3D_Bottom[0] }, tolerance);
                            if (polygon3D == null)
                                continue;

                            Face3D face3D = new Face3D(polygon3D);
                            face3Ds_Shell.Add(face3D);
                        }

                        Face3D face3D_Bottom = plane_Top.Convert(face2D_Temp);
                        if (face3D_Bottom == null)
                            continue;

                        Face2D face2D_Bottom_Flipped = plane_Bottom_Flipped.Convert(face3D_Bottom);
                        face3D_Bottom = plane_Bottom_Flipped.Convert(face2D_Bottom_Flipped);

                        face3Ds_Shell.Add(face3D_Bottom);

                        Face3D face3D_Top = plane_Top.Convert(face2D_Temp);
                        if (face3D_Top == null)
                            continue;

                        face3Ds_Shell.Add(face3D_Top);
                    }

                    result.Add(Create.Shell(face3Ds_Shell));
                }
            }

            return result;
        }

    }
}