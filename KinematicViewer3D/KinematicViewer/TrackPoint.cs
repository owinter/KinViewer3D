using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class TrackPoint : GeometricalElement
    {
        private const double RADIUS = 10;
        private const int ELEMENTS = 10;

        private Vector3D _oVAxisOfRotation;
        private Material _oTrackPointMaterial;
        private Point3D _oPoint;
        private List<Point3D> _oLCoordsTrackPoint;

        public TrackPoint(Point3D startPoint, Vector3D axisOfRotation,  Material mat = null)
            :base(mat)
        {
            StartPoint = startPoint;
            AxisOfRotation = axisOfRotation;
            TrackPointMaterial = new DiffuseMaterial(Brushes.Gray);
            CoordsTrackPoint = new List<Point3D>();
            CoordsTrackPoint.Add(startPoint);
        }

        public List<Point3D> CoordsTrackPoint
        {
            get { return _oLCoordsTrackPoint; }
            set { _oLCoordsTrackPoint = value; }
        }

        public Point3D StartPoint
        {
            get { return _oPoint; }
            set { _oPoint = value; }
        }

        public Material TrackPointMaterial
        {
            get { return _oTrackPointMaterial; }
            set { _oTrackPointMaterial = value; }
        }

        public Vector3D AxisOfRotation
        {
            get { return _oVAxisOfRotation; }
            set { _oVAxisOfRotation = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();
            double curOpenVal = guide.CurValue / ELEMENTS;
            double openValue  = curOpenVal;

            for (int i = 0; i< ELEMENTS; i++)
            {
                Point3D p = TransformationUtilities.rotateNewPoint(AxisOfRotation, openValue, StartPoint);
                Res.AddRange(new Sphere(p, RADIUS, TrackPointMaterial).GetGeometryModel(guide));
                openValue += curOpenVal;   
            }
            return Res.ToArray();
        }
    }
}
