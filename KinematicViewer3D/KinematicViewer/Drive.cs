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
        private GeometryModel3D cylinderGeometry;
        private Point3D point1, point2;
        private Cylinder cylinder;

        public Drive(Point3D point1, Point3D point2, Model3DGroup groupDriveVisual)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.groupDriveVisual = groupDriveVisual;
            makeDrive(point1, point2);
        }

        private void makeDrive(Point3D point1, Point3D point2)
        {
            generateCylinder(point1, point2);
        }

        private void generateCylinder(Point3D point1, Point3D point2)
        {
            MeshGeometry3D mesh_Cylinder = new MeshGeometry3D();
            cylinder = new Cylinder(mesh_Cylinder, point1, point2, 25, 128);

            cylinderGeometry = new GeometryModel3D(mesh_Cylinder, new DiffuseMaterial(Brushes.Red));
            cylinderGeometry.Transform = new Transform3DGroup();
            groupDriveVisual.Children.Add(cylinderGeometry);
        }
    }
}
