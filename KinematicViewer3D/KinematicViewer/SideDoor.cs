using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class SideDoor : GeometricalElement, IGuide
    {
        private const double OFFSET = 130.0;
        private const double DOORDEPTH = 200.0;
        private const double DOORHEIGHTBODY = 750.0;
        private const double DOORHEIGHTWINDOW = 450;
        private const double DOORHEIGHT = 1200.0;

        private const double DOORWIDTH = 1200;

        private double _dCurVal;

        //maximaler Öffnungswinkel
        private double _dMaxOpen;

        private Vector3D _oAxisOfRotation;
        private Point3D _oAxisPoint;
        private Point3D _oPointLatch;


        private List<Point3D> _oLCoordsBodyPart;
        private List<Point3D> _oLCoordsWindowPart;

        private double _dModelThickness;

        private Vector3D _oVAxisToHandE1;

        public SideDoor(Point3D axisPoint, Point3D latch, Vector3D axisOfRotation, double modelThickness)
        {
            //AxisOfRotation = axisOfRotation;
            //AxisOfRotation = new Vector3D(0, 0, 1);
            AxisOfRotation = new Vector3D(13.94, 399.21, 20.94);
            AxisOfRotation = TransformationUtilities.ScaleVector(AxisOfRotation, 1);
            

            AxisPoint = axisPoint;
            PointLatch = latch;
            MaxValue = 100.5;

            ModelThickness = modelThickness;

            VAxisToHandE1 = PointLatch - AxisPoint;

            CoordsBodyPart = makeCoordsBodyPart();
            CoordsWindowPart = makeCoordsWindowPart();
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


        public List<Point3D> CoordsBodyPart
        {
            get { return _oLCoordsBodyPart; }
            private set { _oLCoordsBodyPart = value; }
        }

        public List<Point3D> CoordsWindowPart
        {
            get { return _oLCoordsWindowPart; }
            private set { _oLCoordsWindowPart = value; }
        }

        public Vector3D VAxisToHandE1
        {
            get { return _oVAxisToHandE1; }
            set { _oVAxisToHandE1 = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            //Seitentüre
            //Oberer Part
            //for (int i = 0; i <= CoordsWindowPart.Count - 2; i++)
            //{
            //    Res.AddRange(new Sphere(CoordsWindowPart[i], ModelThickness, Brushes.Cyan).GetGeometryModel(guide));
            //    Res.AddRange(new Cuboid(CoordsWindowPart[i], CoordsWindowPart[i + 1], ModelThickness).GetGeometryModel(guide));
            //}


            //Karossierie Part
            for (int i = 0; i <= CoordsBodyPart.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsBodyPart[i], ModelThickness, Brushes.Cyan).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsBodyPart[i], CoordsBodyPart[i + 1], ModelThickness).GetGeometryModel(guide));
            }

            Res.AddRange(new Sphere(CoordsBodyPart[3], ModelThickness, Brushes.Cyan).GetGeometryModel(guide));
            Res.AddRange(new Cuboid(CoordsBodyPart[3], CoordsBodyPart[0], ModelThickness).GetGeometryModel(guide));

            //Handle
            Res.AddRange(new Sphere(PointLatch, 50, Brushes.Red).GetGeometryModel(guide));

            //Drehachse
            Point3D p1 = AxisPoint + AxisOfRotation * DOORWIDTH * 1 / 2;
            Point3D p2 = AxisPoint - AxisOfRotation * DOORWIDTH * 1 / 2;
            //Point3D p1 = AxisPoint + TransformationUtilities.ScaleVector(AxisOfRotation, DOORHEIGHT * 0.5);
            //Point3D p2 = AxisPoint - TransformationUtilities.ScaleVector(AxisOfRotation, DOORHEIGHT * 0.5);

            Res.AddRange(new Sphere(AxisPoint, 50, Brushes.Red).GetGeometryModel(guide));
            Res.AddRange(new Cylinder(p1, p2, 10, Brushes.Red).GetGeometryModel(guide));

            return Res.ToArray();
        }


        //Erstelle Koordinaten für den mittleren Karosserieteil der Seitentüre
        private List<Point3D> makeCoordsBodyPart()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D vAxisE2 = Vector3D.CrossProduct(VAxisToHandE1, new Vector3D(0, 0, 1));
            //vAxisE2.Normalize();

            Point3D p0 = AxisPoint + vAxisE2;
            Point3D p1 = p0 + VAxisToHandE1;
            Point3D p2 = p1 - vAxisE2;
            Point3D p3 = p2 - VAxisToHandE1;

            Vector3D vN = Vector3D.CrossProduct(vAxisE2, VAxisToHandE1);
            vN.Normalize();

            points.Add(p0);
            points.Add(p1);
            points.Add(p2);
            points.Add(p3);

            return points;
        }

        //Erstelle Koordinaten für den oberen Fenster Teil der Seitentüre
        private List<Point3D> makeCoordsWindowPart()
        {
            List<Point3D> points = new List<Point3D>();
           

         

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
    }
}