namespace ReInvented.CrossSections.Models
{
    /// <summary>
    /// Stores and retrieves the properties of a leg of an angle section.
    /// </summary>
    public sealed class AngleLeg
    {
        #region Default Constructor

        public AngleLeg()
        {

        }

        #endregion

        #region Parameterized Constructor

        public AngleLeg(double length, double thickness)
        {
            Length = length;
            Tw = thickness;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Length of the angle leg.
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Thickness of the leg usually measured at a specific distance in case of a inclined inside face.
        /// </summary>
        public double Tw { get; set; }
        /// <summary>
        /// Slope of the inclined face with respect to the edge perpendicular to the straight face of the leg. Default value is 90°.
        /// </summary>
        public double Slope { get; set; } = 90.0;
        /// <summary>
        /// Radius of the toe of the leg.
        /// </summary>
        public double ToeRadius { get; set; } = 0.0;

        #endregion

        #region Read-only Properties

        /// <summary>
        /// Slope of the inclined face with reference to the straight face of leg.
        /// </summary>
        public double SlopeWithFace => Slope - 90.0;
        /// <summary>
        /// Geometry of toe of the leg of type <see cref="CurveTriangleGeometry"/>.
        /// </summary>
        public CurveTriangleGeometry Toe => new CurveTriangleGeometry(ToeRadius, SlopeWithFace);
        /// <summary>
        /// Geometry of the main triangle formed by the inclined face (intersecting at the toe curve and the root curve) of type<see cref="TriangleGeometry"/>.
        /// </summary>
        public TriangleGeometry MainTriangle { get; set; }

        #endregion

    }
}
