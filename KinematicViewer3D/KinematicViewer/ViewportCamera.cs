using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public PerspectiveCamera oP_Camera_CoordSystem;

        //orthografische Camera
        public OrthographicCamera oO_Camera;

        //Kamerabreite für orthographische Kamera
        private double _dOrthoWidth;
        private double _dZoomFactor;

        //Mitte des ViewPort
        private Point _oCenterOfViewport;

        //Speicherung der yaw und pitch Werte
        private double _dYaw;
        private double _dPit;

        //benutzbare Instanzen
        private Viewport3D viewport;
        private Viewport3D viewportCoordSystem;
        private Grid mainGrid;
        private CoordSystem coordSystem;
        private Transformation trans;



        private Transformation trans_CSS;

        private double cssYaw;
        private double cssPitch;

        //Konstruktor
        public ViewportCamera(Grid mainGrid, Viewport3D viewport, Viewport3D viewportCoordSystem, CoordSystem coordSystem, Transformation trans)
        {
            this.mainGrid = mainGrid;
            this.viewport = viewport;
            this.coordSystem = coordSystem;
            this.viewportCoordSystem = viewportCoordSystem;
            this.trans = trans;
            

            trans_CSS = new Transformation();
            oP_Camera_CoordSystem = coordSystem.Camera_CoordSystem;
            
            
            //trans_CSS.Reset(c_SystemSmall.Camera_CoordSystem);
           

            OrthoWidth = 3200;
            ZoomFactor = 100;
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
            trans.Reset(oP_Camera);
            viewport.Camera = oP_Camera;
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
            trans.Reset(oO_Camera);
            viewport.Camera = oO_Camera;
            
        }

        public void makeCoordSystemCamera()
        {
            oP_Camera_CoordSystem = new PerspectiveCamera();
            oP_Camera_CoordSystem.Position = new Point3D(0, 100, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(0, -100, -2000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
            oP_Camera_CoordSystem.FieldOfView = 60;
            oP_Camera_CoordSystem.FarPlaneDistance = 15000;
            oP_Camera_CoordSystem.NearPlaneDistance = 0.125;
            trans_CSS.Reset(oP_Camera_CoordSystem);
            viewportCoordSystem.Camera = oP_Camera_CoordSystem;

            // updatePositionCamera_CoordinateSystem(coord_CameraR, coord_CameraPhi, coord_CameraTheta);
        }

        //Position der Camera updaten
        public void updatePositionCamera()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        Yaw = trans.Yaw;
                        Pit = trans.Pitch;

                        //Werte für CSS updaten
                        cssYaw = trans_CSS.Yaw;
                        cssPitch = trans_CSS.Pitch;
                    }
                    break;

                case Cam.Orthographic:
                    {
                        Yaw = trans.Yaw;
                        Pit = trans.Pitch;
                        oO_Camera.Width = OrthoWidth;

                        //Werte für CSS updaten
                        cssYaw = trans_CSS.Yaw;
                        cssPitch = trans_CSS.Pitch;
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
            OrthoWidth = 3000;

            //Kamera des Koordinatensystems reloaden
            oP_Camera_CoordSystem.Position = new Point3D(0, 0, 4000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(0, 0, -4000);
            oP_Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);

        }

        //Listener für das Mausrad --> ZOOM Funtkion
        public void viewport_Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomCam(e.Delta);
            
            //c_SystemSmall.updateC_SystemSmall(trans);

            //Kamera im Fenster des Koordinatensystems ändern
            ////c_SystemSmall.updatePositionCamera_CoordinateSystem(cameraR, cameraPhi, cameraTheta);
        }

        public void rotateCam()
        {
            //Liefert die Mausposition relativ zum Viewport3D und transformiert sie zum Center
            //actualRelativePos beinhaltet die X und Y Entfernung vom Center des Viewports
            Point relativePos = Mouse.GetPosition(viewport);
            Point actualRelativePos = new Point(relativePos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - relativePos.Y);

            //dx und dy sind die Beträge, um jene die Maus dieses Maus Move Events bewegt.
            //Beim Rücksetzen der Maus zum Center, ist dies einfach die neue Position der Maus , relativ zum Center 
            double dx = actualRelativePos.X;
            double dy = actualRelativePos.Y;

            trans.Yaw = trans.Yaw + dx;
            trans.Pitch = trans.Pitch + dy;

            trans_CSS.Yaw = trans_CSS.Yaw + dx;
            trans_CSS.Pitch = trans_CSS.Pitch + dy;

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        trans.Rotate(oP_Camera);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        trans.Rotate(oO_Camera);
                    }
                    break;       
            }
            //c_SystemSmall.updateC_SystemSmall(trans_CSS);
            trans_CSS.Rotate(oP_Camera_CoordSystem);
            //coordSystem.updateC_SystemSmall();

            
            //Rücksetzen der MausPosition zum Center vom Viewport in Bildschirm Koordinaten
            MouseUtilities.SetPosition(_oCenterOfViewport);
        }



        public void setMouseToCenter()
        {
            // Berechnen vom Center des Viewports in Bildschirmkoordinaten
            _oCenterOfViewport = viewport.PointToScreen(new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2));

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
                        oP_Camera.Transform = new Transform3DGroup();
                        trans.Yaw = 0;
                        trans.Pitch = 0;
                    }
                    break;

                case Cam.Orthographic:
                    {
                        oO_Camera.Position = new Point3D(oO_Camera.Position.X, oO_Camera.Position.Y, 4000);
                        oO_Camera.LookDirection = new Vector3D(-oO_Camera.Position.X, -oO_Camera.Position.Y, -4000);
                        OrthoWidth = 3000;
                        oO_Camera.Width = OrthoWidth;
                        oO_Camera.Transform = new Transform3DGroup();
                        trans.Yaw = 0;
                        trans.Pitch = 0;  
                    }
                    break;
            }
            oP_Camera_CoordSystem.Position = new Point3D(oP_Camera_CoordSystem.Position.X, oP_Camera_CoordSystem.Position.Y, 2000);
            oP_Camera_CoordSystem.LookDirection = new Vector3D(-oP_Camera_CoordSystem.Position.X, -oP_Camera_CoordSystem.Position.Y, -2000);
            oP_Camera_CoordSystem.Transform = new Transform3DGroup();
            trans_CSS.Yaw = 0;
            trans_CSS.Pitch = 0;
           


        }
        //Listener für die Tastatureingabe 
        public void viewport_KeyDown( object sender, KeyEventArgs e)
        {
            double value = 30;
            double zoomFactor = 50;

            switch (e.Key)
            {
                case Key.Up:
                    {
                        if(MyCam == Cam.Perspective)
                            trans.doPitch(oP_Camera, -value);

                        if(MyCam == Cam.Orthographic)
                            trans.doPitch(oO_Camera, -value);
                    }
                    trans_CSS.doPitch(oP_Camera_CoordSystem, -value);
                    e.Handled = true;
                    break;

                case Key.Down:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.doPitch(oP_Camera, value);

                        if (MyCam == Cam.Orthographic)
                            trans.doPitch(oO_Camera, value);
                    }
                    trans_CSS.doPitch(oP_Camera_CoordSystem, value);
                    e.Handled = true;
                    break;

                case Key.Left:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.doYaw(oP_Camera, value);

                        if (MyCam == Cam.Orthographic)
                            trans.doYaw(oO_Camera, value);
                    }
                    trans.doYaw(oP_Camera_CoordSystem, value);
                    e.Handled = true;
                    break;

                case Key.Right:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.doYaw(oP_Camera, -value);

                        if (MyCam == Cam.Orthographic)
                            trans.doYaw(oO_Camera, -value);
                    }
                    trans_CSS.doYaw(oP_Camera_CoordSystem, -value);
                    e.Handled = true;
                    break;

                case Key.Add:
                case Key.OemPlus:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.Zoom(oP_Camera, zoomFactor);

                        if (MyCam == Cam.Orthographic)
                        {
                            OrthoWidth -= zoomFactor;
                            updatePositionCamera();
                        }    
                    }
                    trans_CSS.Zoom(oP_Camera_CoordSystem, zoomFactor);
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.OemMinus:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.Zoom(oP_Camera, -zoomFactor);

                        if (MyCam == Cam.Orthographic)
                        {
                            OrthoWidth += zoomFactor;
                            updatePositionCamera();
                        }   
                    }
                    trans_CSS.Zoom(oP_Camera_CoordSystem, -zoomFactor);
                    e.Handled = true;
                    break;
            }
        }

        private void zoomCam(double value)
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        //je kleiner die Division desto schneller wird gezoomt und umgekehrt
                        //Änderung der Kameraentfernung um das Delta des Mausrades
                        trans.Zoom(oP_Camera, value / 1);
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
            trans_CSS.Zoom(oP_Camera_CoordSystem, value / 1);
            updatePositionCamera();
        }

        /// <summary>
        /// TOOLBAR Funktionen
        /// </summary>

        public void viewFront()
        {
            resetCam();
        }

        public void viewBack()
        {   
            resetCam();
            //540 entspr. 180°
            if (MyCam == Cam.Perspective)
                trans.doYaw(oP_Camera, 540);
            if (MyCam == Cam.Orthographic)
                trans.doYaw(oO_Camera, 540);

            //Koordinatensystem aktualisieren
            trans_CSS.doYaw(oP_Camera_CoordSystem, 540);
        }

        public void viewRight()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doYaw(oP_Camera, -270); 
            if (MyCam == Cam.Orthographic)
                trans.doYaw(oO_Camera, -270);

            //Koordinatensystem aktualisieren
            trans_CSS.doYaw(oP_Camera_CoordSystem, -270);
        }

        public void viewLeft()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doYaw(oP_Camera, 270);
            if (MyCam == Cam.Orthographic)
                trans.doYaw(oO_Camera, 270);

            //Koordinatensystem aktualisieren
            trans_CSS.doYaw(oP_Camera_CoordSystem, 270);
        }

        public void viewTop()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doPitch(oP_Camera, -270);
            if (MyCam == Cam.Orthographic)
                trans.doPitch(oO_Camera, -270);

            //Koordinatensystem aktualisieren
            trans_CSS.doPitch(oP_Camera_CoordSystem, -270);
        }

        public void viewBottom()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doPitch(oP_Camera, 270);
            if (MyCam == Cam.Orthographic)
                trans.doPitch(oO_Camera, 270);

            //Koordinatensystem aktualisieren
            trans_CSS.doPitch(oP_Camera_CoordSystem, 270);
        }

        public void zoomIn()
        {
            zoomCam(ZoomFactor);
        }

        public void zoomOut()
        {
            zoomCam(-ZoomFactor);
        }


        //Öffentliche Getter & Setter Methoden
        public Cam MyCam
        {
            get { return _cCam; }
            set
            {
                _cCam = value;
                //updatePositionCamera();
                //resetCam();
            }
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

        public double ZoomFactor
        {
            get { return _dZoomFactor; }
            set { _dZoomFactor = value; }
        }
    }
}
