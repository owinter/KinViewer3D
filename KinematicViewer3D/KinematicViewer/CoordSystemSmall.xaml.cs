using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinematicViewer
{
    /// <summary>
    /// Interaction logic for CoordSystemSmall.xaml
    /// </summary>
    public partial class CoordSystemSmall : UserControl
    {
        private ModelVisual3D _oCoordSystem_Visual;

        //ModelGroup für das Koordinatensystem
        private Model3DGroup _oCoordSystem_ModelGroup;
        
        //3D KoordinatenSystem Model der jeweiligen farbigen Achsen
        private GeometryModel3D _oAxes_Model_X;
        private GeometryModel3D _oAxes_Model_Y;
        private GeometryModel3D _oAxes_Model_Z;

        //Perspektiviesche Kamera des Koordinatensystem
        private PerspectiveCamera _oPCamera_CoordSystem;
        /*
                //Änderung der Kamera bei Tastatureingabe up and down Pfeiltaste
                private const double coord_CameraDPhi = 0.06;

                //Änderung der Kamera bei Tastatureingabe left and right Preiltaste
                private const double coord_CameraDTheta = 0.06;

                //Änderung der Kamera bei Tastatureingabe von + und -
                private const double coord_CameraDR = 0.3;

                //Aktuelle Camera Positionen
                private double coord_CameraPhi = Math.PI / 6.0 - 1 * coord_CameraDPhi;
                private double coord_CameraTheta = Math.PI / 6.0 + 5 * coord_CameraDTheta;
                private double coord_CameraR = 13.0;
        */

        // Lichter für das Koordinatensystem.
        private List<Light> _oLLights;

        //Transformationen für das Koordinatensystem
        private Transformation _oTransCoordSystem;

      
        public CoordSystemSmall()
        {
            InitializeComponent();
            _oCoordSystem_ModelGroup = new Model3DGroup();
            _oCoordSystem_Visual = new ModelVisual3D();
            _oTransCoordSystem = new Transformation();

            Lights = new List<Light>();
            makeCamera();
            DefineLights();
            buildCoordinateSystem();
        }


        public List<Light> Lights
        {
            get { return _oLLights; }
            private set { _oLLights = value; }
        }

        public PerspectiveCamera Camera_CoordSystem
        {
            get { return _oPCamera_CoordSystem; }
            set { _oPCamera_CoordSystem = value; }
        }

        //KOORDINATENSYSTEM im rechten DOCK PANEL
        //Viewport Rechts unten im Bild für Koordinatensystem --> Testweise
        //Definiert ein Geometry Model für das farbige Koordinatensystem

        //Erstellt das visuelle Model eines farbigen Koordinatensystem im rechten DOCK Panel Viewport

        private void makeCamera()
        {
            Camera_CoordSystem = new PerspectiveCamera();
            Camera_CoordSystem.Position = new Point3D(0, 100, 2000);
            Camera_CoordSystem.LookDirection = new Vector3D(0, -100, -2000);
            Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
            Camera_CoordSystem.FieldOfView = 60;
            Camera_CoordSystem.FarPlaneDistance = 15000;
            Camera_CoordSystem.NearPlaneDistance = 0.125;
            viewportCoordSystemSmall.Camera = Camera_CoordSystem;

           // updatePositionCamera_CoordinateSystem(coord_CameraR, coord_CameraPhi, coord_CameraTheta);
        }
        public void buildCoordinateSystem()
        {
            DefineCoordinateSystem(out _oAxes_Model_X, out _oAxes_Model_Y, out _oAxes_Model_Z);
            _oCoordSystem_ModelGroup.Children.Add(_oAxes_Model_X);
            _oCoordSystem_ModelGroup.Children.Add(_oAxes_Model_Y);
            _oCoordSystem_ModelGroup.Children.Add(_oAxes_Model_Z);

            
            _oCoordSystem_Visual.Content = _oCoordSystem_ModelGroup;

            viewportCoordSystemSmall.Children.Add(_oCoordSystem_Visual);
        }

        //Definiert die 3 Achsen mit jeweiligen Materialien und Farben 
        private void DefineCoordinateSystem(out GeometryModel3D axes_Model_x, out GeometryModel3D axes_Model_y, out GeometryModel3D axes_Model_z)
        {
            // Make the axes model.
            MeshGeometry3D axes_mesh_x = new MeshGeometry3D();
            MeshGeometry3D axes_mesh_y = new MeshGeometry3D();
            MeshGeometry3D axes_mesh_z = new MeshGeometry3D();

            Point3D origin = new Point3D(0, 0, 0);
            Point3D xmax = new Point3D(1000, 0, 0);
            Point3D ymax = new Point3D(0, 1000, 0);
            Point3D zmax = new Point3D(0, 0, 1000);
            AddSegment(axes_mesh_x, origin, xmax, new Vector3D(0, 1, 0));         //RED Achse     X Achse
            AddSegment(axes_mesh_z, origin, zmax, new Vector3D(0, 1, 0));         //BLUE Achse    Z Achse
            AddSegment(axes_mesh_y, origin, ymax, new Vector3D(1, 0, 0));         //GREEN Achse   Y Achse

            SolidColorBrush axes_brush_x = Brushes.Red;
            SolidColorBrush axes_brush_z = Brushes.Blue;
            SolidColorBrush axes_brush_y = Brushes.Green;
            DiffuseMaterial axes_material_x = new DiffuseMaterial(axes_brush_x);
            DiffuseMaterial axes_material_z = new DiffuseMaterial(axes_brush_z);
            DiffuseMaterial axes_material_y = new DiffuseMaterial(axes_brush_y);
            axes_Model_x = new GeometryModel3D(axes_mesh_x, axes_material_x);
            axes_Model_y = new GeometryModel3D(axes_mesh_y, axes_material_y);
            axes_Model_z = new GeometryModel3D(axes_mesh_z, axes_material_z);
        }


        //Erstelle ein dünnes Rechteck zwischen zwei Punkten
        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up)
        {
            //Dicke des  Koordinatensystems
            const double thickness = 50;

            // Vektor zwischen Ursprung und Segment-Endpunkt berechnen
            Vector3D v = point2 - point1;

            // Breite des Segmentes entsprechend Skalieren
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Erstellt einen senkrechten skalierten Vektor zu n1
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Erstellen eines kleinen dünnen Rechtecks.
            // p1pm bedeutet point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;    //0
            Point3D p1mp = point1 - n1 + n2;    //1
            Point3D p1pm = point1 + n1 - n2;    //2
            Point3D p1mm = point1 - n1 - n2;    //3

            Point3D p2pp = point2 + n1 + n2;    //4
            Point3D p2mp = point2 - n1 + n2;    //5
            Point3D p2pm = point2 + n1 - n2;    //6
            Point3D p2mm = point2 - n1 - n2;    //7

            //Seiten
            //                  0   1       5
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            //                  0   5       4
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            //                  0   4       6
            AddTriangle(mesh, p1pp, p2pp, p2pm);
            //                  0   6       2
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            //                  2   6       7
            AddTriangle(mesh, p1pm, p2pm, p2mm);
            //                  2   7       3
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            //                  3   7       5
            AddTriangle(mesh, p1mm, p2mm, p2mp);
            //                  3   5       1
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Enden.
            //                  0   2       3
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            //                  0   3       1
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            //                  4   5       7
            AddTriangle(mesh, p2pp, p2mp, p2mm);
            //                  4   7       6
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }


        // Do not reuse points so triangles don't share normals.
        //Fügt die Seiten an das Mesh vom Koordinatensystem
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index++);
            mesh.TriangleIndices.Add(index++);
            mesh.TriangleIndices.Add(index);
        }


        // Vektor Länge für das Koordinatensystem
        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        // Position der Camera für das Koordinatensystem.
        public void updatePositionCamera_CoordinateSystem(double coord_CameraR, double coord_CameraPhi, double coord_CameraTheta)
        {

            // Berechnung der aktuellen Kamera Position.
            double y = coord_CameraR * Math.Sin(coord_CameraPhi);
            double hyp = coord_CameraR * Math.Cos(coord_CameraPhi);
            double x = hyp * Math.Cos(coord_CameraTheta);
            double z = hyp * Math.Sin(coord_CameraTheta);
            Camera_CoordSystem.Position = new Point3D(x, y, z);

            // Blickrichtung auf KoordinatenUrsprung gerichtet.
            Camera_CoordSystem.LookDirection = new Vector3D(-x, -y, -z);

            // UP Direction der Kamera bestimmen.
            Camera_CoordSystem.UpDirection = new Vector3D(0, 1, 0);
        }


        // Beleuchtung für das Koordinatensystem
        private void DefineLights()
        {
            Color color64 = Color.FromArgb(255, 64, 64, 64);
            Color color128 = Color.FromArgb(255, 128, 128, 128);
            Lights.Add(new AmbientLight(color64));
            Lights.Add(new DirectionalLight(color128, new Vector3D(-1.0, -2.0, -3.0)));
            Lights.Add(new DirectionalLight(color128, new Vector3D(1.0, 2.0, 3.0)));

            //füge jedem Visual ein Licht hinzu
            foreach (Light light in Lights)
            {
                _oCoordSystem_ModelGroup.Children.Add(light);
            }
        }

        public void reloadCoordinateSystem()
        {
            //Kamera Position für Koordinatensystem Viewer reloaden
            Camera_CoordSystem.Position = new Point3D(0, 100, 2000);
            Camera_CoordSystem.LookDirection = new Vector3D(0, -100, -2000);
            _oAxes_Model_X.Transform = new Transform3DGroup();
            _oAxes_Model_Y.Transform = new Transform3DGroup();
            _oAxes_Model_Z.Transform = new Transform3DGroup();

            /*
            //Winkel der Kamera zurücksetzen auf default Werte
            coord_CameraPhi = Math.PI / 6.0 - 1 * coord_CameraDPhi;
            coord_CameraTheta = Math.PI / 6.0 + 5 * coord_CameraDTheta;
            coord_CameraR = 13;*/
        }

        public void updateC_SystemSmall()
        {
            _oTransCoordSystem.Rotate(Camera_CoordSystem);
        }
    }
}
