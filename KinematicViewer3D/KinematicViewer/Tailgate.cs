using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Tailgate : GeometricalElement, IGuide
    {
        private const double OFFSET = 160.0;
        private const double TAILDEPTH = 200.0;
        private const double TAILWIDTH = 1250.0;

        private double _dTailWidth = 1250.0;
        private double _dToGo;
        private double _dCurVal;
        private double _dMaxOpen;
        private double _dMinOpen;
        private double _dLength;
        private double _dModelThickness;
        private bool _bTransparent;

        private Vector3D _oAxisOfRotation;
        private Point3D _oAxisPoint;
        private Point3D _oPointLatch;

        private List<Point3D> _oLCoordsDownTail;
        private List<Point3D> _oLCoordsMidTail;
        private List<Point3D> _oLCoordsUpTail;

        private Vector3D _oVAxisToHandE1;

        private Material _oAxisMaterial;

        public Tailgate(Point3D axisPoint, Point3D handPoint, Vector3D axisOfRotation, double modelThickness, bool transparent, double tailWidth = TAILWIDTH, Material mat = null)
            : base(mat)
        {
            AxisOfRotation = axisOfRotation;
            AxisLength = AxisOfRotation.Length;
            AxisOfRotation = TransformationUtilities.ScaleVector(AxisOfRotation, 1);

            AxisPoint = axisPoint;
            HandPoint = handPoint;
            MaxValue = 62.5;
            MinValue = 0.0;

            //if (!Double.IsNaN((double)tailWidth))
            TailWidth = tailWidth;
            //else
            //TailWidth = TAILWIDTH;

            ModelThickness = modelThickness;
            Transparent = transparent;

            VAxisToHandE1 = HandPoint - AxisPoint;

            CoordsMidTail = makeCoordsMidTail();
            CoordsUpTail = makeCoordsUpTail();
            CoordsDownTail = makeCoordsDownTail();

            AxisMaterial = new DiffuseMaterial(Brushes.Red);

            //if (mat == null)
            //    AxisMaterial = new DiffuseMaterial(Brushes.Red);
            //if (mat != null)
            //    AxisMaterial = mat;
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

        public Point3D HandPoint
        {
            get { return _oPointLatch; }
            private set { _oPointLatch = value; }
        }

        public double ModelThickness
        {
            get { return _dModelThickness; }
            private set { _dModelThickness = value; }
        }

        public List<Point3D> CoordsDownTail
        {
            get { return _oLCoordsDownTail; }
            private set { _oLCoordsDownTail = value; }
        }

        public List<Point3D> CoordsMidTail
        {
            get { return _oLCoordsMidTail; }
            private set { _oLCoordsMidTail = value; }
        }

        public List<Point3D> CoordsUpTail
        {
            get { return _oLCoordsUpTail; }
            private set { _oLCoordsUpTail = value; }
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

        public double TailWidth
        {
            get { return _dTailWidth; }
            set { _dTailWidth = value; }
        }

        public bool Transparent
        {
            get { return _bTransparent; }
            set { _bTransparent = value; }
        }

        public override GeometryModel3D[] GetGeometryModel(IGuide guide)
        {
            List<GeometryModel3D> Res = new List<GeometryModel3D>();

            //Klappe
            //Oberer Part
            for (int i = 0; i <= CoordsUpTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsUpTail[i], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsUpTail[i], CoordsUpTail[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            //Unterer Part
            for (int i = 0; i <= CoordsDownTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsDownTail[i], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsDownTail[i], CoordsDownTail[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            //Mittlerer Part
            for (int i = 0; i <= CoordsMidTail.Count - 2; i++)
            {
                Res.AddRange(new Sphere(CoordsMidTail[i], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
                Res.AddRange(new Cuboid(CoordsMidTail[i], CoordsMidTail[i + 1], ModelThickness, Material).GetGeometryModel(guide));
            }

            Res.AddRange(new Sphere(CoordsMidTail[3], ModelThickness * 1.5, 16, 16, Material).GetGeometryModel(guide));
            Res.AddRange(new Cuboid(CoordsMidTail[3], CoordsMidTail[0], ModelThickness, Material).GetGeometryModel(guide));

            if (!Transparent)
            {
                //Handle
                Res.AddRange(new Sphere(HandPoint, 50, 16, 16, AxisMaterial).GetGeometryModel(guide));

                //Drehachse
                Point3D p1 = AxisPoint + AxisOfRotation * TailWidth * 1 / 2;
                Point3D p2 = AxisPoint - AxisOfRotation * TailWidth * 1 / 2;

                Res.AddRange(new Sphere(AxisPoint, 50, 16, 16, AxisMaterial).GetGeometryModel(guide));
                Res.AddRange(new Cylinder(p1, p2, 10, AxisMaterial).GetGeometryModel(guide));
            }
            else
            {
                //Handle in Transparentem Material
                Res.AddRange(new Sphere(HandPoint, 50, 16, 16, Material).GetGeometryModel(guide));
            }

            return Res.ToArray();
        }

        //Erstelle Koordinaten für den unteren Teil der Heckklappe
        private List<Point3D> makeCoordsDownTail()
        {
            List<Point3D> points = new List<Point3D>();

            Vector3D v1 = TransformationUtilities.scaleToOffset(new Vector3D(0, 1, 0), TAILDEPTH * 2);
            //Vector3D v1 = TransformationUtilities.ScaleVector(new Vector3D(0, 1, 0), TAILDEPTH * 2 );

            Point3D pUpL = CoordsMidTail[1];
            Point3D pUpR = CoordsMidTail[2];
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

            Vector3D vAxisQuerE2 = Vector3D.CrossProduct(VAxisToHandE1, new Vector3D(0, 1, 0));
            vAxisQuerE2.Normalize();
            //vAxisQuerE2 = TransformationUtilities.ScaleVector(vAxisQuerE2, 1);

            Vector3D vN = Vector3D.CrossProduct(vAxisQuerE2, VAxisToHandE1);
            vN.Normalize();
            //vN = TransformationUtilities.ScaleVector(vN, 1);

            Point3D pUpL = AxisPoint + (vN * OFFSET) + (vAxisQuerE2 * TailWidth / 2);
            Point3D pUpR = AxisPoint + (vN * OFFSET) - (vAxisQuerE2 * TailWidth / 2);
            Point3D pDownL = HandPoint + (vN * OFFSET) + (vAxisQuerE2 * TailWidth / 2);
            Point3D pDownR = HandPoint + (vN * OFFSET) - (vAxisQuerE2 * TailWidth / 2);

            points.Add(pUpL);
            points.Add(pDownL);
            points.Add(pDownR);
            points.Add(pUpR);

            return points;
        }

        //Erstelle Koordinaten für den oberen Teil der Heckklappe
        private List<Point3D> makeCoordsUpTail()
        {
            List<Point3D> points = new List<Point3D>();
            Vector3D v1 = new Vector3D(VAxisToHandE1.X, 0, VAxisToHandE1.Z);
            v1 = TransformationUtilities.scaleToOffset(v1, TAILDEPTH);
            //v1 = TransformationUtilities.ScaleVector(v1, TAILDEPTH);

            Point3D pDownL = CoordsMidTail[0];
            Point3D pDownR = CoordsMidTail[3];
            Point3D pUpL = pDownL - v1;
            Point3D pUpR = pDownR - v1;

            points.Add(pDownL);
            points.Add(pUpL);
            points.Add(pUpR);
            points.Add(pDownR);

            return points;
        }

        public void InitiateMove(double per)
        {
            double NewVal = per * MaxValue;

            _dToGo = NewVal - CurValue;
            CurValue = NewVal;
        }

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

        public void Move(Model3DGroup groupActive, double per = 0)
        {
            InitiateMove(per);
            Transformation.rotateModel(CurValue, AxisOfRotation, AxisPoint, groupActive);
        }

        public void Move(double per = 0)
        {

            InitiateMove(per);

            for (int i = 0; i < CoordsUpTail.Count; i++)
            {
                CoordsUpTail[i] = TransformationUtilities.rotateNewPoint(CoordsUpTail[i], _dToGo, AxisOfRotation, AxisPoint);
            }

            for (int i = 0; i < CoordsMidTail.Count; i++)
            {
                CoordsMidTail[i] = TransformationUtilities.rotateNewPoint(CoordsMidTail[i], _dToGo, AxisOfRotation, AxisPoint);
            }

            for (int i = 0; i < CoordsDownTail.Count; i++)
            {
                CoordsDownTail[i] = TransformationUtilities.rotateNewPoint(CoordsDownTail[i], _dToGo, AxisOfRotation, AxisPoint);
            }

            HandPoint = TransformationUtilities.rotateNewPoint(HandPoint, _dToGo, AxisOfRotation, AxisPoint);
        }

    }
}