using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

using ReInvented.CrossSections.Interfaces;
using ReInvented.CrossSections.Models;

namespace ReInvented.CrossSections.ViewModels
{
    public class ShapeViewModel : INotifyPropertyChanged
    {

        public ShapeViewModel()
        {
            ///SectionGeometry = new HSectionGeometry(400, 140, 8.9, 16, 14, 7, 98);
            ///SectionGeometry = new CSectionGeometry(300, 90, 7.8, 13.6, 13, 3.2, 96);
            ///SectionGeometry = new ChsSectionGeometry(150, 12);
            ///SectionGeometry = new RhsSectionGeometry(300, 200, 10, 10, 5);

            ///SectionGeometry = new LSectionGeometry(150, 100, 12, 10);

            SectionGeometry = new LSectionGeometry()
            {
                LongLeg = new AngleLeg() { Length = 150, Tw = 12, Slope = 92, ToeRadius = 0 },
                ShortLeg = new AngleLeg() { Length = 100, Tw = 10, Slope = 92, ToeRadius = 0 },
                RootRadius = 25
            };

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
