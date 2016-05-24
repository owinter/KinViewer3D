using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Drive: GeometricalElement
    {
        private Point3D _oStartPoint;
        private Point3D _oEndPoint;
        private Vector3D vDrive;

        private double _dRetractedLength;
        private double _dExtractedLength;
        private double _dStroke;
        private int _iRadiusBody;
        private int _iRadiusDoor;

        public Drive(Point3D point1, Point3D point2)
        {
            StartPoint = point1;
            EndPoint = point2;

            RadiusBody = 15;
            RadiusDoor = 25;
            
            vDrive = EndPoint - StartPoint;
            _dRetractedLength = vDrive.Length;
            ExtractedLength = 648.8;
            Stroke = _dExtractedLength - _dRetractedLength;
        }

        public double RetractedLenght
        {
            get { return _dRetractedLength; }
            private set { _dRetractedLength = value; }
        }

        public double ExtractedLength
        {
            get { return _dExtractedLength; }
            private set { _dExtractedLength = value; }
        }

        public Point3D StartPoint
        {
            get { return _oStartPoint; }
            set { _oStartPoint = value; }
        }

        public Point3D EndPoint
        {
            get { return _oEndPoint; }
            set { _oEndPoint = value; }
        }

        public double Stroke
        {
            get { return _dStroke; }
            private set { _dStroke = value; }
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


        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D attPointDoor = guide.MovePoint(EndPoint);

            Vector3D vDriveUpdated = attPointDoor - StartPoint;
            double vLength = vDriveUpdated.Length;

            if (vLength <= ExtractedLength - Stroke * 2 / 3)
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + 0.25 * vDriveUpdated, attPointDoor, RadiusDoor, Brushes.YellowGreen).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));
            }

            else if ((vLength > ExtractedLength - Stroke * 2 / 3) && !(vLength <= ExtractedLength - Stroke * 2 / 3))
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody,  Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + 0.25 * vDriveUpdated, attPointDoor, RadiusDoor, Brushes.Orange).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));
            }

            else if ((vLength > ExtractedLength - Stroke * 1 / 3) && !(vLength <= ExtractedLength - Stroke * 2 / 3))
            {
                Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Sphere(StartPoint, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(StartPoint + 0.25 * vDriveUpdated, attPointDoor, RadiusDoor, Brushes.OrangeRed).GetGeometryModel(guide));
                Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));
            }

            //Farblicher Anbindungspunkt an die Heckklappe in Cyan
            Res.AddRange(new Sphere(attPointDoor, 40, Brushes.Cyan).GetGeometryModel(guide));
            
            return Res.ToArray();
        }
    }
}

    