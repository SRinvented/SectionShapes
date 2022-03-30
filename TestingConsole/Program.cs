using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReInvented.Graphics.Interfaces;
using ReInvented.Graphics.Models;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ISectionGeometry sectionGeometry = new LSectionGeometry(150, 100, 14, 12) { LongLegSlope = 94, ShortLegSlope = 92, RootRadius = 10, LongLegToeRadius = 6, ShortLegToeRadius = 4 };

            sectionGeometry.GeneratePoints();

            sectionGeometry.PointsCollection.ForEach(pc => pc.ToList().ForEach(p => Console.WriteLine($"{p.X:N5} {p.Y:N5}")));

            Console.ReadLine();

        }
    }
}
