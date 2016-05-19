using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class SideDoor: VisualObject
    {
        private List<Point3D> axisPoints;

        private Vector3D axisOfRotation;
        private Vector3D vY = new Vector3D(0, 1, 0);
        private Vector3D vAxisToHandE1;

        private Point3D axisPoint;
        private Point3D handPoint;
        private Point3D attPointBody;
        private Point3D attPointDoor;

        private List<Point3D> coordsUpDoor;
        private List<Point3D> coordsMidDoor;
        private List<Point3D> coordsDrive;

        private const double doorWidth = 1200.0;
        private const double doorHeight = 200.0;
        private const double offset = 130.0;
        private double modelThickness;

        public SideDoor(List<Point3D> axisPoints, Vector3D axisOfRotation, Model3DGroup groupModelVisual, Model3DGroup groupDriveVisual, double modelThickness)
        {
            this.axisPoints = axisPoints;
            axisPoint = axisPoints[0];
            handPoint = axisPoints[1];
            this.axisOfRotation = axisOfRotation;

            this.groupModelVisual = groupModelVisual;
            this.groupDriveVisual = groupDriveVisual;
            this.modelThickness = modelThickness;

            coordsMidDoor = makeCoordsMidDoor();
            coordsUpDoor = makeCoordsUpDoor();
            coordsDrive = makeCoordsDrive();

            clearModel();
            buildSideDoor();
            buildDrive();
        }

        private List<Point3D> makeCoordsMidDoor()
        {
            List<Point3D> points = new List<Point3D>();
            return points;
        }

        private List<Point3D> makeCoordsUpDoor()
        {
            List<Point3D> points = new List<Point3D>();
            return points;
        }

        private List<Point3D> makeCoordsDrive()
        {
            List<Point3D> points = new List<Point3D>();
            return points;
        }

        private void buildDrive()
        {
            drive = new Drive(coordsDrive[0], coordsDrive[1], groupDriveVisual);
        }

        private void buildSideDoor()
        {
            buildMidPart();
            buildUpPart();
            buildAxisAndHandPoint();
            buildAttachmentToDrive();
        }

        private void buildAttachmentToDrive()
        {
            generateSphere(coordsMidDoor[4], 40, groupModelVisual, new DiffuseMaterial(Brushes.Cyan));
        }

        private void buildAxisAndHandPoint()
        {
            generateSphere(axisPoint, 50, groupModelVisual, new DiffuseMaterial(Brushes.Red));
            generateSphere(handPoint, 50, groupModelVisual, new DiffuseMaterial(Brushes.Red));
        }

        private void buildMidPart()
        {

        }

        private void buildUpPart()
        {

        }


        public void clearModel()
        {
            groupModelVisual.Children.Clear();
            groupDriveVisual.Children.Clear();
            //group2.Children.Remove(cuboidGeometry);
            //group2.Children.Remove(cylinderGeometry);
            //group2.Children.Remove(sphereGeometry);
        }


    }
}
