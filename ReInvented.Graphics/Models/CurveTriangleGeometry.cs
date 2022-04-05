using System;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.CrossSections.Models
{
    /// <summary>
    /// Used to calculate the properties triangles formed by inclined faces at a curve.
    /// This class considers only one inclined face intersecting on to the curve.
    /// </summary>
    public sealed class CurveTriangleGeometry
    {

        #region Parameterized Constructor

        public CurveTriangleGeometry(double radius, double angle)
        {
            Radius = radius;
            Angle = angle;
        }

        #endregion

        #region Public Properties

        public double Radius { get; set; }

        public double Angle { get; set; }

        #endregion

        #region Read-only Properties

        public TriangleGeometry SmallTriangle => new TriangleGeometry(LargeTriangle.Opposite / Math.Cos(Angle.ToRadians()), 90 - Angle);

        public TriangleGeometry LargeTriangle => new TriangleGeometry(Radius, Angle);

        #endregion

    }
}
