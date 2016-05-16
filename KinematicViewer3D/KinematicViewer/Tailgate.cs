using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Tailgate
    {
        private Model3DGroup groupModelVisual, 
                             groupDriveVisual;

        private GeometryModel3D cuboidGeometry,
                                cylinderGeometry,
                                sphereGeometry;
        private Drive drive;
        private Cuboid cube;
        private Cylinder cylinder;
        private Sphere sphere;

        private List<Point3D> axisPoints;

        private Vector3D axisOfRotation;

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
        private const double offset = 100.0;
        private double modelThickness;


        public Tailgate(List<Point3D> axisPoints, Vector3D axisOfRotation, Model3DGroup groupModelVisual, Model3DGroup groupDriveVisual, double modelThickness)
        {
            this.axisOfRotation = axisOfRotation;

            this.axisPoints = axisPoints;
            this.axisPoint = axisPoints[0];
            this.handPoint = axisPoints[1];
            this.attPointBodyL = axisPoints[2];
            this.attPointDoorL = axisPoints[3];
            addSecondDriveToList(axisPoints);
            this.attPointBodyR = axisPoints[4];
            this.attPointDoorR = axisPoints[5];

            



            this.groupModelVisual = groupModelVisual;
            this.groupDriveVisual = groupDriveVisual;
            this.modelThickness = modelThickness;

            coordsUpTail = makeCoordsUpTail();
            coordsDownTail = makeCoordsDownTail();
            coordsMidTail = makeCoordsMidTail();
            coordsDrive = makeCoordsDrive();

            

            clearModel();

            buildTail();
            buildDrive();
    
        }

        

        //Erstelle Koordinaten für den oberen Teil der Heckklappe
        private List<Point3D> makeCoordsUpTail()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D vAxisToHand = handPoint - axisPoint;

            Vector3D v1 = Vector3D.CrossProduct(vAxisToHand, axisOfRotation);
            v1.Normalize();

            Vector3D v2 = Vector3D.CrossProduct(-vAxisToHand, axisOfRotation);
            v2.Normalize();

            Point3D pBL = axisPoint + (v1 * -offset) + (axisOfRotation * 0.5 * tailWidth);
            Point3D pBR = axisPoint + (v1 * -offset) - axisOfRotation * 0.5 * tailWidth;

            Vector3D v3 = (pBL - pBR);
            v3.Normalize();

            RotateTransform3D pFL_rotation = new RotateTransform3D(new AxisAngleRotation3D((v3 * tailDepth), 90));
            Point3D pFL = pFL_rotation.Transform(pBL);

            RotateTransform3D pFR_rotation = new RotateTransform3D(new AxisAngleRotation3D((v3 * tailDepth), 90));
            Point3D pFR = pFR_rotation.Transform(pBR);

            points.Add(pBL);
            points.Add(pFL);
            points.Add(pFR);
            points.Add(pBR);
            

            /* Point3D pM  = new Point3D(axisPoint.X, axisPoint.Y + 100, axisPoint.Z);
             Point3D pML = new Point3D(pM.X, pM.Y, pM.Z + tailWidth/2);
             Point3D pFL = new Point3D(pML.X + tailDepth/2, pML.Y, pML.Z);
             Point3D pFR = new Point3D(pFL.X, pFL.Y, pFL.Z - tailWidth);
             Point3D pMR = new Point3D(pFR.X - tailDepth/2, pFR.Y, pFR.Z);
             Point3D pBR = new Point3D(pMR.X - tailDepth/2, pMR.Y, pMR.Z);
             Point3D pBL = new Point3D(pBR.X, pBR.Y, pBR.Z + tailWidth);

             points.Add(pM);
             points.Add(pML);
             points.Add(pFL);
             points.Add(pFR);
             points.Add(pMR);
             points.Add(pBR);
             points.Add(pBL);*/

            return points;
        }

        //Erstelle Koordinaten für den unteren Teil der Heckklappe
        private List<Point3D> makeCoordsDownTail()
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pM  = new Point3D(handPoint.X + 100, handPoint.Y, handPoint.Z);
            Point3D pML = new Point3D(pM.X, pM.Y, pM.Z + tailWidth / 2);
            Point3D pFL = new Point3D(pML.X, pML.Y - tailDepth, pML.Z);
            Point3D pFR = new Point3D(pFL.X, pFL.Y, pFL.Z - tailWidth);
            Point3D pMR = new Point3D(pFR.X, pFR.Y + tailDepth, pFR.Z);
            Point3D pBR = new Point3D(pMR.X, pMR.Y + tailDepth/2, pMR.Z);
            Point3D pBL = new Point3D(pBR.X, pBR.Y, pBR.Z + tailWidth);

            points.Add(pM);
            points.Add(pML);
            points.Add(pFL);
            points.Add(pFR);
            points.Add(pMR);
            points.Add(pBR);
            points.Add(pBL);

            return points;
        }

        //Erstelle Koordinaten für den mittleren Teil der Heckklappe
        private List<Point3D> makeCoordsMidTail()
        {
            List<Point3D> points = new List<Point3D>();
            Vector3D vAxisToHand = handPoint - axisPoint;
 
            Vector3D v1 = Vector3D.CrossProduct(vAxisToHand, axisOfRotation);
            v1.Normalize();
            
            
            Vector3D v2 = Vector3D.CrossProduct(-vAxisToHand, axisOfRotation);
            v2.Normalize();

            Point3D pUpL = axisPoint + (v1 * -offset) + (axisOfRotation * 0.5 * tailWidth);
            Point3D pAttDL = attPointDoorL;
            Point3D pDownL = handPoint + (v2 * offset ) + (axisOfRotation * 0.5 * tailWidth);
            Point3D pDownR = handPoint + (v2 * offset ) - (axisOfRotation * 0.5 * tailWidth);
            Point3D pAttDR = attPointDoorR;
            Point3D pUpR = axisPoint + (v1 * -offset) - axisOfRotation * 0.5 * tailWidth;

            points.Add(pUpL);
            points.Add(pAttDL);
            points.Add(pDownL);
            points.Add(pDownR);
            points.Add(pAttDR);
            points.Add(pUpR);
 
       
             

            /*List<Point3D> points = new List<Point3D>();

            Vector3D vL = (coordsDownTail[6] - coordsUpTail[2]) /4;
            Vector3D vR = (coordsDownTail[5] - coordsUpTail[3]) /4;

            //Linke seite der Heckklappe
            Point3D pL0  = coordsUpTail[2];
            Point3D pL1  = pL0 + vL;
            Point3D pL11 = pL1 + vL / 2;
            Point3D pL2  = pL11 + vL / 2;
            Point3D pL21 = pL2 + vL / 2;
            Point3D pL3  = pL21 + vL / 2;
            Point3D pL4  = coordsDownTail[6];

            //Rechte Seite der Heckklappe
            Point3D pR5 = coordsUpTail[3];
            Point3D pR6 = pR5 + vR;
            Point3D pR61 = pR6 + vR / 2;
            Point3D pR7 = pR61 + vR / 2;
            Point3D pR71 = pR7 + vR / 2;
            Point3D pR8 = pR71 + vR / 2;
            Point3D pR9 = coordsDownTail[5];

            points.Add(pL0);
            points.Add(pL1);
            points.Add(pL11);
            points.Add(pL2);
            points.Add(pL21);
            points.Add(pL3);
            points.Add(pL4);
            points.Add(pR5);
            points.Add(pR6);
            points.Add(pR61);
            points.Add(pR7);
            points.Add(pR71);
            points.Add(pR8);
            points.Add(pR9);*/

            return points;
        }

        private List<Point3D> makeCoordsDrive()
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pL0 = attPointBodyL;
            Point3D pL1 = attPointDoorL;
            //Vector3D vDL = pL1 - pL0;


            Point3D pR0 = attPointBodyR;
            Point3D pR1 = attPointDoorR;
            

            points.Add(pL0);
            points.Add(pL1);
            points.Add(pR0);
            points.Add(pR1);

            return points;
        }

        private void buildDrive()
        {
            /*
            for(int i=0; i<= coordsDrive.Count-4; i++)
            {
                drive = new Drive(coordsDrive[i], coordsDrive[i + 1], groupDriveVisual);
            }
            for(int i=2; i<= coordsDrive.Count-2; i++)
            {
                drive = new Drive(coordsDrive[i], coordsDrive[i + 1], groupDriveVisual);
            }*/

            drive = new Drive(coordsDrive[0], coordsDrive[1], groupDriveVisual);
            drive = new Drive(coordsDrive[2], coordsDrive[3], groupDriveVisual);
        }

        private void buildTail()
        {
            buildUpTail();
            buildMidTail();
            buildDownTail();
        }

        private void buildUpTail()
        {
            for(int i=0; i<=coordsUpTail.Count-2; i++)
            {
                generateSphere(coordsUpTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsUpTail[i], coordsUpTail[i + 1], modelThickness);
            }

            //generateSphere(coordsUpTail[6], modelThickness, new DiffuseMaterial(Brushes.Cyan));
            //generateCuboid(coordsUpTail[6], coordsUpTail[1], modelThickness);

            //Visuellen Achsenpunkt hinzufügen
            generateSphere(axisPoint, modelThickness, new DiffuseMaterial(Brushes.Red));
            
        }

        private void buildDownTail()
        {
            for (int i = 1; i <= coordsDownTail.Count-2; i++)
            {
                generateSphere(coordsDownTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsDownTail[i], coordsDownTail[i + 1], modelThickness);
            }

            generateSphere(coordsDownTail[6], modelThickness, new DiffuseMaterial(Brushes.Cyan));
            generateCuboid(coordsDownTail[6], coordsDownTail[1], modelThickness);

            //Visuellen Handangriffspunkt hinzufügen
            generateSphere(handPoint, modelThickness, new DiffuseMaterial(Brushes.Red));
        }

        private void buildMidTail()
        {
            for (int i = 0; i <= coordsMidTail.Count - 2; i++)
            {
                generateSphere(coordsMidTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness);
            }
            generateCuboid(coordsMidTail[5], coordsMidTail[0], modelThickness);

            generateSphere(axisPoint, modelThickness, new DiffuseMaterial(Brushes.Red));
            generateSphere(handPoint, modelThickness, new DiffuseMaterial(Brushes.Red));
            /*for(int i=0; i<=coordsMidTail.Count-9; i++)
            {
                generateSphere(coordsMidTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness);
            }

            for(int i=7; i<=coordsMidTail.Count-2; i++)
            {
                generateSphere(coordsMidTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness);
            }*/
        }


        private void generateSphere(Point3D point, double modelThickness, DiffuseMaterial mat)
        {
            MeshGeometry3D mesh_Sphere = new MeshGeometry3D();
            sphere = new Sphere(point, modelThickness, mesh_Sphere);
           
            sphereGeometry = new GeometryModel3D(mesh_Sphere, mat);
            sphereGeometry.Transform = new Transform3DGroup();  
            groupModelVisual.Children.Add(sphereGeometry);
        }

        private void generateCuboid(Point3D point1, Point3D point2, double modelThickness)
        {
            MeshGeometry3D mesh_Cuboid = new MeshGeometry3D();
            cube = new Cuboid(point1, point2, mesh_Cuboid, modelThickness);

            cuboidGeometry = new GeometryModel3D(mesh_Cuboid, new DiffuseMaterial(Brushes.Cyan));
            cuboidGeometry.Transform = new Transform3DGroup();
            groupModelVisual.Children.Add(cuboidGeometry);
        }

        private void generateCylinder(Point3D point1, Point3D point2)
        {
            MeshGeometry3D mesh_Cylinder = new MeshGeometry3D();
            cylinder = new Cylinder(mesh_Cylinder, point1, point2, 25, 128);

            cylinderGeometry = new GeometryModel3D(mesh_Cylinder, new DiffuseMaterial(Brushes.Red));
            cylinderGeometry.Transform = new Transform3DGroup();
            groupModelVisual.Children.Add(cylinderGeometry);
        }

        public void clearModel()
        {
            groupModelVisual.Children.Clear();
            groupDriveVisual.Children.Clear();
            //group2.Children.Remove(cuboidGeometry);
            //group2.Children.Remove(cylinderGeometry);
            //group2.Children.Remove(sphereGeometry);
        }

        public Model3DGroup GroupModelVisual
        {
            get { return groupModelVisual; }
            set { groupModelVisual = value; }
        }

        public Model3DGroup GroupDriveVisual
        {
            get { return groupDriveVisual; }
            set { groupDriveVisual = value; }
        }

        private void addSecondDriveToList(List<Point3D> axisPoints)
        {
            Point3D p1 = new Point3D();
            Point3D p2 = new Point3D();
            p2 = attPointDoorL - axisOfRotation * tailWidth;
            p1 = p2 - (attPointDoorL - attPointBodyL);
            
            axisPoints.Add(p1);
            axisPoints.Add(p2);
        }

        public Point3D getPosition()
        {
            Point3D point = sphere.getPosition();
            return point;
        }

    }
}
