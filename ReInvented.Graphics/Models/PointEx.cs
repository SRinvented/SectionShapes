using System;
using System.Windows.Media;

namespace ReInvented.Graphics.Models
{
    public class PointEx
    {

        public PointEx()
        {

        }

        public PointEx(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointEx(double x, double y, PathSegmentType pathSegmentType) : this(x, y)
        {
            PathSegmentType = pathSegmentType;
        }

        ///public Point Point { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public PathSegmentType PathSegmentType { get; set; }

        public double Radius { get; set; }

        public SweepDirection SweepDirection { get; set; }

        public double AngleWithX => Math.Atan2(Y, X);

    }
}
