using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using ReInvented.CrossSections.Interfaces;

namespace ReInvented.CrossSections.Models
{
    public class PathGeometryBuilder
    {
        #region Private Fields

        private readonly ISectionGeometry _sectionGeometry;

        #endregion

        #region Parameterized Constructor

        public PathGeometryBuilder(ISectionGeometry sectionGeometry)
        {
            _sectionGeometry = sectionGeometry;
        }

        #endregion

        #region Public Methods

        public PathGeometry Build()
        {
            if (_sectionGeometry == null)
            {
                return null;
            }

            if (_sectionGeometry.PointsCollection == null)
            {
                _sectionGeometry.GeneratePoints();
            }

            PathGeometry pathGeometry = new PathGeometry();
            PathFigureCollection pathFigures = new PathFigureCollection();

            foreach (IEnumerable<ShapePoint> p in _sectionGeometry.PointsCollection)
            {
                List<ShapePoint> points = p.ToList();

                PathFigure pathFigure = new PathFigure
                {
                    StartPoint = new Point(points[0].X, points[0].Y)
                };

                for (int i = 1; i < points.Count; i++)
                {
                    if (points[i].PathSegmentType == PathSegmentType.Line)
                    {
                        LineSegment lineSegment = new LineSegment() { Point = new Point(points[i].X, points[i].Y) };
                        pathFigure.Segments.Add(lineSegment);
                    }
                    else
                    {
                        ArcSegment arcSegment = new ArcSegment()
                        {
                            Point = new Point(points[i].X, points[i].Y),
                            Size = new Size(points[i].Radius, points[i].Radius),
                            SweepDirection = points[i].SweepDirection
                        };

                        pathFigure.Segments.Add(arcSegment);
                    }
                }
                pathFigures.Add(pathFigure);
            }
            pathGeometry.Figures = pathFigures;

            return pathGeometry;
        }

        #endregion
    }
}
