using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Cylinder
    {
        public Cylinder(MeshGeometry3D mesh, Point3D end_point, Point3D start_point, double radius, int num_sides)
        {
            Vector3D axis = start_point - end_point;
            AddCylinder(mesh, end_point, axis, radius, num_sides);
        }

        // Add a cylinder.
        private void AddCylinder(MeshGeometry3D mesh, Point3D end_point, Vector3D axis, double radius, int num_sides)
        {
            // Berechnung von zwei senkrechten Vektoren
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);

            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Top Seite erstellen
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                AddTriangle(mesh, end_point, p1, p2);
            }

            // Bottom Seite erstellen
            Point3D end_point2 = end_point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                AddTriangle(mesh, end_point2, p2, p1);
            }

            // Seiten erstellen
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = end_point + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;

                Point3D p3 = p1 + axis;
                Point3D p4 = p2 + axis;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }
        }

        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }
    }
}
