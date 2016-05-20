using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Diagnostics;

namespace KinematicViewer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainViewPortControl : UserControl
    {
        private bool mouseDownRight;
        private bool mouseDownLeft;

        //Klasse für Kameraeinstellungen und deren Positionen
        private ViewportCamera viewportCam;

        //Koordinatensystem für rechten Dock Panel erstellen
        private CoordSystemSmall c_SystemSmall;

        //Klasse für Transformationen aller Art
        private Transformation trans;

        //public Cuboid cube;
        //public Cuboid2 cube2;
        //public Sphere sphere;
        //public Cylinder cylinder;
        //public Tailgate tail;

       
        private IGuide Guide;

        public List<GeometricalElement> ElementsStatic;
        public List<GeometricalElement> ElementsMoving;

        //Achsenpunkte der Benutzereingabe
        //private List<Point3D> axisPoints;

        //Mittelpunkt des Objektes
        private Point3D mPoint;

        //Breite bzw Dicke des jeweiligen Models
        private double modelThickness;

        //maximaler Öffnungswinkel
        double maxOpen = 62.5;

        private Vector3D axisOfRotation;

        private string s_coords;
        private TextBlock statusPane;

        public void AddMovingElement(GeometricalElement elem)
        {
            ElementsMoving.Add(elem);
        }

        public void addStaticElement(GeometricalElement elem)
        {
            ElementsStatic.Add(elem);
        }

        public MainViewPortControl()
        {

            
            InitializeComponent();
            //axisPoints = new List<Point3D>();
            trans = new Transformation();
            c_SystemSmall = new CoordSystemSmall();
            viewportCam = new ViewportCamera(MainGrid, viewport, c_SystemSmall, trans);

            viewportCam.startPerspectiveCamera();
            viewportCam.MyCam = Cam.Perspective;
            viewportCam.resetCam();

            setAxisOfRotation(0, 0, 1);


            //viewportCam.resetCam();
            CanMoveCamera = true;
        }

        private void generateVisualModel()
        {
            //tail = new Tailgate(AxisPoints, axisOfRotation, modelThickness);

            foreach (GeometricalElement e in ElementsMoving)
                foreach (Model3D m in e.GetGeometryModel())
                    groupModelVisual.Children.Add(m);

            trans.resetModelTransformation(groupModelVisual);
            trans.resetModelTransformation(groupDriveVisual);

            ////Kamera für Main Viewport updaten
            viewportCam.updatePositionCamera();

            //Kamera im Fenster des Koordinatensystems ändern
            //c_SystemSmall.updatePositionCamera_CoordinateSystem(viewportCam.CameraR, viewportCam.CameraPhi, viewportCam.CameraTheta);

            //Mittelpunkt des Modells berechnen
            calculateMPoint();
        }


        //MAUSSTEUERUNG im MainGrid
        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CanMoveCamera)
            {
                viewportCam.viewport_Grid_MouseWheel(sender, e);
            }
        }

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (CanMoveCamera)
            {
                // Wenn Rechte Maustaste "nicht" gedrückt dann passiert auch nichts
                if (!mouseDownRight) return;

                viewportCam.rotateCam();
            }

            if (!CanMoveCamera)
            {
                if (!mouseDownLeft) return;

                showScreenCoords(sender, e);
            }
        }

        private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanMoveCamera)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;

                mouseDownRight = true;
                viewportCam.setMouseToCenter();

                // Curser unsichtbar machen
                this.Cursor = Cursors.None;
            }
        }

        private void MainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Maustaste ist nicht länger gedrückt, also wird Curser wieder sichtbar
            mouseDownRight = false;
            Cursor = Cursors.Arrow;
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownLeft = true;
            CanMoveCamera = false;

            //dem Viewport den Focus übergeben, sodass die Tasteneingabe funktioniert
            viewport_MouseDown(sender, e);

            //Testverfahren für mögliches Hittesting
            Point pt = e.GetPosition(viewport);
            VisualTreeHelper.HitTest(viewport, null, HitTestDown, new PointHitTestParameters(pt));
        }

        //HitTest Verhalten wenn auf einen Antrieb oder ein visuelles Model geklickt wird
        HitTestResultBehavior HitTestDown(HitTestResult result)
        {
            RayMeshGeometry3DHitTestResult resultMesh = result as RayMeshGeometry3DHitTestResult;

            if (resultMesh == null)
                return HitTestResultBehavior.Continue;

            ModelVisual3D vis = resultMesh.VisualHit as ModelVisual3D;

            if (vis == null)
                return HitTestResultBehavior.Continue;

            if(vis == (viewport.FindName("driveVisual") as ModelVisual3D))
            {
                changeModelColorRandom(resultMesh);
                return HitTestResultBehavior.Stop;
            }

            if(vis == (viewport.FindName("modelVisual") as ModelVisual3D))
            {
                changeModelColorRandom(resultMesh);
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
            
        }

        private void MainGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDownLeft = false;
            CanMoveCamera = true;
        }

        //TASTATURSTEUERUNG für KEY Down
        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            viewportCam.viewport_KeyDown(sender, e);
        }

        //Dem Viewport bzw. dem MainGrid den Focus übergeben
        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            e.Handled = true;
        }

        public void switchToPerspective()
        {
            viewportCam.startPerspectiveCamera();
            viewportCam.MyCam = Cam.Perspective;
        }

        public void switchToOrthographic()
        {
            viewportCam.startOrthographicCamera();
            viewportCam.MyCam = Cam.Orthographic;
        }

        public void resetCam()
        {
            viewportCam.resetCam();
        }

        public void reloadCameraPositionDefault()
        {
            viewportCam.reloadCameraPositionDefault();
        }

        public void clearModel()
        {
            groupModelVisual.Children.Clear();
            groupDriveVisual.Children.Clear();
        }

        public void createModel()
        {
            generateVisualModel();   
        }

        public void viewFrontSide()
        {
            viewportCam.viewFront();
        }

        public void viewBackSide()
        {
            viewportCam.viewBack();
        }

        public void viewRightSide()
        {
            viewportCam.viewRight();
        }

        public void viewLeftSide()
        {
            viewportCam.viewLeft();
        }

        public void viewTopSide()
        {
            viewportCam.viewTop();
        }

        public void viewBottomSide()
        {
            viewportCam.viewBottom();
        }

        public void zoomIn()
        {
            viewportCam.zoomIn();
        }

        public void zoomOut()
        {
            viewportCam.zoomOut();
        }

        private void calculateMPoint()
        {
            double count = axisPoints.Count;
            double x = 0;
            double y = 0;
            double z = 0;
            for (int i = 0; i < count; i++)
            {
                x += axisPoints[i].X / count;
                y += axisPoints[i].Y / count;
                z += axisPoints[i].Z / count;
            }
            mPoint = new Point3D(x, y, z);
        }

        public void sliderRotate(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            double axisAngle = (e.NewValue *maxOpen / 100);

            Guide.Move(e.NewValue, ElementsMoving);

            //trans.rotateModel(axisAngle, axisOfRotation, axisPoints, groupModelVisual);
            ////trans.rotateDrive(axisAngle, axisOfRotation, axisPoints, groupDriveVisual);

            //Point3D attPDLeft = trans.rotateDrivePoint(axisAngle, axisOfRotation, axisPoints, tail.AttachmentPointDoorLeft);
            //Point3D attPDRight = trans.rotateDrivePoint(axisAngle, axisOfRotation, axisPoints, tail.AttachmentPointDoorRight);
            //tail.updateDrives(attPDLeft, attPDRight);
            
        }

        public void resetModelTransformation(object sender, RoutedEventArgs e)
        {
            trans.resetModelTransformation(groupModelVisual);
            e.Handled = true;
        }




        private void changeModelColorRandom(RayMeshGeometry3DHitTestResult resultMesh)
        {
            GeometryModel3D model = resultMesh.ModelHit as GeometryModel3D;
            DiffuseMaterial mat = model.Material as DiffuseMaterial;

            Random rand = new Random();
            mat.Brush = new SolidColorBrush(Color.FromRgb((byte)rand.Next(256),
                                                          (byte)rand.Next(256),
                                                          (byte)rand.Next(256)));
        }

        private void showScreenCoords(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(viewport);
            s_coords = string.Format("Bild-Koordinaten: ({0:d}, {1:d})", (int)p.X, (int)p.Y);
            this.statusPane.Text = s_coords;
        }



        //Öffentliche Getter & Setter Methoden
        public bool CanMoveCamera { get; set; }

        public double ModelThickness
        {
            get { return modelThickness; }
            set { modelThickness = value; }
        }

        public List<Point3D> AxisPoints
        {
            get { return axisPoints; }
            set { axisPoints = value; }
        }

        public string S_Coords
        {
            get { return s_coords; }
            set { s_coords = value; }
        }

        
        public void setAxisOfRotation(double x, double y, double z)
        {
            axisOfRotation = new Vector3D(x, y, z);
        }

        //Übergeben eines TextBlockObjectes an das ViewportControl
        public void setTextBlock(TextBlock statusPane)
        {
            this.statusPane = statusPane;
        }

    }
}
