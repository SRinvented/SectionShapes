namespace ReInvented.Graphics.Models
{
    public sealed class AngleLeg
    {
        public AngleLeg()
        {

        }

        public AngleLeg(double length, double thickness)
        {
            Length = length;
            Tw = thickness;
        }

        public double Length { get; set; }

        public double Tw { get; set; }

        public double Slope { get; set; } = 90.0;

        public double ToeRadius { get; set; } = 0.0;

        public double SlopeWithFace => Slope - 90.0;

        public CurveTriangleGeometry Toe => new CurveTriangleGeometry(ToeRadius, SlopeWithFace);

        public TriangleGeometry MainTriangle { get; set; }

    }
}
