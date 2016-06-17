using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class LineOfAction : GeometricalElement
    {
        private const double RADIUS = 2;
        private const double ELEMENTLENGTH = 15;
        private const double OFFSET = 2;

        private double _dDistancePerpendicular;

        private Point3D _oAxisPoint;
        private Point3D _oAttachmentPointBody;
        private Point3D _oAttachmentPointDoor;

        private Vector3D _oVAxisOfRotation;

        private Material _oLineOfActionMaterial;
        private Material _oPerpendicularMaterial;

        private List<Point3D> _oLCoordsLinesOfActionAxisSegments;
        private List<Point3D> _oLCoordsLineOfActionDriveSegments;
        private List<Point3D> _oLPerpendiculars;

        public LineOfAction(Point3D axisPoint, Point3D attachmentPointBody, Point3D attachmentPointDoor, Vector3D axisOfRotation, Material mat = null)
            : base(mat)
        {
            AxisPoint = axisPoint;
            AttachmentPointBody = attachmentPointBody;
            AttachmentPointDoor = attachmentPointDoor;
            AxisOfRotation = axisOfRotation;

            LineOfActionMaterial = new DiffuseMaterial(Brushes.Red);
            PerpendicularMaterial = new DiffuseMaterial(Brushes.Black);
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

        public Material PerpendicularMaterial
        {
            get { return _oPerpendicularMaterial; }
            set { _oPerpendicularMaterial = value; }
        }

        public Point3D AttachmentPointDoor
        {
            get { return _oAttachmentPointDoor; }
            set { _oAttachmentPointDoor = value; }
        }

        public List<Point3D> CoordsLineOfActionAxisSegments
        {
            get { return _oLCoordsLinesOfActionAxisSegments; }
            set { _oLCoordsLinesOfActionAxisSegments = value; }
        }

        public List<Point3D> CoordsLineOfActionDriveSegments
        {
            get { return _oLCoordsLineOfActionDriveSegments; }
            set { _oLCoordsLineOfActionDriveSegments = value; }
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
            Vector3D vDriveUpdated = AttachmentPointBody - attPointDoorUpdated;

            Perpendiculars = calculatePerpendiculars(AxisOfRotation, vDriveUpdated, AxisPoint, AttachmentPointBody);

            CoordsLineOfActionAxisSegments = makeCoordsLineOfActionAxisSegment(Perpendiculars[1], Perpendiculars[0]);
            CoordsLineOfActionDriveSegments = makeCoordsLineOfActionDriveSegment(AttachmentPointBody, Perpendiculars[0]);

            //Lotfußpunkt auf der Drehachse
            Res.AddRange(new Sphere(Perpendiculars[1], RADIUS * 4, 16, 8, PerpendicularMaterial).GetGeometryModel(guide));

            //Elemente entlang des kürzesten Vektors zwischen beiden Lotpunkten
            for (int i = 0; i < CoordsLineOfActionAxisSegments.Count; i += 2)
            {
                Res.AddRange(new Cylinder(CoordsLineOfActionAxisSegments[i], CoordsLineOfActionAxisSegments[i + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            }

            //Elemente entlang des Antriebs zum Lotfußpunkt
            for (int i = 0; i < CoordsLineOfActionDriveSegments.Count; i += 2)
            {
                Res.AddRange(new Cylinder(CoordsLineOfActionDriveSegments[i], CoordsLineOfActionDriveSegments[i + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            }

            //Lotfußpunkt auf der verlängerten Antriebsachse
            Res.AddRange(new Sphere(Perpendiculars[0], RADIUS * 4, 16, 8, PerpendicularMaterial).GetGeometryModel(guide));

            //Res.AddRange(new Cylinder(Perpendiculars[0], Perpendiculars[1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            //Res.AddRange(new Cylinder(Perpendiculars[0], AttachmentPointBody, RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            //Res.AddRange(new Sphere(Perpendiculars[0], RADIUS * 2, 16, 16, LineOfActionMaterial).GetGeometryModel(guide));

            return Res.ToArray();
        }

        private List<Point3D> makeCoordsLineOfActionAxisSegment(Point3D startPoint, Point3D perpendicularDrive)
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D vDirection = perpendicularDrive - startPoint;

            int elements = Convert.ToInt16((Math.Floor(vDirection.Length / (ELEMENTLENGTH + OFFSET))));
            double leftOver = vDirection.Length % (ELEMENTLENGTH + OFFSET);

            vDirection = TransformationUtilities.ScaleVector(vDirection, 1);

            Point3D startElem = startPoint;
            for (int i = 0; i < elements; i++)
            {
                points.Add(startElem);
                points.Add(startElem + vDirection * ELEMENTLENGTH);
                startElem = (startElem + vDirection * ELEMENTLENGTH) + (vDirection * OFFSET);
            }
            if (leftOver > 0)
            {
                points.Add(startElem);
                points.Add(startElem + vDirection * leftOver);
            }

            return points;
        }

        private List<Point3D> makeCoordsLineOfActionDriveSegment(Point3D startPoint, Point3D perpendicularDrive)
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D vDirection = perpendicularDrive - startPoint;

            int elements = Convert.ToInt16((Math.Ceiling(vDirection.Length / ELEMENTLENGTH)) + 5);

            vDirection = TransformationUtilities.ScaleVector(vDirection, 1);

            Point3D startElem = startPoint;
            for (int i = 0; i < elements; i++)
            {
                points.Add(startElem);
                points.Add(startElem + vDirection * ELEMENTLENGTH);
                startElem = (startElem + vDirection * ELEMENTLENGTH) + (vDirection * OFFSET);
            }

            return points;
        }

        public List<Point3D> calculatePerpendiculars(Vector3D axisOfRotation, Vector3D vDrive, Point3D axisPoint, Point3D attachmentPointBody)
        {
            const double SMALL_DIVISOR = 0.00000001; // "Division overflow", um nicht durch 0 zu teilen

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

            if (D < SMALL_DIVISOR)                    // Falls die Vektoren parallel
            {
                sc = 0.0;
                tc = (b > c ? (d / b) : (e / c));     // den größten wert nehmen
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