using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using ReInvented.Graphics.Interfaces;

using SRi.XamlUIThickenerApp.Shared;

namespace ReInvented.Graphics.Models
{
    /// <summary>
    /// Stores and retrieves dimensional details of the an L/Angle cross section.
    /// Also, calculates the key cross section points to facilitate the generates of geometry. It shall be noted that, this class considers the thickness of flange measured at (B - Tw)/2.
    /// </summary>
    public sealed class LSectionGeometry : ISectionGeometry
    {
        #region Parameterized Constructor

        public LSectionGeometry()
        {

        }

        public LSectionGeometry(double legLength, double legThickness)
            : this(legLength, legLength, legThickness, legThickness)
        {

        }

        public LSectionGeometry(double longLegLength, double shortLegLength, double longLegThickness, double shortLegThickness)
        {
            LongLegLength = longLegLength;
            ShortLegLength = shortLegLength;
            TwLong = longLegThickness;
            TwShort = shortLegThickness;
        }


        #endregion

        #region Dimensions

        public double LongLegLength { get; set; }

        public double ShortLegLength { get; set; }

        public double TwLong { get; set; }

        public double TwShort { get; set; }

        public double LongLegSlope { get; set; } = 90.0;

        public double ShortLegSlope { get; set; } = 90.0;


        public double RootRadius { get; set; }

        public double LongLegToeRadius { get; set; }

        public double ShortLegToeRadius { get; set; }


        ///public double LegSlope { get; set; }

        #endregion

        #region Read-only Properties

        public double LongLegSlopeWithLegFace => LongLegSlope - 90.0;

        public double ShortLegSlopeWithLegFace => ShortLegSlope - 90.0;

        public CurveTriangleGeometry LongLegToe => new CurveTriangleGeometry(LongLegToeRadius, LongLegSlopeWithLegFace);

        public CurveTriangleGeometry ShortLegToeGeometry => new CurveTriangleGeometry(ShortLegToeRadius, ShortLegSlopeWithLegFace);

        public CurveDualTriangleGeometry Root => new CurveDualTriangleGeometry(RootRadius, LongLegSlopeWithLegFace, ShortLegSlopeWithLegFace);

        public TriangleGeometry LongLegMainTriangle { get; private set; }

        public TriangleGeometry ShortLegMainTriangle { get; private set; }

        public List<IEnumerable<PointEx>> PointsCollection { get; private set; }

        ///        public AngleLeg LongLeg { get; set; }


        #endregion

        #region Public Methods

