﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Tailgate : GeometricalElement, IGuide
    {
        private const double OFFSET = 130.0;
        private const double TAILDEPTH = 200.0;
        private const double TAILWIDTH = 1250.0;

        private double _dCurVal;

        //maximaler Öffnungswinkel
        private double _dMaxOpen;

        private double _dMinOpen;

        private Vector3D _oAxisOfRotation;
        private Point3D _oAxisPoint;
        private Point3D _oPointLatch;

        private List<Point3D> _oLCoordsDownTail;
        private List<Point3D> _oLCoordsMidTail;
        private List<Point3D> _oLCoordsUpTail;

        private double _dModelThickness;

        private Vector3D _oVAxisToHandE1;

        private Material _oAxisMaterial;

        public Tailgate(Point3D axisPoint, Point3D latch, Vector3D axisOfRotation, double modelThickness, Material mat = null)
            :base(mat)
        {
            AxisOfRotation = axisOfRotation;

            AxisPoint = axisPoint;
            PointLatch = latch;
            MaxValue = 62.5;
            MinValue = 0.0;

            ModelThickness = modelThickness;

            VAxisToHandE1 = PointLatch - AxisPoint;

            CoordsMidTail = makeCoordsMidTail();
            CoordsUpTail = makeCoordsUpTail();
            CoordsDownTail = makeCoordsDownTail();

            if(mat == null)
                AxisMaterial = new DiffuseMaterial(Brushes.Red);
            if (mat != null)
                AxisMaterial = mat;
        }

        public Vector3D AxisOfRotation
        {
            get { return _oAxisOfRotation; }
            private set { _oAxisOfRotation = value; }
        }

        public Point3D AxisPoint
        {
            get { return _oAxisPoint; }
            private set { _oAxisPoint = value; }
        }

        public double CurValue
        {
            get { return _dCurVal; }
            set { _dCurVal = value; }
        }

        public double MaxValue
        {
            get { return _dMaxOpen; }
            set { _dMaxOpen = value; }
        }

        public double MinValue
        {
            get { return _dMinOpen; }
            set { _dMinOpen = value; }
        }

        public Point3D PointLatch
        {
            get { return _oPointLatch; }
            private set { _oPointLatch = value; }
        }

        public double ModelThickness
        {
            get { return _dModelThickness; }
            private set { _dModelThickness = value; }
        }

        public List<Point3D> CoordsDownTail
        {
            get { return _oLCoordsDownTail; }
            private set { _oLCoordsDownTail = value; }
        }

        public List<Point3D> CoordsMidTail
        {
            get { return _oLCoordsMidTail; }
            private set { _oLCoordsMidTail = value; }
        }

        public List<Point3D> CoordsUpTail
        {
            get { return _oLCoordsUpTail; }
            private set { _oLCoordsUpTail = value; }
        }

        public Vector3D VAxisToHandE1
        {
            get { return _oVAxisToHandE1; }
            set { _oVAxisToHandE1 = value; }
        }

        public Material AxisMaterial
        {
            get { return _oAxisMaterial; }
            set { _oAxisMaterial = value; }
        }

        

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();



            //Klappe
            //Oberer Part
            for (int i = 0; i <= CoordsUpTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsUpTail[i], ModelThickness, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsUpTail[i], CoordsUpTail[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            //Unterer Part
            for (int i = 0; i <= CoordsDownTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsDownTail[i], ModelThickness, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsDownTail[i], CoordsDownTail[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            //Mittlerer Part
            for (int i = 0; i <= CoordsMidTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsMidTail[i], ModelThickness, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsMidTail[i], CoordsMidTail[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            Res.AddRange(new Sphere(CoordsMidTail[3], ModelThickness, Material).GetGeometryModel(guide));
            Res.AddRange(new Cuboid(CoordsMidTail[3], CoordsMidTail[0], ModelThickness, Material).GetGeometryModel(guide));

            //Handle
            Res.AddRange(new Sphere(PointLatch, 50, AxisMaterial).GetGeometryModel(guide));

            //Drehachse
            Point3D p1 = AxisPoint + AxisOfRotation * TAILWIDTH * 1 / 2;
            Point3D p2 = AxisPoint - AxisOfRotation * TAILWIDTH * 1 / 2;

            Res.AddRange(new Sphere(AxisPoint, 50, AxisMaterial).GetGeometryModel(guide));
            Res.AddRange(new Cylinder(p1, p2, 10, AxisMaterial).GetGeometryModel(guide));

            return Res.ToArray();
        }

        //Erstelle Koordinaten für den unteren Teil der Heckklappe
        private List<Point3D> makeCoordsDownTail()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D v1 = TransformationUtilities.scaleToOffset(new Vector3D(0, 1, 0), TAILDEPTH * 2);
            //Vector3D v1 = TransformationUtilities.ScaleVector(new Vector3D(0, 1, 0), TAILDEPTH * 2 );

            Point3D pUpL = CoordsMidTail[1];
            Point3D pUpR = CoordsMidTail[2];
            Point3D pDownL = pUpL - v1;
            Point3D pDownR = pUpR - v1;

            points.Add(pUpL);
            points.Add(pDownL);
            points.Add(pDownR);
            points.Add(pUpR);

            return points;
        }

        //Erstelle Koordinaten für den mittleren Teil der Heckklappe
        private List<Point3D> makeCoordsMidTail()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D vAxisQuerE2 = Vector3D.CrossProduct(VAxisToHandE1, new Vector3D(0, 1, 0));
            vAxisQuerE2.Normalize();
            //vAxisQuerE2 = TransformationUtilities.ScaleVector(vAxisQuerE2, 1);

            Vector3D vN = Vector3D.CrossProduct(vAxisQuerE2, VAxisToHandE1);
            vN.Normalize();
            //vN = TransformationUtilities.ScaleVector(vN, 1);

            Point3D pUpL = AxisPoint + (vN * OFFSET) + (vAxisQuerE2 * TAILWIDTH / 2);
            Point3D pUpR = AxisPoint + (vN * OFFSET) - (vAxisQuerE2 * TAILWIDTH / 2);
            Point3D pDownL = PointLatch + (vN * OFFSET) + (vAxisQuerE2 * TAILWIDTH / 2);
            Point3D pDownR = PointLatch + (vN * OFFSET) - (vAxisQuerE2 * TAILWIDTH / 2);

            points.Add(pUpL);
            points.Add(pDownL);
            points.Add(pDownR);
            points.Add(pUpR);

            return points;
        }

        //Erstelle Koordinaten für den oberen Teil der Heckklappe
        private List<Point3D> makeCoordsUpTail()
        {
            List<Point3D> points = new List<Point3D>();
            Vector3D v1 = new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z);
            v1 = TransformationUtilities.scaleToOffset(v1, TAILDEPTH);
            //v1 = TransformationUtilities.ScaleVector(v1, TAILDEPTH);

            Point3D pDownL = CoordsMidTail[0];
            Point3D pDownR = CoordsMidTail[3];
            Point3D pUpL = pDownL - v1;
            Point3D pUpR = pDownR - v1;

            points.Add(pDownL);
            points.Add(pUpL);
            points.Add(pUpR);
            points.Add(pDownR);

            return points;
        }

        public void InitiateMove(double per)
        {
            CurValue = per * MaxValue;
        }

        public Point3D MovePoint(Point3D endPoint)
        {
            Point3D outputPoint = new Point3D();

            try
            {
                AxisAngleRotation3D aARot = new AxisAngleRotation3D(AxisOfRotation, CurValue);
                RotateTransform3D rotation = new RotateTransform3D(aARot, AxisPoint);
                outputPoint = rotation.Transform(endPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Zuerst 3D Modell erstellen, dann erst Öfnungswinkel verändern. \n"
                    + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return outputPoint;
        }

        public void Move(Model3DGroup groupActive, double per = 0)
        {
            InitiateMove(per);
            Transformation.rotateModel(CurValue, AxisOfRotation, AxisPoint, groupActive);
        }

        //public void MoveMinAngle(Model3DGroup groupStaticMinAngle, double per = 0)
        //{
        //    InitiateMove(per);
        //    Transformation.rotateModel(CurValue, AxisOfRotation, AxisPoint, groupStaticMinAngle);
        //}

        //public void MoveMaxAngle(Model3DGroup groupStaticMaxAngle, double per = 1)
        //{
        //    InitiateMove(per);
        //    Transformation.rotateModel(CurValue, AxisOfRotation, AxisPoint, groupStaticMaxAngle);
        //}
    }
}