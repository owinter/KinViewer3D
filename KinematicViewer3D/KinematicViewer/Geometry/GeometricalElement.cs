using KinematicViewer.Geometry.Guides;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer.Geometry
{
    public abstract class GeometricalElement
    {
        private Material _oMaterial = new DiffuseMaterial(Brushes.Cyan);
        private string _sName;

        public GeometricalElement(Material mat = null)
        {
            if (mat != null)
                Material = mat;
        }

        public Material Material
        {
            get { return _oMaterial; }
            set { _oMaterial = value; }
        }

        public string Name
        {
            get { return _sName; }
            set { _sName = value; }
        }

        public abstract GeometryModel3D[] GetGeometryModel(IGuide guide);
    }
}