        public void GeneratePoints()
        {

            #region Sloped Faces Intersection Point Coordinates Calculation

            /// Indicates the horizontal distance to the point where the thickness on the leg is measured.
            double faceToThicknessLongLeg = (LongLegLength - TwShort) / 2;
            double faceToThicknessShortLeg = (ShortLegLength - TwLong) / 2;

            /// Distances to the intersection point of inclined faces measured from the point where thickness lines intersect.
            double intersectionAlongLongLeg = (faceToThicknessShortLeg - faceToThicknessLongLeg *
                                               Math.Tan(LongLegSlopeWithLegFace.ToRadians())) * Math.Tan(ShortLegSlopeWithLegFace.ToRadians()) /
                                               (1 - (Math.Tan(LongLegSlopeWithLegFace.ToRadians()) * Math.Tan(ShortLegSlopeWithLegFace.ToRadians())));

            double intersectionAlongShortLeg = (faceToThicknessLongLeg - intersectionAlongLongLeg) * Math.Tan(LongLegSlopeWithLegFace.ToRadians());

            /// Distances to the intersection point of inclined faces measured from the point where outer faces of the legs intersect.
            intersectionAlongLongLeg = TwShort + intersectionAlongLongLeg;
            intersectionAlongShortLeg = TwLong + intersectionAlongShortLeg;

            /// Properties of the root curve
            double includedAngleAtCenter = 90 - LongLegSlopeWithLegFace - ShortLegSlopeWithLegFace;
            double chordLength = 2 * RootRadius * Math.Sin(includedAngleAtCenter.ToRadians() / 2);
            double includedAngleAtIntersection = 90 + LongLegSlopeWithLegFace + ShortLegSlopeWithLegFace;
            double includedAngleAtCurvePoints = (180 - includedAngleAtIntersection) / 2;

            /// Distance to the points where curve intersects the inclined faces measured from the point where inclined faces intersect with each other.
            double rootIntersectionToCurvePoints = chordLength / 2 / Math.Cos(includedAngleAtCurvePoints.ToRadians());

            /// Length of the inclined face on respective leg measured from the intersection point to the point where thickness is measured on that leg.
            double sideL = (TwShort + faceToThicknessLongLeg - intersectionAlongLongLeg) / Math.Cos(LongLegSlopeWithLegFace.ToRadians());
            double sideS = (TwLong + faceToThicknessShortLeg - intersectionAlongShortLeg) / Math.Cos(ShortLegSlopeWithLegFace.ToRadians());

            /// Length of the inclined face on the repective leg measured from the root curve intersection point to the point where thickness is measured on that leg.
            double alignedLegLengthToCurvePointOnLongLeg = sideL - rootIntersectionToCurvePoints;
            double alignedLegLengthToCurvePointOnShortLeg = sideS - rootIntersectionToCurvePoints;

            /// Horizontal distance from the point where thickness is measured on the respective leg to the point where inclined face intersects the toe curve.
            double distanceToThicknessFromToeLongLeg = faceToThicknessLongLeg - (LongLegToeRadius - LongLegToe.LargeTriangle.Opposite);
            double distanceToThicknessFromToeShortLeg = faceToThicknessShortLeg - (ShortLegToeRadius - ShortLegToeGeometry.LargeTriangle.Opposite);

            /// Total length of the inclined face of respective leg.
            //double longLegTriangleHypotenuse = alignedLegLengthToCurvePointOnLongLeg + (faceToThicknessLongLeg - (LongLegToeRadius - LongLegToeGeometry.LargeTriangle.Opposite)) / Math.Cos(LongLegSlopeWithLegFace.ToRadians());
            //double shortLegTriangleHypotenuse = alignedLegLengthToCurvePointOnShortLeg + (faceToThicknessShortLeg - (ShortLegToeRadius - ShortLegToeGeometry.LargeTriangle.Opposite)) / Math.Cos(ShortLegSlopeWithLegFace.ToRadians());


            double longLegTriangleHypotenuse = alignedLegLengthToCurvePointOnLongLeg + (distanceToThicknessFromToeLongLeg / Math.Cos(LongLegSlopeWithLegFace.ToRadians()));
            double shortLegTriangleHypotenuse = alignedLegLengthToCurvePointOnShortLeg + (distanceToThicknessFromToeShortLeg / Math.Cos(ShortLegSlopeWithLegFace.ToRadians()));


            LongLegMainTriangle = new TriangleGeometry(longLegTriangleHypotenuse, LongLegSlopeWithLegFace);
            ShortLegMainTriangle = new TriangleGeometry(shortLegTriangleHypotenuse, ShortLegSlopeWithLegFace);


            double heightAtThicknessLongLeg = LongLegMainTriangle.Opposite / LongLegMainTriangle.Adjacent * distanceToThicknessFromToeLongLeg;
            double heightAtThicknessShortLeg = ShortLegMainTriangle.Opposite / ShortLegMainTriangle.Adjacent * distanceToThicknessFromToeShortLeg;

            double thicknessAtToeLongLeg = TwLong - heightAtThicknessLongLeg;
            double thicknessAtToeShortLeg = TwShort - heightAtThicknessShortLeg;

            #endregion

            ///#region Data Required for Points Calculation

            ///#region Leg Slope Main Triangles Properties

            ///double mainTriangleHorizontal = (LongLegLength - TwShort) / 2 - (LongLegToeRadius - LongLegToeGeometry.LargeTriangle.Opposite);

            List<PointEx> shapePoints = new List<PointEx> { new PointEx(0, 0, PathSegmentType.Line) };

            shapePoints.Add(new PointEx(thicknessAtToeLongLeg, 0, PathSegmentType.Line));

            #region Toe - Long Leg

            if (LongLegToeRadius > 0)
            {
                shapePoints.Add(new PointEx()
                {
                    X = thicknessAtToeLongLeg + LongLegToe.LargeTriangle.Adjacent,
                    Y = LongLegToeRadius - LongLegToe.LargeTriangle.Opposite,
                    PathSegmentType = PathSegmentType.Arc,
                    Radius = LongLegToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx()
            {
                X = thicknessAtToeLongLeg + LongLegToe.LargeTriangle.Adjacent + LongLegMainTriangle.Opposite,
                Y = LongLegToeRadius - LongLegToe.LargeTriangle.Opposite + LongLegMainTriangle.Adjacent,
                PathSegmentType = PathSegmentType.Line
            });

            #region Root

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx()
                {
                    X = thicknessAtToeLongLeg + LongLegToe.LargeTriangle.Adjacent + LongLegMainTriangle.Opposite + Root.LongSideLargeTriangle.Adjacent + Root.ShortSideSmallTriangle.Adjacent,
                    Y = LongLegToeRadius - LongLegToe.LargeTriangle.Opposite + LongLegMainTriangle.Adjacent - Root.LongSideLargeTriangle.Opposite + RootRadius - Root.ShortSideSmallTriangle.Opposite,
                    PathSegmentType = PathSegmentType.Arc,
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx()
            {
                X = shapePoints[shapePoints.Count - 1].X + ShortLegMainTriangle.Adjacent,
                Y = shapePoints[shapePoints.Count - 1].Y + ShortLegMainTriangle.Opposite,
                PathSegmentType = PathSegmentType.Line
            });


            //#region Data Required for Points Calculation

            //#region Long Leg Slope Triangle Properties

            //double triSeg1 = (H - Tw) / 2 -

            //double longLegTriangleHorizontal = H - Tw - (RootRadius - RootGeometry.LargeTriangle.Horizontal) - (LongLegToeRadius - ToeGeometry.LargeTriangle.Horizontal);
            //double longLegTriangleHypotenuse = longLegTriangleHorizontal / Math.Cos(LegSlopeWithHorizontal.ToRadians());

            //LongLegTriangle = new TriangleGeometry(longLegTriangleHypotenuse, 90 - LegSlopeWithHorizontal);

            //double apexToLongLegThickness = ((H - Tw) / 2) - (LongLegToeRadius - ToeGeometry.LargeTriangle.Horizontal);
            //double heightAtLongLegThickness = LongLegTriangle.Vertical * apexToLongLegThickness / LongLegTriangle.Horizontal;


            //#endregion

            //#region Short Leg Slope Triangle Properties

            //double shortLegTriangleHorizontal = B - Tw - (RootRadius - RootGeometry.LargeTriangle.Horizontal) - (LongLegToeRadius - ToeGeometry.LargeTriangle.Horizontal);
            //double shortLegTriangleHypotenuse = shortLegTriangleHorizontal / Math.Cos(LegSlopeWithHorizontal.ToRadians());

            //ShortLegTriangle = new TriangleGeometry(shortLegTriangleHypotenuse, 90 - LegSlopeWithHorizontal);

            //double apexToShortLegThickness = ((B - Tw) / 2) - (LongLegToeRadius - ToeGeometry.LargeTriangle.Horizontal);
            //double heightAtShortLegThickness = ShortLegTriangle.Vertical * apexToShortLegThickness / ShortLegTriangle.Horizontal;


            //#endregion


            ///// Vertical distance to the root intersection with short from long leg edge parallel to leg.
            //double heightAtRootIntersectionWithShortLeg = Tw - heightAtLongLegThickness + LongLegTriangle.Vertical + RootGeometry.LargeTriangle.Vertical;

            //double heightAtRootIntersectionWithLongLeg = Tw - heightAtShortLegThickness + ShortLegTriangle.Vertical + RootGeometry.LargeTriangle.Vertical;

            //#endregion

            //#region Geometry Points

            //List<PointEx> shapePoints = new List<PointEx>
            //{
            //    new PointEx(0, 0, PathSegmentType.Line),
            //    ///new PointEx(0, H, PathSegmentType.Line)
            //};

            //shapePoints.Add(new PointEx(Tw - heightAtLongLegThickness - ToeGeometry.LargeTriangle.Vertical, 0, PathSegmentType.Line));

            //#region Toe - Top

            //if (LongLegToeRadius > 0)
            //{
            //    shapePoints.Add(new PointEx(Tw - heightAtLongLegThickness, LongLegToeRadius - ToeGeometry.LargeTriangle.Horizontal, PathSegmentType.Arc)
            //    {
            //        Radius = LongLegToeRadius,
            //        SweepDirection = SweepDirection.Clockwise
            //    });
            //}

            //#endregion

            //shapePoints.Add(new PointEx(Tw - heightAtLongLegThickness + LongLegTriangle.Vertical, LongLegToeRadius - ToeGeometry.LargeTriangle.Horizontal + LongLegTriangle.Horizontal, PathSegmentType.Line));

            //#region Root

            //if (RootRadius > 0)
            //{
            //    ///shapePoints.Add(new PointEx(heightAtRootIntersectionWithShortLeg, ToeRadius - ToeGeometry.LargeTriangle.Horizontal + LongLegTriangle.Horizontal + RootGeometry.LargeTriangle.Horizontal, PathSegmentType.Arc)
            //    shapePoints.Add(new PointEx(heightAtRootIntersectionWithShortLeg, H - heightAtRootIntersectionWithLongLeg + RootGeometry.LargeTriangle.Horizontal, PathSegmentType.Arc)
            //    {
            //        Radius = RootRadius,
            //        SweepDirection = SweepDirection.Counterclockwise
            //    });
            //}

            //#endregion

            ////shapePoints.Add(new PointEx(B - heightAtRootIntersectionWithShortLeg, Tw, PathSegmentType.Line));

            ////#region Root - Bottom

            ////if (RootRadius > 0)
            ////{
            ////    shapePoints.Add(new PointEx(Tw + (RootRadius - RootGeometry.LargeTriangle.Horizontal), H - heightAtRootIntersectionWithShortLeg + RootGeometry.LargeTriangle.Vertical, PathSegmentType.Arc)
            ////    {
            ////        Radius = RootRadius,
            ////        SweepDirection = SweepDirection.Counterclockwise
            ////    });
            ////}

            ////#endregion

            ////shapePoints.Add(new PointEx(B - (ToeRadius - ToeGeometry.LargeTriangle.Horizontal), H - heightAtRootIntersectionWithShortLeg + RootGeometry.LargeTriangle.Vertical + LongLegTriangle.Vertical, PathSegmentType.Line));

            ////#region Toe - Bottom

            ////if (ToeRadius > 0)
            ////{
            ////    shapePoints.Add(new PointEx(B, H - heightAtRootIntersectionWithShortLeg + RootGeometry.LargeTriangle.Vertical + LongLegTriangle.Vertical + ToeGeometry.LargeTriangle.Vertical, PathSegmentType.Arc)
            ////    {
            ////        Radius = ToeRadius,
            ////        SweepDirection = SweepDirection.Clockwise
            ////    });
            ////}

            ////#endregion

            ////shapePoints.Add(new PointEx(B, H, PathSegmentType.Line));
            ////shapePoints.Add(new PointEx(0, H, PathSegmentType.Line));
            ////shapePoints.Add(new PointEx(0, H - Tw + heightAtLongLegThickness + ToeGeometry.LargeTriangle.Vertical, PathSegmentType.Line));

            ////shapePoints.Add(new PointEx(0, 0, PathSegmentType.Line));

            //#endregion

            PointsCollection = new List<IEnumerable<PointEx>>()
            {
                shapePoints
            };

        }

        #endregion

    }
}
