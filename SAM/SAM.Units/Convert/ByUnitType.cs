﻿namespace SAM.Units
{
    public static partial class Convert
    {
        public static double ByUnitType(double value, UnitType from, UnitType to)
        {
            switch (from)
            {
                case UnitType.Meter:
                    switch (to)
                    {
                        case UnitType.Feet:
                            return value * 3.280839895;

                        case UnitType.Meter:
                            return value;
                    }
                    break;

                case UnitType.Feet:
                    switch (to)
                    {
                        case UnitType.Meter:
                            return value * 0.3048;

                        case UnitType.Feet:
                            return value;
                    }
                    break;
            }

            return double.NaN;
        }
    }
}