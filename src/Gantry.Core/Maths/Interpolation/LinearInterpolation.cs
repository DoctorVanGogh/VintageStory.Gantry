﻿namespace Gantry.Core.Maths.Interpolation
{
    /// <summary>
    ///     Interpolation between two points, in a straight line.
    /// </summary>
    /// <seealso cref="InterpolationBase" />
    public class LinearInterpolation : InterpolationBase
    {
        /// <summary>
        ///     Initialises a new instance of the <see cref="LinearInterpolation"/> class.
        /// </summary>
        /// <param name="times">How changes in speed and direction, as we traverse the path.</param>
        /// <param name="points">The points that dictate the angle of the curves within a path.</param>
        public LinearInterpolation(double[] times, double[] points) : base(times, points)
        {
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="LinearInterpolation"/> class.
        /// </summary>
        /// <param name="points">The points that dictate the angle of the curves within a path.</param>
        public LinearInterpolation(params double[] points) : base(points)
        {
        }

        /// <summary>
        ///     Calculates the value at a specific point between two nodes.
        /// </summary>
        /// <param name="mu">The fractional point between with two nodes.</param>
        /// <param name="pointIndex">The start node index.</param>
        /// <param name="pointIndexNext">The end node index.</param>
        /// <returns>
        ///     A <see cref="double" /> value that represents a specific point along a curve between two nodes of a path.
        /// </returns>
        public override double ValueAt(double mu, int pointIndex, int pointIndexNext)
        {
            return GetValue(pointIndexNext) - GetValue(pointIndex) * mu + GetValue(pointIndex);
        }
    }
}