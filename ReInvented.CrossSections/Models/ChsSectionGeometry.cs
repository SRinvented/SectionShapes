using System;
using System.Collections.Generic;
using System.Windows.Media;
using SRi.XamlUIThickenerApp.Shared;

using ReInvented.CrossSections.Interfaces;
using ReInvented.SectionProfiles.Interfaces;
using ReInvented.SectionProfiles.Models;

namespace ReInvented.CrossSections.Models
{
    public sealed class ChsSectionGeometry : ISectionGeometry
    {

        #region Parameterized Constructors

        public ChsSectionGeometry(double outerDiameter, double tw, int numOfPoints = 12)
        {
            OD = outerDiameter;
            Tw = tw;
            NumOfPoints = numOfPoints;
        }

        public ChsSectionGeometry(IRolledSection chsSection)
        {
            InitializeProperties(chsSection as RolledSectionOShape);
        }

        #endregion

        #region Public Properties

        public double OD { get; set; }

        public double Tw { get; set; }

        public int NumOfPoints { get; set; } = 12;

        #endregion

        #region Read-only Properties

        public List<IEnumerable<ShapePoint>> PointsCollection { get; private set; }

        #endregion

        #region Public Methods

        public void GeneratePoints()
        {
            List<ShapePoint> outerCirclePoints = new List<ShapePoint>();

            List<ShapePoint> innerCirclePoints = new List<ShapePoint>();

            for (int i = 0; i <= NumOfPoints; i++)
            {
                double radius = OD / 2;

                double x = radius + (radius * Math.Cos((i * (360.0 / NumOfPoints)).ToRadians()));
                double y = radius + (radius * Math.Sin((i * (360.0 / NumOfPoints)).ToRadians()));
                ShapePoint point = new ShapePoint(x, y, PathSegmentType.Arc)
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

                ShapePoint point = new ShapePoint(x, y, PathSegmentType.Arc)
                {
                    Radius = (OD - (2 * Tw)) / 2,
                    SweepDirection = SweepDirection.Clockwise
                };

                innerCirclePoints.Add(point);
            }


            PointsCollection = new List<IEnumerable<ShapePoint>>
            {
                outerCirclePoints,
                innerCirclePoints
            };
        }

        #endregion

        #region Private Helper Methods

        private void InitializeProperties(RolledSectionOShape chsSection)
        {
            OD = chsSection.OD;
            Tw = chsSection.Tw;
        }

        #endregion

    }
}
