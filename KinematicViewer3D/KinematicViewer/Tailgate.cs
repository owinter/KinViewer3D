﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Tailgate: VisualObject
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


        public Tailgate(List<Point3D> axisPoints, Vector3D axisOfRotation, Model3DGroup groupModelVisual, Model3DGroup groupDriveVisual, double modelThickness)
        {
            this.axisOfRotation = axisOfRotation;

            this.axisPoints = axisPoints;
            this.axisPoint = axisPoints[0];
            this.handPoint = axisPoints[1];

            this.vAxisToHandE1 = handPoint - axisPoint;
            vAxisQuerE2 = Vector3D.CrossProduct(vAxisToHandE1, vY);
            vAxisQuerE2.Normalize();

            vN = Vector3D.CrossProduct(vAxisQuerE2, vAxisToHandE1);
            vN.Normalize();

            this.attPointBodyL = axisPoints[2];
            this.attPointDoorL = axisPoints[3];
            addSecondDriveToList(axisPoints);
            this.attPointBodyR = axisPoints[4];
            this.attPointDoorR = axisPoints[5];

            this.groupModelVisual = groupModelVisual;
            this.groupDriveVisual = groupDriveVisual;
            this.modelThickness = modelThickness;

            coordsMidTail = makeCoordsMidTail();
            coordsUpTail = makeCoordsUpTail();
            coordsDownTail = makeCoordsDownTail();
            coordsDrive = makeCoordsDrive();

            clearModel();

            buildTail();
            buildDrive();
    
        }

        

        //Erstelle Koordinaten für den oberen Teil der Heckklappe
        private List<Point3D> makeCoordsUpTail()
        {
            List<Point3D> points = new List<Point3D>();
            Vector3D v1 = new Vector3D(vAxisToHandE1.X, 0, vAxisToHandE1.Z);
            v1 = scaleToOffset(v1, tailDepth);

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

            Vector3D v1 = scaleToOffset(vY, tailDepth * 2);

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
            drive = new Drive(coordsDrive[0], coordsDrive[1], groupDriveVisual);
            drive = new Drive(coordsDrive[2], coordsDrive[3], groupDriveVisual);
        }

        private void buildTail()
        {
            buildMidTail();
            buildUpTail();
            buildDownTail();
            buildAxisAndHandPoint();
            buildAttachmentToDrive();
        }

        private void buildUpTail()
        {
            for(int i=0; i<=coordsUpTail.Count-2; i++)
            {
                generateSphere(coordsUpTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsUpTail[i], coordsUpTail[i + 1], modelThickness);
            }
        }

        private void buildDownTail()
        {
            for (int i = 0; i <= coordsDownTail.Count-2; i++)
            {
                generateSphere(coordsDownTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsDownTail[i], coordsDownTail[i + 1], modelThickness);
            }
        }

        private void buildMidTail()
        {
            for (int i = 0; i <= coordsMidTail.Count - 4; i++)
            {
                generateSphere(coordsMidTail[i], modelThickness, new DiffuseMaterial(Brushes.Cyan));
                generateCuboid(coordsMidTail[i], coordsMidTail[i + 1], modelThickness);
            }
            generateSphere(coordsMidTail[3], modelThickness, new DiffuseMaterial(Brushes.Cyan));
            generateCuboid(coordsMidTail[3], coordsMidTail[0], modelThickness);
        }

        private void buildAttachmentToDrive()
        {
            generateSphere(coordsMidTail[4], 40, new DiffuseMaterial(Brushes.Cyan));
            generateSphere(coordsMidTail[5], 40, new DiffuseMaterial(Brushes.Cyan));
        }

        private void buildAxisAndHandPoint()
        {
            generateSphere(axisPoint, 50, new DiffuseMaterial(Brushes.Red));
            generateSphere(handPoint, 50, new DiffuseMaterial(Brushes.Red));
        }


        public void clearModel()
        {
            groupModelVisual.Children.Clear();
            groupDriveVisual.Children.Clear();
            //group2.Children.Remove(cuboidGeometry);
            //group2.Children.Remove(cylinderGeometry);
            //group2.Children.Remove(sphereGeometry);
        }

        

        private void addSecondDriveToList(List<Point3D> axisPoints)
        {
            
            Point3D p2 = new Point3D();
            p2 = attPointDoorL - axisOfRotation * tailWidth;

            Vector3D vAxisToHand = handPoint - axisPoint;

            Point3D p1 = rotateNewPoint(vAxisToHand, 180, attPointBodyL);




            //p1 = p2 - (attPointDoorL - attPointBodyL);
            
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
