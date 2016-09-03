using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace KinematicViewer.Camera
{
    public class CameraTransformation
    {
        // Gieren bzw Schlingern rechts links um y- Achse (Vertikalachse)
        private double _dYaw;

        // Neigen um die Querachse x- Achse
        private double _dPitch; 

        //Rotationsmittelpunkt
        private Point3D _oRotationPoint;

        /// <summary>
        /// Erzeugt ein Objekt zum Verwalten aller Transformationen im UserControl
        /// </summary>
        public CameraTransformation()
        {
            RotationPoint = new Point3D(0, 0, 0);
            Yaw = 0.0;
            Pitch = 0.0;
        }

        public double Yaw
        {
            get { return _dYaw; }
            set { _dYaw = value; }
        }

        public double Pitch
        {
            get { return _dPitch; }
            set { _dPitch = value; }
        }

        public Point3D RotationPoint
        {
            get { return _oRotationPoint; }
            set { _oRotationPoint = value; }
        }

        public void doPitch(ProjectionCamera camera, double amount)
        {
            Pitch += amount;
            Orbit(camera);
        }

        public void doYaw(ProjectionCamera camera, double amount)
        {
            Yaw += amount;
            Orbit(camera);
        }

        public void Zoom(ProjectionCamera camera, double amount)
        {
            // Änderung der Kameraposition über dessen Position auf der Z- Achse
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - amount);
        }

        //Initialisiert eine orbitale Bewegung einer Kamera um einen Mittelpunkt
        public void Orbit(ProjectionCamera camera)
        {
            double theta = Yaw / 3;
            double phi = Pitch / 3;

            //Bereich in dem rotiert wird
            if (phi < -90) phi = -90;
            if (phi > 90) phi = 90;

            camera.UpDirection = new Vector3D(0, 1, 0);

            Vector3D thetaAxis = new Vector3D(0, 1, 0);
            Vector3D phiAxis = new Vector3D(-1, 0, 0);

            Transform3DGroup transformGroup = camera.Transform as Transform3DGroup;
            transformGroup.Children.Clear();
            QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(-phiAxis, phi));

            //Ohne optionalem Parameter wird um den Ursprung (0,0,0) rotiert
            //Mit optionalem Parameter rotationPoint, kann man um den Mittelpunkt eines Objektes rotieren (new RotateTransform3D(r, rotationPoint));
            transformGroup.Children.Add(new RotateTransform3D(r, RotationPoint));
            r = new QuaternionRotation3D(new Quaternion(-thetaAxis, theta));

            //Ohne optionalem Parameter wird um den Ursprung (0,0,0) rotiert
            //Mit optionalem Parameter rotationPoint, kann man um den Mittelpunkt eines Objektes rotieren (new RotateTransform3D(r, rotationPoint));
            transformGroup.Children.Add(new RotateTransform3D(r, RotationPoint));
        }

        //Initialisiert ein Verschieben der Kamera auf XY-Ebene
        public void Drag(ProjectionCamera camera, double dx, double dy)
        {
            camera.Position = new Point3D(camera.Position.X + dx, camera.Position.Y + dy, camera.Position.Z);
            camera.UpDirection = new Vector3D(0, 1, 0);
        }

        //Initialisiert eine Schwenkbewegung der Kamera (Umschauen mit der Kamera in einer Szene)
        public void Pan(ProjectionCamera camera, double dx, double dy)
        {
            camera.LookDirection = new Vector3D((camera.LookDirection.X + dx), (camera.LookDirection.Y + dy), camera.LookDirection.Z);
            camera.UpDirection = new Vector3D(0, 1, 0);
        }

        //Zurücksetzen der Kamera
        public void Reset(ProjectionCamera camera, double value_Z)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, value_Z);
            camera.Transform = new Transform3DGroup();
            Yaw = 0;
            Pitch = 0;
        }
    }
}