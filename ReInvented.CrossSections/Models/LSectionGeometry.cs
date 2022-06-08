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
    /// Stores and retrieves dimensional details of the an L/Angle cross section.
    /// Also, calculates the key cross section points to facilitate the generates of geometry. It shall be noted that, this class considers the thickness of flange measured at (B - Tw)/2.
    /// </summary>
    public sealed class LSectionGeometry : ISectionGeometry
    {

        #region Default Constructor

        public LSectionGeometry()
        {

        }

        #endregion

        #region Parameterized Constructors

        public LSectionGeometry(double legLength, double legThickness)
            : this(legLength, legLength, legThickness, legThickness)
        {

        }

        public LSectionGeometry(double longLegLength, double shortLegLength, double longLegThickness, double shortLegThickness)
        {

            LongLeg = new AngleLeg(longLegLength, longLegThickness)
            {
                Length = longLegLength,
                Tw = longLegThickness
            };

            ShortLeg = new AngleLeg(shortLegLength, shortLegThickness)
            {
                Length = shortLegLength,
                Tw = shortLegThickness
            };
        }

        public LSectionGeometry(IRolledSection angleSection)
        {
            InitializeProperties(angleSection as RolledSectionLShape);
        }

        #endregion

        #region Public Properties

        public double RootRadius { get; set; }

        public AngleLeg LongLeg { get; set; }

        public AngleLeg ShortLeg { get; set; }

        #endregion

        #region Read-only Properties

        public CurveDualTriangleGeometry Root => new CurveDualTriangleGeometry(RootRadius, LongLeg.SlopeWithFace, ShortLeg.SlopeWithFace);

        public List<IEnumerable<ShapePoint>> PointsCollection { get; private set; }

        #endregion

        #region Public Methods

        public void GeneratePoints()
        {

            #region Sloped Faces Intersection Point Coordinates Calculation

            /// Indicates the horizontal distance to the point where the thickness on the leg is measured.
            double faceToThicknessLongLeg = (LongLeg.Length - ShortLeg.Tw) / 2;
            double faceToThicknessShortLeg = (ShortLeg.Length - LongLeg.Tw) / 2;

            /// Distances to the intersection point of inclined faces measured from the point where thickness lines intersect.
            double intersectionAlongLongLeg = (faceToThicknessShortLeg - faceToThicknessLongLeg *
                                               Math.Tan(LongLeg.SlopeWithFace.ToRadians())) * Math.Tan(ShortLeg.SlopeWithFace.ToRadians()) /
                                               (1 - (Math.Tan(LongLeg.SlopeWithFace.ToRadians()) * Math.Tan(ShortLeg.SlopeWithFace.ToRadians())));

            double intersectionAlongShortLeg = (faceToThicknessLongLeg - intersectionAlongLongLeg) * Math.Tan(LongLeg.SlopeWithFace.ToRadians());

            /// Distances to the intersection point of inclined faces measured from the point where outer faces of the legs intersect.
            intersectionAlongLongLeg = ShortLeg.Tw + intersectionAlongLongLeg;
            intersectionAlongShortLeg = LongLeg.Tw + intersectionAlongShortLeg;

            /// Properties of the root curve
            double includedAngleAtCenter = 90 - LongLeg.SlopeWithFace - ShortLeg.SlopeWithFace;
            double chordLength = 2 * RootRadius * Math.Sin(includedAngleAtCenter.ToRadians() / 2);
            double includedAngleAtIntersection = 90 + LongLeg.SlopeWithFace + ShortLeg.SlopeWithFace;
            double includedAngleAtCurvePoints = (180 - includedAngleAtIntersection) / 2;

            /// Distance to the points where curve intersects the inclined faces measured from the point where inclined faces intersect with each other.
            double rootIntersectionToCurvePoints = chordLength / 2 / Math.Cos(includedAngleAtCurvePoints.ToRadians());

            /// Length of the inclined face on respective leg measured from the intersection point to the point where thickness is measured on that leg.
            double sideL = (ShortLeg.Tw + faceToThicknessLongLeg - intersectionAlongLongLeg) / Math.Cos(LongLeg.SlopeWithFace.ToRadians());
            double sideS = (LongLeg.Tw + faceToThicknessShortLeg - intersectionAlongShortLeg) / Math.Cos(ShortLeg.SlopeWithFace.ToRadians());

            /// Length of the inclined face on the repective leg measured from the root curve intersection point to the point where thickness is measured on that leg.
            double alignedLegLengthToCurvePointOnLongLeg = sideL - rootIntersectionToCurvePoints;
            double alignedLegLengthToCurvePointOnShortLeg = sideS - rootIntersectionToCurvePoints;

            /// Horizontal distance from the point where thickness is measured on the respective leg to the point where inclined face intersects the toe curve.
            double distanceToThicknessFromToeLongLeg = faceToThicknessLongLeg - (LongLeg.Toe.Radius - LongLeg.Toe.LargeTriangle.Opposite);
            double distanceToThicknessFromToeShortLeg = faceToThicknessShortLeg - (ShortLeg.Toe.Radius - ShortLeg.Toe.LargeTriangle.Opposite);

            /// Total length of the inclined face of respective leg.
            double longLegTriangleHypotenuse = alignedLegLengthToCurvePointOnLongLeg + (distanceToThicknessFromToeLongLeg / Math.Cos(LongLeg.SlopeWithFace.ToRadians()));
            double shortLegTriangleHypotenuse = alignedLegLengthToCurvePointOnShortLeg + (distanceToThicknessFromToeShortLeg / Math.Cos(ShortLeg.SlopeWithFace.ToRadians()));


            LongLeg.MainTriangle = new TriangleGeometry(longLegTriangleHypotenuse, LongLeg.SlopeWithFace);
            ShortLeg.MainTriangle = new TriangleGeometry(shortLegTriangleHypotenuse, ShortLeg.SlopeWithFace);


            double heightAtThicknessLongLeg = LongLeg.MainTriangle.Opposite / LongLeg.MainTriangle.Adjacent * distanceToThicknessFromToeLongLeg;
            double heightAtThicknessShortLeg = ShortLeg.MainTriangle.Opposite / ShortLeg.MainTriangle.Adjacent * distanceToThicknessFromToeShortLeg;

            double thicknessAtToeLongLeg = LongLeg.Tw - heightAtThicknessLongLeg - LongLeg.Toe.LargeTriangle.Adjacent;
            double thicknessAtToeShortLeg = ShortLeg.Tw - heightAtThicknessShortLeg - ShortLeg.Toe.LargeTriangle.Adjacent;

            #endregion

            List<ShapePoint> shapePoints = new List<ShapePoint> { new ShapePoint(0, 0, PathSegmentType.Line) };

            shapePoints.Add(new ShapePoint(thicknessAtToeLongLeg, 0, PathSegmentType.Line));

            #region Toe - Long Leg

            if (LongLeg.Toe.Radius > 0)
            {
                shapePoints.Add(new ShapePoint()
                {
                    X = thicknessAtToeLongLeg + LongLeg.Toe.LargeTriangle.Adjacent,
                    Y = LongLeg.Toe.Radius - LongLeg.Toe.LargeTriangle.Opposite,
                    PathSegmentType = PathSegmentType.Arc,
                    Radius = LongLeg.Toe.Radius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            #region Inclined Face - Long Leg

            shapePoints.Add(new ShapePoint()
            {
                X = shapePoints[shapePoints.Count - 1].X + LongLeg.MainTriangle.Opposite,
                Y = shapePoints[shapePoints.Count - 1].Y + LongLeg.MainTriangle.Adjacent,
                PathSegmentType = PathSegmentType.Line
            });

            #endregion

            #region Root

            if (RootRadius > 0)
            {
                shapePoints.Add(new ShapePoint()
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

            shapePoints.Add(new ShapePoint()
            {
                X = shapePoints[shapePoints.Count - 1].X + ShortLeg.MainTriangle.Adjacent,
                Y = shapePoints[shapePoints.Count - 1].Y + ShortLeg.MainTriangle.Opposite,
                PathSegmentType = PathSegmentType.Line
            });

            #endregion

            #region Toe - Short Leg

            if (ShortLeg.Toe.Radius > 0)
            {
                shapePoints.Add(new ShapePoint()
                {
                    X = shapePoints[shapePoints.Count - 1].X + ShortLeg.Toe.Radius - ShortLeg.Toe.LargeTriangle.Opposite,
                    Y = shapePoints[shapePoints.Count - 1].Y + ShortLeg.Toe.Radius - ShortLeg.Toe.SmallTriangle.Opposite,
                    PathSegmentType = PathSegmentType.Arc,
                    Radius = ShortLeg.Toe.Radius,
                    SweepDirection = SweepDirection.Clockwise
                });
            }

            #endregion

            shapePoints.Add(new ShapePoint()
            {
                X = shapePoints[shapePoints.Count - 1].X,
                Y = shapePoints[shapePoints.Count - 1].Y + thicknessAtToeShortLeg,
                PathSegmentType = PathSegmentType.Line
            });

            shapePoints.Add(new ShapePoint()
            {
                X = shapePoints[shapePoints.Count - 1].X - ShortLeg.Length,
                Y = shapePoints[shapePoints.Count - 1].Y,
                PathSegmentType = PathSegmentType.Line
            });

            shapePoints.Add(new ShapePoint()
            {
                X = shapePoints[shapePoints.Count - 1].X,
                Y = shapePoints[shapePoints.Count - 1].Y - LongLeg.Length,
                PathSegmentType = PathSegmentType.Line
            });

            PointsCollection = new List<IEnumerable<ShapePoint>>()
            {
                shapePoints
            };

        }

        #endregion

        #region Private Helper Methods

        private void InitializeProperties(RolledSectionLShape angleSection)
        {

            LongLeg = new AngleLeg()
            {
                Length = angleSection.L,
                Tw = angleSection.Tw,
                ToeRadius = angleSection.R2
            };

            ShortLeg = new AngleLeg()
            {
                Length = angleSection.B,
                Tw = angleSection.Tw,
                ToeRadius = angleSection.R2
            };

            RootRadius = angleSection.R1;
        }

        #endregion

    }
}
