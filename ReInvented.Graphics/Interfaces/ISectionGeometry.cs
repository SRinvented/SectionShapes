using System.Collections.Generic;

using ReInvented.Graphics.Models;

namespace ReInvented.Graphics.Interfaces
{
    public interface ISectionGeometry
    {
        List<IEnumerable<PointEx>> PointsCollection { get; }

        void GeneratePoints();
    }
}
