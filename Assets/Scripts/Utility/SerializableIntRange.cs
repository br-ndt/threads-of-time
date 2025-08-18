using System;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// A serializable struct to represent a min/max int range.
    /// Unity cannot serialize ValueTuples, so we use a custom struct.
    /// </summary>
    [Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Overloads the + operator to add a int to both min and max of the range.
        /// </summary>
        /// <param name="range">The IntRange operand.</param>
        /// <param name="value">The int value to add.</param>
        /// <returns>A new IntRange with the value added to min and max.</returns>
        public static IntRange operator +(IntRange range, int value)
        {
            return new IntRange(range.min + value, range.max + value);
        }

        /// <summary>
        /// Overloads the - operator to subtract a int from both min and max of the range.
        /// </summary>
        /// <param name="range">The IntRange operand.</param>
        /// <param name="value">The int value to subtract.</param>
        /// <returns>A new IntRange with the value subtracted from min and max.</returns>
        public static IntRange operator -(IntRange range, int value)
        {
            return new IntRange(range.min - value, range.max - value);
        }

        /// <summary>
        /// Overloads the + operator to allow the int to be on the left-hand side.
        /// </summary>
        /// <param name="value">The int value to add.</param>
        /// <param name="range">The IntRange operand.</param>
        /// <returns>A new IntRange with the value added to min and max.</returns>
        public static IntRange operator +(int value, IntRange range)
        {
            // This calls the other overloaded operator to avoid repeating code.
            return range + value;
        }

        /// <summary>
        /// Overloads the + operator to combine two IntRanges. The mins are naively added to one another, as are the maxes.
        /// </summary>
        /// <param name="range1">The first IntRange operand.</param>
        /// <param name="range2">The second IntRange operand.</param>
        /// <returns>A new IntRange with the value added to min and max.</returns>
        public static IntRange operator +(IntRange range1, IntRange range2)
        {
            // This calls the other overloaded operator to avoid repeating code.
            return new IntRange(range1.min + range2.min, range1.max + range2.max);
        }
    }
}