using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace KinematicViewer
{
    public interface IGuide
    {
        double MaxValue { get; set; }
        double CurValue { get; set; }

        Point3D MovePoint(Point3D endPoint);

        void InitiateMove(double per);
    }
}
