using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public interface IGuide
    {
        double MaxValue { get; set; }
        double MinValue { get; set; }
        double CurValue { get; set; }

        Point3D MovePoint(Point3D endPoint);

        void InitiateMove(double per);

        void Move(Model3DGroup groupActive, double per = 0);
    }
}