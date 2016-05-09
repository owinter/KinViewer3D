using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Transformation
    {
        private double yaw; // Gieren bzw Schlingern rechts links um y- Achse (Vertikalachse)
        private double pitch; // Neigen um die Querachse x- Achse
        private Point3D rotationPoint;

        public Transformation()
        {
            this.yaw = 0.0;
            this.pitch = 0.0;
        }

        public void doPitch(ProjectionCamera camera, double amount)
        {
            pitch += amount;
            this.Rotate(camera);
        }

        public void doYaw(ProjectionCamera camera, double amount)
        {
            yaw += amount;
            this.Rotate(camera);
        }

        public void Zoom(ProjectionCamera camera, double amount)
        {
            // Änderung der Kameraposition über dessen Position auf der Z- Achse
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - amount);
        }

        public void Rotate(ProjectionCamera camera)
        {
            double theta = yaw / 3;
            double phi = pitch / 3;

            //Bereich in dem rotiert wird
            if (phi < -90) phi = -90;
            if (phi > 90) phi = 90;

            
            Vector3D thetaAxis = new Vector3D(0, 1, 0);
            Vector3D phiAxis = new Vector3D(-1, 0, 0);

            Transform3DGroup transformGroup = camera.Transform as Transform3DGroup;
            transformGroup.Children.Clear();
            QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(-phiAxis, phi));

            //Ohne optionalem Parameter wird um den Ursprung (0,0,0) rotiert
            //Mit optionalem Parameter rotationPoint, kann man um den Mittelpunkt eines Objektes rotieren (new RotateTransform3D(r, rotationPoint));
            transformGroup.Children.Add(new RotateTransform3D(r));
            r = new QuaternionRotation3D(new Quaternion(-thetaAxis, theta));

            //Ohne optionalem Parameter wird um den Ursprung (0,0,0) rotiert
            //Mit optionalem Parameter rotationPoint, kann man um den Mittelpunkt eines Objektes rotieren (new RotateTransform3D(r, rotationPoint));
            transformGroup.Children.Add(new RotateTransform3D(r));
        }
      

        public void Reset(ProjectionCamera camera)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 2000);
            camera.Transform = new Transform3DGroup();
            yaw = 0;
            pitch = 0;
        }

        public double getYaw()
        {
            return yaw;
        }

        public double getPitch()
        {
            return pitch;
        }

        public Point3D getRotationPoint()
        {
            return rotationPoint;
        }

        public void setYaw(double value)
        {
            this.yaw = value;
        }

        public void setPitch(double value)
        {
            this.pitch = value;
        }

        public void setRotationPoint(Point3D rotationPoint)
        {
            this.rotationPoint = rotationPoint;
        }
    }
}
