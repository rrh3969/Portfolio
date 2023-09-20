using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gdapsProject_teamF
{
    public static class Math
    {
        /// <summary>
        /// Linearly Interpolates between 2 numbers
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float Lerp(float start, float end, float time)
        {
            return start + (end - start) * LerpClamp(time);
        }
        
        /// <summary>
        /// Clamp specifically for controlling the time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private static float LerpClamp(float time)
        {
            if (time < 0)
            {
                return 0;
            }
            else if(time > 1)
            {
                return 1;
            }
            return time;
        }
        /// <summary>
        /// Static helper method that allows a number to be clamped in between 2 values
        /// </summary>
        /// <param name="number"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(float number, float min, float max)
        {
            if (number < min)
            {
                return min;
            }
            else if (number > max)
            {
                return max;
            }
            return number;
        }

        /// <summary>
        /// Gives the direction of a float(positive or negative) as either a positive or negative 1
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static float NormalizeFloat(float number)
        {
            return number / MathF.Abs(number);
        }
    }
}
