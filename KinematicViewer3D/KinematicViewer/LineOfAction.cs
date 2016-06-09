using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class LineOfAction : GeometricalElement
    {
        private const double RADIUS = 5;
        private const int ELEMENTS = 10;
        private const double ELEMENTLENGTH = 25;
        private const double OFFSET = 25;
        

        private Point3D _oAxisPoint;
        private Point3D _oAttachmentPointBody;
        private Point3D _oAttachmentPointDoor;
        private Point3D _oPerpendicular;

        private Vector3D _oVAxisOfRotation;
       // private Vector3D _oVDrive;
        private Material _oLineOfActionMaterial;

        private List<Point3D> _oLCoordsLinesOfAction;
        private Point3D startP;

        

        public LineOfAction(Point3D axisPoint, Point3D attachmentPointBody, Point3D attachmentPointDoor,  Vector3D axisOfRotation , Material mat = null)
            :base(mat)
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
            get { return _oLineOfActionMaterial;}
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

        public Point3D Perpendicular
        {
            get { return _oPerpendicular; }
            set { _oPerpendicular = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            Point3D attPointDoorUpdated = guide.MovePoint(AttachmentPointDoor);
            

            CoordsLinesOfAction = makeCoordsLinesOfAction(attPointDoorUpdated);


            for (int i = 0; i <= CoordsLinesOfAction.Count - 2; i += 2)
            {
                Res.AddRange(new Cylinder(CoordsLinesOfAction[i], CoordsLinesOfAction[i + 1], RADIUS, LineOfActionMaterial).GetGeometryModel(guide));
            }

            return Res.ToArray();
        }

        private List<Point3D> makeCoordsLinesOfAction(Point3D endPoint)
        {
            List<Point3D> points = new List<Point3D>();

            Point3D startPoint = AttachmentPointBody;
            
            Vector3D vDriveUpdated =  (startPoint - endPoint);
            vDriveUpdated.Normalize();
            //vDriveUpdated = TransformationUtilities.ScaleVector(vDriveUpdated, 1); 
            
            for(int i=0; i < ELEMENTS; i++ )
            {
                points.Add(startPoint);
                points.Add(startPoint + vDriveUpdated * ELEMENTLENGTH);
                startPoint = (startPoint + (vDriveUpdated * ELEMENTLENGTH) + (vDriveUpdated * OFFSET));   
            }

            return points;
        }

        public void calculatePerpendicular(Vector3D axisOfRotation , Vector3D axisDrive)
        {
            Point3D point = new Point3D();


            Perpendicular = point;
        }

    }
}
