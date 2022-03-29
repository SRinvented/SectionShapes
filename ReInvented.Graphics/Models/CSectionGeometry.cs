using System;
using System.Collections.Generic;
using System.Windows.Media;

using ReInvented.Graphics.Interfaces;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    /// <summary>
    /// Stores and retrieves dimensional details of the an C/Channel cross section.
    /// Also, calculates the key cross section points to facilitate the generates of geometry. It shall be noted that, this class considers the thickness of flange measured at (B - Tw)/2.
    /// </summary>
    public sealed class CSectionGeometry : ISectionGeometry
    {
        #region Parameterized Constructor

        public CSectionGeometry(double overallDepth, double flangeWidth, double webThickness, double flangeThickness, double rRoot = 0.0, double rToe = 0.0, double flangeSlope = 90.0)
        {
            H = overallDepth;
            B = flangeWidth;
            Tw = webThickness;
            Tf = flangeThickness;
            RootRadius = rRoot;
            ToeRadius = rToe;
            FlangeSlope = flangeSlope;
        }

        #endregion

        #region Dimensions

        public double H { get; set; }

        public double B { get; set; }

        public double Tw { get; set; }

        public double Tf { get; set; }

        public double RootRadius { get; set; }

        public double ToeRadius { get; set; }

        public double FlangeSlope { get; set; }

        #endregion

        #region Read-only Properties

        public double FlangeSlopeWithHorizontal => FlangeSlope - 90;

        public CurveTriangleGeometry ToeGeometry => new CurveTriangleGeometry(ToeRadius, FlangeSlopeWithHorizontal);

        public CurveTriangleGeometry RootGeometry => new CurveTriangleGeometry(RootRadius, FlangeSlopeWithHorizontal);

        public TriangleGeometry FlangeTriangle { get; private set; }

        public List<IEnumerable<PointEx>> PointsCollection { get; private set; }

        #endregion

        #region Public Methods

        public void GeneratePoints()
        {

            #region Data Required for Points Calculation

            #region Flange Slope Triangle Properties

            double mainTriangleHorizontal = B - Tw - (RootRadius - RootGeometry.LargeTriangle.OppositeSide) - (ToeRadius - ToeGeometry.LargeTriangle.OppositeSide);
            double mainTriangleHypotenuse = mainTriangleHorizontal / Math.Cos(FlangeSlopeWithHorizontal.ToRadians());

            FlangeTriangle = new TriangleGeometry(mainTriangleHypotenuse, 90 - FlangeSlopeWithHorizontal);

            double apexToFlangeThickness = ((B - Tw) / 2) - (ToeRadius - ToeGeometry.LargeTriangle.OppositeSide);
            double heightAtFlangeThickness = FlangeTriangle.AdjacentSide * apexToFlangeThickness / FlangeTriangle.OppositeSide;


            #endregion

            /// Vertical distance to the root intersection with web from nearest flange face.
            double heightAtRootIntersection = Tf - heightAtFlangeThickness + FlangeTriangle.AdjacentSide + RootGeometry.LargeTriangle.AdjacentSide;

            #endregion

            #region Geometry Points

            List<PointEx> shapePoints = new List<PointEx>
            {
                new PointEx(0, 0, PathSegmentType.Line),
                new PointEx(B, 0, PathSegmentType.Line)
            };

            shapePoints.Add(new PointEx(B, Tf - heightAtFlangeThickness - ToeGeometry.LargeTriangle.AdjacentSide, PathSegmentType.Line));

            #region Toe - Top

            if (ToeRadius > 0)
            {
                shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.OppositeSide), Tf - heightAtFlangeThickness, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.OppositeSide) - FlangeTriangle.OppositeSide, Tf - heightAtFlangeThickness + FlangeTriangle.AdjacentSide, PathSegmentType.Line));

            #region Root - Top

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx(Tw, heightAtRootIntersection, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(Tw, H - heightAtRootIntersection, PathSegmentType.Line));

            #region Root - Bottom

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx(Tw + (RootRadius - RootGeometry.LargeTriangle.OppositeSide), H - heightAtRootIntersection + RootGeometry.LargeTriangle.AdjacentSide, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.OppositeSide), H - heightAtRootIntersection + RootGeometry.LargeTriangle.AdjacentSide + FlangeTriangle.AdjacentSide, PathSegmentType.Line));

            #region Toe - Bottom

            if (ToeRadius > 0)
            {
                shapePoints.Add(new PointEx(B, H - heightAtRootIntersection + RootGeometry.LargeTriangle.AdjacentSide + FlangeTriangle.AdjacentSide + ToeGeometry.LargeTriangle.AdjacentSide, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(B, H, PathSegmentType.Line));
            shapePoints.Add(new PointEx(0, H, PathSegmentType.Line));
            shapePoints.Add(new PointEx(0, H - Tf + heightAtFlangeThickness + ToeGeometry.LargeTriangle.AdjacentSide, PathSegmentType.Line));

            shapePoints.Add(new PointEx(0, 0, PathSegmentType.Line));

            #endregion

            PointsCollection = new List<IEnumerable<PointEx>>
            {
                shapePoints
            };
        }

        #endregion

    }
}
