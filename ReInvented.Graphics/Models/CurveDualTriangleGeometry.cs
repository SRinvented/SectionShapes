using System;
using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    public sealed class CurveDualTriangleGeometry
    {
        public CurveDualTriangleGeometry(double radius, double longSideAngle, double shortSideAngle)
        {
            Radius = radius;
            LongSideAngle = longSideAngle;
            ShortSideAngle = shortSideAngle;
        }

        public double Radius { get; set; }

        public double LongSideAngle { get; set; }

        public double ShortSideAngle { get; set; }


        public TriangleGeometry LongSideSmallTriangle => new TriangleGeometry(LongSideLargeTriangle.Opposite / Math.Cos(LongSideAngle.ToRadians()), LongSideAngle);

        public TriangleGeometry LongSideLargeTriangle => new TriangleGeometry(Radius, LongSideAngle);

        public TriangleGeometry ShortSideSmallTriangle => new TriangleGeometry(ShortSideLargeTriangle.Opposite / Math.Cos(ShortSideAngle.ToRadians()), ShortSideAngle);

        public TriangleGeometry ShortSideLargeTriangle => new TriangleGeometry(Radius, ShortSideAngle);


    }
}
