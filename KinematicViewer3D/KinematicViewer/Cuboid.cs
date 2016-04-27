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
        private GeometryModel3D modelGeometry;
        private MeshGeometry3D mesh;
        private Model3DGroup group;

        public Cuboid(GeometryModel3D modelGeometry, MeshGeometry3D mesh, Model3DGroup group)
        {
            this.modelGeometry = modelGeometry;
            this.mesh = mesh;
            this.group = group;
        }

        public void buildCube()
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
        }
    }
}
