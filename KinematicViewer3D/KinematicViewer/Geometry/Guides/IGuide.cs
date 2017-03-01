using System.Windows.Media.Media3D;

namespace KinematicViewer.Geometry.Guides
{
    public interface IGuide
    {
        //Minimal- und Maximalwert des Öffnungswinkels
        double MaxValue { get; set; }

        double MinValue { get; set; }
        double CurValue { get; set; }

        /// <summary>
        /// Setzt den Anbindungspunkt eines Antriebs an eine Heckklappe/ Seitentür in Bewegung
        /// </summary>
        /// <param name="endPoint">Anbindungspunkt des Antriebs an den Guide</param>
        /// <returns>Liefert einen rotierten Anbindungspunkt zurück</returns>
        Point3D MovePoint(Point3D endPoint);

        void InitiateMove(double per);

        /// <summary>
        /// Bewegt alle Modelle innerhalb der Gruppe groupActive
        /// Punktkoordianten werden nicht gespeichert
        /// Eignet sich für die transparenten Objekte
        /// </summary>
        /// <param name="groupActive">Objekte welche aktiv bewegt werden</param>
        /// <param name="per">Anteil der Bewegung in % [0-100]</param>
        void Move(Model3DGroup groupActive, double per = 0);

        /// <summary>
        /// Bewegt ein visuelles Modell
        /// Alle Punktkoordinaten werden gespeichert
        /// Eignet sich für das "echte" visuelle Modell einer Tür oder Heckklappe
        /// </summary>
        /// <param name="per">Anteil der Bewegung in % [0-100]</param>
        void Move(double per = 0);
    }
}