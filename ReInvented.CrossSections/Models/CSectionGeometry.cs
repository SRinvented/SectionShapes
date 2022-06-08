using System;
using System.Collections.Generic;
using System.Windows.Media;

using ReInvented.CrossSections.Interfaces;
using ReInvented.SectionProfiles.Interfaces;
using ReInvented.SectionProfiles.Models;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.CrossSections.Models
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

        public CSectionGeometry(IRolledSection section)
        {
            InitializeProperties(section as RolledSectionCShape);
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

        public List<IEnumerable<ShapePoint>> PointsCollection { get; private set; }

        #endregion

        #region Public Methods

        public void GeneratePoints()
        {

            #region Data Required for Points Calculation

            #region Flange Slope Triangle Properties

            double mainTriangleHorizontal = B - Tw - (RootRadius - RootGeometry.LargeTriangle.Opposite) - (ToeRadius - ToeGeometry.LargeTriangle.Opposite);
            double mainTriangleHypotenuse = mainTriangleHorizontal / Math.Cos(FlangeSlopeWithHorizontal.ToRadians());

            FlangeTriangle = new TriangleGeometry(mainTriangleHypotenuse, 90 - FlangeSlopeWithHorizontal);

            double apexToFlangeThickness = ((B - Tw) / 2) - (ToeRadius - ToeGeometry.LargeTriangle.Opposite);
            double heightAtFlangeThickness = FlangeTriangle.Adjacent * apexToFlangeThickness / FlangeTriangle.Opposite;


            #endregion

            /// Vertical distance to the root intersection with web from nearest flange face.
            double heightAtRootIntersection = Tf - heightAtFlangeThickness + FlangeTriangle.Adjacent + RootGeometry.LargeTriangle.Adjacent;

            #endregion

            #region Geometry Points

            List<ShapePoint> shapePoints = new List<ShapePoint>
            {
                new ShapePoint(0, 0, PathSegmentType.Line),
                new ShapePoint(B, 0, PathSegmentType.Line)
            };

            shapePoints.Add(new ShapePoint(B, Tf - heightAtFlangeThickness - ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Line));

            #region Toe - Top

            if (ToeRadius > 0)
            {
                shapePoints.Add(new ShapePoint(B - (ToeRadius - ToeGeometry.LargeTriangle.Opposite), Tf - heightAtFlangeThickness, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new ShapePoint(B - (ToeRadius - ToeGeometry.LargeTriangle.Opposite) - FlangeTriangle.Opposite, Tf - heightAtFlangeThickness + FlangeTriangle.Adjacent, PathSegmentType.Line));

            #region Root - Top

            if (RootRadius > 0)
            {
                shapePoints.Add(new ShapePoint(Tw, heightAtRootIntersection, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new ShapePoint(Tw, H - heightAtRootIntersection, PathSegmentType.Line));

            #region Root - Bottom

            if (RootRadius > 0)
            {
                shapePoints.Add(new ShapePoint(Tw + (RootRadius - RootGeometry.LargeTriangle.Opposite), H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new ShapePoint(B - (ToeRadius - ToeGeometry.LargeTriangle.Opposite), H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent + FlangeTriangle.Adjacent, PathSegmentType.Line));

            #region Toe - Bottom

            if (ToeRadius > 0)
            {
                shapePoints.Add(new ShapePoint(B, H - heightAtRootIntersection + RootGeometry.LargeTriangle.Adjacent + FlangeTriangle.Adjacent + ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Arc)
                {
                    Radius = ToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new ShapePoint(B, H, PathSegmentType.Line));
            shapePoints.Add(new ShapePoint(0, H, PathSegmentType.Line));
            shapePoints.Add(new ShapePoint(0, H - Tf + heightAtFlangeThickness + ToeGeometry.LargeTriangle.Adjacent, PathSegmentType.Line));

            shapePoints.Add(new ShapePoint(0, 0, PathSegmentType.Line));

            #endregion

            PointsCollection = new List<IEnumerable<ShapePoint>>
            {
                shapePoints
            };
        }

        #endregion

        #region Private Helper Methods

        private void InitializeProperties(RolledSectionCShape channelSection)
        {
            H = channelSection.H;
            B = channelSection.Bf;
            Tw = channelSection.Tw;
            Tf = channelSection.Tf;
            RootRadius = channelSection.R1;
            ToeRadius = channelSection.R2;
            FlangeSlope = channelSection.Alpha;
        }

        #endregion

    }
}
