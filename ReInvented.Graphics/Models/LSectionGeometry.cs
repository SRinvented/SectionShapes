using System;
using System.Collections.Generic;
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
            LLong = longLegLength;
            LShort = shortLegLength;
            TwLong = longLegThickness;
            TwShort = shortLegThickness;
        }


        #endregion

        #region Dimensions

        public double LLong { get; set; }

        public double LShort { get; set; }

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

        public double LongLegSlopeWithHorizontal => LongLegSlope - 90.0;

        public double ShortLegSlopeWithHorizontal => ShortLegSlope - 90.0;

        public CurveTriangleGeometry LongLegToeGeometry => new CurveTriangleGeometry(LongLegToeRadius, LongLegSlopeWithHorizontal);

        public CurveTriangleGeometry ShortLegToeGeometry => new CurveTriangleGeometry(ShortLegToeRadius, ShortLegSlopeWithHorizontal);

        public CurveDualTriangleGeometry RootGeometry => new CurveDualTriangleGeometry(RootRadius, LongLegSlopeWithHorizontal, ShortLegSlopeWithHorizontal);

        public TriangleGeometry LongLegTriangle { get; private set; }

        public TriangleGeometry ShortLegTriangle { get; private set; }

        public TriangleGeometry LongLegMainTriangle { get; private set; }

        public TriangleGeometry ShortLegMainTriangle { get; private set; }

        public List<IEnumerable<PointEx>> PointsCollection { get; private set; }

