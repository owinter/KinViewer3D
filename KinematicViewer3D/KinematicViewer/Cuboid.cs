using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Cuboid : GeometricalElement
    {
        private Point3D _oPointStart;
        private Point3D _oPointEnd;
        private double _dThickness;

       

        public Cuboid(Point3D startPointP1, Point3D endPointP2, double modelThickness, System.Windows.Media.Brush mat = null)
            : base(mat)
        {
            StartPointP1 = startPointP1;
            EndPointP2 = endPointP2;
            Thickness = modelThickness;
        }

        public Point3D StartPointP1
        {
            get { return _oPointStart; }
            set { _oPointStart = value; }

        }

        public Point3D EndPointP2
        {
            get { return _oPointEnd; }
            set { _oPointEnd = value; }
        }

        public double Thickness
        {
            get { return _dThickness; }
            set { _dThickness = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D[] p_around = makeVertices();

            //Berechnung aller 6 Seiten eines Rechtecks (mit bestimmten Normalenvektoren für alle Faces / Seiten)
            //Rechte Seite
            buildRectangle(mesh, p_around[0], p_around[1], p_around[5], p_around[4]); //, new Vector3D(1, 0, 0)

            //Front Seite
            buildRectangle(mesh, p_around[0], p_around[4], p_around[6], p_around[2]); //, new Vector3D(0, 0, 1)

            //Linke Seite
            buildRectangle(mesh, p_around[2], p_around[6], p_around[7], p_around[3]); //, new Vector3D(-1, 0, 0)

            //Back Seite
            buildRectangle(mesh, p_around[3], p_around[7], p_around[5], p_around[1]); //, new Vector3D(0, 0, -1)

            //Top Seite
            buildRectangle(mesh, p_around[4], p_around[5], p_around[7], p_around[6]); //, new Vector3D(0, 1, 0)

            //Bottom Seite
            buildRectangle(mesh, p_around[0], p_around[2], p_around[3], p_around[1]); //, new Vector3D(0, -1, 0)


            GeometryModel3D model = new GeometryModel3D(mesh, Material);
            model.Transform = new Transform3DGroup();

            return new GeometryModel3D[] { model };
        }

        //Rechteck erstellen und NormalenVektoren den einzelnen Seiten hinzufügen
        private void buildRectangle(MeshGeometry3D mesh, Point3D a, Point3D b, Point3D c, Point3D d) //, Vector3D normal
        {
            int baseIndex = mesh.Positions.Count;

            //Vertices hinzufügen
            mesh.Positions.Add(a);
            mesh.Positions.Add(b);
            mesh.Positions.Add(c);
            mesh.Positions.Add(d);

            //Normalen Vektoren hinzufügen
            /* mesh.Normals.Add(normal);
             mesh.Normals.Add(normal);
             mesh.Normals.Add(normal);
             mesh.Normals.Add(normal);*/

            //Indices hinzufügen
            mesh.TriangleIndices.Add(baseIndex + 0);
            mesh.TriangleIndices.Add(baseIndex + 1);
            mesh.TriangleIndices.Add(baseIndex + 2);
            mesh.TriangleIndices.Add(baseIndex + 0);
            mesh.TriangleIndices.Add(baseIndex + 2);
            mesh.TriangleIndices.Add(baseIndex + 3);
        }

        //Erstellen der Nachbarpuntke aus Benutzereingabe
        private Point3D[] makeVertices()
        {
            // Vektor zwischen Ursprung und Segment-Endpunkt berechnen
            Vector3D v = EndPointP2 - StartPointP1;

            // Breite des Segmentes entsprechend Skalieren
            Vector3D n1 = scaleVector(new Vector3D(-v.Z, -v.Z, v.X + v.Y), Thickness / 2.0);

            // Erstellt einen senkrechten skalierten Vektor zu n1
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = scaleVector(n2, Thickness / 2.0);

            // Erstellen eines kleinen dünnen Rechtecks.
            // p1pm bedeutet P1 PLUS n1 MINUS n2.
            List<Point3D> points = new List<Point3D>();

            Point3D p1pp = StartPointP1 + n1 + n2;
            Point3D p1mp = StartPointP1 - n1 + n2;
            Point3D p1pm = StartPointP1 + n1 - n2;
            Point3D p1mm = StartPointP1 - n1 - n2;
            Point3D p2pp = EndPointP2 + n1 + n2;
            Point3D p2mp = EndPointP2 - n1 + n2;
            Point3D p2pm = EndPointP2 + n1 - n2;
            Point3D p2mm = EndPointP2 - n1 - n2;

            //Punkte der Point3D List / Array hinzufügen
            points.Add(p1pp);
            points.Add(p1mp);
            points.Add(p1pm);
            points.Add(p1mm);
            points.Add(p2pp);
            points.Add(p2mp);
            points.Add(p2pm);
            points.Add(p2mm);

            return points.ToArray();
        }
        // Vektor Länge für das zu erstellende Segment
        private Vector3D scaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }
    }
}