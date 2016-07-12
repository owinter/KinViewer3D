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

        //Zoom Faktoren
        private double _dZoomFactor;
        private double _dZoomFactorMouse;

        //Mitte des ViewPort in Bildschirmkoordinaten
        private Point _oCenterOfViewport;

        //angeklickter Rotationspunkt
        private Point3D _oPointClicked;

        //Speicherung der yaw und pitch Werte
        private double _dYaw;
        private double _dPitch;

        //benutzbare eigene Instanzen
        private Viewport3D _oViewport;
        private Viewport3D _oViewportCoordSystem;
        private CoordSystem _oCoordSystem;
        private CameraTransformation _oTrans;
        private CameraTransformation _oTrans_CSS;

        /// <summary>
        /// Verwaltung aller Viewport Kameras im UserControl
        /// </summary>
        /// <param name="viewport">Objekt des Main Viewports</param>
        /// <param name="viewportCoordSystem">Objekt des Viewports für das ein-/ ausschaltbare Koordinatensystem</param>
        /// <param name="coordSystem">Objekt des ein-/ ausschaltbaren Koordinatensystems</param>
        /// <param name="trans">Objekt für jegliche Transformationen im Main Viewport</param>
        public ViewportCamera(Viewport3D viewport, Viewport3D viewportCoordSystem, CoordSystem coordSystem, CameraTransformation trans)
        {
            Viewport = viewport;
            CoordSystem = coordSystem;
            ViewportCoordSystem = viewportCoordSystem;
            TransformationViewport = trans;
            TransformationViewport_CoordSystem = new CameraTransformation();

            OrthoWidth = 3200;
            ZoomFactor = 300;
            ZoomFactorMouse = 5;
            PointClicked = new Point3D(0, 0, 0);
            TransformationViewport.RotationPoint = PointClicked;
        }

        public double Yaw
        {
            get { return _dYaw; }
            private set { _dYaw = value; }
        }

        public double Pitch
        {
            get { return _dPitch; }
            private set { _dPitch = value; }
        }

        public double OrthoWidth
        {
            get { return _dOrthoWidth; }
            set { _dOrthoWidth = value; }
        }

        public double ZoomFactor
        {
            get { return _dZoomFactor; }
            set { _dZoomFactor = value; }
        }

        public double ZoomFactorMouse
        {
            get { return _dZoomFactorMouse; }
            set { _dZoomFactorMouse = value; }
        }

        public Point CenterOfViewport
        {
            get { return _oCenterOfViewport; }
            set { _oCenterOfViewport = value; }
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

        public CoordSystem CoordSystem
        {
            get { return _oCoordSystem; }
            private set { _oCoordSystem = value; }
        }

        public CameraTransformation TransformationViewport
        {
            get { return _oTrans; }
            private set { _oTrans = value; }
        }

        public CameraTransformation TransformationViewport_CoordSystem
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
            TransformationViewport.Reset(oP_Camera, 4000);
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
            TransformationViewport.Reset(oO_Camera, 4000);
            Viewport.Camera = oO_Camera;
        }

        public void startCoordSystemCamera()
        {
            oP_Camera_CoordSystem = new PerspectiveCamera();
            oP_Camera_CoordSystem.Position = new Point3D(0, 0, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(0, 0, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
            oP_Camera_CoordSystem.FieldOfView = 60;
            oP_Camera_CoordSystem.FarPlaneDistance = 3000;
            oP_Camera_CoordSystem.NearPlaneDistance = 0.125;
            TransformationViewport_CoordSystem.Reset(oP_Camera_CoordSystem, 2000);
            ViewportCoordSystem.Camera = oP_Camera_CoordSystem;
        }

        //Position der Camera updaten
        public void updatePositionCamera()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Yaw = TransformationViewport.Yaw;
                        Pitch = TransformationViewport.Pitch;
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Yaw = TransformationViewport.Yaw;
                        Pitch = TransformationViewport.Pitch;
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

            TransformationViewport.Yaw = 0.0;
            TransformationViewport.Pitch = 0.0;
            TransformationViewport.RotationPoint = new Point3D(0, 0, 0);

            //Kamera des Koordinatensystems reloaden
            oP_Camera_CoordSystem.Position = new Point3D(0, 0, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(0, 0, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);

            TransformationViewport_CoordSystem.Yaw = 0.0;
            TransformationViewport_CoordSystem.Pitch = 0.0;
        }

        //Zoom über das Drehen des Mausrades
        public void viewport_Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomCamera(e.Delta);
        }

        //HineinZoomen über ToolBox
        public void ToolBoxZoomIn()
        {
            zoomCamera(ZoomFactor);
        }

        //HerausZoomen über ToolBox
        public void ToolBoxZoomOut()
        {
            zoomCamera(-ZoomFactor);
        }

        //Zoom über die vertikale Mausbewegung
        public void zoomCamMouseMove()
        {
            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Math.Ceiling(Viewport.ActualWidth / 2), Math.Ceiling(Viewport.ActualHeight / 2) - relativePos.Y);

            double dy = actualRelativePos.Y * ZoomFactorMouse;
            zoomCamera(dy);

            //Rücksetzen der MausPosition zum Center des Viewports
            MouseUtilities.SetPosition(CenterOfViewport);
        }

        private void zoomCamera(double value)
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        TransformationViewport.Zoom(oP_Camera, value);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        //je kleiner die Division desto schneller wird gezoomt und umgekehrt
                        //Änderung der Kamerabreite um das Delta des Mausrades
                        OrthoWidth -= value / 2.5D;
                        oO_Camera.Width = OrthoWidth;
                    }
                    break;
            }
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

            TransformationViewport.Yaw += dx;
            TransformationViewport.Pitch += dy;

            TransformationViewport_CoordSystem.Yaw = TransformationViewport.Yaw;
            TransformationViewport_CoordSystem.Pitch = TransformationViewport.Pitch;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        TransformationViewport.Orbit(oP_Camera);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        TransformationViewport.Orbit(oO_Camera);
                    }
                    break;
            }

            TransformationViewport_CoordSystem.Orbit(oP_Camera_CoordSystem);

            //Rücksetzen der MausPosition zum Center des Viewports
            MouseUtilities.SetPosition(CenterOfViewport);
        }

        public void dragCam()
        {
            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Math.Ceiling(Viewport.ActualWidth / 2), Math.Ceiling(Viewport.ActualHeight / 2) - relativePos.Y);
            double dx = actualRelativePos.X;
            double dy = actualRelativePos.Y;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        TransformationViewport.Drag(oP_Camera, -dx, -dy);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        TransformationViewport.Drag(oO_Camera, -dx, -dy);
                    }
                    break;
            }

            MouseUtilities.SetPosition(CenterOfViewport);
        }

        public void panCam()
        {
            Point relativePos = Mouse.GetPosition(Viewport);
            Point actualRelativePos = new Point(relativePos.X - Math.Ceiling(Viewport.ActualWidth / 2), Math.Ceiling(Viewport.ActualHeight / 2) - relativePos.Y);
            double dx = actualRelativePos.X;
            double dy = actualRelativePos.Y;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        TransformationViewport.Pan(oP_Camera, dx, dy);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        TransformationViewport.Pan(oO_Camera, dx, dy);
                    }
                    break;
            }
            MouseUtilities.SetPosition(CenterOfViewport);
        }

        public void setMouseToCenter()
        {
            // Berechnen vom Center des Viewports in Bildschirmkoordinaten
            CenterOfViewport = Viewport.PointToScreen(new Point(Math.Ceiling(Viewport.ActualWidth / 2), Math.Ceiling(Viewport.ActualHeight / 2)));

            // Rücksetzen der Maus auf diese Position (Mitte des Viewports)
            MouseUtilities.SetPosition(CenterOfViewport);
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
                        TransformationViewport.Yaw = 0;
                        TransformationViewport.Pitch = 0;
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
                        TransformationViewport.Yaw = 0;
                        TransformationViewport.Pitch = 0;
                    }
                    break;
            }
            oP_Camera_CoordSystem.Position = new Point3D(oP_Camera_CoordSystem.Position.X, oP_Camera_CoordSystem.Position.Y, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(-oP_Camera_CoordSystem.Position.X, -oP_Camera_CoordSystem.Position.Y, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
            oP_Camera_CoordSystem.Transform = new Transform3DGroup();
            TransformationViewport_CoordSystem.Yaw = 0;
            TransformationViewport_CoordSystem.Pitch = 0;
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
                        TransformationViewport.doPitch(oP_Camera, 0.001);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Point3D actualCameraPositionOrtho = new Point3D(oO_Camera.Position.X, oO_Camera.Position.Y, oO_Camera.Position.Z);
                        oO_Camera.Position = actualCameraPositionOrtho;
                        oO_Camera.LookDirection = PointClicked - actualCameraPositionOrtho;
                        oO_Camera.UpDirection = new Vector3D(0, 1, 0);

                        //minimaler Pitch um neue TransformationGroup zu erzeugen,  sonst springt die Kamera beim Orbit
                        TransformationViewport.doPitch(oO_Camera, 0.001);
                    }
                    break;
            }
        }

        //Listener für die Tastatureingabe
        public void viewport_KeyDown(object sender, KeyEventArgs e)
        {
            double movingAmoung = 30;

            switch (e.Key)
            {
                case Key.Up:
                    {
                        pitchCam(-movingAmoung);
                    }
                    e.Handled = true;
                    break;

                case Key.Down:
                    {
                        pitchCam(movingAmoung);
                    }
                    e.Handled = true;
                    break;

                case Key.Left:
                    {
                        yawCam(movingAmoung);
                    }
                    e.Handled = true;
                    break;

                case Key.Right:
                    {
                        yawCam(-movingAmoung);
                    }
                    e.Handled = true;
                    break;

                case Key.Add:
                case Key.OemPlus:
                    {
                        zoomCamera(ZoomFactor);
                    }
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.OemMinus:
                    {
                        zoomCamera(-ZoomFactor);
                    }
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
            //540 entspr. 180°
            InitToolBoxYawCamera(540);
        }

        public void viewRight()
        {
            //270 entspr. 90°
            InitToolBoxYawCamera(-270);
        }

        public void viewLeft()
        {
            //270 entspr. 90°
            InitToolBoxYawCamera(270);
        }

        public void viewTop()
        {
            //270 entspr. 90°
            InitToolBoxPitchCam(-270);
        }

        public void viewBottom()
        {
            //270 entspr. 90°
            InitToolBoxPitchCam(270);
        }

        private void InitToolBoxYawCamera(double value)
        {
            reloadCameraPositionDefault();
            resetCam();
            yawCam(value);
        }

        private void InitToolBoxPitchCam(double value)
        {
            reloadCameraPositionDefault();
            resetCam();
            pitchCam(value);
        }

        private void pitchCam(double value)
        {
            if (MyCam == Cam.Perspective)
                TransformationViewport.doPitch(oP_Camera, value);
            if (MyCam == Cam.Orthographic)
                TransformationViewport.doPitch(oO_Camera, value);

            //Koordinatensystem aktualisieren
            TransformationViewport_CoordSystem.doPitch(oP_Camera_CoordSystem, value);
        }

        private void yawCam(double value)
        {
            if (MyCam == Cam.Perspective)
                TransformationViewport.doYaw(oP_Camera, value);
            if (MyCam == Cam.Orthographic)
                TransformationViewport.doYaw(oO_Camera, value);

            //Koordinatensystem aktualisieren
            TransformationViewport_CoordSystem.doYaw(oP_Camera_CoordSystem, value);
        }

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