///        public AngleLeg LongLeg { get; set; }


        #endregion

        #region Public Methods

        public void GeneratePoints()
        {

            /// Important:
            /// Read Vertical as Horizontal and Horizontal as Vertial for Long Leg in Triangles due to the fact that the triangle properties
            /// are created keeping the I (or) Channel sections in view. For Angles the legs will be perpendicular to each other.
            /// This is not applicable for Short Leg.

            #region Sloped Faces Intersection Point Coordinates Calculation

            /// Note and TODO: Variables' names may found to be voilating the best practices in this section. However, in order to be consisting with
            /// manual calculation to make the calculations meaningful, these names are used and may be changed later on to fit in to best practices.

            double L = Math.Sqrt(((LLong - TwShort) / 2).Squared() + ((LShort - TwLong) / 2).Squared());

            double betaL = Math.Atan((LShort - TwLong) / (LLong - TwShort)).ToDegrees();
            double betaS = 90.0 - betaL; ///Math.Atan((LLong - TwShort) / (LShort - TwLong)).ToDegrees();

            double thetaL = betaL - LongLegSlopeWithHorizontal;
            double thetaS = betaS - ShortLegSlopeWithHorizontal;

            double L1 = L * Math.Tan(thetaL.ToRadians()) / (Math.Tan(thetaL.ToRadians()) + Math.Tan(thetaS.ToRadians())); ///L / (1 + Math.Tan(thetaS / thetaL)); 
            double L2 = L - L1; /// L * Math.Tan(thetaL.ToRadians()) / (Math.Tan(thetaL.ToRadians()) + Math.Tan(thetaS.ToRadians()));

            double SideL = L2 / Math.Cos(thetaL.ToRadians());
            double SideS = L1 / Math.Cos(thetaS.ToRadians());

            LongLegTriangle = new TriangleGeometry(SideL, LongLegSlopeWithHorizontal);
            ShortLegTriangle = new TriangleGeometry(SideS, ShortLegSlopeWithHorizontal);

            double xCoordinate = TwShort + ShortLegTriangle.OppositeSide;
            double yCoordinate = TwLong + LongLegTriangle.OppositeSide;

            /// Alternative Method

            double LOne = (LLong - TwShort) / 2;
            double LTwo = (LShort - TwLong) / 2;

            double xCoord = (LTwo - LOne * Math.Tan(LongLegSlopeWithHorizontal.ToRadians())) * Math.Tan(ShortLegSlopeWithHorizontal.ToRadians()) / (1 - (Math.Tan(LongLegSlopeWithHorizontal.ToRadians()) * Math.Tan(ShortLegSlopeWithHorizontal.ToRadians())));
            double yCoord = (LOne - xCoord) * Math.Tan(LongLegSlopeWithHorizontal.ToRadians());




            double includedAngleAtCenter = 90 - LongLegSlopeWithHorizontal - ShortLegSlopeWithHorizontal;
            double chordLength = 2 * RootRadius * Math.Sin(includedAngleAtCenter.ToRadians() / 2);
            double includedAngleAtIntersection = 90 + LongLegSlopeWithHorizontal + ShortLegSlopeWithHorizontal;
            double includedAngleAtCurvePoints = (180 - includedAngleAtIntersection) / 2;

            double alignedLengthToCurvePoints = chordLength / 2 / Math.Cos(includedAngleAtCurvePoints.ToRadians());

            double adjacentLong = alignedLengthToCurvePoints * Math.Cos(LongLegSlopeWithHorizontal.ToRadians());
            double oppositeLong = alignedLengthToCurvePoints * Math.Sin(LongLegSlopeWithHorizontal.ToRadians());

            double adjacentShort = alignedLengthToCurvePoints * Math.Cos(ShortLegSlopeWithHorizontal.ToRadians());
            double oppositeShort = alignedLengthToCurvePoints * Math.Sin(ShortLegSlopeWithHorizontal.ToRadians());

            double alignedLegLengthToCurvePointOnLongLeg = SideL - alignedLengthToCurvePoints;
            double alignedLegLengthToCurvePointOnShortLeg = SideS - alignedLengthToCurvePoints;

            double longLegTriangleHypotenuse = alignedLegLengthToCurvePointOnLongLeg + (LOne - (LongLegToeRadius - LongLegToeGeometry.LargeTriangle.OppositeSide)) / Math.Cos(LongLegSlopeWithHorizontal.ToRadians());
            double shortLegTriangleHypotenuse = alignedLegLengthToCurvePointOnShortLeg + (LTwo - (ShortLegToeRadius - ShortLegToeGeometry.LargeTriangle.OppositeSide)) / Math.Cos(ShortLegSlopeWithHorizontal.ToRadians());

            LongLegMainTriangle = new TriangleGeometry(longLegTriangleHypotenuse, LongLegSlopeWithHorizontal);
            ShortLegMainTriangle = new TriangleGeometry(shortLegTriangleHypotenuse, ShortLegSlopeWithHorizontal);


            #endregion

            ///#region Data Required for Points Calculation

            ///#region Leg Slope Main Triangles Properties

            double longLegMainTriangleHorizontal = (LLong - TwShort) / 2 - (LongLegToeRadius - LongLegToeGeometry.LargeTriangle.OppositeSide);

            //double longLegMainTriangleHypotenuse = mainTriangleHorizontal / Math.Cos(FlangeSlopeWithHorizontal.ToRadians());

            //LongLegMainTriangle = new TriangleGeometry(mainTriangleHypotenuse, 90 - FlangeSlopeWithHorizontal);

            //double apexToFlangeThickness = ((B - Tw) / 2) - (ToeRadius - ToeGeometry.LargeTriangle.OppositeSide);
            //double heightAtFlangeThickness = FlangeTriangle.AdjacentSide * apexToFlangeThickness / FlangeTriangle.OppositeSide;


            //#endregion

            ///// Vertical distance to the root intersection with web from nearest flange face.
            //double heightAtRootIntersection = Tf - heightAtFlangeThickness + FlangeTriangle.AdjacentSide + RootGeometry.LargeTriangle.AdjacentSide;

            //#endregion


            //List<PointEx> shapePoints = new List<PointEx> { new PointEx(0, 0, PathSegmentType.Line) };

            //shapePoints.Add(new PointEx(TwLong - heightAtLongLegThickness - ToeGeometry.LargeTriangle.Vertical, 0, PathSegmentType.Line));

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

            ///PointsCollection = shapePoints;

        }

        #endregion

    }
}
