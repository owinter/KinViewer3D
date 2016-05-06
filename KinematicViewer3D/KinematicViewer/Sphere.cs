using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Sphere
    {
        private int slices = 32;
        private int stacks = 16;
        private Point3D center = new Point3D(); // Mittelpunkt 
        private double radius = 100; // in Millimeter

        public Sphere(Point3D center , double radius, MeshGeometry3D mesh)
        {
            this.center = center;
            this.radius = radius /2;
            buildSphere(mesh);
            
        }

        public int Slices
        {
            get { return slices; }
            set { slices = value; }
        }

        public int Stacks
        {
            get { return stacks; }
            set { stacks = value; }
        }

        public Point3D Center
        {
            get { return center; }
            set { center = value; }
        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

       /* public MeshGeometry3D SphereGeometry
        {
            get { return CalculateMesh(); }
        }*/
        

        private void buildSphere(MeshGeometry3D mesh)
        {
            for (int stack = 0; stack <= Stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / Stacks; 
                double y = radius * Math.Sin(phi); 
                double scale = -radius * Math.Cos(phi);

                for (int slice = 0; slice <= Slices; slice++)
                {
                    double theta = slice * 2 * Math.PI / Slices; 
                    double x = scale * Math.Sin(theta); 
                    double z = scale * Math.Cos(theta); 

                    Vector3D normal = new Vector3D(x, y, z); 
                    mesh.Normals.Add(normal);
                    mesh.Positions.Add(normal + Center);      
                    mesh.TextureCoordinates.Add(new Point((double)slice / Slices, (double)stack / Stacks));
                }
            }

            for (int stack = 0; stack <= Stacks; stack++)
            {
                int top = (stack + 0) * (Slices + 1);
                int bot = (stack + 1) * (Slices + 1);

                for (int slice = 0; slice < Slices; slice++)
                {
                    if (stack != 0)
                    {
                        mesh.TriangleIndices.Add(top + slice);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(top + slice + 1);
                    }

                    if (stack != Stacks - 1)
                    {
                        mesh.TriangleIndices.Add(top + slice + 1);
                        mesh.TriangleIndices.Add(bot + slice);
                        mesh.TriangleIndices.Add(bot + slice + 1);
                    }
                }
            }
           // return mesh;
        }
    }
}
