using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Tailgate: GeometricalElement, IGuide
    {

        private List<Point3D> axisPoints;

        private Vector3D axisOfRotation;
        private Vector3D vY = new Vector3D(0,1,0);
        private Vector3D vAxisToHandE1;
        private Vector3D vAxisQuerE2;
        private Vector3D vN;

        private Point3D axisPoint;
        private Point3D handPoint;
        private Point3D attPointBodyL;
        private Point3D attPointDoorL;
        private Point3D attPointBodyR;
        private Point3D attPointDoorR;


        private List<Point3D> coordsUpTail;
        private List<Point3D> coordsDownTail;
        private List<Point3D> coordsMidTail;
        private List<Point3D> coordsDrive;

        private const double tailWidth = 1250.0;
        private const double tailDepth = 200.0;
        private const double offset = 130.0;
        private double modelThickness;


        public void Move(double val)
        {

        }


        public Tailgate(List<Point3D> axisPoints, Vector3D axisOfRotation, double modelThickness)
        {
            this.axisOfRotation = axisOfRotation;

            this.axisPoints = axisPoints;
            axisPoint = axisPoints[0];
            handPoint = axisPoints[1];

            vAxisToHandE1 = handPoint - axisPoint;
            vAxisQuerE2 = Vector3D.CrossProduct(vAxisToHandE1, vY);
            vAxisQuerE2.Normalize();

            vN = Vector3D.CrossProduct(vAxisQuerE2, vAxisToHandE1);
            vN.Normalize();

            attPointBodyL = axisPoints[2];
            attPointDoorL = axisPoints[3];
            //addSecondDriveToList(axisPoints);
            attPointBodyR = axisPoints[4];
            attPointDoorR = axisPoints[5];

            this.modelThickness = modelThickness;

            coordsMidTail = makeCoordsMidTail();
            coordsUpTail = makeCoordsUpTail();
            coordsDownTail = makeCoordsDownTail();
            //coordsDrive = makeCoordsDrive();
        }

        

        //Erstelle Koordinaten für den oberen Teil der Heckklappe
        private List<Point3D> makeCoordsUpTail()
        {
            List<Point3D> points = new List<Point3D>();
            Vector3D v1 = new Vector3D(vAxisToHandE1.X, 0, vAxisToHandE1.Z);
            v1 = Renderer.scaleToOffset(v1, tailDepth);

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

        //Erstelle Koordinaten für den unteren Teil der Heckklappe
        private List<Point3D> makeCoordsDownTail()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D v1 = Renderer.scaleToOffset(vY, tailDepth * 2);

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

            Point3D pUpL    = axisPoint + (vN * offset) + (vAxisQuerE2 * tailWidth / 2);
            Point3D pUpR    = axisPoint + (vN * offset) - (vAxisQuerE2 * tailWidth / 2);
            Point3D pDownL  = handPoint + (vN * offset) + (vAxisQuerE2 * tailWidth / 2);
            Point3D pDownR  = handPoint + (vN * offset) - (vAxisQuerE2 * tailWidth / 2);

            points.Add(pUpL);
            points.Add(pDownL);
            points.Add(pDownR);
            points.Add(pUpR);
            points.Add(attPointDoorL);
            points.Add(attPointDoorR);

            return points;
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

        //private void buildDrive()
        //{
        //    drive = new Drive(coordsDrive[0], coordsDrive[1]);
        //    drive = new Drive(coordsDrive[2], coordsDrive[3],);
        //}

        public override GeometryModel3D[] GetGeometryModel()
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            //Klappe
            for (int i = 0; i <= coordsUpTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(coordsUpTail[i], modelThickness, Brushes.Cyan).GetGeometryModel());
                Res.AddRange(new Cuboid(coordsUpTail[i], coordsUpTail[i + 1], modelThickness).GetGeometryModel());
            }

            for (int i = 0; i <= coordsDownTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(coordsDownTail[i], modelThickness, Brushes.Cyan).GetGeometryModel());
                Res.AddRange(new Cuboid(coordsDownTail[i], coordsDownTail[i + 1], modelThickness).GetGeometryModel());
            }

            for (int i = 0; i <= coordsMidTail.Count - 4; i++)
            {
                Res.AddRange(new Sphere(coordsMidTail[i], modelThickness, Brushes.Cyan).GetGeometryModel());
                Res.AddRange(new Cuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness).GetGeometryModel());
            }

            Res.AddRange(new Sphere(coordsMidTail[3], modelThickness, Brushes.Cyan).GetGeometryModel());
            Res.AddRange(new Cuboid(coordsMidTail[3], coordsMidTail[0], modelThickness).GetGeometryModel());
            
            Res.AddRange(new Sphere(coordsMidTail[4], 40, Brushes.Cyan).GetGeometryModel());
            Res.AddRange(new Sphere(coordsMidTail[5], 40, Brushes.Cyan).GetGeometryModel());

            //Handle
            Res.AddRange(new Sphere(handPoint, 50, Brushes.Red).GetGeometryModel());

            //Drehachse
            Point3D p1 = axisPoint + axisOfRotation * tailWidth * 1/2;
            Point3D p2 = axisPoint - axisOfRotation * tailWidth * 1/2;

            Res.AddRange(new Sphere(axisPoint, 50, Brushes.Red).GetGeometryModel());
            Res.AddRange(new Cylinder(p1, p2, 10, Brushes.Red).GetGeometryModel());

            return Res.ToArray();
        }        

        //private void addSecondDriveToList(List<Point3D> axisPoints)
        //{
        //    Vector3D vR = new Vector3D(axisPoint.X, axisPoint.Y, axisPoint.Z);
        //    Vector3D vE2 = Vector3D.CrossProduct(vAxisToHandE1, vY);

        //    double d = Vector3D.DotProduct(vR, vE2);

        //    Point3D p1 = Renderer.reflectPoint(attPointBodyL, vR, vE2, d);
        //    Point3D p2 = Renderer.reflectPoint(attPointDoorL, vR, vE2, d);
            
        //    axisPoints.Add(p1);
        //    axisPoints.Add(p2);
        //}

        public Point3D AttachmentPointDoorLeft
        {
            get { return attPointDoorL; }
            set { attPointDoorL = value; }
        }

        public Point3D AttachmentPointDoorRight
        {
            get { return attPointDoorR; }
            set { attPointDoorR = value; }
        }

    }
}
