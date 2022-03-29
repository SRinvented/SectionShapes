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

        public double Opposite => Hypotenuse * Math.Sin(Angle.ToRadians());

        public double Adjacent => Hypotenuse * Math.Cos(Angle.ToRadians());


    }
}
