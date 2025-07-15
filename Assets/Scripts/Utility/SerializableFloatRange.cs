using System;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// A serializable struct to represent a min/max float range.
    /// Unity cannot serialize ValueTuples, so we use a custom struct.
    /// </summary>
    [Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public FloatRange(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Overloads the + operator to add a float to both min and max of the range.
        /// </summary>
        /// <param name="range">The FloatRange operand.</param>
        /// <param name="value">The float value to add.</param>
        /// <returns>A new FloatRange with the value added to min and max.</returns>
        public static FloatRange operator +(FloatRange range, float value)
        {
            return new FloatRange(range.min + value, range.max + value);
        }

        /// <summary>
        /// Overloads the - operator to subtract a float from both min and max of the range.
        /// </summary>
        /// <param name="range">The FloatRange operand.</param>
        /// <param name="value">The float value to subtract.</param>
        /// <returns>A new FloatRange with the value subtracted from min and max.</returns>
        public static FloatRange operator -(FloatRange range, float value)
        {
            return new FloatRange(range.min - value, range.max - value);
        }

        /// <summary>
        /// Overloads the + operator to allow the float to be on the left-hand side.
        /// </summary>
        /// <param name="value">The float value to add.</param>
        /// <param name="range">The FloatRange operand.</param>
        /// <returns>A new FloatRange with the value added to min and max.</returns>
        public static FloatRange operator +(float value, FloatRange range)
        {
            // This calls the other overloaded operator to avoid repeating code.
            return range + value;
        }

        /// <summary>
        /// Overloads the + operator to combine two FloatRanges. The mins are naively added to one another, as are the maxes.
        /// </summary>
        /// <param name="range1">The first FloatRange operand.</param>
        /// <param name="range2">The second FloatRange operand.</param>
        /// <returns>A new FloatRange with the value added to min and max.</returns>
        public static FloatRange operator +(FloatRange range1, FloatRange range2)
        {
            // This calls the other overloaded operator to avoid repeating code.
            return new FloatRange(range1.min + range2.min, range1.max + range2.max);
        }
    }
}