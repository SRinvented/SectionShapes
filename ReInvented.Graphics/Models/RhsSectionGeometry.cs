using System;
using System.Collections.Generic;
using System.Windows.Media;

using ReInvented.CrossSections.Interfaces;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.CrossSections.Models
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


        public List<IEnumerable<ShapePoint>> PointsCollection { get; private set; }

        public void GeneratePoints()
        {

            PointsCollection = new List<IEnumerable<ShapePoint>>
            {
                /// Points for outer boundary
                FormulateBoundaryPoints(0, 0, H, B, ROuter),
                /// Points for inner boundary
                FormulateBoundaryPoints(Tw, Tw, H - (2* Tw), B - (2*Tw), RInner)
            };
        }

        #region Private Functions

        private List<ShapePoint> FormulateBoundaryPoints(double xOffset, double yOffset, double height, double width, double curveRadius)
        {

            List<ShapePoint> boundaryPoints = new List<ShapePoint>();

            boundaryPoints.Add(new ShapePoint(xOffset + curveRadius, yOffset, PathSegmentType.Line));
            boundaryPoints.Add(new ShapePoint(xOffset + width - curveRadius, yOffset, PathSegmentType.Line));

            #region Curve - Right Top

            boundaryPoints.Add(new ShapePoint(xOffset + width, yOffset + curveRadius, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            boundaryPoints.Add(new ShapePoint(xOffset + width, yOffset + height - curveRadius, PathSegmentType.Line));

            #region Curve - Right Bottom

            boundaryPoints.Add(new ShapePoint(xOffset + width - curveRadius, yOffset + height, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            boundaryPoints.Add(new ShapePoint(xOffset + curveRadius, yOffset + height, PathSegmentType.Line));

            #region Curve - Left Bottom

            boundaryPoints.Add(new ShapePoint(xOffset, yOffset + height - curveRadius, PathSegmentType.Arc)
            {
                Radius = curveRadius,
                SweepDirection = SweepDirection.Clockwise
            });

            #endregion

            boundaryPoints.Add(new ShapePoint(xOffset, yOffset + curveRadius, PathSegmentType.Line));

            #region Curve - Left Top

            boundaryPoints.Add(new ShapePoint(xOffset + curveRadius, yOffset, PathSegmentType.Arc)
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
