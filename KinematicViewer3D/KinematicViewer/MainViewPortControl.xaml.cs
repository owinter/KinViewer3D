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
        private bool _bMouseDownRight;
        private bool _bMouseDownLeft;

        //Klasse für Kameraeinstellungen und deren Positionen
        private ViewportCamera _oViewportCam;

        //Koordinatensystem für rechten Dock Panel erstellen
        private CoordSystemSmall _oC_SystemSmall;

        //Klasse für Transformationen aller Art
        private Transformation trans;
       
        private IGuide _oGuide;

        public List<GeometricalElement> ElementsPassive;
        public List<GeometricalElement> ElementsActive;


        //Mittelpunkt des Objektes
        private Point3D _oMPoint;

        //Breite bzw Dicke des jeweiligen Models
        private double _dModelThickness;

        private string s_coords;
        private TextBlock statusPane;


        public MainViewPortControl()
        {

            
            InitializeComponent();
            //axisPoints = new List<Point3D>();
            trans = new Transformation();
            _oC_SystemSmall = new CoordSystemSmall();
            ViewportCam = new ViewportCamera(MainGrid, viewport, _oC_SystemSmall, trans);

            ViewportCam.startPerspectiveCamera();
            ViewportCam.MyCam = Cam.Perspective;
            ViewportCam.resetCam();

            ElementsActive = new List<GeometricalElement>();
            ElementsPassive = new List<GeometricalElement>();


            //viewportCam.resetCam();
            CanMoveCamera = true;
        }

        public void AddActiveElement(GeometricalElement elem)
        {
            ElementsActive.Add(elem);
            UpdateActiveElements();
        }

        public void AddPassiveElement(GeometricalElement elem)
        {
            ElementsPassive.Add(elem);
            UpdatePassiveElements();
        }

        private void UpdateActiveElements()
        {
            trans.resetModelTransformation(groupActive);

            foreach (GeometricalElement e in ElementsActive)
                foreach (Model3D m in e.GetGeometryModel(Guide))
                    groupActive.Children.Add(m);
        }

        private void UpdatePassiveElements()
        {
            groupPassive.Children.Clear();

            foreach (GeometricalElement e in ElementsPassive)
                foreach (Model3D m in e.GetGeometryModel(Guide))
                    groupPassive.Children.Add(m);
        }


        //MAUSSTEUERUNG im MainGrid
        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CanMoveCamera)
            {
                ViewportCam.viewport_Grid_MouseWheel(sender, e);
            }
        }

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (CanMoveCamera)
            {
                // Wenn Rechte Maustaste "nicht" gedrückt dann passiert auch nichts
                if (!_bMouseDownRight) return;

                ViewportCam.rotateCam();
            }

            if (!CanMoveCamera)
            {
                if (!_bMouseDownLeft) return;

                showScreenCoords(sender, e);
            }
        }

        private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanMoveCamera)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;

                _bMouseDownRight = true;
                ViewportCam.setMouseToCenter();

                // Curser unsichtbar machen
                this.Cursor = Cursors.None;
            }
        }

        private void MainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Maustaste ist nicht länger gedrückt, also wird Curser wieder sichtbar
            _bMouseDownRight = false;
            Cursor = Cursors.Arrow;
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _bMouseDownLeft = true;
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
            _bMouseDownLeft = false;
            CanMoveCamera = true;
        }

        //TASTATURSTEUERUNG für KEY Down
        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            ViewportCam.viewport_KeyDown(sender, e);
        }

        //Dem Viewport bzw. dem MainGrid den Focus übergeben
        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            e.Handled = true;
        }

        public void switchToPerspective()
        {
            ViewportCam.startPerspectiveCamera();
            ViewportCam.MyCam = Cam.Perspective;
        }

        public void switchToOrthographic()
        {
            ViewportCam.startOrthographicCamera();
            ViewportCam.MyCam = Cam.Orthographic;
        }

        public void resetCam()
        {
            ViewportCam.resetCam();
        }

        public void reloadCameraPositionDefault()
        {
            ViewportCam.reloadCameraPositionDefault();
        }

        public void clearModel()
        {
            groupActive.Children.Clear();
            groupPassive.Children.Clear();
        }

        public void viewFrontSide()
        {
            ViewportCam.viewFront();
        }

        public void viewBackSide()
        {
            ViewportCam.viewBack();
        }

        public void viewRightSide()
        {
            ViewportCam.viewRight();
        }

        public void viewLeftSide()
        {
            ViewportCam.viewLeft();
        }

        public void viewTopSide()
        {
            ViewportCam.viewTop();
        }

        public void viewBottomSide()
        {
            ViewportCam.viewBottom();
        }

        public void zoomIn()
        {
            ViewportCam.zoomIn();
        }

        public void zoomOut()
        {
            ViewportCam.zoomOut();
        }

        //private void calculateMPoint()
        //{
        //    double count = AxisPoints.Count;
        //    double x = 0;
        //    double y = 0;
        //    double z = 0;
        //    for (int i = 0; i < count; i++)
        //    {
        //        x += AxisPoints[i].X / count;
        //        y += AxisPoints[i].Y / count;
        //        z += AxisPoints[i].Z / count;
        //    }
        //    _oMPoint = new Point3D(x, y, z);
        //}

        //public delegate void ViewPortEventHandler(object sender, ProgressEventArgs e);
        //public event ViewPortEventHandler ViewUpdated;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="per">Anteil der Bewegung in % [0-100]</param>
        public void Move(double per)
        {           

            Guide.InitiateMove(per);

            Tailgate tail = ElementsActive[0] as Tailgate;
            trans.rotateModel(tail.CurValue, tail.AxisOfRotation,tail.AxisPoint, groupActive);

            ////trans.rotateDrive(axisAngle, _oVAxisOfRotation, axisPoints, groupDriveVisual);

            /*Point3D attPDLeft = trans.rotateDrivePoint(axisAngle, _oVAxisOfRotation, _oLAxisPoints, tail.AttachmentPointDoorLeft);
            Point3D attPDRight = trans.rotateDrivePoint(axisAngle, _oVAxisOfRotation, _oLAxisPoints, tail.AttachmentPointDoorRight);
            updateDrive(_oLAxisPoints[2], attPDLeft);*/

            //if (ViewUpdated != null)
            //    ViewUpdated(this, new ProgressEventArgs());    

            UpdatePassiveElements();      
        }

        public void resetModelTransformation()
        {
            trans.resetModelTransformation(groupActive);  
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
            get { return _dModelThickness; }
            set { _dModelThickness = value; }
        }

        public string S_Coords
        {
            get { return s_coords; }
            set { s_coords = value; }
        }

        public ViewportCamera ViewportCam
        {
            get
            {
                return _oViewportCam;
            }

            set
            {
                _oViewportCam = value;
            }
        }

        public IGuide Guide
        {
            get
            {
                return _oGuide;
            }

            set
            {
                _oGuide = value;
            }
        }

        //Übergeben eines TextBlockObjectes an das ViewportControl
        public void setTextBlock(TextBlock statusPane)
        {
            this.statusPane = statusPane;
        }

    }
}
