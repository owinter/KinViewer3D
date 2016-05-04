using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Cuboid
    {       
         public Cuboid(Point3D point1, Point3D point2, MeshGeometry3D mesh, double modelThickness)
        {
            buildCuboid(point1, point2, mesh, modelThickness);
        }

        //Erstellen des 3D Modell
        private void buildCuboid(Point3D point1, Point3D point2, MeshGeometry3D mesh, double modelThickness)
        {

            //Umgebungspunkte berechnen
            Point3D[] p_around = makeVertices(point1, point2, modelThickness);

            //Berechnung aller 6 Seiten eines Rechtecks mit jeweiligen Normalenvektoren für alle Faces / Seiten
            //Rechte Seite
            buildRectangle(mesh, p_around[0], p_around[1], p_around[5], p_around[4], new Vector3D(1, 0, 0));

            //Front Seite
            buildRectangle(mesh, p_around[0], p_around[4], p_around[6], p_around[2], new Vector3D(0, 0, 1));

            //Linke Seite
            buildRectangle(mesh, p_around[2], p_around[6], p_around[7], p_around[3], new Vector3D(-1, 0, 0));

            //Back Seite
            buildRectangle(mesh, p_around[3], p_around[7], p_around[5], p_around[1], new Vector3D(0, 0, -1));

            //Top Seite
            buildRectangle(mesh, p_around[4], p_around[5], p_around[7], p_around[6], new Vector3D(0, 1, 0));

            //Bottom Seite
            buildRectangle(mesh, p_around[0], p_around[2], p_around[3], p_around[1], new Vector3D(0, -1, 0));
        }



        //Erstellen der Nachbarpuntke aus Benutzereingabe
        private Point3D[] makeVertices(Point3D point1, Point3D point2, double modelThickness)
        {

            /* //Punkte durch Benutzereingabe
             Point3D point1 = new Point3D(x1, y1, z1);
             Point3D point2 = new Point3D(x2, y2, z2);*/

            /*//Mittelpunkt des Models berechnen
            mPoint = new Point3D((point1.X + point2.X) / 2,
                                 (point1.Y + point2.Y) / 2,
                                 (point1.Z + point2.Z) / 2);*/


            // Vektor zwischen Ursprung und Segment-Endpunkt berechnen
            Vector3D v = point2 - point1;

            // Breite des Segmentes entsprechend Skalieren
            Vector3D n1 = scaleVector(new Vector3D(-v.Z, -v.Z, v.X + v.Y), modelThickness / 2.0);

            // Erstellt einen senkrechten skalierten Vektor zu n1
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = scaleVector(n2, modelThickness / 2.0);

            // Erstellen eines kleinen dünnen Rechtecks.
            // p1pm bedeutet point1 PLUS n1 MINUS n2.
            List<Point3D> points = new List<Point3D>();

            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            //Punkte der Point3D List / Array hinzufügen
            points.Add(p1pp);
            points.Add(p1mp);
            points.Add(p1pm);
            points.Add(p1mm);
            points.Add(p2pp);
            points.Add(p2mp);
            points.Add(p2pm);
            points.Add(p2mm);


            /*   points.Add(new Point3D(x1 + modelThickness / 2, y1, z1 + modelThickness / 2));
               points.Add(new Point3D(x1 + modelThickness / 2, y1, z1 - modelThickness / 2));
               points.Add(new Point3D(x1 - modelThickness / 2, y1, z1 - modelThickness / 2));
               points.Add(new Point3D(x1 - modelThickness / 2, y1, z1 + modelThickness / 2));
               points.Add(new Point3D(x2 + modelThickness / 2, y2, z2 + modelThickness / 2));
               points.Add(new Point3D(x2 + modelThickness / 2, y2, z2 - modelThickness / 2));
               points.Add(new Point3D(x2 - modelThickness / 2, y2, z2 - modelThickness / 2));
               points.Add(new Point3D(x2 - modelThickness / 2, y2, z2 + modelThickness / 2));*/

            //VerifyEdgeLengths(2, points.ToArray());

            return points.ToArray();
        }


        //Rechteck erstellen und NormalenVektoren den einzelnen Seiten hinzufügen
        private void buildRectangle(MeshGeometry3D mesh, Point3D a, Point3D b, Point3D c, Point3D d, Vector3D normal)
        {
            int baseIndex = mesh.Positions.Count;

            //Vertices hinzufügen
            mesh.Positions.Add(a);
            mesh.Positions.Add(b);
            mesh.Positions.Add(c);
            mesh.Positions.Add(d);

            //Normalen Vektoren hinzufügen
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            //Indices hinzufügen
            mesh.TriangleIndices.Add(baseIndex + 0);
            mesh.TriangleIndices.Add(baseIndex + 1);
            mesh.TriangleIndices.Add(baseIndex + 2);
            mesh.TriangleIndices.Add(baseIndex + 0);
            mesh.TriangleIndices.Add(baseIndex + 2);
            mesh.TriangleIndices.Add(baseIndex + 3);
        }

        // Vektor Länge für das zu erstellende Segment
        private Vector3D scaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        












        /*
        public void buildCube(MeshGeometry3D mesh, GeometryModel3D modelGeometry, Model3DGroup group)
        {
            mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-100, -100, 100));
            //mesh.Normals.Add(new Vector3D(0, 0, 1)); 
            mesh.Positions.Add(new Point3D(100, -100, 100));
            //mesh.Normals.Add(new Vector3D(0, 0, 1)); 
            mesh.Positions.Add(new Point3D(100, 100, 100));
            //mesh.Normals.Add(new Vector3D(0, 0, 1)); 
            mesh.Positions.Add(new Point3D(-100, 100, 100));
            //mesh.Normals.Add(new Vector3D(0, 0, 1)); 


            mesh.Positions.Add(new Point3D(-100, -100, -100));
            //mesh.Normals.Add(new Vector3D(0, 0, -1)); 
            mesh.Positions.Add(new Point3D(100, -100, -100));
            //mesh.Normals.Add(new Vector3D(0, 0, -1)); 
            mesh.Positions.Add(new Point3D(100, 100, -100));
            //mesh.Normals.Add(new Vector3D(0, 0, -1)); 
            mesh.Positions.Add(new Point3D(-100, 100, -100));
            //mesh.Normals.Add(new Vector3D(0, 0, -1));

            // Bottom Seite --> gegen Uhrzeigersinn 
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);

            // Top Seite --> gegen Uhrzeigersinn 
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Back Seite --> gegen Uhrzeigersinn 
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(1);

            // Linke Seite --> gegen Uhrzeigersinn 
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(7);

            // Rechte Seite --> gegen Uhrzeigersinn 
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);

            // Front Seite --> gegen Uhrzeigersinn 
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);

            // Geometry creation 
            modelGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Cyan));
            modelGeometry.BackMaterial = new DiffuseMaterial(Brushes.Red);
            modelGeometry.Transform = new Transform3DGroup();
            group.Children.Add(modelGeometry);

            
        }*/
    }
}
