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

        //private MainViewPortControl mainViewPortControl;

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
            set { _dRetractedLength = value; }
        }

        public double ExtractedLength
        {
            get { return _dExtractedLength; }
            set { _dExtractedLength = value; }
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
            set { _dStroke = value; }
        }

        public int RadiusBody
        {
            get { return _iRadiusBody; }
            set { _iRadiusBody = value; }
        }

        public int RadiusDoor
        {
            get { return _iRadiusDoor; }
            set { _iRadiusDoor = value; }
        }


        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D attPointDoor = guide.MovePoint(EndPoint);

            Vector3D vDriveUpdated = attPointDoor - StartPoint;

            Res.AddRange(new Cylinder(StartPoint, attPointDoor - 0.25 * vDriveUpdated, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
            Res.AddRange(new Sphere(StartPoint, RadiusBody, Brushes.Gray).GetGeometryModel(guide));
            Res.AddRange(new Cylinder(StartPoint + 0.25 * vDriveUpdated, attPointDoor, RadiusDoor, Brushes.YellowGreen).GetGeometryModel(guide));
            Res.AddRange(new Sphere(attPointDoor, RadiusDoor, Brushes.Gray).GetGeometryModel(guide));

            Res.AddRange(new Sphere(attPointDoor, 40, Brushes.Cyan).GetGeometryModel(guide));
            

            return Res.ToArray();
        }

        //public void updateDrive(Point3D point1, Point3D point2)
        //{
        //    vDrive = point2 - point1;
        //    double vLength = vDrive.Length; 
        //    if(vLength <= extractedLength - stroke * 2/3)
        //    {
        //        generateCylinder(point1, point2 - 0.25 * vDrive, rBody, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //        generateSphere(point1, rBody, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //        generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, groupDriveVisual, new DiffuseMaterial(Brushes.YellowGreen));
        //        generateSphere(point2, rDoor, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //    }
            
        //    else if ( (vLength > extractedLength - stroke * 2/3) && !(vLength <= extractedLength - stroke * 2/3) )
        //    {
        //        generateCylinder(point1, point2 - 0.25 * vDrive, rBody, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //        generateSphere(point1, rBody, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //        generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, groupDriveVisual, new DiffuseMaterial(Brushes.Orange));
        //        generateSphere(point2, rDoor, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //    }
        //    else if( (vLength > extractedLength - stroke * 1/3) && !(vLength <= extractedLength - stroke * 2/3) )
        //    {
        //        generateCylinder(point1, point2 - 0.25 * vDrive, rBody, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //        generateSphere(point1, rBody, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //        generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, groupDriveVisual, new DiffuseMaterial(Brushes.OrangeRed));
        //        generateSphere(point2, rDoor, groupDriveVisual, new DiffuseMaterial(Brushes.Gray));
        //    }
            
        //}

       
    }
}

    