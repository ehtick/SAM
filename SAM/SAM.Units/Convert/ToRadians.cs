﻿namespace SAM.Units
{
    public static partial class Convert
    {
        public static double ToRadians(double value)
        {
            return value * System.Math.PI / 180;
        }
    }
}