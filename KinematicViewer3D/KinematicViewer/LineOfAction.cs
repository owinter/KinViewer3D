using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class LineOfAction : GeometricalElement
    {
        private const double RADIUS = 5;
        private const int ELEMENTS = 10;
        private const double ELEMENTLENGTH = 20;
        private const double OFFSET = 10;

        private double _dDistancePerpendicular;

        private Point3D _oAxisPoint;
        private Point3D _oAttachmentPointBody;
        private Point3D _oAttachmentPointDoor;

        private Vector3D _oVAxisOfRotation;

        // private Vector3D _oVDrive;
        private Material _oLineOfActionMaterial;

        private List<Point3D> _oLCoordsLinesOfAction;
        private List<Point3D> _oLPerpendiculars;

        public LineOfAction(Point3D axisPoint, Point3D attachmentPointBody, Point3D attachmentPointDoor, Vector3D axisOfRotation, Material mat = null)
            : base(mat)
        {
            AxisPoint = axisPoint;
            AttachmentPointBody = attachmentPointBody;
            AttachmentPointDoor = attachmentPointDoor;
            AxisOfRotation = axisOfRotation;
            //AxisOfRotation = new Vector3D(0, 0, 1);

            //Vector3D vDrive = AttachmentPointBody - AttachmentPointDoor;

            LineOfActionMaterial = new DiffuseMaterial(Brushes.Red);
        }

        public Point3D AxisPoint
        {
            get { return _oAxisPoint; }
            set { _oAxisPoint = value; }
        }

        public Point3D AttachmentPointBody
        {
            get { return _oAttachmentPointBody; }
            private set { _oAttachmentPointBody = value; }
        }

        public Vector3D AxisOfRotation
        {
            get { return _oVAxisOfRotation; }
            set { _oVAxisOfRotation = value; }
        }

        public Material LineOfActionMaterial
        {
            get { return _oLineOfActionMaterial; }
            set { _oLineOfActionMaterial = value; }
        }

        public Point3D AttachmentPointDoor
        {
            get { return _oAttachmentPointDoor; }
            set { _oAttachmentPointDoor = value; }
        }

        public List<Point3D> CoordsLinesOfAction
        {
            get { return _oLCoordsLinesOfAction; }
            set { _oLCoordsLinesOfAction = value; }
        }

        public List<Point3D> Perpendiculars
        {
            get { return _oLPerpendiculars; }
            set { _oLPerpendiculars = value; }
        }

        public double DistancePerpendicular
        {
            get { return _dDistancePerpendicular; }
            set { _dDistancePerpendicular = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D attPointDoorUpdated = guide.MovePoint(AttachmentPointDoor);

            //Test Perpendicular
            Vector3D vDriveU = AttachmentPointBody - attPointDoorUpdated;
            Perpendiculars = NearestPointsOfTwoVectors(AxisOfRotation, vDriveU, AxisPoint, AttachmentPointBody);

            CoordsLinesOfAction = makeCoordsLinesOfAction(Perpendiculars[0], Perpendiculars[1]);

            //for(int i = 0; i <= CoordsLinesOfAction.Count / 2 - 2; i +=2)
            //{
            //    Res.AddRange(new Cylinder(CoordsLinesOfAction[i], CoordsLinesOfAction[i + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            //}
            //for(int j = CoordsLinesOfAction.Count/2 +1; j <= CoordsLinesOfAction.Count - 2; j += 2 )
            //{
            //    Res.AddRange(new Cylinder(CoordsLinesOfAction[j], CoordsLinesOfAction[j + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            //}

            //for (int i = 0; i <= CoordsLinesOfAction.Count - 2; i += 2)
            //{
            //    Res.AddRange(new Cylinder(CoordsLinesOfAction[i], CoordsLinesOfAction[i + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            //}

            //for (int i = 0; i <=Perpendiculars.Count -2; i += 2)
            //{
            //    Res.AddRange(new Cylinder(Perpendiculars[i], Perpendiculars[i + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            //}
            Res.AddRange(new Cylinder(Perpendiculars[0], Perpendiculars[1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            Res.AddRange(new Cylinder(Perpendiculars[0], AttachmentPointBody, RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            Res.AddRange(new Sphere(Perpendiculars[0], RADIUS * 2, 16, 16, LineOfActionMaterial).GetGeometryModel(guide));

            return Res.ToArray();
        }

        private List<Point3D> makeCoordsLinesOfAction(Point3D perpendicularAxis, Point3D perpendicularDrive)
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D vD = perpendicularDrive - AttachmentPointBody;
            Vector3D vA = perpendicularAxis - perpendicularDrive;

            int elementsD = Convert.ToInt16(Math.Ceiling(vD.Length / ELEMENTLENGTH));
            int elementsA = Convert.ToInt16(Math.Ceiling(vA.Length / ELEMENTLENGTH));

            vD.Normalize();
            vA.Normalize();

            Point3D startD = perpendicularDrive;
            Point3D startA = perpendicularAxis;

            for (int i = 0; i < elementsD; i++)
            {
                points.Add(startD);
                points.Add(startD + vD * ELEMENTLENGTH);
                startD = (startD + (vD * ELEMENTLENGTH) + (vD * OFFSET));
            }

            for (int j = 0; j < elementsA; j++)
            {
                points.Add(startA);
                points.Add(startA + vA * ELEMENTLENGTH);
                startA = (startA + (vA * ELEMENTLENGTH) + (vA * OFFSET));
            }

            //Point3D startPoint = AttachmentPointBody;

            //Vector3D vDriveUpdated = (startPoint - endPoint);
            //vDriveUpdated.Normalize();
            ////vDriveUpdated = TransformationUtilities.ScaleVector(vDriveUpdated, 1);

            //for (int i = 0; i < ELEMENTS; i++)
            //{
            //    points.Add(startPoint);
            //    points.Add(startPoint + vDriveUpdated * ELEMENTLENGTH);
            //    startPoint = (startPoint + (vDriveUpdated * ELEMENTLENGTH) + (vDriveUpdated * OFFSET));
            //}

            return points;
        }

        public List<Point3D> NearestPointsOfTwoVectors(Vector3D axisOfRotation, Vector3D vDrive, Point3D axisPoint, Point3D attachmentPointBody)
        {
            const double SMALL_NUM = 0.00000001; // "Division overflow" um nicht durch 0 zu teilen

            List<Point3D> points = new List<Point3D>();

            Vector3D u = axisOfRotation;
            Vector3D v = vDrive;
            Vector3D w = attachmentPointBody - axisPoint;

            double a = Vector3D.DotProduct(u, u);   // >= 0
            double b = Vector3D.DotProduct(u, v);
            double c = Vector3D.DotProduct(v, v);   // >= 0
            double d = Vector3D.DotProduct(u, w);
            double e = Vector3D.DotProduct(v, w);
            double D = a * c - b * b;               // >= 0
            double sc, tc;

            if (D < SMALL_NUM)
            {
                sc = 0.0;
                tc = (b > c ? (d / b) : (e / c)); // den größten wert nehmen
            }
            else
            {
                sc = (b * e - c * d) / D;
                tc = (a * e - b * d) / D;
            }
            Point3D p0 = new Point3D((w - (tc * v)).X, (w - (tc * v)).Y, (w - (tc * v)).Z); // Lotfußpunkt auf dem Drive Vektor
            Vector3D dP = w + (sc * u) - (tc * v);                                          // kürzester Vektor zwischen beiden Vektoren
            Point3D p1 = p0 - dP;                                                           // Lotfußpunkt auf der Drehachse

            DistancePerpendicular = dP.Length;  //Länge des kürzesten Vektors

            points.Add(p0); // Lotfußpunkt auf dem Drive Vektor
            points.Add(p1); // Lotfußpunkt auf der Drehachse

            return points;
        }
    }
}