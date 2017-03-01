using KinematicViewer.Camera;
using KinematicViewer.Geometry;
using KinematicViewer.Geometry.GuidedElements;
using KinematicViewer.Geometry.Guides;
using KinematicViewer.Transformation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace KinematicViewer.UserControlLibrary
{
    /// <summary>
    /// Interaction logic for MainViewPortControl.xaml
    /// </summary>
    public partial class MainViewPortControl : UserControl
    {
        //Abfrage für die Maussteuerung
        private bool _bMouseDownRight;

        private bool _bMouseDownLeft;
        private bool _bMouseDownMiddle;
        private bool _bZoom = false;
        private bool _bDrag = false;
        private bool _bOrbit = false;
        private bool _bPan = false;
        private bool _bModelSelected = false;

        //Mausposition bei Klicken der linken Maustaste
        private Point _oPt_leftClick;

        //Selektionen von Visuellen Objekten und dessen Materialien für das Highlightinh
        private GeometricalElement _oSelectedElement;

        private Material _oSelectedMaterial;
        private Material _oHighlightedMaterial;

        //Klasse für Kameraeinstellungen und deren Positionen
        private ViewportCamera _oViewportCam;

        // Koordinatensystem im eigenen viewport
        private CoordSystem _oCoordSystem;

        private Viewport3D _oViewportCoordSystem;
        private Grid _oGridCoordSystem;

        //Klasse für Transformationen aller Art
        private CameraTransformation trans;

        //Interface- Instanz für den "GUIDE"
        private IGuide _oGuide;

        //Listen für alle visuellen Elemente
        public List<GeometricalElement> ElementsPassive;

        public List<GeometricalElement> ElementsActive;
        public List<GeometricalElement> ElementsStaticMinAngle;
        public List<GeometricalElement> ElementsStaticMaxAngle;
        public List<GeometricalElement> ElementsLineOfAction;
        public List<GeometricalElement> ElementsTrackPoint;

        //Rotationsachse
        private Vector3D _oVAxisOfRotation;

        //RotationsDrehpunkt
        private Point3D _oPAxisPoint;

        //Breite bzw Dicke des jeweiligen Models
        private double _dModelThickness;

        //Textblock um Hebelarm als Dezimalzahl darzustellen
        private TextBlock _oStatusPane;

        /// <summary>
        /// Viewport UserControl
        /// </summary>
        public MainViewPortControl()
        {
            InitializeComponent();
            //axisPoints = new List<Point3D>();

            trans = new CameraTransformation();

            //Viewport des Koordinatensystems erstellen
            ViewportCoordSystem = new Viewport3D();
            ViewportCoordSystem.ClipToBounds = true;

            //Visuelles Koordinatensystem erstellen
            createCoordSystem();

            //Listen für visuelle Elemente erzeugen
            ElementsActive = new List<GeometricalElement>();
            ElementsPassive = new List<GeometricalElement>();
            ElementsStaticMinAngle = new List<GeometricalElement>();
            ElementsStaticMaxAngle = new List<GeometricalElement>();
            ElementsLineOfAction = new List<GeometricalElement>();
            ElementsTrackPoint = new List<GeometricalElement>();

            //Kameras für die zwei Viewports erstellen
            ViewportCam = new ViewportCamera(viewport, ViewportCoordSystem, CoordSystem, trans);
            ViewportCam.startCoordSystemCamera();
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

        public Point Pt_leftClick
        {
            get { return _oPt_leftClick; }
            set { _oPt_leftClick = value; }
        }

        public GeometricalElement SelectedElement
        {
            get { return _oSelectedElement; }
            set { _oSelectedElement = value; }
        }

        public Material SelectedMaterial
        {
            get { return _oSelectedMaterial; }
            set { _oSelectedMaterial = value; }
        }

        public Material HighlightedMaterial
        {
            get { return _oHighlightedMaterial; }
            set { _oHighlightedMaterial = value; }
        }

        //Übergeben eines TextBlockObjectes an das ViewportControl
        public void setTextBlock(TextBlock statusPane)
        {
            this._oStatusPane = statusPane;
        }

        //Listen für visuelle Elemente befüllen
        //Aktives Element in entspr. Liste hinzufügen (Heckklappen- / Seitentürelemente)
        public void AddActiveElement(GeometricalElement elem)
        {
            ElementsActive.Add(elem);
            UpdateActiveGroup();
        }

        //Passives Element in entspr. Liste hinzufügen (Antriebselemente)
        public void AddPassiveElement(GeometricalElement elem)
        {
            ElementsPassive.Add(elem);
            UpdatePassiveGroup();
        }

        //Transparentes Element mit minimalen Öffnungswinkel in entspr. Liste hinzufügen
        public void AddStaticElementMinAngle(GeometricalElement elem)
        {
            ElementsStaticMinAngle.Add(elem);
            UpdateStaticMinAngleGroup();
        }

        //Transparentes Element mit maximalen Öffnungswinkel in entspr. Liste hinzufügen
        public void AddStaticElementMaxAngle(GeometricalElement elem)
        {
            ElementsStaticMaxAngle.Add(elem);
            UpdateStaticMaxAngleGroup();
        }

        //Wirkungslinien-Element in entspr. Liste hinzufügen
        public void AddLineOfActionElement(GeometricalElement elem)
        {
            ElementsLineOfAction.Add(elem);
            UpdateLineOfActionGroup();
        }

        //Spurpunkt-Element in entspr. Liste hinzufügen
        public void AddTrackPointElement(GeometricalElement elem)
        {
            ElementsTrackPoint.Add(elem);
            UpdateTrackPointGroup();
        }

        //Aktives Element aus entspr. Liste löschen
        public void RemoveActiveElement(GeometricalElement elem)
        {
            ElementsActive.Remove(elem);
            UpdateActiveGroup();
        }

        public void RemovePassiveElement(int i)
        {
            RemovePassiveElement(ElementsPassive[i]);
        }

        //Passives Element aus entspr. Liste löschen
        public void RemovePassiveElement(GeometricalElement elem)
        {
            if (ElementsPassive.Contains(elem))
            {
                ElementsPassive.Remove(elem);
                UpdatePassiveGroup();
            }
        }

        //Transparentes Element mit minimalem Öffnungswinkel aus entspr. Liste löschen
        public void RemoveStaticElementMinAngle(GeometricalElement elem)
        {
            ElementsStaticMinAngle.Remove(elem);
            UpdateStaticMinAngleGroup();
        }

        //Transparentes Element mit maximalem Öffnungswinkel aus entspr. Liste löschen
        public void RemoveStaticElementMaxAngle(GeometricalElement elem)
        {
            ElementsStaticMaxAngle.Remove(elem);
            UpdateStaticMaxAngleGroup();
        }

        //Wirkungslinien- Element aus entspr. Liste löschen
        public void RemoveLineOfActionElement(GeometricalElement elem)
        {
            ElementsLineOfAction.Remove(elem);
            UpdateLineOfActionGroup();
        }

        //Spurpunkt Element aus entspr. Liste löschen
        public void RemoveTrackPointElement(GeometricalElement elem)
        {
            ElementsTrackPoint.Remove(elem);
            UpdateTrackPointGroup();
        }

        //Der Render Gruppe für aktive Elemente die entspr. Liste hinzufügen und anschließend rendern
        private void UpdateActiveGroup()
        {
            groupActive.Children.Clear();

            foreach (GeometricalElement e in ElementsActive)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupActive.Children.Add(m);
        }

        //Der Render Gruppe für passive Elemente die entspr. Liste hinzufügen und anschließend rendern
        private void UpdatePassiveGroup()
        {
            groupPassive.Children.Clear();

            foreach (GeometricalElement e in ElementsPassive)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupPassive.Children.Add(m);
        }

        //Der Render Gruppe für transparente Elemente mit minimalem Öffnungswinkel die entspr. Liste hinzufügen und anschließend rendern
        private void UpdateStaticMinAngleGroup()
        {
            groupStaticMinAngle.Children.Clear();

            foreach (GeometricalElement e in ElementsStaticMinAngle)
                foreach (GeometryModel3D m in e.GetGeometryModel(null))
                    groupStaticMinAngle.Children.Add(m);
        }

        //Der Render Gruppe für transparente Elemente mit maximalem Öffnungswinkel die entspr. Liste hinzufügen und anschließend rendern
        private void UpdateStaticMaxAngleGroup()
        {
            groupStaticMaxAngle.Children.Clear();

            foreach (GeometricalElement e in ElementsStaticMinAngle)
                foreach (GeometryModel3D m in e.GetGeometryModel(null))
                    groupStaticMaxAngle.Children.Add(m);
        }

        //Der Render Gruppe für Wirkungslinien Elemente die entspr. Liste hinzufügen und anschließend rendern
        private void UpdateLineOfActionGroup()
        {
            groupLineOfAction.Children.Clear();

            foreach (GeometricalElement e in ElementsLineOfAction)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupLineOfAction.Children.Add(m);
        }

        //Der Render Gruppe für Spurpunkt Elemente die entspr. Liste hinzufügen und anschließend rendern
        private void UpdateTrackPointGroup()
        {
            groupTrackPoint.Children.Clear();

            foreach (GeometricalElement e in ElementsTrackPoint)
                foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    groupTrackPoint.Children.Add(m);
        }

        //Transparentes visuelles Bauraummodell in der Gruppe für minimale Öffnungswinkel entsprechend rotieren
        public void ShowStaticElementMin(double per)
        {
            //Guide.Move(groupStaticMinAngle, 0);
            VisualObjectTransformation.rotateModelGroup(per, AxisOfRotation, AxisPoint, groupStaticMinAngle);
            UpdateStaticMinAngleGroup();
        }

        //Transparentes visuelles Bauraummodell in der Gruppe für maximale Öffnungswinkel entsprechend rotieren
        public void ShowStaticElementMax(double per)
        {
            //Guide.Move(groupStaticMaxAngle, 1);
            VisualObjectTransformation.rotateModelGroup(per, AxisOfRotation, AxisPoint, groupStaticMinAngle);
            UpdateStaticMaxAngleGroup();
        }

        //Löschen der Inhalte in der Render Gruppe für transparente Elemente mit minimalem Öffnungswinkel
        public void RemoveAllStaticElementsMin()
        {
            ElementsStaticMinAngle.Clear();
            UpdateStaticMinAngleGroup();
        }

        //Löschen der Inhalte in der Render Gruppe für transparente Elemente mit maximalem Öffnungswinkel
        public void RemoveAllStaticElementsMax()
        {
            ElementsStaticMaxAngle.Clear();
            UpdateStaticMaxAngleGroup();
        }

        //Löschen der Inhalte in der Render Gruppe für Wirkungslinien
        public void RemoveAllLineOfActionElements()
        {
            ElementsLineOfAction.Clear();
            UpdateLineOfActionGroup();
        }

        //Löschen der Inhalte in der Render Gruppe für Spurpunkte
        public void RemoveAllTrackPointElements()
        {
            ElementsTrackPoint.Clear();
            UpdateTrackPointGroup();
        }

        //Visuelles Koordinatensystem erstellen und dem Viewport3D bzw. dem Main Grid hinzufügen
        private void createCoordSystem()
        {
            //Hintergrundfarbe im gleichen Farbton wie das MainViewport
            //Color c = (Color)ColorConverter.ConvertFromString("#eee9e9");
            Color c = (Color)ColorConverter.ConvertFromString("#E9E9E9");

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

        //Dem Viewport3D bzw dessen Main Grid das Koordinatensystem hinzufügen
        public void showCoordSystem()
        {
            MainGrid.Children.Add(GridCoordSystem);
        }

        //Koordinatensystem aus dem Main Grid entfernen
        public void removeCoordSystem()
        {
            MainGrid.Children.Remove(GridCoordSystem);
        }

        //MAUSSTEUERUNG im MainGrid
        //Mausrad
        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CanMoveCamera)
            {
                ViewportCam.viewport_Grid_MouseWheel(sender, e);
            }
        }

        //Mausbewegung
        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (CanMoveCamera)
            {
                if (_bDrag)
                    ViewportCam.dragCam();
                if (_bOrbit)
                    ViewportCam.orbitCam();
                if (_bZoom)
                    ViewportCam.zoomCamMouseMove();
                if (_bPan)
                    ViewportCam.panCam();
            }
        }

        //Mittlere Maustaste gedrückt
        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && !(_bMouseDownLeft || _bMouseDownRight))
            {
                _bMouseDownMiddle = true;
                Cursor = Cursors.None;
                _bDrag = true;

                Point pt = e.GetPosition(viewport);
                VisualTreeHelper.HitTest(viewport, null, HitTestMiddleDown, new PointHitTestParameters(pt));

                //Zentrierung des MausCursers im Mittelpunkt des Viewports für jegliche Transformationen
                ViewportCam.setMouseToCenter();
            }
        }

        //Mittlere Maustaste losgelassen
        private void MainGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _bMouseDownMiddle = false;
                Cursor = Cursors.Arrow;
                _bDrag = false;
                _bOrbit = false;
                _bZoom = false;
                _bPan = false;
            }
        }

        //Rechte Maustaste gedrückt
        private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _bMouseDownRight = true;
            if (_bMouseDownMiddle)
            {
                //Cursor = Cursors.Cross;
                _bDrag = false;
                _bZoom = false;
                _bOrbit = true;
            }
            if (!_bMouseDownMiddle && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                ViewportCam.setMouseToCenter();
                Cursor = Cursors.SizeAll;
                _bPan = true;
            }
        }

        //Rechte Maustaste losgelassen
        private void MainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _bMouseDownRight = false;
            if (_bMouseDownMiddle)
            {
                Cursor = Cursors.ScrollNS;
                _bDrag = false;
                _bOrbit = false;
                _bZoom = true;
            }
            if (!_bMouseDownMiddle && (!Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
            {
                Cursor = Cursors.Arrow;
                _bPan = false;
            }
        }

        //Linke Maustaste gedrückt
        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _bMouseDownLeft = true;

            //Focusübergabe an den Viewport
            viewport_MouseDown(sender, e);

            if (!_bMouseDownRight && !_bMouseDownMiddle)
            {
                //Testverfahren für mögliches Hittesting
                Point pt = e.GetPosition(viewport);
                Pt_leftClick = pt;
                VisualTreeHelper.HitTest(viewport, null, HitTestLeftDown, new PointHitTestParameters(pt));
            }
        }

        //Linke Maustaste losgelassen
        private void MainGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _bMouseDownLeft = false;

            if (_bModelSelected == false) return;
            else
            {
                _bModelSelected = false;

                if (SelectedMaterial != null && SelectedElement != null)
                {
                    SelectedElement.Material = SelectedMaterial;
                    SelectedElement = null;
                    UpdateActiveGroup();
                    //UpdatePassiveGroup();
                }
            }
        }

        //HitTest Verhalten, wenn mit mittlerer Maustaste auf das visuelle Objekt geklickt wird
        //Wenn mit mittlerer Maustaste auf einen Antrieb oder das Bauraummodell geklickt wird, fixiert sich die Kamera und Blickrichtung neu
        private HitTestResultBehavior HitTestMiddleDown(HitTestResult result)
        {
            RayMeshGeometry3DHitTestResult resultMesh = result as RayMeshGeometry3DHitTestResult;

            if (resultMesh == null)
                return HitTestResultBehavior.Continue;

            ModelVisual3D vis = resultMesh.VisualHit as ModelVisual3D;
            GeometryModel3D selectedModel = resultMesh.ModelHit as GeometryModel3D;

            if (vis == null)
                return HitTestResultBehavior.Continue;

            //Aktiv bewegte Bauraummodelle wie Heckklappen und Seitentüren
            if (vis == (viewport.FindName("activeVisual") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsActive)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            //MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}",e.Name,m.Bounds.X, m.Bounds.Y,m.Bounds.Z));
                            Point3D pointClicked = new Point3D(m.Bounds.X, m.Bounds.Y, m.Bounds.Z);
                            trans.RotationPoint = pointClicked;
                            ViewportCam.PointClicked = pointClicked;
                            ViewportCam.setCam();
                        }
                    }
                return HitTestResultBehavior.Stop;
            }

            //Passiv bewegte Bauraummodelle, also "guided" Elemente wie Antriebe
            if (vis == (viewport.FindName("passiveVisual") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsPassive)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            //MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}",e.Name,m.Bounds.X, m.Bounds.Y,m.Bounds.Z));
                            Point3D pointClicked = new Point3D(m.Bounds.X, m.Bounds.Y, m.Bounds.Z);
                            trans.RotationPoint = pointClicked;
                            ViewportCam.PointClicked = pointClicked;
                            ViewportCam.setCam();
                        }
                    }
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        //HitTest Verhalten, wenn mit Linker Maustaste auf das visuelle Objekt geklickt wird
        //Auswahl und Highlightingverfahren per links Klick auf Antrieb oder generelles visuelles Objekt im Viewport
        private HitTestResultBehavior HitTestLeftDown(HitTestResult result)
        {
            _bModelSelected = true;

            RayMeshGeometry3DHitTestResult resultMesh = result as RayMeshGeometry3DHitTestResult;

            if (resultMesh == null)
                return HitTestResultBehavior.Continue;

            ModelVisual3D vis = resultMesh.VisualHit as ModelVisual3D;
            GeometryModel3D selectedModel = resultMesh.ModelHit as GeometryModel3D;

            //Wenn kein visuelles Objekt angeklickt wird oder daneben soll mit Hit Testing weitergemacht werden
            if (vis == null)
                return HitTestResultBehavior.Continue;

            //Aktiv bewegte Bauraummodelle, wie Heckklappen und Seitentüren
            if (vis == (viewport.FindName("activeVisual") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsActive)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            if (e is Tailgate)
                            {
                                HighlightingElement((Tailgate)e);
                                UpdateActiveGroup();
                            }

                            if (e is SideDoor)
                            {
                                HighlightingElement((SideDoor)e);
                                UpdateActiveGroup();
                            }

                            MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}", e.Name, m.Bounds.X, m.Bounds.Y, m.Bounds.Z));
                        }
                    }
                return HitTestResultBehavior.Stop;
            }

            //Passiv bewegte Bauraummodelle, also "guided" Elemente wie Antriebe
            if (vis == (viewport.FindName("passiveVisual") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsPassive)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            if (e is Drive)
                            {
                                HighlightingElement((Drive)e);
                                UpdatePassiveGroup();
                            }
                            //MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}", e.Name, m.Bounds.X, m.Bounds.Y, m.Bounds.Z));
                        }
                    }
                return HitTestResultBehavior.Stop;
            }

            //Wirkungslinie
            if (vis == (viewport.FindName("lineOfActionVisual") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsLineOfAction)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}", e.Name, m.Bounds.X, m.Bounds.Y, m.Bounds.Z));
                        }
                    }
                return HitTestResultBehavior.Stop;
            }

            //Transparentes Bauraummodell minimaler Öffnungswinkel
            if (vis == (viewport.FindName("staticVisualMinAngle") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsStaticMinAngle)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}", e.Name, m.Bounds.X, m.Bounds.Y, m.Bounds.Z));
                        }
                    }
                return HitTestResultBehavior.Stop;
            }

            //Transparentes Bauraummodell maximaler Öffnungswinkel
            if (vis == (viewport.FindName("staticVisualMaxAngle") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsStaticMaxAngle)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}", e.Name, m.Bounds.X, m.Bounds.Y, m.Bounds.Z));
                        }
                    }
                return HitTestResultBehavior.Stop;
            }

            //Spurenpunkte
            if (vis == (viewport.FindName("trackPointVisual") as ModelVisual3D))
            {
                foreach (GeometricalElement e in ElementsTrackPoint)
                    foreach (GeometryModel3D m in e.GetGeometryModel(Guide))
                    {
                        if (m.Bounds == selectedModel.Bounds)
                        {
                            MessageBox.Show(String.Format("model found: {0} | {1},{2},{3}", e.Name, m.Bounds.X, m.Bounds.Y, m.Bounds.Z));
                        }
                    }
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        //TASTATURSTEUERUNG für KEY Down
        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            ViewportCam.viewport_KeyDown(sender, e);
        }

        //Dem Viewport bzw. dem MainGrid den Focus übergeben durch Mausklick Event
        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            e.Handled = true;
        }

        //Dem Viewport bzw. dem MainGrid den Focus übergeben durch ein Button Event
        public void FocusToViewport(object sender, RoutedEventArgs e)
        {
            MainGrid.Focus();
            e.Handled = true;
        }

        //Starten der perspektivischen Kamera
        public void switchToPerspective()
        {
            ViewportCam.startPerspectiveCamera();
            ViewportCam.MyCam = Cam.Perspective;
        }

        //Starten der orthographischen Kamera
        public void switchToOrthographic()
        {
            ViewportCam.startOrthographicCamera();
            ViewportCam.MyCam = Cam.Orthographic;
        }

        //Kamerartransformation zurücksetzen
        public void resetCam()
        {
            ViewportCam.reloadCameraPositionDefault();
            ViewportCam.resetCam();
        }

        //Default Position der Kamera laden
        public void reloadCameraPositionDefault()
        {
            ViewportCam.reloadCameraPositionDefault();
        }

        // Alle visuellen Objekte aus dem Viewport löschen
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

        //QUICKSTEUERUNG DER KAMERA mittels Toolbar Buttons
        //Die Szene von vorne betrachten
        public void viewFrontSide()
        {
            ViewportCam.viewFront();
        }

        //Die Szene von hinten betrachten
        public void viewBackSide()
        {
            ViewportCam.viewBack();
        }

        //Die Szene von rechts betrachten
        public void viewRightSide()
        {
            ViewportCam.viewRight();
        }

        //Die Szene von links betrachten
        public void viewLeftSide()
        {
            ViewportCam.viewLeft();
        }

        //Die Szene von oben betrachten
        public void viewTopSide()
        {
            ViewportCam.viewTop();
        }

        //Die Szene von unten betrachten
        public void viewBottomSide()
        {
            ViewportCam.viewBottom();
        }

        //Die Szene vergrößern
        public void zoomIn()
        {
            ViewportCam.ToolBoxZoomIn();
        }

        //Die Szene verkleiner
        public void zoomOut()
        {
            ViewportCam.ToolBoxZoomOut();
        }

        /// <summary>
        ///Bewegt den "Guide" um einen bestimmten Anteil
        /// </summary>
        /// <param name="per">Anteil der Bewegung in % [0-100]</param>
        public void Move(double per)
        {
            if (Guide == null)
                return;

            Guide.Move(per);
            UpdateActiveGroup();

            UpdatePassiveGroup();
            UpdateLineOfActionGroup();
            UpdateTrackPointGroup();
        }

        //Zurücksetzen der visuellen Objekttransformation inklusive aller Antriebe
        public void resetModelTransformation()
        {
            VisualObjectTransformation.resetModelGroupTransformation(groupActive);
        }

        //Farbiges Hervorheben der angeklickten visuellen Objekte (Hightlighting)
        private void HighlightingElement(GeometricalElement element)
        {
            HighlightedMaterial = new DiffuseMaterial(Brushes.LightSeaGreen);

            SelectedElement = element;
            if (SelectedMaterial == null)
                SelectedMaterial = element.Material;

            element.Material = HighlightedMaterial;
        }
    }
}