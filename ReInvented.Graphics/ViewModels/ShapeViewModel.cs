using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

using ReInvented.Graphics.Interfaces;
using ReInvented.Graphics.Models;

namespace ReInvented.Graphics.ViewModels
{
    public class ShapeViewModel : INotifyPropertyChanged
    {

        public ShapeViewModel()
        {
            ///SectionGeometry = new HSectionGeometry(400, 140, 8.9, 16, 14, 7, 98);
            ///SectionGeometry = new CSectionGeometry(300, 90, 7.8, 13.6, 13, 3.2, 96);
            ///SectionGeometry = new ChsSectionGeometry(150, 12);
            ///SectionGeometry = new RhsSectionGeometry(300, 200, 10, 10, 5);

            ///SectionGeometry = new LSectionGeometry(150, 100, 12, 10, 5, 92);

            SectionGeometry = new LSectionGeometry(150, 100, 14, 12) { LongLegSlope = 94, ShortLegSlope = 92, RootRadius = 10, LongLegToeRadius = 6, ShortLegToeRadius = 4 };

            PathGeometryBuilder = new PathGeometryBuilder(SectionGeometry);
        }


        ///public ISectionGeometry SectionGeometry { get; set; }

        public ISectionGeometry SectionGeometry { get; set; }


        public PathGeometryBuilder PathGeometryBuilder { get; set; }

        public PathGeometry ShapeGeometry => PathGeometryBuilder.Build();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
