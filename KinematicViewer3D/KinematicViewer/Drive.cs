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
        private Point3D point1, point2;

        private double retractedLength;
        private double extractedLength = 648.8;
        private double stroke;

        private Vector3D vDrive;

        private Int32 rBody = 15;
        private Int32 rDoor = 25;

        public Drive(Point3D point1, Point3D point2)
        {
            this.point1 = point1;
            this.point2 = point2;

            vDrive = point2 - point1;
            retractedLength = vDrive.Length;
            stroke = extractedLength - retractedLength;
        }

        public override GeometryModel3D[] GetGeometryModel()
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Res.AddRange(new Cylinder(point1, point2 - 0.25 * vDrive, rBody, Brushes.Gray).GetGeometryModel());
            Res.AddRange(new Sphere(point1, rBody, Brushes.Gray).GetGeometryModel());
            Res.AddRange(new Cylinder(point1 + 0.25 * vDrive, point2, rDoor, Brushes.YellowGreen).GetGeometryModel());
            Res.AddRange(new Sphere(point2, rDoor, Brushes.Gray).GetGeometryModel());

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

        public double RetractedLenght
        {
            get { return retractedLength; }
            set { retractedLength = value; }
        }

        public double ExtractedLength
        {
            get { return extractedLength; }
            set { extractedLength = value; }
        }
    }
}
