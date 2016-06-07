using System;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Cylinder : GeometricalElement
    {
        //Standard Wert der Seitenanzahl an Rechtecken
        public const int STANDARD_NUM_SIDES = 128;

        //aktuell genutzte Seitenanzahl
        private int _iSides;

        private double _dRadius;

        private Point3D _oPointStart;
        private Point3D _oPointEnd;

        public Cylinder(Point3D startPoint, Point3D endPoint, double radius, Material mat = null)
            : base(mat)
        {
            EndPoint = endPoint;
            StartPoint = startPoint;
            Radius = radius;
            Sides = STANDARD_NUM_SIDES;
        }

        public Point3D StartPoint
        {
            get { return _oPointEnd; }
            set { _oPointEnd = value; }
        }

        public Point3D EndPoint
        {
            get { return _oPointStart; }
            set { _oPointStart = value; }
        }

        public double Radius
        {
            get { return _dRadius; }
            set { _dRadius = value; }
        }

        public int Sides
        {
            get { return _iSides; }
            set { _iSides = value; }
        }

        // Cyliner erzeugen.
        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            Vector3D vAxis = EndPoint - StartPoint;
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Berechnung von zwei senkrechten Vektoren
            Vector3D v1;
            if ((vAxis.Z < -0.01) || (vAxis.Z > 0.01))
                v1 = new Vector3D(vAxis.Z, vAxis.Z, -vAxis.X - vAxis.Y);
            else
                v1 = new Vector3D(-vAxis.Y - vAxis.Z, vAxis.X, vAxis.X);

            Vector3D v2 = Vector3D.CrossProduct(v1, vAxis);

            // V1 und V2 auf die Länge des Radius skalieren.
            v1 *= (Radius / v1.Length);
            v2 *= (Radius / v2.Length);

            // Top Seite erstellen
            double theta = 0;
            double dtheta = 2 * Math.PI / Sides;
            for (int i = 0; i < Sides; i++)
            {
                Point3D p1 = StartPoint + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = StartPoint + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                AddTriangle(mesh, StartPoint, p1, p2);
            }

            // Bottom Seite erstellen
            Point3D PointEnd2 = StartPoint + vAxis;
            theta = 0;
            for (int i = 0; i < Sides; i++)
            {
                Point3D p1 = PointEnd2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = PointEnd2 + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                AddTriangle(mesh, PointEnd2, p2, p1);
            }

            //  Seitenummantellung erstellen
            theta = 0;
            for (int i = 0; i < Sides; i++)
            {
                Point3D p1 = StartPoint + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;
                theta += dtheta;
                Point3D p2 = StartPoint + Math.Cos(theta) * v1 + Math.Sin(theta) * v2;

                Point3D p3 = p1 + vAxis;
                Point3D p4 = p2 + vAxis;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }

            //Geometry aus Mesh erzeugen
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