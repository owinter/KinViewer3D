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
        public PerspectiveCamera p_Camera;

        //orthografische Camera
        public OrthographicCamera o_Camera;

        //Kamerabreite für orthographische Kamera
        private double o_Width = 3000;

        private double zoomFactor = 100;

        //Mitte des ViewPort
        private Point centerOfViewport;

        //Speicherung der yaw und pitch Werte
        private double ya;
        private double pit;

        //benutzbare Instanzen
        private Viewport3D viewport;
        private Grid mainGrid;
        private CoordSystemSmall c_SystemSmall;
        private Transformation trans;

        //Konstruktor
        public ViewportCamera(Grid mainGrid, Viewport3D viewport, CoordSystemSmall c_SystemSmall, Transformation trans)
        {
            this.mainGrid = mainGrid;
            this.viewport = viewport;
            this.c_SystemSmall = c_SystemSmall;
            this.trans = trans;
        }

        //Erstelle perspektivische Kamerasicht
        public void startPerspectiveCamera()
        {
            p_Camera = new PerspectiveCamera();
            p_Camera.Position = new Point3D(0, 100, 3500);
            p_Camera.LookDirection = new Vector3D(0, -100, -3500);
            p_Camera.UpDirection = new Vector3D(0, 1, 0);
            p_Camera.FieldOfView = 45;
            p_Camera.FarPlaneDistance = 25000;
            p_Camera.NearPlaneDistance = 0.125;
            trans.Reset(p_Camera);
            viewport.Camera = p_Camera;
        }

        //Erstelle orthographische Kamerasicht
        public void startOrthographicCamera()
        {
            o_Camera = new OrthographicCamera();
            o_Camera.Position = new Point3D(0, 100, 3500);
            o_Camera.LookDirection = new Vector3D(0, -100, -3500);
            o_Camera.UpDirection = new Vector3D(0, 1, 0);
            o_Camera.FarPlaneDistance = 25000;
            o_Camera.NearPlaneDistance = 0.125;
            o_Camera.Width = o_Width;
            trans.Reset(o_Camera);
            viewport.Camera = o_Camera;
            
        }

        //Position der Camera updaten
        public void updatePositionCamera()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        /* double y = cameraR * Math.Sin(cameraPhi);
                         double hyp = cameraR * Math.Cos(cameraPhi);
                         double x = hyp * Math.Cos(cameraTheta);
                         double z = hyp * Math.Sin(cameraTheta);
                         //Neue Koordinate für die perspektivische Kamera
                         p_Camera.Position = new Point3D(x, y, z);
                         //Lookdirection zum Ursprung
                         p_Camera.LookDirection = new Vector3D(-x, -y, -z);
                         //Der Kamera sagen, wo oben ist --> Y
                         p_Camera.UpDirection = new Vector3D(0, 1, 0);*/
                        ya = trans.getYaw();
                        pit = trans.getPitch();


                    }
                    break;

                case Cam.Orthographic:
                    {
                        /*double y = cameraR * Math.Sin(cameraPhi);
                        double hyp = cameraR * Math.Cos(cameraPhi);
                        double x = hyp * Math.Cos(cameraTheta);
                        double z = hyp * Math.Sin(cameraTheta);
                        //Neue Koordinate für die orthographische Kamera
                        o_Camera.Position = new Point3D(x, y, z);
                        //Lookdirection zum Ursprung
                        o_Camera.LookDirection = new Vector3D(-x, -y, -z);
                        //Der Kamera sagen, wo oben ist --> Y
                        o_Camera.UpDirection = new Vector3D(0, 1, 0);
                        //updaten der Kamerabreite
                        o_Camera.Width = o_Cam_Width;*/
                        ya = trans.getYaw();
                        pit = trans.getPitch();
                        o_Camera.Width = o_Width;
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
                        p_Camera.Position = new Point3D(0, 100, 3500);
                        p_Camera.LookDirection = new Vector3D(0, -100, -3500);
                        p_Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        // Kamera Postion für Main Viewer reloaden
                        o_Camera.Position = new Point3D(0, 100, 3500);
                        o_Camera.LookDirection = new Vector3D(0, -100, -3500);
                        o_Camera.UpDirection = new Vector3D(0, 1, 0);
                    }
                    break;
            }

            //Winkel der Kamera zurücksetzen auf default Werte
            /* cameraPhi = Math.PI / 6.0 - 1 * cameraDPhi;
             cameraTheta = Math.PI / 6.0 + 5 * cameraDTheta;
             cameraR = 13;*/

            //orthographische Kamerabreite zurücksetzen
            o_Width = 3000;

            //Kamera des Koordinatensystems reloaden
            c_SystemSmall.reloadCoordinateSystem();
        }

        //Listener für das Mausrad --> ZOOM Funtkion
        public void viewport_Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoomCam(e.Delta);
            
            //c_SystemSmall.updateC_System(trans);

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

            trans.setYaw(trans.getYaw() + dx);
            trans.setPitch(trans.getPitch() + dy);

            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        trans.Rotate(p_Camera);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        trans.Rotate(o_Camera);
                    }
                    break;
                    
            }
            
            //c_SystemSmall.updateC_System(trans);
            //Rücksetzen der MausPosition zum Center vom Viewport in Bildschirm Koordinaten
            MouseUtilities.SetPosition(centerOfViewport);
        }



        public void setMouseToCenter()
        {
            // Berechnen vom Center des Viewports in Bildschirmkoordinaten
            centerOfViewport = viewport.PointToScreen(new Point(viewport.ActualWidth / 2, viewport.ActualHeight / 2));

            // Rücksetzen der Maus auf diese Position (Mitte des Viewports)
            MouseUtilities.SetPosition(centerOfViewport);
        }

        public void resetCam()
        {
            switch (MyCam)
            {
                case Cam.Perspective:
                    {
                        p_Camera.Position = new Point3D(p_Camera.Position.X, p_Camera.Position.Y, 3500);
                        p_Camera.LookDirection = new Vector3D(-p_Camera.Position.X, -p_Camera.Position.Y, -3500);
                        p_Camera.Transform = new Transform3DGroup();
                        trans.setYaw(0);
                        trans.setPitch(0);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        o_Camera.Position = new Point3D(o_Camera.Position.X, o_Camera.Position.Y, 3500);
                        o_Camera.LookDirection = new Vector3D(-o_Camera.Position.X, -o_Camera.Position.Y, -3500);
                        o_Width = 3000;
                        o_Camera.Width = o_Width;
                        o_Camera.Transform = new Transform3DGroup();
                        trans.setYaw(0);
                        trans.setPitch(0);  
                    }
                    break;
            }
            
            
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
                            trans.doPitch(p_Camera, -value);

                        if(MyCam == Cam.Orthographic)
                            trans.doPitch(o_Camera, -value);
                    }
                    e.Handled = true;
                    break;

                case Key.Down:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.doPitch(p_Camera, value);

                        if (MyCam == Cam.Orthographic)
                            trans.doPitch(o_Camera, value);
                    }
                    e.Handled = true;
                    break;

                case Key.Left:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.doYaw(p_Camera, value);

                        if (MyCam == Cam.Orthographic)
                            trans.doYaw(o_Camera, value);
                    }
                    e.Handled = true;
                    break;

                case Key.Right:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.doYaw(p_Camera, -value);

                        if (MyCam == Cam.Orthographic)
                            trans.doYaw(o_Camera, -value);
                    }
                    e.Handled = true;
                    break;

                case Key.Add:
                case Key.OemPlus:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.Zoom(p_Camera, zoomFactor);

                        if (MyCam == Cam.Orthographic)
                        {
                            o_Width -= zoomFactor;
                            updatePositionCamera();
                        }    
                    }
                    e.Handled = true;
                    break;

                case Key.Subtract:
                case Key.OemMinus:
                    {
                        if (MyCam == Cam.Perspective)
                            trans.Zoom(p_Camera, -zoomFactor);

                        if (MyCam == Cam.Orthographic)
                        {
                            o_Width += zoomFactor;
                            updatePositionCamera();
                        }   
                    }
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
                        trans.Zoom(p_Camera, value / 1);
                    }
                    break;

                case Cam.Orthographic:
                    {
                        //je kleiner die Division desto schneller wird gezoomt und umgekehrt
                        //Änderung der Kamerabreite um das Delta des Mausrades
                        //CameraR = CameraR - e.Delta / 250D;
                        o_Width -= value / 2.5D;
                    }
                    break;

            }
            updatePositionCamera();
        }

        /// <summary>
        /// TOOLBOX Funktionen
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
                trans.doYaw(p_Camera, 540);
            if (MyCam == Cam.Orthographic)
                trans.doYaw(o_Camera, 540);
        }

        public void viewRight()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doYaw(p_Camera, -270); 
            if (MyCam == Cam.Orthographic)
                trans.doYaw(o_Camera, -270);
        }

        public void viewLeft()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doYaw(p_Camera, 270);
            if (MyCam == Cam.Orthographic)
                trans.doYaw(o_Camera, 270);
        }

        public void viewTop()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doPitch(p_Camera, -270);
            if (MyCam == Cam.Orthographic)
                trans.doPitch(o_Camera, -270);
        }

        public void viewBottom()
        {   
            resetCam();
            //270 entspr. 90°
            if (MyCam == Cam.Perspective)
                trans.doPitch(p_Camera, 270);
            if (MyCam == Cam.Orthographic)
                trans.doPitch(o_Camera, 270);
        }

        public void zoomIn()
        {
            zoomCam(zoomFactor);
        }

        public void zoomOut()
        {
            zoomCam(-zoomFactor);
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
    }
}
