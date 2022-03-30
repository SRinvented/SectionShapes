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
            TwLongLeg = longLegThickness;
            TwShortLeg = shortLegThickness;
        }


        #endregion

        #region Dimensions

        public double LongLegLength { get; set; }

        public double ShortLegLength { get; set; }

        public double TwLongLeg { get; set; }

        public double TwShortLeg { get; set; }

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

        public CurveTriangleGeometry ShortLegToe => new CurveTriangleGeometry(ShortLegToeRadius, ShortLegSlopeWithLegFace);

        public CurveDualTriangleGeometry Root => new CurveDualTriangleGeometry(RootRadius, LongLegSlopeWithLegFace, ShortLegSlopeWithLegFace);

        public TriangleGeometry LongLegMainTriangle { get; private set; }

        public TriangleGeometry ShortLegMainTriangle { get; private set; }

        public List<IEnumerable<PointEx>> PointsCollection { get; private set; }

        ///public AngleLeg LongLeg { get; set; }


        #endregion

        #region Public Methods

        public void GeneratePoints()
        {

            #region Sloped Faces Intersection Point Coordinates Calculation

            /// Indicates the horizontal distance to the point where the thickness on the leg is measured.
            double faceToThicknessLongLeg = (LongLegLength - TwShortLeg) / 2;
            double faceToThicknessShortLeg = (ShortLegLength - TwLongLeg) / 2;

            /// Distances to the intersection point of inclined faces measured from the point where thickness lines intersect.
            double intersectionAlongLongLeg = (faceToThicknessShortLeg - faceToThicknessLongLeg *
                                               Math.Tan(LongLegSlopeWithLegFace.ToRadians())) * Math.Tan(ShortLegSlopeWithLegFace.ToRadians()) /
                                               (1 - (Math.Tan(LongLegSlopeWithLegFace.ToRadians()) * Math.Tan(ShortLegSlopeWithLegFace.ToRadians())));

            double intersectionAlongShortLeg = (faceToThicknessLongLeg - intersectionAlongLongLeg) * Math.Tan(LongLegSlopeWithLegFace.ToRadians());

            /// Distances to the intersection point of inclined faces measured from the point where outer faces of the legs intersect.
            intersectionAlongLongLeg = TwShortLeg + intersectionAlongLongLeg;
            intersectionAlongShortLeg = TwLongLeg + intersectionAlongShortLeg;

            /// Properties of the root curve
            double includedAngleAtCenter = 90 - LongLegSlopeWithLegFace - ShortLegSlopeWithLegFace;
            double chordLength = 2 * RootRadius * Math.Sin(includedAngleAtCenter.ToRadians() / 2);
            double includedAngleAtIntersection = 90 + LongLegSlopeWithLegFace + ShortLegSlopeWithLegFace;
            double includedAngleAtCurvePoints = (180 - includedAngleAtIntersection) / 2;

            /// Distance to the points where curve intersects the inclined faces measured from the point where inclined faces intersect with each other.
            double rootIntersectionToCurvePoints = chordLength / 2 / Math.Cos(includedAngleAtCurvePoints.ToRadians());

            /// Length of the inclined face on respective leg measured from the intersection point to the point where thickness is measured on that leg.
            double sideL = (TwShortLeg + faceToThicknessLongLeg - intersectionAlongLongLeg) / Math.Cos(LongLegSlopeWithLegFace.ToRadians());
            double sideS = (TwLongLeg + faceToThicknessShortLeg - intersectionAlongShortLeg) / Math.Cos(ShortLegSlopeWithLegFace.ToRadians());

            /// Length of the inclined face on the repective leg measured from the root curve intersection point to the point where thickness is measured on that leg.
            double alignedLegLengthToCurvePointOnLongLeg = sideL - rootIntersectionToCurvePoints;
            double alignedLegLengthToCurvePointOnShortLeg = sideS - rootIntersectionToCurvePoints;

            /// Horizontal distance from the point where thickness is measured on the respective leg to the point where inclined face intersects the toe curve.
            double distanceToThicknessFromToeLongLeg = faceToThicknessLongLeg - (LongLegToeRadius - LongLegToe.LargeTriangle.Opposite);
            double distanceToThicknessFromToeShortLeg = faceToThicknessShortLeg - (ShortLegToeRadius - ShortLegToe.LargeTriangle.Opposite);

            /// Total length of the inclined face of respective leg.
            //double longLegTriangleHypotenuse = alignedLegLengthToCurvePointOnLongLeg + (faceToThicknessLongLeg - (LongLegToeRadius - LongLegToeGeometry.LargeTriangle.Opposite)) / Math.Cos(LongLegSlopeWithLegFace.ToRadians());
            //double shortLegTriangleHypotenuse = alignedLegLengthToCurvePointOnShortLeg + (faceToThicknessShortLeg - (ShortLegToeRadius - ShortLegToeGeometry.LargeTriangle.Opposite)) / Math.Cos(ShortLegSlopeWithLegFace.ToRadians());


            double longLegTriangleHypotenuse = alignedLegLengthToCurvePointOnLongLeg + (distanceToThicknessFromToeLongLeg / Math.Cos(LongLegSlopeWithLegFace.ToRadians()));
            double shortLegTriangleHypotenuse = alignedLegLengthToCurvePointOnShortLeg + (distanceToThicknessFromToeShortLeg / Math.Cos(ShortLegSlopeWithLegFace.ToRadians()));


            LongLegMainTriangle = new TriangleGeometry(longLegTriangleHypotenuse, LongLegSlopeWithLegFace);
            ShortLegMainTriangle = new TriangleGeometry(shortLegTriangleHypotenuse, ShortLegSlopeWithLegFace);


            double heightAtThicknessLongLeg = LongLegMainTriangle.Opposite / LongLegMainTriangle.Adjacent * distanceToThicknessFromToeLongLeg;
            double heightAtThicknessShortLeg = ShortLegMainTriangle.Opposite / ShortLegMainTriangle.Adjacent * distanceToThicknessFromToeShortLeg;

            double thicknessAtToeLongLeg = TwLongLeg - heightAtThicknessLongLeg - LongLegToe.LargeTriangle.Adjacent;
            double thicknessAtToeShortLeg = TwShortLeg - heightAtThicknessShortLeg - ShortLegToe.LargeTriangle.Adjacent;

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

            #region Inclined Face - Long Leg

            shapePoints.Add(new PointEx()
            {
                X = shapePoints[shapePoints.Count - 1].X + LongLegMainTriangle.Opposite,
                Y = shapePoints[shapePoints.Count - 1].Y + LongLegMainTriangle.Adjacent,
                PathSegmentType = PathSegmentType.Line
            });

            #endregion

            #region Root

            if (RootRadius > 0)
            {
                shapePoints.Add(new PointEx()
                {
                    X = shapePoints[shapePoints.Count - 1].X + Root.LongSideLargeTriangle.Adjacent - Root.ShortSideSmallTriangle.Adjacent,
                    Y = shapePoints[shapePoints.Count - 1].Y - Root.LongSideLargeTriangle.Opposite + Root.ShortSideLargeTriangle.Adjacent,
                    PathSegmentType = PathSegmentType.Arc,
                    Radius = RootRadius,
                    SweepDirection = SweepDirection.Counterclockwise
                });
            }

            #endregion

            #region Inclined Face - Short Leg

            shapePoints.Add(new PointEx()
            {
                X = shapePoints[shapePoints.Count - 1].X + ShortLegMainTriangle.Adjacent,
                Y = shapePoints[shapePoints.Count - 1].Y + ShortLegMainTriangle.Opposite,
                PathSegmentType = PathSegmentType.Line
            });

            #endregion

            #region Toe - Short Leg

            if (ShortLegToeRadius > 0)
            {
                shapePoints.Add(new PointEx()
                {
                    X = shapePoints[shapePoints.Count - 1].X + ShortLegToeRadius - ShortLegToe.LargeTriangle.Opposite,
                    Y = shapePoints[shapePoints.Count - 1].Y + ShortLegToeRadius - ShortLegToe.SmallTriangle.Opposite,
                    PathSegmentType = PathSegmentType.Arc,
                    Radius = ShortLegToeRadius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new PointEx()
            {
                X = shapePoints[shapePoints.Count - 1].X,
                Y = shapePoints[shapePoints.Count - 1].Y + thicknessAtToeShortLeg,
                PathSegmentType = PathSegmentType.Line
            });

            shapePoints.Add(new PointEx()
            {
                X = shapePoints[shapePoints.Count - 1].X - ShortLegLength,
                Y = shapePoints[shapePoints.Count - 1].Y,
                PathSegmentType = PathSegmentType.Line
            });

            shapePoints.Add(new PointEx()
            {
                X = shapePoints[shapePoints.Count - 1].X,
                Y = shapePoints[shapePoints.Count - 1].Y - LongLegLength,
                PathSegmentType = PathSegmentType.Line
            });

            PointsCollection = new List<IEnumerable<PointEx>>()
            {
                shapePoints
            };

        }

        #endregion

    }
}
