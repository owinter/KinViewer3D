using System;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Cylinder : GeometricalElement
    {
        public const int STANDARD_NUM_SIDES = 128;

        private double _dRadius;

        private int _iSides;

        private Point3D
            _oPointStart,
            _oPointEnd;

        public Cylinder(Point3D end_point, Point3D start_point, double radius, System.Windows.Media.Brush mat = null)
            : base(mat)
        {
            PointStart = start_point;
            PointEnd = end_point;
            Radius = radius;
            Sides = STANDARD_NUM_SIDES;
        }

        public Point3D PointEnd
        {
            get
            {
                return _oPointEnd;
            }

            set
            {
                _oPointEnd = value;
            }
        }

        public Point3D PointStart
        {
            get
            {
                return _oPointStart;
            }

            set
            {
                _oPointStart = value;
            }
        }

        public double Radius
        {
            get
            {
                return _dRadius;
            }

            set
            {
                _dRadius = value;
            }
        }

        public int Sides
        {
            get
            {
                return _iSides;
            }

            set
            {
                _iSides = value;
            }
        }

        // Add a cylinder.
        public override GeometryModel3D[] GetGeometryModel()
        {
            Vector3D axis = PointStart - PointEnd;
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Berechnung von zwei senkrechten Vektoren
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);

            Vector3D v2 = Vector3D.CrossProduct(v1, axis);

            // Make the vectors have length Radius.
            v1 *= (Radius / v1.Length);
            v2 *= (Radius / v2.Length);

            // Top Seite erstellen
            double theta = 0;
            double dtheta = 2 * Math.PI / Sides;
            for (int i = 0; i < Sides; i++)
            {
                Point3D p1 = PointEnd + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = PointEnd + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                AddTriangle(mesh, PointEnd, p1, p2);
            }

            // Bottom Seite erstellen
            Point3D PointEnd2 = PointEnd + axis;
            theta = 0;
            for (int i = 0; i < Sides; i++)
            {
                Point3D p1 = PointEnd2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = PointEnd2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                AddTriangle(mesh, PointEnd2, p2, p1);
            }

            // Seiten erstellen
            theta = 0;
            for (int i = 0; i < Sides; i++)
            {
                Point3D p1 = PointEnd + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = PointEnd + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;

                Point3D p3 = p1 + axis;
                Point3D p4 = p2 + axis;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }


            GeometryModel3D model = new GeometryModel3D(mesh, Material);
            model.Transform = new Transform3DGroup();

            return new GeometryModel3D[] { model };
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