using System;
using System.Collections.Generic;
using System.Windows.Media;
using SRi.XamlUIThickenerApp.Shared;

using ReInvented.Graphics.Interfaces;

namespace ReInvented.Graphics.Models
{
    public sealed class ChsSectionGeometry : ISectionGeometry
    {

        public ChsSectionGeometry(double outerDiameter, double tw, int numOfPoints = 12)
        {
            OD = outerDiameter;
            Tw = tw;
            NumOfPoints = numOfPoints;
        }

        public double OD { get; set; }

        public double Tw { get; set; }

        public int NumOfPoints { get; set; }

        public List<IEnumerable<PointEx>> PointsCollection { get; private set; }

        public void GeneratePoints()
        {
            List<PointEx> outerCirclePoints = new List<PointEx>();

            List<PointEx> innerCirclePoints = new List<PointEx>();

            for (int i = 0; i <= NumOfPoints; i++)
            {
                double radius = OD / 2;

                double x = radius + (radius * Math.Cos((i * (360.0 / NumOfPoints)).ToRadians()));
                double y = radius + (radius * Math.Sin((i * (360.0 / NumOfPoints)).ToRadians()));
                PointEx point = new PointEx(x, y, PathSegmentType.Arc)
                {
                    Radius = OD / 2,
                    SweepDirection = SweepDirection.Clockwise
                };

                outerCirclePoints.Add(point);
            }


            for (int i = 0; i <= NumOfPoints; i++)
            {
                double radius = (OD - (2 * Tw)) / 2;

                double x = radius + Tw + (radius * Math.Cos((i * (360.0 / NumOfPoints)).ToRadians()));
                double y = radius + Tw + (radius * Math.Sin((i * (360.0 / NumOfPoints)).ToRadians()));

                PointEx point = new PointEx(x, y, PathSegmentType.Arc)
                {
                    Radius = (OD - (2 * Tw)) / 2,
                    SweepDirection = SweepDirection.Clockwise
                };

                innerCirclePoints.Add(point);
            }


            PointsCollection = new List<IEnumerable<PointEx>>
            {
                outerCirclePoints,
                innerCirclePoints
            };
        }

        //private List<PointEx> FormulateBoundaryPoints(double xOffset, double yOffset, double radius, int numOfPoints)
        //{
        //    List<PointEx> boundaryPoints = new List<PointEx>();

        //    for (int i = 0; i <= numOfPoints; i++)
        //    {
        //        double x = xOffset + (radius * Math.Cos((i * (360.0 / numOfPoints)).ToRadians()));
        //        double y = radius + Tw + (radius * Math.Sin((i * (360.0 / numOfPoints)).ToRadians()));

        //        PointEx point = new PointEx(x, y, PathSegmentType.Arc)
        //        {
        //            Radius = (OD - (2 * Tw)) / 2,
        //            SweepDirection = SweepDirection.Clockwise
        //        };

        //        boundaryPoints.Add(point);
        //    }
        //}

    }
}
