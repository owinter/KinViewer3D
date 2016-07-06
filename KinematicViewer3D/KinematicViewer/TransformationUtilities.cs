using System.Windows.Media.Media3D;

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
        public static Point3D rotateNewPoint(Point3D point, double angle, Vector3D axis, Point3D rotationCenter)
        {
            Point3D p = new Point3D();
            RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(axis, angle), rotationCenter);
            p = rotation.Transform(point);
            return p;
        }

        public static Point3D rotateExistingPoint(Point3D point, double angle, Vector3D axis, Point3D rotationCenter)
        {
            RotateTransform3D rotation = new RotateTransform3D(new AxisAngleRotation3D(axis, angle), rotationCenter);
            return rotation.Transform(point);
        }

        public static Point3D reflectPoint(Point3D axisPoint, Point3D handPoint, Point3D drivePoint)
        {
            /*
             * Hessische Form : Px * N - d = 0   ==> d = Px * N
             *
             * vR ist der Punkt auf der Drehachse in Vektorform
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

            Vector3D vR = new Vector3D(axisPoint.X, axisPoint.Y, axisPoint.Z);
            Vector3D vAxisToHandE1 = handPoint - axisPoint;
            Vector3D vE2 = Vector3D.CrossProduct(vAxisToHandE1, new Vector3D(0, 1, 0));

            double d = Vector3D.DotProduct(vR, vE2);

            double lambda = ((d - Vector3D.DotProduct(new Vector3D(drivePoint.X, drivePoint.Y, drivePoint.Z), vE2)) / (Vector3D.DotProduct(vE2, vE2)));

            Point3D point = drivePoint + 2 * lambda * vE2;

            return point;
        }

        public static double calculateTailWidth(Point3D axisPoint, Point3D handPoint, Point3D attPointDoor)
        {
            Vector3D vR = new Vector3D(axisPoint.X, axisPoint.Y, axisPoint.Z);
            Vector3D vAxisToHandE1 = handPoint - axisPoint;
            Vector3D vE2 = Vector3D.CrossProduct(vAxisToHandE1, new Vector3D(0, 1, 0));

            double d = Vector3D.DotProduct(vR, vE2);

            double lambda = ((d - Vector3D.DotProduct(new Vector3D(attPointDoor.X, attPointDoor.Y, attPointDoor.Z), vE2)) / (Vector3D.DotProduct(vE2, vE2)));

            Point3D point = attPointDoor + 2 * lambda * vE2;

            double length = (point - attPointDoor).Length;
            return length;
        }

        public static double MinDistVectorToVector(Vector3D v1, Vector3D v2, Point3D p1, Point3D p2)
        {
            const double SMALL_NUM = 0.00000001;

            Vector3D u = v1;
            Vector3D v = v2;
            Vector3D w = p1 - p2;

            double a = Vector3D.DotProduct(u, u);
            double b = Vector3D.DotProduct(u, v);
            double c = Vector3D.DotProduct(v, v);
            double d = Vector3D.DotProduct(u, w);
            double e = Vector3D.DotProduct(v, w);
            double D = a * c - b * b;

            double sc, tc;

            if (D < SMALL_NUM)
            {
                sc = 0.0;
                tc = (b > c ? d / b : e / c); // den größten wert nehmen
            }
            else
            {
                sc = (b * e - c * d) / D;
                tc = (a * e - b * d) / D;
            }

            Vector3D dP = w + (sc * u) - (tc * v);

            double res = dP.Length;

            return res;
        }
    }
}