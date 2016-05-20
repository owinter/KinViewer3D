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
        private DiffuseMaterial _oMaterial = new DiffuseMaterial(Brushes.Cyan);

        public GeometricalElement(Brush brush = null)
        {
            if (brush != null)
                Material = new DiffuseMaterial(brush);
        }

        public DiffuseMaterial Material
        {
            get { return _oMaterial; }
            set { _oMaterial = value; }
        }

        public abstract GeometryModel3D[] GetGeometryModel();
    }
}
