﻿using System;

namespace SAM.Core
{
    public static partial class Modify
    {
        public static double Round(this double value, double tolerance = Tolerance.Distance)
        {
            if (double.IsNaN(value))
                return double.NaN;

            if (tolerance.Equals(0.0))
                return value;

            return (double)(Math.Round((decimal)value / (decimal)tolerance) * (decimal)tolerance);
        }
    }
}