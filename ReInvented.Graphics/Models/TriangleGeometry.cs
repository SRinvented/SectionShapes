using System;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    public class TriangleGeometry
    {

        public TriangleGeometry(double hypotenuse, double flangeSlopeWithHorizontal)
        {
            Hypotenuse = hypotenuse;
            Angle = flangeSlopeWithHorizontal;
        }

        public double Hypotenuse { get; set; }

        public double Angle { get; set; }

        public double OppositeSide => Hypotenuse * Math.Sin(Angle.ToRadians());

        public double AdjacentSide => Hypotenuse * Math.Cos(Angle.ToRadians());


    }
}
