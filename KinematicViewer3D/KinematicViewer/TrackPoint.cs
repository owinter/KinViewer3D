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

        private Material _oTrackPointMaterial;
        private Point3D _oPoint;
        private List<Point3D> _oLCoordsTrackPoint;

        public TrackPoint(Point3D startPoint, Material mat = null)
            :base(mat)
        {
            StartPoint = startPoint;
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

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D pointUpdated = guide.MovePoint(StartPoint);
            CoordsTrackPoint.Add(pointUpdated);
            foreach (Point3D p in CoordsTrackPoint)
                Res.AddRange(new Sphere(p, RADIUS, TrackPointMaterial).GetGeometryModel(guide));

            return Res.ToArray();
        }
    }
}
