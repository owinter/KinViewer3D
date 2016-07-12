﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public static class VisualObjectTransformation
    {
        //Rotiert einen bestehenden 3D Punkt um eine Achse mit Achsenmittelpunkt
        public static Point3D rotatePoint(Point3D point, double angle, Vector3D axis, Point3D rotationCenter)
        {
            RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(axis, angle), rotationCenter);
            return rotation.Transform(point);
        }

        //Rotiert eine Model3DGroup um eine Achse mit Achsmittelpunkt
        public static void rotateModelGroup(double axisAngle, Vector3D axisOfRotation, Point3D axisPoint, Model3DGroup groupActive)
        {
                AxisAngleRotation3D aARot = new AxisAngleRotation3D(axisOfRotation, axisAngle);
                RotateTransform3D rotation = new RotateTransform3D(aARot, axisPoint);
                groupActive.Transform = rotation;  
        }

        //Zurücksetzen der Transformation
        public static void resetModelGroupTransformation(Model3DGroup groupActive)
        {
            groupActive.Transform = new Transform3DGroup();
        }
    }
}
