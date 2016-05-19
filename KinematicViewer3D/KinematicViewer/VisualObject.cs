using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public abstract class VisualObject
    {
        protected Model3DGroup groupModelVisual, groupDriveVisual;
        protected GeometryModel3D cuboidGeometry, cylinderGeometry, sphereGeometry;

        protected Drive drive;
        protected Cuboid cube;
        protected Cuboid2 cube2;
        protected Cylinder cylinder;
        protected Sphere sphere;

        protected void generateSphere(Point3D point, double modelThickness, Model3DGroup vgroup, DiffuseMaterial mat)
        {
            MeshGeometry3D mesh_Sphere = new MeshGeometry3D();
            sphere = new Sphere(point, modelThickness, mesh_Sphere);

            sphereGeometry = new GeometryModel3D(mesh_Sphere, mat);
            sphereGeometry.Transform = new Transform3DGroup();
            vgroup.Children.Add(sphereGeometry);
        }

        protected void generateCuboid(Point3D point1, Point3D point2, double modelThickness, Model3DGroup vgroup)
        {
            MeshGeometry3D mesh_Cuboid = new MeshGeometry3D();
            cube = new Cuboid(point1, point2, mesh_Cuboid, modelThickness);
            //cube2 = new Cuboid2(point1, point2, mesh_Cuboid, modelThickness);

            cuboidGeometry = new GeometryModel3D(mesh_Cuboid, new DiffuseMaterial(Brushes.Cyan));
            cuboidGeometry.Transform = new Transform3DGroup();
            vgroup.Children.Add(cuboidGeometry);
        }

        protected void generateCylinder(Point3D point1, Point3D point2, int radius, Model3DGroup vgroup, DiffuseMaterial mat)
        {
            MeshGeometry3D mesh_Cylinder = new MeshGeometry3D();
            cylinder = new Cylinder(mesh_Cylinder, point1, point2, radius, 128);

            cylinderGeometry = new GeometryModel3D(mesh_Cylinder, mat);
            cylinderGeometry.Transform = new Transform3DGroup();
            vgroup.Children.Add(cylinderGeometry);
        }

        protected void generateVisualAxisOfRotation(Point3D point1, Point3D point2, int radius, Model3DGroup vgroup, DiffuseMaterial mat)
        {
            MeshGeometry3D mesh_VisualAxisOfRotaion = new MeshGeometry3D();
            cylinder = new Cylinder(mesh_VisualAxisOfRotaion, point1, point2, radius, 128);

            cylinderGeometry = new GeometryModel3D(mesh_VisualAxisOfRotaion, mat);
            cylinderGeometry.Transform = new Transform3DGroup();
            vgroup.Children.Add(cylinderGeometry);
        }

        //Skaliere einen Vektor mit einem offset Wert
        protected Vector3D scaleToOffset(Vector3D vScale, double value)
        {
            vScale.Normalize();
            vScale = vScale * value;
            return vScale;
        }

        //Rotiere einen neuen Punkt um einen bestehenden und eine Achse
        protected Point3D rotateNewPoint(Vector3D v, double angle, Point3D point)
        {
            Point3D p = new Point3D();
            RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(v, angle));
            p = rotation.Transform(point);
            return p;
        }




        //Getter & Setter
        protected Model3DGroup GroupModelVisual
        {
            get { return groupModelVisual; }
            set { groupModelVisual = value; }
        }

        protected Model3DGroup GroupDriveVisual
        {
            get { return groupDriveVisual; }
            set { groupDriveVisual = value; }
        }
    }
}
