using System;
using System.Collections.Generic;
using System.Windows.Media;

using ReInvented.Graphics.Interfaces;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    public sealed class RhsSectionGeometry : ISectionGeometry
    {
        public RhsSectionGeometry(double overallDepth, double overallWidth, double wallThicknes)
            : this(overallDepth, overallWidth, wallThicknes, 2 * wallThicknes, 2 * wallThicknes)
        {

        }

        public RhsSectionGeometry(double overallDepth, double overallWidth, double wallThickness, double outerRadius = 0.0, double innerRadius = 0.0)
        {
            H = overallDepth;
            B = overallWidth;
            Tw = wallThickness;
            ROuter = outerRadius;
            RInner = innerRadius;
        }


        public double H { get; set; }

        public double B { get; set; }

        public double Tw { get; set; }

        public double ROuter { get; set; }

        public double RInner { get; set; }


        public List<IEnumerable<PointEx>> PointsCollection { get; private set; }

        public void GeneratePoints()
        {

            PointsCollection = new List<IEnumerable<PointEx>>
            {
                /// Points for outer boundary
                FormulateBoundaryPoints(0, 0, H, B, ROuter),
                /// Points for inner boundary
                FormulateBoundaryPoints(Tw, Tw, H - (2* Tw), B - (2*Tw), RInner)
            };
        }

        #region Private Functions

        private List<PointEx> FormulateBoundaryPoints(double xOffset, double yOffset, double height, double width, double curveRadius)
        {

            List<PointEx> boundaryPoints = new List<PointEx>();

            boundaryPoints.Add(new PointEx(xOffset + curveRadius, yOffset, PathSegmentType.Line));
            boundaryPoints.Add(new PointEx(xOffset + width - curveRadius, yOffset, PathSegmentType.Line));

            #region Curve - Right Top

            boundaryPoints.Add(new PointEx(xOffset + width, yOffset + curveRadius, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            boundaryPoints.Add(new PointEx(xOffset + width, yOffset + height - curveRadius, PathSegmentType.Line));

            #region Curve - Right Bottom

            boundaryPoints.Add(new PointEx(xOffset + width - curveRadius, yOffset + height, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            boundaryPoints.Add(new PointEx(xOffset + curveRadius, yOffset + height, PathSegmentType.Line));

            #region Curve - Left Bottom

            boundaryPoints.Add(new PointEx(xOffset, yOffset + height - curveRadius, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            boundaryPoints.Add(new PointEx(xOffset, yOffset + curveRadius, PathSegmentType.Line));

            #region Curve - Left Top

            boundaryPoints.Add(new PointEx(xOffset + curveRadius, yOffset, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            return boundaryPoints;

        }

        #endregion

    }
}
