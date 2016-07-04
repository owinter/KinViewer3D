using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public enum Cam
    {
        Perspective,
        Orthographic
    };

    public class ViewportCamera
    {
        //Entscheidung für perspektivische oder orthographische Kamera
        private Cam _cCam;

        //perspektivische Camera
        public PerspectiveCamera oP_Camera;

        //Camera für kleines Koordinatensystem
        public PerspectiveCamera oP_Camera_CoordSystem;

        //orthografische Camera
        public OrthographicCamera oO_Camera;

        //Kamerabreite für orthographische Kamera
        private double _dOrthoWidth;

        private double _dZoomFactorToolBox;
        private double _dZoomFactorMouse;

        //Mitte des ViewPort
        private Point _oCenterOfViewport;

        //angeklickter Rotationspunkt
        private Point3D _oPointClicked;

        //Speicherung der yaw und pitch Werte
        private double _dYaw;

        private double _dPit;

        //benutzbare Instanzen
        private Viewport3D _oViewport;

        private Viewport3D _oViewportCoordSystem;
        private Grid _oMainGrid;
        private CoordSystem _oCoordSystem;
        private Transformation _oTrans;
        private Transformation _oTrans_CSS;

        //Konstruktor
        public ViewportCamera(Grid mainGrid, Viewport3D viewport, Viewport3D viewportCoordSystem, CoordSystem coordSystem, Transformation trans)
        {
            MainGrid = mainGrid;
            Viewport = viewport;
            CoordSystem = coordSystem;
            ViewportCoordSystem = viewportCoordSystem;
            Trans = trans;

            Trans_CSS = new Transformation();

            OrthoWidth = 3200;
            ZoomFactorToolBox = 300;
            ZoomFactorMouse = 5;
            PointClicked = new Point3D(0, 0, 0);
            Trans.RotationPoint = PointClicked;
        }

        public double Yaw
        {
            get { return _dYaw; }
            set { _dYaw = value; }
        }

        public double Pit
        {
            get { return _dPit; }
            set { _dPit = value; }
        }

        public double OrthoWidth
        {
            get { return _dOrthoWidth; }
            set { _dOrthoWidth = value; }
        }

        public double ZoomFactorToolBox
        {
            get { return _dZoomFactorToolBox; }
            set { _dZoomFactorToolBox = value; }
        }

        public double ZoomFactorMouse
        {
            get { return _dZoomFactorMouse; }
            set { _dZoomFactorMouse = value; }
        }

        public Point3D PointClicked
        {
            get { return _oPointClicked; }
            set { _oPointClicked = value; }
        }

        public Viewport3D Viewport
        {
            get { return _oViewport; }
            set { _oViewport = value; }
        }

        public Viewport3D ViewportCoordSystem
        {
            get { return _oViewportCoordSystem; }
            private set { _oViewportCoordSystem = value; }
        }

        public Grid MainGrid
        {
            get { return _oMainGrid; }
            private set { _oMainGrid = value; }
        }

        public CoordSystem CoordSystem
        {
            get { return _oCoordSystem; }
            private set { _oCoordSystem = value; }
        }

        public Transformation Trans
        {
            get { return _oTrans; }
            private set { _oTrans = value; }
        }

        public Transformation Trans_CSS
        {
            get { return _oTrans_CSS; }
            private set { _oTrans_CSS = value; }
        }

        //Erstelle perspektivische Kamerasicht
        public void startPerspectiveCamera()
        {
            oP_Camera = new PerspectiveCamera();
            oP_Camera.Position = new Point3D(0, 0, 4000);
            oP_Camera.LookDirection = new Vector3D(0, 0, -4000);
            oP_Camera.UpDirection = new Vector3D(0, 1, 0);
            oP_Camera.FieldOfView = 45;
            oP_Camera.FarPlaneDistance = 25000;
            oP_Camera.NearPlaneDistance = 0.125;
            Trans.Reset(oP_Camera);
            Viewport.Camera = oP_Camera;
        }

        //Erstelle orthographische Kamerasicht
        public void startOrthographicCamera()
        {
            oO_Camera = new OrthographicCamera();
            oO_Camera.Position = new Point3D(0, 0, 4000);
            oO_Camera.LookDirection = new Vector3D(0, 0, -4000);
            oO_Camera.UpDirection = new Vector3D(0, 1, 0);
            oO_Camera.FarPlaneDistance = 25000;
            oO_Camera.NearPlaneDistance = 0.125;
            oO_Camera.Width = OrthoWidth;
            Trans.Reset(oO_Camera);
            Viewport.Camera = oO_Camera;
        }

        public void makeCoordSystemCamera()
        {
            oP_Camera_CoordSystem = new PerspectiveCamera();
            oP_Camera_CoordSystem.Position = new Point3D(0, 0, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(0, 0, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
            oP_Camera_CoordSystem.FieldOfView = 60;
            oP_Camera_CoordSystem.FarPlaneDistance = 5000;
            oP_Camera_CoordSystem.NearPlaneDistance = 0.125;
            //Trans_CSS.Reset(oP_Camera_CoordSystem);
            oP_Camera_CoordSystem.Transform = new Transform3DGroup();
            Trans_CSS.Yaw = 0;
            Trans_CSS.Pitch = 0;
            ViewportCoordSystem.Camera = oP_Camera_CoordSystem;
        }

        //Position der Camera updaten
        public void updatePositionCamera()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Yaw = Trans.Yaw;
                        Pit = Trans.Pitch;
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Yaw = Trans.Yaw;
                        Pit = Trans.Pitch;
                        oO_Camera.Width = OrthoWidth;
                    }
                    break;

                default:
                    throw new Exception("unidentified Cam");
            }
        }

        //Default Werte der Camera position und Objekttransformation aus XAML Datei laden
        public void reloadCameraPositionDefault()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        // Kamera Postion für Main Viewer reloaden
                        oP_Camera.Position = new Point3D(0, 0, 4000);
                        oP_Camera.LookDirection = new Vector3D(0, 0, -4000);
                        oP_Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        // Kamera Postion für Main Viewer reloaden
                        oO_Camera.Position = new Point3D(0, 0, 4000);
                        oO_Camera.LookDirection = new Vector3D(0, 0, -4000);
                        oO_Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;
            }

            //orthographische Kamerabreite zurücksetzen
            OrthoWidth = 3200;

            Trans.Yaw = 0.0;
            Trans.Pitch = 0.0;
            Trans.RotationPoint = new Point3D(0, 0, 0);

            //Kamera des Koordinatensystems reloaden
            oP_Camera_CoordSystem.Position = new Point3D(0, 0, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(0, 0, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);

            Trans_CSS.Yaw = 0.0;
            Trans_CSS.Pitch = 0.0;
        }

        

        //Zoom über das Drehen des Mausrades
        public void viewport_Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomCamera(e.Delta);
        }

        //HineinZoomen über ToolBox
        public void zoomIn()
        {
            zoomCamera(ZoomFactorToolBox);
        }

        //HerausZoomen über ToolBox
        public void zoomOut()
        {
            zoomCamera(-ZoomFactorToolBox);
        }

        //Zoom über die vertikale Mausbewegung
        public void zoomCam()
        {
            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Math.Ceiling(Viewport.ActualWidth / 2), Math.Floor(Viewport.ActualHeight / 2) - relativePos.Y);

            double value_y = actualRelativePos.Y * ZoomFactorMouse;

            zoomCamera(value_y);

            MouseUtilities.SetPosition(_oCenterOfViewport);
        }

        private void zoomCamera(double value)
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        //je kleiner die Division desto schneller wird gezoomt und umgekehrt
                        //Änderung der Kameraentfernung um das Delta des Mausrades
                        Trans.Zoom(oP_Camera, value / 1);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        //je kleiner die Division desto schneller wird gezoomt und umgekehrt
                        //Änderung der Kamerabreite um das Delta des Mausrades
                        //CameraR = CameraR - e.Delta / 250D;
                        OrthoWidth -= value / 2.5D;
                    }
                    break;
            }
            //trans_CSS.Zoom(oP_Camera_CoordSystem, value / 1);
            updatePositionCamera();
        }


        public void orbitCam()
        {
            //Liefert die Mausposition relativ zum Viewport3D und transformiert sie zum Center
            //actualRelativePos beinhaltet die X und Y Entfernung vom Center des Viewports
            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Math.Ceiling(Viewport.ActualWidth / 2), Math.Ceiling(Viewport.ActualHeight / 2) - relativePos.Y);

            //dx und dy sind die Beträge, um jene die Maus dieses Maus Move Events bewegt.
            //Beim Rücksetzen der Maus zum Center, ist dies einfach die neue Position der Maus , relativ zum Center
            double dx = actualRelativePos.X;
            double dy = actualRelativePos.Y;

            Trans.Yaw += dx;
            Trans.Pitch += dy;

            Trans_CSS.Yaw = Trans.Yaw;
            Trans_CSS.Pitch = Trans.Pitch;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Trans.Orbit(oP_Camera);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Trans.Orbit(oO_Camera);
                    }
                    break;
            }

            Trans_CSS.Orbit(oP_Camera_CoordSystem);

            //Rücksetzen der MausPosition zum Center vom Viewport in Bildschirm Koordinaten
            MouseUtilities.SetPosition(_oCenterOfViewport);
        }

        public void dragCam()
        {
            //Liefert die Mausposition relativ zum Viewport3D und transformiert sie zum Center
            //actualRelativePos beinhaltet die X und Y Entfernung vom Center des Viewports

            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Viewport.ActualWidth / 2, Viewport.ActualHeight / 2 - relativePos.Y);

            //dx und dy sind die Beträge, um jene die Maus dieses Maus Move Events bewegt.
            //Beim Rücksetzen der Maus zum Center, ist dies einfach die neue Position der Maus , relativ zum Center
            double dx = actualRelativePos.X;
            double dy = actualRelativePos.Y;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Trans.Drag(oP_Camera, -dx, -dy);
                        //oP_Camera_CoordSystem.Position = new Point3D(oP_Camera.Position.X, oP_Camera.Position.Y, oP_Camera_CoordSystem.Position.Z);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Trans.Drag(oO_Camera, -dx, -dy);
                        //oP_Camera_CoordSystem.Position = new Point3D(oO_Camera.Position.X, oO_Camera.Position.Y, oP_Camera_CoordSystem.Position.Z);
                    }
                    break;
            }
            
            MouseUtilities.SetPosition(_oCenterOfViewport);
        }

        public void panCam()
        {  
            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Viewport.ActualWidth / 2, Viewport.ActualHeight / 2 - relativePos.Y);
            double dx = actualRelativePos.X;
            double dy = actualRelativePos.Y;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Trans.Pan(oP_Camera, dx, dy);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Trans.Pan(oO_Camera, dx, dy);
                    }
                    break;
            }
            MouseUtilities.SetPosition(_oCenterOfViewport);
        }

        public void setMouseToCenter()
        {
            // Berechnen vom Center des Viewports in Bildschirmkoordinaten
            _oCenterOfViewport = Viewport.PointToScreen(new Point(Viewport.ActualWidth / 2, Viewport.ActualHeight / 2));

            // Rücksetzen der Maus auf diese Position (Mitte des Viewports)
            MouseUtilities.SetPosition(_oCenterOfViewport);
        }

        public void resetCam()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        oP_Camera.Position = new Point3D(oP_Camera.Position.X, oP_Camera.Position.Y, 4000);
                        oP_Camera.LookDirection = new Vector3D(-oP_Camera.Position.X, -oP_Camera.Position.Y, -4000);
                        oP_Camera.UpDirection = new Vector3D(0, 1, 0);
                        oP_Camera.Transform = new Transform3DGroup();
                        Trans.Yaw = 0;
                        Trans.Pitch = 0;
                    }
                    break;

                case Cam.Orthographic:
                    {
                        oO_Camera.Position = new Point3D(oO_Camera.Position.X, oO_Camera.Position.Y, 4000);
                        oO_Camera.LookDirection = new Vector3D(-oO_Camera.Position.X, -oO_Camera.Position.Y, -4000);
                        oO_Camera.UpDirection = new Vector3D(0, 1, 0);
                        OrthoWidth = 3000;
                        oO_Camera.Width = OrthoWidth;
                        oO_Camera.Transform = new Transform3DGroup();
                        Trans.Yaw = 0;
                        Trans.Pitch = 0;
                    }
                    break;
            }
            oP_Camera_CoordSystem.Position = new Point3D(oP_Camera_CoordSystem.Position.X, oP_Camera_CoordSystem.Position.Y, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(-oP_Camera_CoordSystem.Position.X, -oP_Camera_CoordSystem.Position.Y, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
            oP_Camera_CoordSystem.Transform = new Transform3DGroup();
            Trans_CSS.Yaw = 0;
            Trans_CSS.Pitch = 0;
        }

        public void setCam()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Point3D actualCameraPositionPers = new Point3D(oP_Camera.Position.X, oP_Camera.Position.Y, oP_Camera.Position.Z);
                        oP_Camera.Position = actualCameraPositionPers;
                        oP_Camera.LookDirection = PointClicked - actualCameraPositionPers;
                        oP_Camera.UpDirection = new Vector3D(0, 1, 0);

                        //minimaler Pitch um neue TransformationGroup zu erzeugen,  sonst springt die Kamera beim Orbit
                        Trans.doPitch(oP_Camera, 0.001);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Point3D actualCameraPositionOrtho = new Point3D(oO_Camera.Position.X, oO_Camera.Position.Y, oO_Camera.Position.Z);
                        oO_Camera.Position = actualCameraPositionOrtho;
                        oO_Camera.LookDirection = PointClicked - actualCameraPositionOrtho;
                        oO_Camera.UpDirection = new Vector3D(0, 1, 0);

                        //minimaler Pitch um neue TransformationGroup zu erzeugen,  sonst springt die Kamera beim Orbit
                        Trans.doPitch(oO_Camera, 0.001);
                    }
                    break;
            }
        }

        //Listener für die Tastatureingabe
        public void viewport_KeyDown(object sender, KeyEventArgs e)
        {
            double value = 30;
            double zoomFactor = 50;

            switch (e.Key)
            {
                case Key.Up:
                    {
                        if (MyCam == Cam.Perspective)
                            Trans.doPitch(oP_Camera, -value);

                        if (MyCam == Cam.Orthographic)
                            Trans.doPitch(oO_Camera, -value);
                    }
                    Trans_CSS.doPitch(oP_Camera_CoordSystem, -value);
                    e.Handled = true;
                    break;

                case Key.Down:
                    {
                        if (MyCam == Cam.Perspective)
                            Trans.doPitch(oP_Camera, value);

                        if (MyCam == Cam.Orthographic)
                            Trans.doPitch(oO_Camera, value);
                    }
                    Trans_CSS.doPitch(oP_Camera_CoordSystem, value);
                    e.Handled = true;
                    break;

                case Key.Left:
                    {
                        if (MyCam == Cam.Perspective)
                            Trans.doYaw(oP_Camera, value);

                        if (MyCam == Cam.Orthographic)
                            Trans.doYaw(oO_Camera, value);
                    }
                    Trans_CSS.doYaw(oP_Camera_CoordSystem, value);
                    e.Handled = true;
                    break;

                case Key.Right:
                    {
                        if (MyCam == Cam.Perspective)
                            Trans.doYaw(oP_Camera, -value);

                        if (MyCam == Cam.Orthographic)
                            Trans.doYaw(oO_Camera, -value);
                    }
                    Trans_CSS.doYaw(oP_Camera_CoordSystem, -value);
                    e.Handled = true;
                    break;

                case Key.Add:
                case Key.OemPlus:
                    {
                        if (MyCam == Cam.Perspective)
                            Trans.Zoom(oP_Camera, zoomFactor);

                        if (MyCam == Cam.Orthographic)
                        {
                            OrthoWidth -= zoomFactor;
                            updatePositionCamera();
                        }
                    }
                    //Trans_CSS.Zoom(oP_Camera_CoordSystem, zoomFactor);
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.OemMinus:
                    {
                        if (MyCam == Cam.Perspective)
                            Trans.Zoom(oP_Camera, -zoomFactor);

                        if (MyCam == Cam.Orthographic)
                        {
                            OrthoWidth += zoomFactor;
                            updatePositionCamera();
                        }
                    }
                    //Trans_CSS.Zoom(oP_Camera_CoordSystem, -zoomFactor);
                    e.Handled = true;
                    break;
            }
        }

        

        /// <summary>
        /// TOOLBAR Funktionen
        /// </summary>

        public void viewFront()
        {
            reloadCameraPositionDefault();
            resetCam();
        }

        public void viewBack()
        {
            reloadCameraPositionDefault();
            resetCam();
            //540 entspr. 180°
            if (MyCam == Cam.Perspective)
                Trans.doYaw(oP_Camera, 540);
            if (MyCam == Cam.Orthographic)
                Trans.doYaw(oO_Camera, 540);

            //Koordinatensystem aktualisieren
            Trans_CSS.doYaw(oP_Camera_CoordSystem, 540);
        }

        public void viewRight()
        {
            reloadCameraPositionDefault();
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                Trans.doYaw(oP_Camera, -270);
            if (MyCam == Cam.Orthographic)
                Trans.doYaw(oO_Camera, -270);

            //Koordinatensystem aktualisieren
            Trans_CSS.doYaw(oP_Camera_CoordSystem, -270);
        }

        public void viewLeft()
        {
            reloadCameraPositionDefault();
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                Trans.doYaw(oP_Camera, 270);
            if (MyCam == Cam.Orthographic)
                Trans.doYaw(oO_Camera, 270);

            //Koordinatensystem aktualisieren
            Trans_CSS.doYaw(oP_Camera_CoordSystem, 270);
        }

        public void viewTop()
        {
            reloadCameraPositionDefault();
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                Trans.doPitch(oP_Camera, -270);
            if (MyCam == Cam.Orthographic)
                Trans.doPitch(oO_Camera, -270);

            //Koordinatensystem aktualisieren
            Trans_CSS.doPitch(oP_Camera_CoordSystem, -270);
        }

        public void viewBottom()
        {
            reloadCameraPositionDefault();
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                Trans.doPitch(oP_Camera, 270);
            if (MyCam == Cam.Orthographic)
                Trans.doPitch(oO_Camera, 270);

            //Koordinatensystem aktualisieren
            Trans_CSS.doPitch(oP_Camera_CoordSystem, 270);
        }

        

        //Öffentliche Getter & Setter Methoden
        public Cam MyCam
        {
            get { return _cCam; }
            set
            {
                _cCam = value;
                reloadCameraPositionDefault();
                resetCam();
            }
        }

        
    }
}