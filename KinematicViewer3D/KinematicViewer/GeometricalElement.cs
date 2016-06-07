using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public abstract class GeometricalElement
    {
        private Material _oMaterial = new DiffuseMaterial(Brushes.Cyan);

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

        public abstract GeometryModel3D[] GetGeometryModel(IGuide guide);
    }
}
