using KinematicViewer.Geometry.Figures;
using KinematicViewer.Transformation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer.Geometry.Guides
{
    public class SideDoor : GeometricalElement, IGuide
    {
        private const double OFFSET = 200.0;
        private const double DOORDEPTH = 200.0;
        private const double DOORHEIGHTBODY = 750.0;
        private const double DOORHEIGHTWINDOW = 450;
        private const double DOORHEIGHT = 1200.0;
        private const double DOORWIDTH = 1250;

        private double _dCurVal;
        private double _dDeltaCurVal;
        private double _dMaxOpen;
        private double _dMinOpen;
        private double _dModelThickness;
        private double _dLength;

        private bool _bTransparent;

        private Vector3D _vY = new Vector3D(0, 1, 0);
        private Vector3D _oVAxisToHandE1;
        private Vector3D _oAxisOfRotation;
        private Point3D _oAxisPoint;
        private Point3D _oPointLatch;

        private List<Point3D> _oLCoordsBodyPart;
        private List<Point3D> _oLCoordsWindowPart;

        private Material _oAxisMaterial;

        /// <summary>
        /// Erzeugt ein visuelles Modell einer Seitentür
        /// </summary>
        /// <param name="axisPoint">Mittelpunkt der Drehachse</param>
        /// <param name="latch">Schlosspunkt der Seitentüre</param>
        /// <param name="axisOfRotation">Drehachse als Vektor</param>
        /// <param name="modelThickness">Rahmendicke der Seitentüre</param>
        /// <param name="transparent">andelt es sich um eine transparente Türe, um minimalen und maximalen Öffnungswinkel darzustellen</param>
        /// <param name="mat">Oberflächenmaterial der Seitentüre</param>
        public SideDoor(Point3D axisPoint, Point3D latch, Vector3D axisOfRotation, double modelThickness, bool transparent, Material mat = null)
            : base(mat)
        {
            AxisOfRotation = axisOfRotation;
            //AxisOfRotation.Normalize();
            AxisLength = axisOfRotation.Length;

            AxisOfRotation = TransformationUtilities.ScaleVector(AxisOfRotation, 1);

            AxisPoint = axisPoint;
            LatchPoint = latch;
            MaxValue = 80.0;
            MinValue = 0.0;

            ModelThickness = modelThickness;
            Transparent = transparent;

            VAxisToHandE1 = LatchPoint - AxisPoint;
            VAxisToHandE1 = TransformationUtilities.ScaleVector(VAxisToHandE1, 1);

            CoordsBodyPart = makeCoordsBodyPart();
            CoordsWindowPart = makeCoordsWindowPart();

            AxisMaterial = new DiffuseMaterial(Brushes.Red);
        }

        public Vector3D AxisOfRotation
        {
            get { return _oAxisOfRotation; }
            private set { _oAxisOfRotation = value; }
        }

        public Point3D AxisPoint
        {
            get { return _oAxisPoint; }
            private set { _oAxisPoint = value; }
        }

        public double CurValue
        {
            get { return _dCurVal; }
            set { _dCurVal = value; }
        }

        public double DeltaCurValue
        {
            get { return _dDeltaCurVal; }
            set { _dDeltaCurVal = value; }
        }

        public double MaxValue
        {
            get { return _dMaxOpen; }
            set { _dMaxOpen = value; }
        }

        public double MinValue
        {
            get { return _dMinOpen; }
            set { _dMinOpen = value; }
        }

        public Point3D LatchPoint
        {
            get { return _oPointLatch; }
            private set { _oPointLatch = value; }
        }

        public double ModelThickness
        {
            get { return _dModelThickness; }
            private set { _dModelThickness = value; }
        }

        public List<Point3D> CoordsBodyPart
        {
            get { return _oLCoordsBodyPart; }
            private set { _oLCoordsBodyPart = value; }
        }

        public List<Point3D> CoordsWindowPart
        {
            get { return _oLCoordsWindowPart; }
            private set { _oLCoordsWindowPart = value; }
        }

        public Vector3D VAxisToHandE1
        {
            get { return _oVAxisToHandE1; }
            set { _oVAxisToHandE1 = value; }
        }

        public Material AxisMaterial
        {
            get { return _oAxisMaterial; }
            set { _oAxisMaterial = value; }
        }

        public double AxisLength
        {
            get { return _dLength; }
            set { _dLength = value; }
        }

        public bool Transparent
        {
            get { return _bTransparent; }
            set { _bTransparent = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            //Seitentüre
            //Oberer Part
            for (int i = 0; i <= CoordsWindowPart.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsWindowPart[i], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsWindowPart[i], CoordsWindowPart[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            //Karossierie Part
            for (int i = 0; i <= CoordsBodyPart.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsBodyPart[i], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsBodyPart[i], CoordsBodyPart[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            Res.AddRange(new Sphere(CoordsBodyPart[3], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
            Res.AddRange(new Cuboid(CoordsBodyPart[3], CoordsBodyPart[0], ModelThickness, Material).GetGeometryModel(guide));

            //Bei transparentem Modell wird die Drehachse weggelassen
            if (!Transparent)
            {
                //Handle
                Res.AddRange(new Sphere(LatchPoint, 50, 16, 16, AxisMaterial).GetGeometryModel(guide));

                //Drehachse
                Point3D p1 = AxisPoint + AxisOfRotation * AxisLength / 2;
                Point3D p2 = AxisPoint - AxisOfRotation * AxisLength / 2;

                //Point3D p1 = AxisPoint + TransformationUtilities.ScaleVector(AxisOfRotation, DOORHEIGHT * 0.5);
                //Point3D p2 = AxisPoint - TransformationUtilities.ScaleVector(AxisOfRotation, DOORHEIGHT * 0.5);

                Res.AddRange(new Sphere(AxisPoint, 50, 16, 16, AxisMaterial).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(p1, p2, 10, AxisMaterial).GetGeometryModel(guide));
            }
            else
            {
                //Handle
                Res.AddRange(new Sphere(LatchPoint, 50, 16, 16, Material).GetGeometryModel(guide));
            }

            return Res.ToArray();
        }

        //Erstelle Koordinaten für den mittleren Karosserieteil der Seitentüre
        private List<Point3D> makeCoordsBodyPart()
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pUpR = LatchPoint + (new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z) * OFFSET) + (_vY * OFFSET);
            Point3D pUpL = pUpR - (new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z) * DOORWIDTH);
            Point3D pDownL = pUpL - (_vY * DOORHEIGHTBODY);
            Point3D pDownR = pDownL + (new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z) * DOORWIDTH);

            points.Add(pUpR);
            points.Add(pUpL);
            points.Add(pDownL);
            points.Add(pDownR);

            return points;
        }

        //Erstelle Koordinaten für den oberen Fenster Teil der Seitentüre
        private List<Point3D> makeCoordsWindowPart()
        {
            List<Point3D> points = new List<Point3D>();

            Point3D pDownR = LatchPoint + (new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z) * OFFSET) + (_vY * OFFSET);
            Point3D pDownL = pDownR - (new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z) * DOORWIDTH);
            Point3D pUpR = pDownR + _vY * DOORHEIGHTWINDOW;
            Point3D pUpL = pUpR - (new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z) * DOORWIDTH / 2);

            points.Add(pDownR);
            points.Add(pUpR);
            points.Add(pUpL);
            points.Add(pDownL);

            return points;
        }

        public void InitiateMove(double per)
        {
            double NewVal = per * MaxValue;

            DeltaCurValue = NewVal - CurValue;
            CurValue = NewVal;
        }

        /// <summary>
        /// Bewegt den Anbindungspunkt des Antriebs an die Seitentür
        /// </summary>
        /// <param name="endPoint">Anbindungspunkt des Antriebs an Seitentür (Attachment Point Door)</param>
        /// <returns>Liefert einen rotierten Anbindungspunkt zurück</returns>
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

        /// <summary>
        /// Versetzt die transparente Seitentür in minimal oder maximal Stellung des Öffnungswinkels
        /// </summary>
        /// <param name="groupActive">Seitentür als komplette Model3DGroup</param>
        /// <param name="per">Anteil der Bewegung in [%]</param>
        public void Move(Model3DGroup groupActive, double per = 0)
        {
            InitiateMove(per);
            VisualObjectTransformation.rotateModelGroup(CurValue, AxisOfRotation, AxisPoint, groupActive);
        }

        /// <summary>
        /// Versetzt jeden Koordinatenpunkt der Seitentür in eine Bewegung
        /// </summary>
        /// <param name="per">Anteil der Bewegung in [%]</param>
        public void Move(double per = 0)
        {
            InitiateMove(per);

            for (int i = 0; i < CoordsBodyPart.Count; i++)
            {
                CoordsBodyPart[i] = VisualObjectTransformation.rotatePoint(CoordsBodyPart[i], DeltaCurValue, AxisOfRotation, AxisPoint);
            }
            for (int i = 0; i < CoordsWindowPart.Count; i++)
            {
                CoordsWindowPart[i] = VisualObjectTransformation.rotatePoint(CoordsWindowPart[i], DeltaCurValue, AxisOfRotation, AxisPoint);
            }

            LatchPoint = VisualObjectTransformation.rotatePoint(LatchPoint, DeltaCurValue, AxisOfRotation, AxisPoint);
        }
    }
}