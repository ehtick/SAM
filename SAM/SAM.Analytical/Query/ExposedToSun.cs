﻿
namespace SAM.Analytical
{
    public static partial class Query
    {
        public static bool ExposedToSun(this PanelType panelType)
        {
            switch (panelType)
            {
                case Analytical.PanelType.CurtainWall:
                case Analytical.PanelType.FloorExposed:
                case Analytical.PanelType.FloorRaised:
                case Analytical.PanelType.Roof:
                case Analytical.PanelType.Shade:
                case Analytical.PanelType.SolarPanel:
                case Analytical.PanelType.WallExternal:
                    return true;
                default:
                    return false;
            }
        }
    }
}