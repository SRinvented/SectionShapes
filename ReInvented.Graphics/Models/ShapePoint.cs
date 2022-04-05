using System;
using System.Windows.Media;

namespace ReInvented.CrossSections.Models
{
    public sealed class ShapePoint
    {

        #region Default Constructor

        public ShapePoint()
        {

        }

        #endregion

        #region Parameterized Constructors

        public ShapePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public ShapePoint(double x, double y, PathSegmentType pathSegmentType) : this(x, y)
        {
            PathSegmentType = pathSegmentType;
        }

        #endregion

        #region Public Properties

        public double X { get; set; }

        public double Y { get; set; }

        public double Radius { get; set; }

        public PathSegmentType PathSegmentType { get; set; }

        public SweepDirection SweepDirection { get; set; }

        #endregion

        #region Read-only Properties

        public double AngleWithX => Math.Atan2(Y, X);

        #endregion

    }
}
