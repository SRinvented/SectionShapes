using System;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    public sealed class CurveTriangleGeometry
    {
        public CurveTriangleGeometry(double radius, double angle)
        {
            Radius = radius;
            Angle = angle;
        }

        public double Radius { get; set; }

        public double Angle { get; set; }

        public TriangleGeometry SmallTriangle => new TriangleGeometry(LargeTriangle.Opposite / Math.Cos(Angle.ToRadians()), 90 - Angle);

        public TriangleGeometry LargeTriangle => new TriangleGeometry(Radius, Angle);


    }
}
