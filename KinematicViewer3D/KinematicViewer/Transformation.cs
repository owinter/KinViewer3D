using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public class Transformation
    {
        private double _dYaw; // Gieren bzw Schlingern rechts links um y- Achse (Vertikalachse)
        private double _dPitch; // Neigen um die Querachse x- Achse
        private Point3D _oRotationPoint;

        public Transformation()
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

        public void Drag(ProjectionCamera camera, double dx, double dy)
        {
            camera.Position = new Point3D(camera.Position.X + dx, camera.Position.Y + dy, camera.Position.Z);
            camera.UpDirection = new Vector3D(0, 1, 0);
        }

        public void Pan(ProjectionCamera camera, double dx, double dy)
        {
            camera.LookDirection = new Vector3D((camera.LookDirection.X + dx), (camera.LookDirection.Y + dy), camera.LookDirection.Z);
            camera.UpDirection = new Vector3D(0, 1, 0);
        }

        public void Reset(ProjectionCamera camera)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 4000);
            camera.Transform = new Transform3DGroup();
            Yaw = 0;
            Pitch = 0;
        }

        public static void rotateModel(double axisAngle, Vector3D axisOfRotation, Point3D AxisPoint, Model3DGroup groupActive)
        {
            try
            {
                Point3D axisPoint = AxisPoint;
                AxisAngleRotation3D aARot = new AxisAngleRotation3D(axisOfRotation, axisAngle);
                RotateTransform3D rotation = new RotateTransform3D(aARot, axisPoint);
                groupActive.Transform = rotation;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Zuerst 3D Modell erstellen, dann erst Öfnungswinkel verändern. \n"
                    + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void resetModelTransformation(Model3DGroup groupActive)
        {
            groupActive.Transform = new Transform3DGroup();
        }
    }
}

//////////////////////////////////////////////////////////////////////////
/*
 *
 * Beispielhafte Rotation um eine achse mit Quaternionen
 *
 * */
/*
void OnRendering(object sender, EventArgs args)
{
   // Detach collection from MeshGeometry3D.
   Point3DCollection points = mesh.Positions;
   mesh.Positions = null;
   points.Clear();

   // Calculation rotation quaternion.
   double angle = 360.0 * (stopwatch.Elapsed.TotalSeconds %
                                   secondsPerCycle) / secondsPerCycle;
   Quaternion qRotate = new Quaternion(axis, angle);
   Quaternion qConjugate = qRotate;
   qConjugate.Conjugate();

   // Apply rotation to each point.
   foreach (Point3D point in pointsCuboid)
   {
       Quaternion qPoint = new Quaternion(point.X, point.Y, point.Z, 0);
       qPoint -= qCenter;
       Quaternion qRotatedPoint = qRotate * qPoint * qConjugate;
       qRotatedPoint += qCenter;
       points.Add(new Point3D(qRotatedPoint.X, qRotatedPoint.Y,
                                               qRotatedPoint.Z));
   }

   // Re-attach collections to MeshGeometry3D.
   mesh.Positions = points;
}
*/