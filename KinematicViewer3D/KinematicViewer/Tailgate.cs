using System;
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
        private double _dMaxOpen = 62.5;
        private Vector3D _oAxisOfRotation;
        private Point3D _oAxisPoint;
        private Point3D _oPointLatch;
        private List<Point3D> coordsDownTail;
        private List<Point3D> coordsMidTail;
        private List<Point3D> coordsUpTail;

        //maximaler Öffnungswinkel
        private double modelThickness;

        private Vector3D vAxisQuerE2;
        private Vector3D vAxisToHandE1;
        private Vector3D vN;

        public Tailgate(Point3D axisPoint, Point3D latch, double modelThickness)
        {
            AxisOfRotation = new Vector3D(0, 0, 1);

            AxisPoint = axisPoint;
            PointLatch = latch;

            //attPointBodyL = points[2];
            //attPointDoorL = points[3];
            //addSecondDriveToList(axisPoints);
            //attPointBodyR = axisPoints[4];
            //attPointDoorR = axisPoints[5];

            this.modelThickness = modelThickness;

            coordsMidTail = makeCoordsMidTail();
            coordsUpTail = makeCoordsUpTail();
            coordsDownTail = makeCoordsDownTail();
            //coordsDrive = makeCoordsDrive();
        }

        public Vector3D AxisOfRotation
        {
            get
            {
                return _oAxisOfRotation;
            }

            private set
            {
                _oAxisOfRotation = value;
            }
        }

        public Point3D AxisPoint
        {
            get
            {
                return _oAxisPoint;
            }

            private set
            {
                _oAxisPoint = value;
            }
        }

        public double CurValue
        {
            get
            {
                return _dCurVal;
            }

            set
            {
                _dCurVal = value;
            }
        }

        public double MaxValue
        {
            get
            {
                return _dMaxOpen;
            }

            set
            {
                _dMaxOpen = value;
            }
        }

        //public Point3D AttachmentPointDoorRight
        //{
        //    get { return attPointDoorR; }
        //    set { attPointDoorR = value; }
        //}
        public Point3D PointLatch
        {
            get
            {
                return _oPointLatch;
            }

            private set
            {
                _oPointLatch = value;
            }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            //Klappe
            //Oberer Part
            for (int i = 0; i <= coordsUpTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(coordsUpTail[i], modelThickness, Brushes.Cyan).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(coordsUpTail[i], coordsUpTail[i + 1], modelThickness).GetGeometryModel(guide));
            }

            //Unterer Part
            for (int i = 0; i <= coordsDownTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(coordsDownTail[i], modelThickness, Brushes.Cyan).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(coordsDownTail[i], coordsDownTail[i + 1], modelThickness).GetGeometryModel(guide));
            }

            //Mittlerer Part
            for (int i = 0; i <= coordsMidTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(coordsMidTail[i], modelThickness, Brushes.Cyan).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness).GetGeometryModel(guide));
            }

            Res.AddRange(new Sphere(coordsMidTail[3], modelThickness, Brushes.Cyan).GetGeometryModel(guide));
            Res.AddRange(new Cuboid(coordsMidTail[3], coordsMidTail[0], modelThickness).GetGeometryModel(guide));

            //Handle
            Res.AddRange(new Sphere(PointLatch, 50, Brushes.Red).GetGeometryModel(guide));

            //Drehachse
            Point3D p1 = AxisPoint + AxisOfRotation * TAILWIDTH * 1 / 2;
            Point3D p2 = AxisPoint - AxisOfRotation * TAILWIDTH * 1 / 2;

            Res.AddRange(new Sphere(AxisPoint, 50, Brushes.Red).GetGeometryModel(guide));
            Res.AddRange(new Cylinder(p1, p2, 10, Brushes.Red).GetGeometryModel(guide));

            return Res.ToArray();
        }

        

        //Erstelle Koordinaten für den unteren Teil der Heckklappe
        private List<Point3D> makeCoordsDownTail()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D v1 = TransformationUtilities.scaleToOffset(new Vector3D(0, 1, 0), TAILDEPTH * 2);

            Point3D pUpL = coordsMidTail[1];
            Point3D pUpR = coordsMidTail[2];
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

            vAxisToHandE1 = PointLatch - AxisPoint;
            vAxisQuerE2 = Vector3D.CrossProduct(vAxisToHandE1, new Vector3D(0, 1, 0));
            vAxisQuerE2.Normalize();

            vN = Vector3D.CrossProduct(vAxisQuerE2, vAxisToHandE1);
            vN.Normalize();

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
            Vector3D v1 = new Vector3D(vAxisToHandE1.X, 0, vAxisToHandE1.Z);
            v1 = TransformationUtilities.scaleToOffset(v1, TAILDEPTH);

            Point3D pDownL = coordsMidTail[0];
            Point3D pDownR = coordsMidTail[3];
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


        //private List<Point3D> makeCoordsDrive()
        //{
        //    List<Point3D> points = new List<Point3D>();

        //    Point3D pL0 = attPointBodyL;
        //    Point3D pL1 = coordsMidTail[4];

        //    Point3D pR0 = attPointBodyR;
        //    Point3D pR1 = coordsMidTail[5];

        //    /*Point3D pL0 = attPointBodyL;
        //    Point3D pL1 = attPointDoorL;
        //    //Vector3D vDL = pL1 - pL0;

        //    Point3D pR0 = attPointBodyR;
        //    Point3D pR1 = attPointDoorR;*/

        //    points.Add(pL0);
        //    points.Add(pL1);
        //    points.Add(pR0);
        //    points.Add(pR1);

        //    return points;
        //}
        //private void addSecondDriveToList(List<Point3D> AxisPoints)
        //{
        //    Vector3D vR = new Vector3D(AxisPoint.X, AxisPoint.Y, AxisPoint.Z);
        //    Vector3D vE2 = Vector3D.CrossProduct(vAxisToHandE1, vY);

        //    double d = Vector3D.DotProduct(vR, vE2);

        //    Point3D p1 = TransformationUtilities.reflectPoint(attPointBodyL, vR, vE2, d);
        //    Point3D p2 = TransformationUtilities.reflectPoint(attPointDoorL, vR, vE2, d);

        //    AxisPoints.Add(p1);
        //    AxisPoints.Add(p2);
        //}

        //public Point3D AttachmentPointDoorLeft
        //{
        //    get { return attPointDoorL; }
        //    set { attPointDoorL = value; }
        //}
    }
}