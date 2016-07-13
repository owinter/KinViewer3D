//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows;
//using System.Windows.Media;
//using System.Windows.Media.Media3D;

//namespace KinematicViewer
//{
//    public class Cuboid2
//    {

//        private double width;
//        private double height;
//        private double depth;

//        private Int32 stacks = 4;
//        private Int32 slices = 4;
//        private Int32 slivers =4;


//        private Point3D origin;

//        private Point3DCollection vertices = new Point3DCollection();
//        private Int32Collection indices = new Int32Collection();
//        private PointCollection textures = new PointCollection();


//        public Cuboid2(Point3D point1, Point3D point2, MeshGeometry3D mesh, double modelThickness)
//        {
//            this.Height = (point2 - point1).Length;
//            this.Width = modelThickness /2;
//            this.Depth = modelThickness /2;
//            this.Origin = point1;
//            this.vertices = new Point3DCollection();
//            this.indices = new Int32Collection();
//            this.textures = new PointCollection();

//            buildCuboid(mesh);
//            //showCuboid(mesh);
//        }
//        // Width property.
//        public double Width
//        {
//            set { width= value; }
//            get { return width; }
//        }

//        // Height property.

//        public double Height
//        {
//            set { height = value; }
//            get { return height; }
//        }

//        // Depth property.
//        public double Depth
//        {
//            set { depth = value; }
//            get { return depth; }
//        }

//        // Origin property.
//        public Point3D Origin
//        {
//            set { origin = value; }
//            get { return origin; }
//        }


//        // Slices property.
//        public Int32 Slices
//        {
//            get { return slices; }
//            set { slices = value; }
//        }

//        // Stacks property.
//        public Int32 Stacks
//        {
//            get { return stacks; }
//            set { stacks = value; }
//        }

//        // Slivers property.
//        public Int32 Slivers
//        {
//            get { return slivers; }
//            set {slivers = value; }
//        }

//        static bool ValidateSlices(object obj)
//        {
//            return (int)obj > 0;
//        }

//        private void showCuboid(MeshGeometry3D mesh)
//        {
//            foreach (Point3D vertex in mesh.Positions)
//                vertices.Add(vertex);


//            foreach (int index in mesh.TriangleIndices)
//                indices.Add(index);
//        }


//        private void buildCuboid(MeshGeometry3D mesh)
//        {
//             vertices.Clear();
//             indices.Clear();
//             textures.Clear();

            

//            // Front.
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + ix * Width / Slices;
//                    vertices.Add(new Point3D(x, y, Origin.Z + Depth));
//                }
//            }

//            // Back
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + Width - ix * Width / Slices;
//                    vertices.Add(new Point3D(x, y, Origin.Z));
//                }
//            }

//            // Left
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int iz = 0; iz <= Slivers; iz++)
//                {
//                    double z = Origin.Z + iz * Depth / Slivers;
//                    vertices.Add(new Point3D(Origin.X, y, z));
//                }
//            }

//            // Right
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int iz = 0; iz <= Slivers; iz++)
//                {
//                    double z = Origin.Z + Depth - iz * Depth / Slivers;
//                    vertices.Add(new Point3D(Origin.X + Width, y, z));
//                }
//            }

//            // Top
//            for (int iz = 0; iz <= Slivers; iz++)
//            {
//                double z = Origin.Z + iz * Depth / Slivers;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + ix * Width / Slices;
//                    vertices.Add(new Point3D(x, Origin.Y + Height, z));
//                }
//            }

//            // Top
//            for (int iz = 0; iz <= Slivers; iz++)
//            {
//                double z = Origin.Z + Depth - iz * Depth / Slivers;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + ix * Width / Slices;
//                    vertices.Add(new Point3D(x, Origin.Y, z));
//                }
//            }

//            for (int side = 0; side < 6; side++)
//            {
//                for (int iy = 0; iy <= Stacks; iy++)
//                {
//                    double y = (double)iy / Stacks;

//                    for (int ix = 0; ix <= Slices; ix++)
//                    {
//                        double x = (double)ix / Slices;
//                        //textures.Add(new Point(x, y));
//                    }
//                }
//            }









//            // Front.
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + ix * Width / Slices;
//                    mesh.Positions.Add(new Point3D(x, y, Origin.Z + Depth));
//                }
//            }

//            // Back
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + Width - ix * Width / Slices;
//                    mesh.Positions.Add(new Point3D(x, y, Origin.Z));
//                }
//            }

//            // Left
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int iz = 0; iz <= Slivers; iz++)
//                {
//                    double z = Origin.Z + iz * Depth / Slivers;
//                    mesh.Positions.Add(new Point3D(Origin.X, y, z));
//                }
//            }

//            // Right
//            for (int iy = 0; iy <= Stacks; iy++)
//            {
//                double y = Origin.Y + Height - iy * Height / Stacks;

//                for (int iz = 0; iz <= Slivers; iz++)
//                {
//                    double z = Origin.Z + Depth - iz * Depth / Slivers;
//                    mesh.Positions.Add(new Point3D(Origin.X + Width, y, z));
//                }
//            }

//            // Top
//            for (int iz = 0; iz <= Slivers; iz++)
//            {
//                double z = Origin.Z + iz * Depth / Slivers;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + ix * Width / Slices;
//                    mesh.Positions.Add(new Point3D(x, Origin.Y + Height, z));
//                }
//            }

//            // Top
//            for (int iz = 0; iz <= Slivers; iz++)
//            {
//                double z = Origin.Z + Depth - iz * Depth / Slivers;

//                for (int ix = 0; ix <= Slices; ix++)
//                {
//                    double x = Origin.X + ix * Width / Slices;
//                    mesh.Positions.Add(new Point3D(x, Origin.Y, z));
//                }
//            }










//            // Front, back, left, right
//            for (int side = 0; side < 6; side++)
//            {
//                for (int iy = 0; iy < Stacks; iy++)
//                    for (int ix = 0; ix < Slices; ix++)
//                    {
//                        indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix);
//                        indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix);
//                        indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1);

//                        indices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1);
//                        indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix);
//                        indices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix + 1);
//                    }
//            }

//            for (int side = 0; side < 6; side++)
//            {
//                for (int iy = 0; iy < Stacks; iy++)
//                    for (int ix = 0; ix < Slices; ix++)
//                    {
//                        mesh.TriangleIndices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix);
//                        mesh.TriangleIndices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix);
//                        mesh.TriangleIndices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1);

//                        mesh.TriangleIndices.Add(side * (Slices + 1) * (Stacks + 1) + iy * (Slices + 1) + ix + 1);
//                        mesh.TriangleIndices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix);
//                        mesh.TriangleIndices.Add(side * (Slices + 1) * (Stacks + 1) + (iy + 1) * (Slices + 1) + ix + 1);
//                    }
//            }


           


//        }

//    }
//}
