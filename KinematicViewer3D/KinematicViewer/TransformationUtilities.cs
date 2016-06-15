﻿using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public static class TransformationUtilities
    {
        //Skaliere einen Vektor mit einem offset Wert
        public static Vector3D scaleToOffset(Vector3D vScale, double value)
        {
            vScale.Normalize();
            vScale = vScale * value;
            return vScale;
        }

        //Skalieren eines Vektors auf eine best. Länge
        public static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        //Rotiere einen neuen Punkt um einen bestehenden und eine Achse
        public static Point3D rotateNewPoint(Point3D point, double angle, Vector3D axis)
        {
            Point3D p = new Point3D();
            RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(axis, angle));
            p = rotation.Transform(point);
            return p;
        }

        public static Point3D rotateExistingPoint(Point3D point, double angle, Vector3D axis)
        {
            RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(axis, angle));
            return rotation.Transform(point);
        }

        public static Point3D reflectPoint(Point3D p, Vector3D vR, Vector3D vE2, double d)
        {
            /*
             * Hessische Form : Px * N - d = 0   ==> d = Px * N
             *
             * vR ist der Punkt auf der Drehachse in Vektorenform
             * vE2 ist Richtungsvektor von Drehachse 90 ° zur Seite. Quasi die Breite der Heckklappe
             *
             * Also:
             * d = vR * vE2
             *
             *
             *
             *          d - attPoint * vE2
             * lamda = --------------------   // lambda ist der Abstand vom zum Spiegelnden Punkt des Drives zur Ebene,
             *              vE2 * vE2         // die durch den Vektor von AxisPoint zu HandPoint und der Y- Achse aufgespannt wird
             *
             *
             *
             * Punkt P = attPoint + 2 * lambda * vE2
             *
             */
            double lambda = ((d - Vector3D.DotProduct(new Vector3D(p.X, p.Y, p.Z), vE2)) / (Vector3D.DotProduct(vE2, vE2)));

            Point3D point = p + 2 * lambda * vE2;

            return point;
        }
    }
}