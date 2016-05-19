using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Drive
    {
        private Model3DGroup groupDriveVisual;
        private GeometryModel3D cylinderGeometry, sphereGeometry;
        private Point3D point1, point2;
        private Cylinder cylinder;
        private Sphere sphere;

        private double retractedLength;
        private double extractedLength = 648.8;
        private double stroke;

        private Vector3D vDrive;

        private Int32 rBody = 15;
        private Int32 rDoor = 25;

        public Drive(Point3D point1, Point3D point2, Model3DGroup groupDriveVisual)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.groupDriveVisual = groupDriveVisual;

            vDrive = point2 - point1;
            retractedLength = vDrive.Length;
            stroke = extractedLength - retractedLength;
            makeDrive();
        }

        private void makeDrive()
        {
            generateCylinder(point1, point2 - 0.25 * vDrive, rBody, new DiffuseMaterial(Brushes.Gray));
            generateSphere(point1, rBody, new DiffuseMaterial(Brushes.Gray));
            generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, new DiffuseMaterial(Brushes.YellowGreen));
            generateSphere(point2, rDoor, new DiffuseMaterial(Brushes.Gray));
        }

        private void generateCylinder(Point3D point1, Point3D point2, Int32 radius, DiffuseMaterial mat)
        {
            MeshGeometry3D mesh_Cylinder = new MeshGeometry3D();
            cylinder = new Cylinder(mesh_Cylinder, point1, point2, radius, 128);

            cylinderGeometry = new GeometryModel3D(mesh_Cylinder, mat);
            cylinderGeometry.Transform = new Transform3DGroup();
            groupDriveVisual.Children.Add(cylinderGeometry);
        }

        private void generateSphere(Point3D point, double radius, DiffuseMaterial mat)
        {
            MeshGeometry3D mesh_Sphere = new MeshGeometry3D();
            sphere = new Sphere(point, radius, mesh_Sphere);

            sphereGeometry = new GeometryModel3D(mesh_Sphere, mat);
            sphereGeometry.Transform = new Transform3DGroup();
            groupDriveVisual.Children.Add(sphereGeometry);
        }

        public void updateDrive(Point3D point1, Point3D point2)
        {
            vDrive = point2 - point1;
            double vLength = vDrive.Length; 
            if(vLength <= extractedLength - stroke * 2/3)
            {
                generateCylinder(point1, point2 - 0.25 * vDrive, rBody, new DiffuseMaterial(Brushes.Gray));
                generateSphere(point1, rBody, new DiffuseMaterial(Brushes.Gray));
                generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, new DiffuseMaterial(Brushes.YellowGreen));
                generateSphere(point2, rDoor, new DiffuseMaterial(Brushes.Gray));
            }
            
            else if ( (vLength > extractedLength - stroke * 2/3) && !(vLength <= extractedLength - stroke * 2/3) )
            {
                generateCylinder(point1, point2 - 0.25 * vDrive, rBody, new DiffuseMaterial(Brushes.Gray));
                generateSphere(point1, rBody, new DiffuseMaterial(Brushes.Gray));
                generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, new DiffuseMaterial(Brushes.Orange));
                generateSphere(point2, rDoor, new DiffuseMaterial(Brushes.Gray));
            }
            else if( (vLength > extractedLength - stroke * 1/3) && !(vLength <= extractedLength - stroke * 2/3) )
            {
                generateCylinder(point1, point2 - 0.25 * vDrive, rBody, new DiffuseMaterial(Brushes.Gray));
                generateSphere(point1, rBody, new DiffuseMaterial(Brushes.Gray));
                generateCylinder(point1 + 0.25 * vDrive, point2, rDoor, new DiffuseMaterial(Brushes.OrangeRed));
                generateSphere(point2, rDoor, new DiffuseMaterial(Brushes.Gray));
            }
            
        }

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
