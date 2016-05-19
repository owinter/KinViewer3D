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

            //Berechnung aller 6 Seiten eines Rechtecks (mit bestimmten Normalenvektoren für alle Faces / Seiten)
            //Rechte Seite
            buildRectangle(mesh, p_around[0], p_around[1], p_around[5], p_around[4]); //, new Vector3D(1, 0, 0)

            //Front Seite
            buildRectangle(mesh, p_around[0], p_around[4], p_around[6], p_around[2] ); //, new Vector3D(0, 0, 1)

            //Linke Seite
            buildRectangle(mesh, p_around[2], p_around[6], p_around[7], p_around[3]); //, new Vector3D(-1, 0, 0)

            //Back Seite
            buildRectangle(mesh, p_around[3], p_around[7], p_around[5], p_around[1]); //, new Vector3D(0, 0, -1)

            //Top Seite
            buildRectangle(mesh, p_around[4], p_around[5], p_around[7], p_around[6]); //, new Vector3D(0, 1, 0)

            //Bottom Seite
            buildRectangle(mesh, p_around[0], p_around[2], p_around[3], p_around[1]); //, new Vector3D(0, -1, 0)
        }



        //Erstellen der Nachbarpuntke aus Benutzereingabe
        private Point3D[] makeVertices(Point3D point1, Point3D point2, double modelThickness)
        {
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

            return points.ToArray();
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




        //Alternativer Cuboid
        /*
         * 
         * 
         * 
         * 
         * public Cuboid()
        {
            PropertyChanged(this, new DependencyPropertyChangedEventArgs());
        }
        // Width property.
        // ---------------
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width",
                typeof(double), typeof(Cuboid),
                new PropertyMetadata(1.0, PropertyChanged));

        public double Width
        {
            set { SetValue(WidthProperty, value); }
            get { return (double)GetValue(WidthProperty); }
        }

        // Height property.
        // ----------------
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height",
                typeof(double), typeof(Cuboid),
                new PropertyMetadata(1.0, PropertyChanged));

        public double Height
        {
            set { SetValue(HeightProperty, value); }
            get { return (double)GetValue(HeightProperty); }
        }

        // Depth property.
        // ---------------
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth",
                typeof(double), typeof(Cuboid),
                new PropertyMetadata(1.0, PropertyChanged));

        public double Depth
        {
            set { SetValue(DepthProperty, value); }
            get { return (double)GetValue(DepthProperty); }
        }

        // Origin property.
        // ----------------

        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin",
                typeof(Point3D), typeof(Cuboid),
                new PropertyMetadata(new Point3D(-0.5, -0.5, -0.5),
                                     PropertyChanged));

        public Point3D Origin
        {
            set { SetValue(OriginProperty, value); }
            get { return (Point3D)GetValue(OriginProperty); }
        }


        // Slices property.
        // ----------------
        public static readonly DependencyProperty SlicesProperty =
            DependencyProperty.Register("Slices", typeof(int), typeof(Cuboid),
                new PropertyMetadata(10, PropertyChanged),
                    ValidateSlices);

        public int Slices
        {
            get { return (int)GetValue(SlicesProperty); }
            set { SetValue(SlicesProperty, value); }
        }

        // Stacks property.
        // ----------------
        public static readonly DependencyProperty StacksProperty =
            DependencyProperty.Register("Stacks", typeof(int), typeof(Cuboid),
                new PropertyMetadata(10, PropertyChanged),
                    ValidateSlices);

        public int Stacks
        {
            get { return (int)GetValue(StacksProperty); }
            set { SetValue(StacksProperty, value); }
        }

        // Slivers property.
        // ----------------
        public static readonly DependencyProperty SliversProperty =
            DependencyProperty.Register("Slivers", typeof(int), typeof(Cuboid),
                new PropertyMetadata(10, PropertyChanged),
                    ValidateSlices);

        public int Slivers
        {
            get { return (int)GetValue(SliversProperty); }
            set { SetValue(SliversProperty, value); }
        }

        static bool ValidateSlices(object obj)
        {
            return (int)obj > 0;
        }


        protected override void Triangulate(DependencyPropertyChangedEventArgs args,
                                            Point3DCollection vertices, 
                                            Vector3DCollection normals, 
                                            Int32Collection indices, 
                                            PointCollection textures)
        {
            vertices.Clear();
            normals.Clear();
            indices.Clear();
            textures.Clear();

            // Front.
            for (int iy = 0; iy <= Stacks; iy++)
            {
                double y = Origin.Y + Height - iy * Height / Stacks;

                for (int ix = 0; ix <= Slices; ix++)
                {
                    double x = Origin.X + ix * Width / Slices;
                    vertices.Add(new Point3D(x, y, Origin.Z + Depth));
                }
            }

            // Back
            for (int iy = 0; iy <= Stacks; iy++)
            {
                double y = Origin.Y + Height - iy * Height / Stacks;

                for (int ix = 0; ix <= Slices; ix++)
                {
                    double x = Origin.X + Width - ix * Width / Slices;
                    vertices.Add(new Point3D(x, y, Origin.Z));
                }
            }

            // Left
            for (int iy = 0; iy <= Stacks; iy++)
            {
                double y = Origin.Y + Height - iy * Height / Stacks;

                for (int iz = 0; iz <= Slivers; iz++)
                {
                    double z = Origin.Z + iz * Depth / Slivers;
                    vertices.Add(new Point3D(Origin.X, y, z));
                }
            }

            // Right
            for (int iy = 0; iy <= Stacks; iy++)
            {
                double y = Origin.Y + Height - iy * Height / Stacks;

                for (int iz = 0; iz <= Slivers; iz++)
                {
                    double z = Origin.Z + Depth - iz * Depth / Slivers;
                    vertices.Add(new Point3D(Origin.X + Width, y, z));
                }
            }

            // Top
            for (int iz = 0; iz <= Slivers; iz++)
            {
                double z = Origin.Z + iz * Depth / Slivers;

                for (int ix = 0; ix <= Slices; ix++)
                {
                    double x = Origin.X + ix * Width / Slices;
                    vertices.Add(new Point3D(x, Origin.Y + Height, z));
                }
            }

            // Top
            for (int iz = 0; iz <= Slivers; iz++)
            {
                double z = Origin.Z + Depth - iz * Depth / Slivers;

                for (int ix = 0; ix <= Slices; ix++)
                {
                    double x = Origin.X + ix * Width / Slices;
                    vertices.Add(new Point3D(x, Origin.Y, z));
                }
            }

            for (int side = 0; side < 6; side++)
            {
                for (int iy = 0; iy <= Stacks; iy++)
                {
                    double y = (double)iy / Stacks;

                    for (int ix = 0; ix <= Slices; ix++)
                    {
                        double x = (double)ix / Slices;
                        textures.Add(new Point(x, y));
                    }
                }
            }

            // Front, back, left, right
            for (int side = 0; side < 6; side++)
            {
                for (int iy = 0; iy < Stacks; iy++)
                    for (int ix = 0; ix < Slices; ix++)
                    {
                        indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix);
                        indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix);
                        indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1);

                        indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1);
                        indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix);
                        indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix + 1);
                    }
            }
        }*/



    }
}
