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
        private Point3D _oAxisPoint;
        private Point3D _oAttachmentPointBody;
        private Vector3D _oVAxisOfRotation;
        private Material _oLineOfActionMaterial;

        

        public LineOfAction(Point3D axisPoint, Point3D attachmentPoint, Vector3D axisOfRotation , Material mat = null)
            :base(mat)
        {

            AxisPoint = axisPoint;
            AttachmentPointBody = attachmentPoint;
            //AxisOfRotation = axisOfRotation;
            AxisOfRotation = new Vector3D(0, 0, 1);
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
            set { _oAttachmentPointBody = value; }
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

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();


            return Res.ToArray();
        }

    }
}
