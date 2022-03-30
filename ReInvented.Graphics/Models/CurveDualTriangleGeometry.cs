using System;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    /// <summary>
    /// Used to calculate the properties triangles formed by inclined faces at a curve.
    /// This class considers two inclined faces intersecting on to the curve.
    /// </summary>
    public sealed class CurveDualTriangleGeometry
    {
        #region Parameterized Constructor

        public CurveDualTriangleGeometry(double radius, double longSideAngle, double shortSideAngle)
        {
            Radius = radius;
            LongSideAngle = longSideAngle;
            ShortSideAngle = shortSideAngle;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Radius of the curve.
        /// </summary>
        public double Radius { get; set; }
        /// <summary>
        /// Slope of the inclined face of long leg with reference to the straight face of the long leg.
        /// </summary>
        public double LongSideAngle { get; set; }
        /// <summary>
        /// Slope of the inclined face of short leg with reference to the straight face of the short leg.
        /// </summary>
        public double ShortSideAngle { get; set; }

        #endregion

        #region Read-only Properties

        public TriangleGeometry LongSideSmallTriangle => new TriangleGeometry(LongSideLargeTriangle.Opposite / Math.Cos(LongSideAngle.ToRadians()), LongSideAngle);

        public TriangleGeometry LongSideLargeTriangle => new TriangleGeometry(Radius, LongSideAngle);

        public TriangleGeometry ShortSideSmallTriangle => new TriangleGeometry(ShortSideLargeTriangle.Opposite / Math.Cos(ShortSideAngle.ToRadians()), ShortSideAngle);

        public TriangleGeometry ShortSideLargeTriangle => new TriangleGeometry(Radius, ShortSideAngle);

        #endregion

    }
}
