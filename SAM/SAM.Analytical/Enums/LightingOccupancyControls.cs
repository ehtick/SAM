﻿using System.ComponentModel;

namespace SAM.Analytical
{
    [Description("Lighting Occupancy Controls")]
    public enum LightingOccupancyControls
    {
        [Description("Undefined")] Undefined,
        [Description("None")] None,
        [Description("AutoOn Dimmed")] AutoOn_Dimmed,
        [Description("AutoOn AutoOff")] AutoOn_AutoOff,
        [Description("ManualOn Dimmed")] ManualOn_Dimmed,
        [Description("ManualOn AutoOff")] ManualOn_AutoOff,
    }
}