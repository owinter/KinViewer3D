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
        private Model3DGroup groupModelVisual;
        private GeometryModel3D cuboidGeometry,
                                cylinderGeometry,
                                sphereGeometry;
        private Drive drive;
        private Cuboid cube;
        private Cylinder cylinder;
        private Sphere sphere;

        private Point3D axisPoint;
        private Point3D handPoint;
        private List<Point3D> coordsUpTail;
        private List<Point3D> coordsDownTail;
        private List<Point3D> coordsMidTail;

        private double tailWidth = 1000.0;
        private double tailDepth = 250.0;
        private double modelThickness;


        public Tailgate(List<Point3D> axisPoints, Model3DGroup groupModelVisual, double modelThickness)
        {
            this.axisPoint = axisPoints[0];
            this.handPoint = axisPoints[1];
            this.groupModelVisual = groupModelVisual;
            this.modelThickness = modelThickness;

            coordsUpTail = makeCoordsUpTail();
            coordsDownTail = makeCoordsDownTail();
            coordsMidTail = makeCoordsMidTail();

            clearModel();

            buildTail();
    
        }

        //Erstelle Koordinaten für den oberen Teil der Heckklappe
        private List<Point3D> makeCoordsUpTail()
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pM  = new Point3D(axisPoint.X, axisPoint.Y + 50, axisPoint.Z);
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
            points.Add(pBL);

            return points;
        }

        //Erstelle Koordinaten für den unteren Teil der Heckklappe
        private List<Point3D> makeCoordsDownTail()
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pM = new Point3D(handPoint.X + 50, handPoint.Y, handPoint.Z);
            Point3D pML = new Point3D(pM.X, pM.Y, pM.Z + tailWidth / 2);
            Point3D pFL = new Point3D(pML.X, pML.Y - tailDepth/2, pML.Z);
            Point3D pFR = new Point3D(pFL.X, pFL.Y, pFL.Z - tailWidth);
            Point3D pMR = new Point3D(pFR.X, pFR.Y + tailDepth/2, pFR.Z);
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

            Vector3D vL = (coordsDownTail[6] - coordsUpTail[2]) /4;
            Vector3D vR = (coordsDownTail[5] - coordsUpTail[3]) /4;

            //Linke seite der Heckklappe
            Point3D pL0 = coordsUpTail[2];
            Point3D pL1 = pL0 + vL;
            Point3D pL2 = pL1 + vL;
            Point3D pL3 = pL2 + vL;
            Point3D pL4 = coordsDownTail[6];

            //Rechte Seite der Heckklappe
            Point3D pR5 = coordsUpTail[3];
            Point3D pR6 = pR5 + vR;
            Point3D pR7 = pR6 + vR;
            Point3D pR8 = pR7 + vR;
            Point3D pR9 = coordsDownTail[5];

            points.Add(pL0);
            points.Add(pL1);
            points.Add(pL2);
            points.Add(pL3);
            points.Add(pL4);
            points.Add(pR5);
            points.Add(pR6);
            points.Add(pR7);
            points.Add(pR8);
            points.Add(pR9);

            return points;
        }

        private void buildTail()
        {
            buildUpTail();
            buildMidTail();
            buildDownTail();
        }

        private void buildUpTail()
        {
            for(int i=1; i<=coordsUpTail.Count-2; i++)
            {
                generateSphere(coordsUpTail[i], modelThickness);
                generateCuboid(coordsUpTail[i], coordsUpTail[i + 1], modelThickness);
            }

            generateSphere(coordsUpTail[6], modelThickness);
            generateCuboid(coordsUpTail[6], coordsUpTail[1], modelThickness);
            
        }

        private void buildDownTail()
        {
            for (int i = 1; i <= coordsDownTail.Count - 2; i++)
            {
                generateSphere(coordsDownTail[i], modelThickness);
                generateCuboid(coordsDownTail[i], coordsDownTail[i + 1], modelThickness);
            }

            generateSphere(coordsDownTail[6], modelThickness);
            generateCuboid(coordsDownTail[6], coordsDownTail[1], modelThickness);
        }

        private void buildMidTail()
        {
            for(int i=0; i<=coordsMidTail.Count-7; i++)
            {
                generateSphere(coordsMidTail[i], modelThickness);
                generateCuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness);
            }

            for(int i=5; i<=coordsMidTail.Count-2; i++)
            {
                generateSphere(coordsMidTail[i], modelThickness);
                generateCuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness);
            }
        }


        private void generateSphere(Point3D point, double modelThickness)
        {
            MeshGeometry3D mesh_Sphere = new MeshGeometry3D();
            sphere = new Sphere(point, modelThickness, mesh_Sphere);
           
            sphereGeometry = new GeometryModel3D(mesh_Sphere, new DiffuseMaterial(Brushes.Cyan));
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

            cylinderGeometry = new GeometryModel3D(mesh_Cylinder, new DiffuseMaterial(Brushes.Cyan));
            cylinderGeometry.Transform = new Transform3DGroup();
            groupModelVisual.Children.Add(cylinderGeometry);
        }

        public void clearModel()
        {
            groupModelVisual.Children.Clear();
            //group2.Children.Remove(cuboidGeometry);
            //group2.Children.Remove(cylinderGeometry);
            //group2.Children.Remove(sphereGeometry);
        }
    }
}
