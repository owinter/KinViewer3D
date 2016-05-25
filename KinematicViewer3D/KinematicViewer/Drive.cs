using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Drive : GeometricalElement
    {
        private double _dExtractedLength;
        private double _dOffsetSecondCylinder;
        private double _dRetractedLength;
        private double _dStroke;
        private int _iRadiusBody;
        private int _iRadiusDoor;

        private Point3D _oEndPoint;
        private Point3D _oStartPoint;
        private Vector3D _oVDrive;

        public Drive(Point3D point1, Point3D point2)
        {
            StartPoint = point1;
            EndPoint = point2;

            RadiusBody = 15;
            RadiusDoor = 25;

            VDrive = EndPoint - StartPoint;
            RetractedLenght = VDrive.Length;
            ExtractedLength = 648.8;
            Stroke = _dExtractedLength - _dRetractedLength;
            OffsetSecondCylinder = (0.25 * VDrive).Length;
        }

        public Point3D EndPoint
        {
            get { return _oEndPoint; }
            set { _oEndPoint = value; }
        }

        public double ExtractedLength
        {
            get { return _dExtractedLength; }
            private set { _dExtractedLength = value; }
        }

        public double OffsetSecondCylinder
        {
            get { return _dOffsetSecondCylinder; }
            private set { _dOffsetSecondCylinder = value; }
        }

        public int RadiusBody
        {
            get { return _iRadiusBody; }
            private set { _iRadiusBody = value; }
        }

        public int RadiusDoor
        {
            get { return _iRadiusDoor; }
            private set { _iRadiusDoor = value; }
        }

        public double RetractedLenght
        {
            get { return _dRetractedLength; }
            private set { _dRetractedLength = value; }
        }

        public Point3D StartPoint
        {
            get { return _oStartPoint; }
            set { _oStartPoint = value; }
        }

        public double Stroke
        {
            get { return _dStroke; }
            private set { _dStroke = value; }
        }

        public Vector3D VDrive
        {
            get { return _oVDrive; }
            set { _oVDrive = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D attPointDoor = guide.MovePoint(EndPoint);

            Vector3D vDriveUpdated = attPointDoor - StartPoint;
            double vLength = vDriveUpdated.Length;
            vDriveUpdated.Normalize();

            //Offset um welchen der zweite Cylinder verschoben wird
            Vector3D vOffset = (OffsetSecondCylinder + (vLength - VDrive.Length)) * vDriveUpdated;

            //grüner Bereich
            if (vLength <= ExtractedLength - Stroke * 2 / 3)
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + vOffset, attPointDoor, RadiusDoor, Brushes.YellowGreen).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));
            }

            //Gelber Bereich
            else if ((vLength > ExtractedLength - Stroke * 2 / 3) && !(vLength <= ExtractedLength - Stroke * 2 / 3))
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + vOffset, attPointDoor, RadiusDoor, Brushes.Orange).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));
            }

            //Roter Bereich
            else if ((vLength > ExtractedLength - Stroke * 1 / 3) && !(vLength <= ExtractedLength - Stroke * 2 / 3))
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + vOffset, attPointDoor, RadiusDoor, Brushes.OrangeRed).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));
            }

            //Farblicher Anbindungspunkt an die Heckklappe in Cyan
            Res.AddRange(new Sphere(attPointDoor, 40, Brushes.Cyan).GetGeometryModel(guide));

            return Res.ToArray();
        }
    }
}