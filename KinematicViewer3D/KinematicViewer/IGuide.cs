using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinematicViewer
{
    public interface IGuide
    {
        void Move(double newValue, List<GeometricalElement> elementsMoving);
    }
}
