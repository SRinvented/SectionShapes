using System.Collections.Generic;
using System.Windows.Media;

using ReInvented.CrossSections.Models;

namespace ReInvented.CrossSections.Interfaces
{
    public interface ISectionGeometry
    {
        /// <summary>
        /// Collection of points for multiple <see cref="PathGeometry"/> to generate the a composite shape. For shapes like I, C it may be only one <see cref="PathGeometry"/>.
        /// </summary>
        List<IEnumerable<ShapePoint>> PointsCollection { get; }
        /// <summary>
        /// Generates a collection of points required to generate the shape of the section.
        /// </summary>
        void GeneratePoints();
    }
}
