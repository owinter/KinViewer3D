using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Cylinder
    {
        
        public Cylinder(Viewport3D viewport, MeshGeometry3D mesh)
        {
            

            // Create a cylinder:
            CreateCylinder(new Point3D(0, 0, 0), 0, 1.2, 20, 20, Colors.LightBlue, viewport, mesh);

            // Create the other cylinder:
            CreateCylinder(new Point3D(0, 0, -4), 0.8, 1.2, 5, 20, Colors.LightCoral, viewport, mesh);

            // Create the other cylinder:
            CreateCylinder(new Point3D(-3, 0, 0), 1, 1.2, 5, 40, Colors.Red, viewport, mesh);
        }

        private Point3D GetPosition(double radius, double theta, double y)
        {
            Point3D pt = new Point3D();
            double sn = Math.Sin(theta * Math.PI / 180);
            double cn = Math.Cos(theta * Math.PI / 180);
            pt.X = radius * cn;
            pt.Y = y;
            pt.Z = -radius * sn;
            return pt;
        }

        private void CreateCylinder(Point3D center, double rin, double rout, double height, int n, Color color, Viewport3D viewport, MeshGeometry3D mesh)
        {
            if (n < 2 || rin == rout)
                return;
            double radius = rin;
            if (rin > rout)
            {
                rin = rout;
                rout = radius;
            }

            double h = height / 2;
            Model3DGroup cylinder = new Model3DGroup();
            Point3D[,] pts = new Point3D[n, 4];
            for (int i = 0; i < n; i++)
            {
                pts[i, 0] = GetPosition(rout, i * 360 / (n - 1), h);
                pts[i, 1] = GetPosition(rout, i * 360 / (n - 1), -h);
                pts[i, 2] = GetPosition(rin, i * 360 / (n - 1), -h);
                pts[i, 3] = GetPosition(rin, i * 360 / (n - 1), h);
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < 4; j++)
                    pts[i, j] += (Vector3D)center;
            }

            Point3D[] p = new Point3D[8];
            for (int i = 0; i < n - 1; i++)
            {
                p[0] = pts[i, 0];
                p[1] = pts[i, 1];
                p[2] = pts[i, 2];
                p[3] = pts[i, 3];
                p[4] = pts[i + 1, 0];
                p[5] = pts[i + 1, 1];
                p[6] = pts[i + 1, 2];
                p[7] = pts[i + 1, 3];
                // Top surface:
                CreateTriangleFace(p[0], p[4], p[3], color, viewport, mesh);
                CreateTriangleFace(p[4], p[7], p[3], color, viewport, mesh);
                // Bottom surface:
                CreateTriangleFace(p[1], p[5], p[2], color, viewport, mesh);
                CreateTriangleFace(p[5], p[6], p[2], color, viewport, mesh);
                // Outer surface:
                CreateTriangleFace(p[0], p[1], p[4], color, viewport, mesh);
                CreateTriangleFace(p[1], p[5], p[4], color, viewport, mesh);
                // Inner surface:
                CreateTriangleFace(p[2], p[7], p[6], color, viewport, mesh);
                CreateTriangleFace(p[2], p[3], p[7], color, viewport, mesh);
            }
        }

        private void CreateTriangleFace(Point3D p0, Point3D p1, Point3D p2,Color color, Viewport3D viewport, MeshGeometry3D mesh)
        {
            //MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color;
            Material material = new DiffuseMaterial(brush);
            GeometryModel3D geometry = new GeometryModel3D(mesh, material);
            ModelUIElement3D model = new ModelUIElement3D();
            model.Model = geometry;
            viewport.Children.Add(model);
            
        }
    }
}
