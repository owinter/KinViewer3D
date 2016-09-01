using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace KinematicViewer.Transformation
{
    public static class VisualObjectTransformation
    {
        
        /// <summary>
        /// Rotiert einen bestehenden 3D Punkt um eine Achse mit Achsenmittelpunkt und gibt den 3D Punkt zurück
        /// </summary>
        /// <param name="point">3D Koordinate</param>
        /// <param name="axisAngle">Winkel mit welchem rotiert werden soll</param>
        /// <param name="axisOfRotation">Scharnierachse</param>
        /// <param name="rotationCenter">Mittelpunkt um den rotiert werden soll</param>
        /// <returns></returns>
        public static Point3D rotatePoint(Point3D point, double axisAngle, Vector3D axisOfRotation, Point3D rotationCenter)
        {
            AxisAngleRotation3D aARot = new AxisAngleRotation3D(axisOfRotation, axisAngle);
            RotateTransform3D rotation = new RotateTransform3D(aARot, rotationCenter);
            return rotation.Transform(point);
        }

        /// <summary>
        /// Rotiert eine Model3DGroup um eine Achse mit Achsmittelpunkt
        /// </summary>
        /// <param name="axisAngle">Winkel um welchen rotiert werden soll</param>
        /// <param name="axisOfRotation">Scharnierachse</param>
        /// <param name="rotationCenter">Mittelpunkt um den rotiert werden soll</param>
        /// <param name="groupActive">Model3DGroup welche komplett rotiert werden soll</param>
        public static void rotateModelGroup(double axisAngle, Vector3D axisOfRotation, Point3D rotationCenter, Model3DGroup groupActive)
        {
                AxisAngleRotation3D aARot = new AxisAngleRotation3D(axisOfRotation, axisAngle);
                RotateTransform3D rotation = new RotateTransform3D(aARot, rotationCenter);
                groupActive.Transform = rotation;  
        }

        /// <summary>
        /// Setzt die Transformationen einer Model3DGroup zurück
        /// </summary>
        /// <param name="groupActive">Model3DGroup welche zurückgesetzt werden soll</param>
        public static void resetModelGroupTransformation(Model3DGroup groupActive)
        {
            groupActive.Transform = new Transform3DGroup();
        }
    }
}
