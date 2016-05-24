﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Sphere : GeometricalElement
    {
        private Point3D _oPointCenter;

        private double _dRadius;

        private int _iSlices;
        private int _iStacks;

        public Sphere(Point3D center, double diameter, System.Windows.Media.Brush mat = null)
            :base(mat)
        {
            Center = center;
            Radius = diameter / 2;
            Slices = 32;
            Stacks = 16;
        }

        /// <summary>
        /// Mittelpunkt
        /// </summary>
        public Point3D Center
        {
            get { return _oPointCenter; }
            set { _oPointCenter = value; }
        }

        public double Radius
        {
            get { return _dRadius; }
            set { _dRadius = value; }
        }

        public int Slices
        {
            get { return _iSlices; }
            set { _iSlices = value; }
        }
        public int Stacks
        {
            get { return _iStacks; }
            set { _iStacks = value; }
        }

        public Point3D getPosition()
        {
            return Center;
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int stack = 0; stack <= Stacks; stack++)
            {
                double phi = Math.PI / 2 - stack * Math.PI / Stacks;
                double y = _dRadius * Math.Sin(phi);
                double scale = -_dRadius * Math.Cos(phi);

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

            //Geometrie erzeugen 
            GeometryModel3D model = new GeometryModel3D(mesh, Material);
            model.Transform = new Transform3DGroup();

            return new GeometryModel3D[] { model };
        }
    }
}