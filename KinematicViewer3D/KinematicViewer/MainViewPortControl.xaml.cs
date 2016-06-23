using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainViewPortControl : UserControl
    {
        private bool _bMouseDownRight;
        private bool _bMouseDownLeft;
        private bool _bMouseDownMiddle;

        //Klasse für Kameraeinstellungen und deren Positionen
        private ViewportCamera _oViewportCam;

        // Koordinatensystem im eigenen viewport
        private CoordSystem _oCoordSystem;

        private Viewport3D _oViewportCoordSystem;
        private Grid _oGridCoordSystem;

        //Klasse für Transformationen aller Art
        private Transformation trans;

        private IGuide _oGuide;

        public List<GeometricalElement> ElementsPassive;
        public List<GeometricalElement> ElementsActive;
        public List<GeometricalElement> ElementsStaticMinAngle;
        public List<GeometricalElement> ElementsStaticMaxAngle;
        public List<GeometricalElement> ElementsLineOfAction;
        public List<GeometricalElement> ElementsTrackPoint;

        //Mittelpunkt des Objektes
        private Point3D _oMPoint;

        //Rotationsachse
        private Vector3D _oVAxisOfRotation;

        //RotationsDrehpunkt
        private Point3D _oPAxisPoint;

        //Breite bzw Dicke des jeweiligen Models
        private double _dModelThickness;

        private string _sScreenCoords;
        private TextBlock _oStatusPane;

        public MainViewPortControl()
        {
            InitializeComponent();
            //axisPoints = new List<Point3D>();

            trans = new Transformation();

            //Viewport des Koordinatensystems erstellen
            ViewportCoordSystem = new Viewport3D();
            ViewportCoordSystem.ClipToBounds = true;

            //Visuelles Koordinatensystem erstellen
            createCoordSystem();

            ElementsActive = new List<GeometricalElement>();
            ElementsPassive = new List<GeometricalElement>();
            ElementsStaticMinAngle = new List<GeometricalElement>();
            ElementsStaticMaxAngle = new List<GeometricalElement>();
            ElementsLineOfAction = new List<GeometricalElement>();
            ElementsTrackPoint = new List<GeometricalElement>();

            //Kameras für die zwei Viewports erstellen
            ViewportCam = new ViewportCamera(MainGrid, viewport, ViewportCoordSystem, CoordSystem, trans);
            ViewportCam.makeCoordSystemCamera();
            ViewportCam.startPerspectiveCamera();
            ViewportCam.MyCam = Cam.Perspective;
            ViewportCam.resetCam();

            CanMoveCamera = true;
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
            get { return _sScreenCoords; }
            set { _sScreenCoords = value; }
        }

        public ViewportCamera ViewportCam
        {
            get { return _oViewportCam; }
            set { _oViewportCam = value; }
        }

        public IGuide Guide
        {
            get { return _oGuide; }
            set { _oGuide = value; }
        }

        public Vector3D AxisOfRotation
        {
            get { return _oVAxisOfRotation; }
            set { _oVAxisOfRotation = value; }
        }

        public Point3D AxisPoint
        {
            get { return _oPAxisPoint; }
            set { _oPAxisPoint = value; }
        }

        public CoordSystem CoordSystem
        {
            get { return _oCoordSystem; }
            set { _oCoordSystem = value; }
        }

        public Viewport3D ViewportCoordSystem
        {
            get { return _oViewportCoordSystem; }
            set { _oViewportCoordSystem = value; }
        }

        public Grid GridCoordSystem
        {
            get { return _oGridCoordSystem; }
            set { _oGridCoordSystem = value; }
        }

        //Übergeben eines TextBlockObjectes an das ViewportControl
        public void setTextBlock(TextBlock statusPane)
        {
            this._oStatusPane = statusPane;
        }

        public void AddActiveElement(GeometricalElement elem)
        {
            ElementsActive.Add(elem);
            UpdateActiveGroup();
        }

        public void AddPassiveElement(GeometricalElement elem)
        {
            ElementsPassive.Add(elem);
            UpdatePassiveGroup();
        }

        public void AddStaticElementMinAngle(GeometricalElement elem)
        {
            ElementsStaticMinAngle.Add(elem);
            UpdateStaticMinAngleGroup();
        }

        public void AddStaticElementMaxAngle(GeometricalElement elem)
        {
            ElementsStaticMaxAngle.Add(elem);
            UpdateStaticMaxAngleGroup();
        }

        public void AddLineOfActionElement(GeometricalElement elem)
        {
            ElementsLineOfAction.Add(elem);
            UpdateLineOfActionGroup();
        }

        public void AddTrackPointElement(GeometricalElement elem)
        {
            ElementsTrackPoint.Add(elem);
            UpdateTrackPointGroup();
        }

        public void RemoveActiveElement(GeometricalElement elem)
        {
            ElementsActive.Remove(elem);
            UpdateActiveGroup();
        }

        public void RemovePassiveElement(GeometricalElement elem)
        {
            ElementsPassive.Remove(elem);
            UpdatePassiveGroup();
        }

        public void RemoveStaticElementMinAngle(GeometricalElement elem)
        {
            ElementsStaticMinAngle.Remove(elem);
            UpdateStaticMinAngleGroup();
        }

        public void RemoveStaticElementMaxAngle(GeometricalElement elem)
        {
            ElementsStaticMaxAngle.Remove(elem);
            UpdateStaticMaxAngleGroup();
        }

        public void RemoveLineOfActionElement(GeometricalElement elem)
        {
            ElementsLineOfAction.Remove(elem);
            UpdateLineOfActionGroup();
        }

        public void RemoveTrackPointElement(GeometricalElement elem)
        {
            ElementsTrackPoint.Remove(elem);
            UpdateTrackPointGroup();
        }

        private void UpdateActiveGroup()
        {
            groupActive.Children.Clear();

            foreach (GeometricalElement e in ElementsActive)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupActive.Children.Add(m);
        }

        private void UpdatePassiveGroup()
        {
            groupPassive.Children.Clear();

            foreach (GeometricalElement e in ElementsPassive)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupPassive.Children.Add(m);
        }

        private void UpdateStaticMinAngleGroup()
        {
            groupStaticMinAngle.Children.Clear();

            foreach (GeometricalElement e in ElementsStaticMinAngle)
                foreach (GeometryModel3D m in e.GetGeometryModel(null))
                    groupStaticMinAngle.Children.Add(m);
        }

        private void UpdateStaticMaxAngleGroup()
        {
            groupStaticMaxAngle.Children.Clear();

            foreach (GeometricalElement e in ElementsStaticMinAngle)
                foreach (GeometryModel3D m in e.GetGeometryModel(null))
                    groupStaticMaxAngle.Children.Add(m);
        }

        private void UpdateLineOfActionGroup()
        {
            groupLineOfAction.Children.Clear();

            foreach (GeometricalElement e in ElementsLineOfAction)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupLineOfAction.Children.Add(m);
        }

        private void UpdateTrackPointGroup()
        {
            groupTrackPoint.Children.Clear();

            foreach (GeometricalElement e in ElementsTrackPoint)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupTrackPoint.Children.Add(m);
        }

        public void ShowStaticElementMin(double per)
        {
            //Guide.Move(groupStaticMinAngle, 0);
            Transformation.rotateModel(per, AxisOfRotation, AxisPoint, groupStaticMinAngle);
            UpdateStaticMinAngleGroup();
        }

        public void ShowStaticElementMax(double per)
        {
            //Guide.Move(groupStaticMaxAngle, 1);
            Transformation.rotateModel(per, AxisOfRotation, AxisPoint, groupStaticMinAngle);
            UpdateStaticMaxAngleGroup();
        }

        public void RemoveAllStaticElementsMin()
        {
            ElementsStaticMinAngle.Clear();
            UpdateStaticMinAngleGroup();
        }

        public void RemoveAllStaticElementsMax()
        {
            ElementsStaticMaxAngle.Clear();
            UpdateStaticMaxAngleGroup();
        }

        public void RemoveAllLineOfActionElements()
        {
            ElementsLineOfAction.Clear();
            UpdateLineOfActionGroup();
        }

        public void RemoveAllTrackPointElements()
        {
            ElementsTrackPoint.Clear();
            UpdateTrackPointGroup();
        }

        //Visuelles Koordinatensystem erstellen
        private void createCoordSystem()
        {
            //Hintergrundfarbe im gleichen Farbton wie das MainViewport
            Color c = (Color)ColorConverter.ConvertFromString("#eee9e9");

            //Grid Koordinatensystem
            GridCoordSystem = new Grid();
            GridCoordSystem.Background = new SolidColorBrush(c);
            GridCoordSystem.Height = 160;
            GridCoordSystem.Width = 160;
            GridCoordSystem.HorizontalAlignment = HorizontalAlignment.Right;
            GridCoordSystem.VerticalAlignment = VerticalAlignment.Top;

            //Border Koordinatensystem
            Border csBorder = new Border();
            csBorder.Background = new SolidColorBrush(c);
            csBorder.BorderBrush = Brushes.Silver;
            csBorder.BorderThickness = new Thickness(2);
            csBorder.Width = 160;
            csBorder.Height = 160;

            //visueller Inhalt ( 3 farbige Achsen ) des Koordinatensystems
            CoordSystem = new CoordSystem(ViewportCoordSystem);

            //dem Grid des Koordinatensystems die Border und viewport hinzufügen
            GridCoordSystem.Children.Add(csBorder);
            GridCoordSystem.Children.Add(ViewportCoordSystem);

            //das Grid Koordinatensystem dem MainGrid hinzufügen
            MainGrid.Children.Add(GridCoordSystem);
        }

        public void showCoordSystem()
        {
            MainGrid.Children.Add(GridCoordSystem);
        }

        public void removeCoordSystem()
        {
            MainGrid.Children.Remove(GridCoordSystem);
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
                if (_bMouseDownRight && (!_bMouseDownLeft || !_bMouseDownMiddle))
                {
                    ViewportCam.orbitCam();
                }

                if (_bMouseDownMiddle && (!_bMouseDownLeft || !_bMouseDownRight))
                {
                    ViewportCam.dragCam();
                }

                if (_bMouseDownLeft)
                {
                    ViewportCam.panCam();
                }
            }
        }

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _bMouseDownMiddle = true;
                ViewportCam.setMouseToCenter();
            }
        }

        private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _bMouseDownMiddle = false;
            }
        }

        private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CanMoveCamera)
            {
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
            //CanMoveCamera = false;
            _bMouseDownLeft = true;
            ViewportCam.setMouseToCenter();

            //Testverfahren für mögliches Hittesting
            Point pt = e.GetPosition(viewport);
            VisualTreeHelper.HitTest(viewport, null, HitTestDown, new PointHitTestParameters(pt));

            //dem Viewport den Focus übergeben, sodass die Tasteneingabe funktioniert
            viewport_MouseDown(sender, e);
        }

        private void MainGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!CanMoveCamera)
            {
                if (!_bMouseDownLeft) return;

                showScreenCoords(sender, e);
            }
            _bMouseDownLeft = false;
            CanMoveCamera = true;
        }

        //HitTest Verhalten wenn auf einen Antrieb oder ein visuelles Model geklickt wird
        private HitTestResultBehavior HitTestDown(HitTestResult result)
        {
            RayMeshGeometry3DHitTestResult resultMesh = result as RayMeshGeometry3DHitTestResult;

            if (resultMesh == null)
                return HitTestResultBehavior.Continue;

            ModelVisual3D vis = resultMesh.VisualHit as ModelVisual3D;

            if (vis == null)
                return HitTestResultBehavior.Continue;

            if (vis == (viewport.FindName("passiveVisual") as ModelVisual3D))
            {
                //changeModelColorRandom(resultMesh);
                return HitTestResultBehavior.Stop;
            }

            if (vis == (viewport.FindName("activeVisual") as ModelVisual3D))
            {
                trans.RotationPoint = AxisPoint;
                ViewportCam.setCam();

                changeModelColorRandom(resultMesh);
                return HitTestResultBehavior.Stop;
            }

            if (vis == (viewport.FindName("lineOfActionVisual") as ModelVisual3D))
            {
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
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

        public void FocusToViewport(object sender, RoutedEventArgs e)
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
            ViewportCam.reloadCameraPositionDefault();
            ViewportCam.resetCam();
        }

        public void reloadCameraPositionDefault()
        {
            ViewportCam.reloadCameraPositionDefault();
        }

        public void clearModel()
        {
            ElementsActive.Clear();
            UpdateActiveGroup();

            ElementsPassive.Clear();
            UpdatePassiveGroup();

            ElementsStaticMinAngle.Clear();
            UpdateStaticMinAngleGroup();

            ElementsStaticMaxAngle.Clear();
            UpdateStaticMaxAngleGroup();

            ElementsLineOfAction.Clear();
            UpdateLineOfActionGroup();

            ElementsTrackPoint.Clear();
            UpdateTrackPointGroup();

            Guide = null;
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


        /// <summary>
        ///Bewegt den "Guide" um einen bestimmten Anteil
        /// </summary>
        /// <param name="per">Anteil der Bewegung in % [0-100]</param>
        public void Move(double per)
        {
            if (Guide == null)
                return;

            Guide.Move(groupActive, per);

            UpdatePassiveGroup();
            UpdateLineOfActionGroup();
            UpdateTrackPointGroup();

            ////Test für die Transparenten Materialien
            //---> hat negativen Einfluss auf die Performance
            //UpdateStaticMinAngleGroup();
            //UpdateStaticMaxAngleGroup();
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
            _sScreenCoords = string.Format("Bild-Koordinaten: ({0:d}, {1:d})", (int)p.X, (int)p.Y);
            _oStatusPane.Text = _sScreenCoords;
        }
    }
}