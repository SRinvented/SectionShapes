﻿namespace ReInvented.Graphics.Models
{
    public class AngleLeg
    {
        public AngleLeg()
        {

        }

        public AngleLeg(double length, double thickness, double slope = 0.0, double toeRadius = 0.0)
        {
            Length = length;
            Tw = thickness;
            Slope = slope;
            ToeRadius = toeRadius;
        }

        public double Length { get; set; }

        public double Tw { get; set; }

        public double Slope { get; set; } = 90.0;

        public double ToeRadius { get; set; } = 0.0;

        public double SlopeWithHorizontal => Slope - 90.0;

        public CurveTriangleGeometry ToeGeometry => new CurveTriangleGeometry(ToeRadius, SlopeWithHorizontal);

        public TriangleGeometry MainTriangle { get; set; }

    }
}