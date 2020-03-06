﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using SAM.Core;
using SAM.Geometry.Spatial;

namespace SAM.Analytical
{
    public class Panel : SAMInstance
    {
        private PanelType panelType;
        private PlanarBoundary3D planarBoundary3D;
        private List<Aperture> apertures;

        public Panel(Panel panel)
            : base(panel)
        {
            planarBoundary3D = new PlanarBoundary3D(panel.planarBoundary3D);
            panelType = panel.panelType;

            if (panel.apertures != null)
                apertures = new List<Aperture>(panel.apertures.ConvertAll(x => new Aperture(x)));
        }

        public Panel(Panel panel, Construction construction)
            : base(construction == null ? null : construction.Name, panel, construction)
        {
            planarBoundary3D = new PlanarBoundary3D(panel.planarBoundary3D);
            panelType = panel.panelType;

            if (panel.apertures != null)
                apertures = new List<Aperture>(panel.apertures.ConvertAll(x => new Aperture(x)));
        }

        public Panel(Panel panel, PanelType panelType)
            : base(panel)
        {
            planarBoundary3D = new PlanarBoundary3D(panel.planarBoundary3D);
            this.panelType = panelType;

            if (panel.apertures != null)
                apertures = new List<Aperture>(panel.apertures.ConvertAll(x => new Aperture(x)));
        }

        public Panel(Construction construction, PanelType panelType, Face3D face)
            : base(construction == null ? null : construction.Name, construction)
        {
            this.panelType = panelType;
            planarBoundary3D = new PlanarBoundary3D(face);
        }

        public Panel(Construction construction, PanelType panelType, PlanarBoundary3D planarBoundary3D)
            : base(construction == null ? null : construction.Name, construction)
        {
            this.panelType = panelType;
            this.planarBoundary3D = planarBoundary3D;
        }

        public Panel(Guid guid, Panel panel, Face3D face, double maxDistance = Geometry.Tolerance.MacroDistance)
            : base(guid, panel)
        {
            panelType = panel.panelType;
            planarBoundary3D = new PlanarBoundary3D(face);

            if (panel.apertures != null)
                apertures = panel.apertures.FindAll(x => Query.IsValid(this, x)).ConvertAll(x => new Aperture(x));
        }

        public Panel(Guid guid, string name, IEnumerable<ParameterSet> parameterSets, Construction construction, PanelType panelType, PlanarBoundary3D planarBoundary3D)
            : base(guid, name, parameterSets, construction)
        {
            this.panelType = panelType;
            this.planarBoundary3D = new PlanarBoundary3D(planarBoundary3D);
        }

        public Panel(JObject jObject)
             : base(jObject)
        {

        }

        public Face3D GetFace3D()
        {
            return planarBoundary3D.GetFace3D();
        }

        public void Snap(IEnumerable<Point3D> point3Ds, double maxDistance = double.NaN)
        {
            planarBoundary3D.Snap(point3Ds, maxDistance);
        }

        public BoundingBox3D GetBoundingBox(double offset = 0)
        {
            return GetFace3D().GetBoundingBox(offset);
        }

        public Construction Construction
        {
            get
            {
                return SAMType as Construction;
            }
        }

        public PanelType PanelType
        {
            get
            {
                return panelType;
            }
        }

        public PlanarBoundary3D PlanarBoundary3D
        {
            get
            {
                return new PlanarBoundary3D(planarBoundary3D);
            }
        }

        public override bool FromJObject(JObject jObject)
        {
            if (!base.FromJObject(jObject))
                return false;

            Enum.TryParse(jObject.Value<string>("PanelType"), out this.panelType);

            planarBoundary3D = new PlanarBoundary3D(jObject.Value<JObject>("PlanarBoundary3D"));

            if (jObject.ContainsKey("Apertures"))
                apertures = Core.Create.IJSAMObjects<Aperture>(jObject.Value<JArray>("Apertures"));

            return true;
        }

        public override JObject ToJObject()
        {
            JObject jObject = base.ToJObject();
            if (jObject == null)
                return jObject;

            jObject.Add("PanelType", panelType.ToString());
            jObject.Add("PlanarBoundary3D", planarBoundary3D.ToJObject());

            if (apertures != null)
                jObject.Add("Apertures", Core.Create.JArray(apertures));
            
            return jObject;
        }

        public Aperture AddAperture(ApertureConstruction apertureConstruction, IClosedPlanar3D closedPlanar3D, bool trimGeometry = true, double maxDistance = Geometry.Tolerance.MacroDistance)
        {
            if (apertureConstruction == null || closedPlanar3D == null)
                return null;

            Plane plane = planarBoundary3D?.Plane;
            if (plane == null)
                return null;

            IClosedPlanar3D closedPlanar3D_Projected = plane.Project(closedPlanar3D);
            if (closedPlanar3D_Projected == null)
                return null;

            Plane plane_ClosedPlanar3D_Projected = closedPlanar3D_Projected.GetPlane();
            if (plane_ClosedPlanar3D_Projected == null)
                return null;

            if (plane_ClosedPlanar3D_Projected.Origin.Distance(closedPlanar3D.GetPlane().Origin) > maxDistance)
                return null;

            if(trimGeometry)
            {
                Geometry.Planar.IClosed2D closed2D = plane.Convert(planarBoundary3D.GetFace3D().GetExternalEdge());
                if(closed2D is Geometry.Planar.ISegmentable2D)
                {
                    Geometry.Planar.IClosed2D closed2D_Aperture = plane.Convert(closedPlanar3D_Projected);
                    if (closed2D_Aperture is Geometry.Planar.ISegmentable2D)
                    {
                        List<Geometry.Planar.Segment2D> segment2Ds = Geometry.Planar.Modify.Split(new List<Geometry.Planar.ISegmentable2D>() { (Geometry.Planar.ISegmentable2D)closed2D, (Geometry.Planar.ISegmentable2D)closed2D_Aperture });
                        Geometry.Planar.CurveGraph2D curveGraph2D = new Geometry.Planar.CurveGraph2D(segment2Ds);
                        List<Geometry.Planar.PolycurveLoop2D> polycurveLoop2Ds = curveGraph2D.GetPolycurveLoop2Ds();
                        if(polycurveLoop2Ds != null && polycurveLoop2Ds.Count > 0)
                        {
                            foreach(Geometry.Planar.PolycurveLoop2D polycurveLoop2D in polycurveLoop2Ds)
                            {
                                Geometry.Planar.Point2D point2D = polycurveLoop2D.GetInternalPoint2D();
                                if (closed2D_Aperture.Inside(point2D) && closed2D.Inside(point2D))
                                {
                                    closedPlanar3D_Projected = plane.Convert(polycurveLoop2D.ToPolygon2D());
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            Aperture aperture = new Aperture(apertureConstruction, closedPlanar3D_Projected);
            if (!Query.IsValid(this, aperture))
                return null;

            if (apertures == null)
                apertures = new List<Aperture>();

            apertures.Add(aperture);
            return aperture;
        }

        public bool AddAperture(Aperture aperture)
        {
            if (!Query.IsValid(this, aperture))
                return false;

            if (apertures == null)
                apertures = new List<Aperture>();

            apertures.Add(aperture);
            return true;
        }

        public List<Aperture> Apertures
        {
            get
            {
                if (apertures == null)
                    return null;
                return apertures.ConvertAll(x => x.Clone());
            }
        }
    }
}
