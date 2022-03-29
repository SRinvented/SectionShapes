using System;
using System.Collections.Generic;
using System.Windows.Media;

using ReInvented.Graphics.Interfaces;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    /// <summary>
    /// Stores and retrieves dimensional details of the an I/H shaped cross section.
    /// Also, calculates the key cross section points to facilitate the generates of geometry. It shall be noted that, this class considers the thickness of flange measured at (B - Tw)/4.
    /// </summary>
    public sealed class HSectionGeometry : ISectionGeometry
    {
        #region Parameterized Constructor

        public HSectionGeometry(double overallDepth, double flangeWidth, double webThickness, double flangeThickness, double rRoot = 0.0, double rToe = 0.0, double flangeSlope = 90.0)
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

            double mainTriangleHorizontal = (B / 2) - (Tw / 2) - (RootRadius - RootGeometry.LargeTriangle.Opposite) - (ToeRadius - ToeGeometry.LargeTriangle.Opposite);
            double mainTriangleHypotenuse = mainTriangleHorizontal / Math.Cos(FlangeSlopeWithHorizontal.ToRadians());

            FlangeTriangle = new TriangleGeometry(mainTriangleHypotenuse, 90 - FlangeSlopeWithHorizontal);

            double apexToFlangeThickness = ((B - Tw) / 4) - (ToeRadius - ToeGeometry.LargeTriangle.Opposite);
            double heightAtFlangeThickness = FlangeTriangle.Adjacent * apexToFlangeThickness / FlangeTriangle.Opposite;


            #endregion

            /// Vertical distance to the root intersection with web from nearest flange face.
            double heightAtRootIntersection = Tf - heightAtFlangeThickness + FlangeTriangle.Adjacent + RootGeometry.LargeTriangle.Adjacent;

            #endregion

            #region Geometry Points

            List<PointEx> shapePoints = new List<PointEx>
            {
                new PointEx(0, 0, PathSegmentType.Line),
                new PointEx(B, 0, PathSegmentType.Line)
            };

            shapePoints.Add(new PointEx(B, Tf - heightAtFlangeThickness - ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Line));

            #region Toe - Top Right

            if (ToeRadius > 0)
            {
                shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.Opposite), Tf - heightAtFlangeThickness, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.Opposite) - FlangeTriangle.Opposite, Tf - heightAtFlangeThickness + FlangeTriangle.Adjacent, PathSegmentType.Line));

            #region Root - Top Right

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx((B / 2) + (Tw / 2), heightAtRootIntersection, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx((B / 2) + (Tw / 2), H - heightAtRootIntersection, PathSegmentType.Line));

            #region Root - Bottom Right

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx((B / 2) + (Tw / 2) + (RootRadius - RootGeometry.LargeTriangle.Opposite), H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.Opposite), H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent + FlangeTriangle.Adjacent, PathSegmentType.Line));

            #region Toe - Bottom Right

            if (ToeRadius > 0)
            {
                shapePoints.Add(new PointEx(B, H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent + FlangeTriangle.Adjacent + ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(B, H, PathSegmentType.Line));
            shapePoints.Add(new PointEx(0, H, PathSegmentType.Line));
            shapePoints.Add(new PointEx(0, H - Tf + heightAtFlangeThickness + ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Line));

            #region Toe - Bottom Left

            if (ToeRadius > 0)
            {
                shapePoints.Add(new PointEx(ToeRadius - ToeGeometry.LargeTriangle.Opposite, H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent + FlangeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx(ToeRadius - ToeGeometry.LargeTriangle.Opposite + FlangeTriangle.Opposite, H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent, PathSegmentType.Line));

            #region Root - Bottom Left

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx((B / 2) - (Tw / 2), H - heightAtRootIntersection, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx((B / 2) - (Tw / 2), heightAtRootIntersection, PathSegmentType.Line));

            #region Root - Top Left

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx((B / 2) - (Tw / 2) - (RootRadius - RootGeometry.LargeTriangle.Opposite), heightAtRootIntersection - RootGeometry.LargeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx()
            {
                X = (B / 2) - (Tw / 2) - (RootRadius - RootGeometry.LargeTriangle.Opposite) - FlangeTriangle.Opposite,
                Y = heightAtRootIntersection - RootGeometry.LargeTriangle.Adjacent - FlangeTriangle.Adjacent,
                PathSegmentType = PathSegmentType.Line
            });

            #region Toe - Top Left

            if (ToeRadius > 0)
            {
                shapePoints.Add(new PointEx(0, heightAtRootIntersection - RootGeometry.LargeTriangle.Adjacent - FlangeTriangle.Adjacent - ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

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
