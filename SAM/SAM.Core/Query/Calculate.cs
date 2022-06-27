﻿using System;

namespace SAM.Core
{
    public static partial class Query
    {
        public static double Calculate(this Func<double, double> func, double value, double start, double stop, out int count, out double calculationValue, int maxCount = 100, double tolerance = Core.Tolerance.MacroDistance)
        {
            calculationValue = double.NaN;
            count = -1;

            if (func == null || double.IsNaN(start) || double.IsNaN(stop))
            {
                return double.NaN;
            }

            double min = Math.Min(start, stop);
            double max = Math.Max(start, stop);

            double value_Min_Temp = func.Invoke(min);
            double value_Max_Temp = func.Invoke(max);

            int sign = Math.Sign(value_Max_Temp - value_Min_Temp);

            double value_Min = Math.Min(value_Min_Temp, value_Max_Temp);
            double value_Max = Math.Max(value_Min_Temp, value_Max_Temp);

            if(value < value_Min || value > value_Max)
            {
                return double.NaN;
            }

            count = 0;

            if(value == value_Min)
            {
                return value_Min;
            }

            if (value == value_Max)
            {
                return value_Max;
            }

            double difference = (max - min) / 2;
            double result = min + difference;

            int sign_Temp = 0;

            for (int i = 1; i <= maxCount; i++)
            {
                count = i;

                if (double.IsNaN(result))
                {
                    return double.NaN;
                }

                calculationValue = func.Invoke(result);
                if (double.IsNaN(calculationValue))
                {
                    return double.NaN;
                }

                if (Math.Abs(value - calculationValue) <= tolerance)
                {
                    return result;
                }

                if (sign_Temp == 0)
                {
                    difference = difference / 2;
                    sign_Temp = Math.Sign(value - calculationValue) * sign;
                    result = result + (difference * sign_Temp);
                }
                else
                {
                    int sign_Temp_New = Math.Sign(value - calculationValue) * sign;
                    if (sign_Temp == sign_Temp_New)
                    {
                        result = result + difference * sign_Temp_New;
                        continue;
                    }
                    else
                    {
                        sign_Temp = sign_Temp_New;
                        difference = difference / 2;
                        result = result + (difference * sign_Temp);
                    }

                }
            }

            return result;
        }

        public static double Calculate(this Func<double, double> func, double value, double start, double stop, int maxCount = 100, double tolerance = Core.Tolerance.MacroDistance)
        {
            return Calculate(func, value, start, stop, out int count, out double calculationValue, maxCount, tolerance);
        }
    }
}