using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using KinematicViewer.Transformation;

namespace KinematicViewer.UserControlLibrary
{
    public class CoordSystem
    {
        //Rendering Objekt
        private ModelVisual3D _oCoordSystem_Visual;

        //ModelGroup für das Koordinatensystem
        private Model3DGroup _oCoordSystem_ModelGroup;

        //3D KoordinatenSystem Model der jeweiligen farbigen Achsen
        private GeometryModel3D _oAxes_Model_X;
        private GeometryModel3D _oAxes_Model_Y;
        private GeometryModel3D _oAxes_Model_Z;
        private GeometryModel3D _oCube_Model;

        //Liste von Lichtern für das Koordinatensystem.
        private List<Light> _oLLights;

        private Viewport3D _oViewportCoordSystem;

        /// <summary>
        /// Erzeugt ein visuelles Modell eines Koordinatensystems
        /// </summary>
        /// <param name="viewportCoordSystem">Objekt des Viewports für das ein-/ ausschaltbare Koordinatensystem</param>
        public CoordSystem(Viewport3D viewportCoordSystem)
        {
            _oViewportCoordSystem = viewportCoordSystem;

            _oCoordSystem_ModelGroup = new Model3DGroup();
            _oCoordSystem_Visual = new ModelVisual3D();
            Lights = new List<Light>();

            DefineLights();
            buildCoordinateSystem();
        }

        private List<Light> Lights
        {
            get { return _oLLights; }
            set { _oLLights = value; }
        }

        //Fügt dem Viewport die einzelnen gerenderten Modelle hinzu
        private void buildCoordinateSystem()
        {
            DefineCoordinateSystem(out _oAxes_Model_X, out _oAxes_Model_Y, out _oAxes_Model_Z, out _oCube_Model);
            _oCoordSystem_ModelGroup.Children.Add(_oAxes_Model_X);
            _oCoordSystem_ModelGroup.Children.Add(_oAxes_Model_Y);
            _oCoordSystem_ModelGroup.Children.Add(_oAxes_Model_Z);
            _oCoordSystem_ModelGroup.Children.Add(_oCube_Model);

            _oCoordSystem_Visual.Content = _oCoordSystem_ModelGroup;

            _oViewportCoordSystem.Children.Add(_oCoordSystem_Visual);
        }

        //Definiert die 3 Achsen mit jeweiligen Materialien und Farben
        private void DefineCoordinateSystem(out GeometryModel3D axes_Model_x, out GeometryModel3D axes_Model_y, out GeometryModel3D axes_Model_z, out GeometryModel3D cube_Model)
        {
            // 3 Achsen erstellen.
            MeshGeometry3D axes_mesh_x = new MeshGeometry3D();
            MeshGeometry3D axes_mesh_y = new MeshGeometry3D();
            MeshGeometry3D axes_mesh_z = new MeshGeometry3D();
            MeshGeometry3D cube_mesh = new MeshGeometry3D();

            Point3D origin = new Point3D(0, 0, 0);
            Point3D cubeStart = new Point3D(125, 0, 125);
            Point3D cubeEnd = new Point3D(125, 250, 125);
            Point3D xmax = new Point3D(1000, 0, 0);
            Point3D ymax = new Point3D(0, 1000, 0);
            Point3D zmax = new Point3D(0, 0, 1000);

            AddSegment(axes_mesh_x, origin, xmax, new Vector3D(0, 1, 0), 75);         //RED Achse     X Achse
            AddSegment(axes_mesh_z, origin, zmax, new Vector3D(0, 1, 0), 75);         //BLUE Achse    Z Achse
            AddSegment(axes_mesh_y, origin, ymax, new Vector3D(1, 0, 0), 75);         //GREEN Achse   Y Achse
            AddSegment(cube_mesh, cubeStart, cubeEnd, new Vector3D(1, 0, 0), 200);     //YELLOW CUBE   CUBE Model

            SolidColorBrush axes_brush_x = Brushes.Red;
            SolidColorBrush axes_brush_z = Brushes.Blue;
            SolidColorBrush axes_brush_y = Brushes.Green;
            SolidColorBrush cube_brush = Brushes.Yellow;

            DiffuseMaterial axes_material_x = new DiffuseMaterial(axes_brush_x);
            DiffuseMaterial axes_material_z = new DiffuseMaterial(axes_brush_z);
            DiffuseMaterial axes_material_y = new DiffuseMaterial(axes_brush_y);
            DiffuseMaterial cube_material = new DiffuseMaterial(cube_brush);

            axes_Model_x = new GeometryModel3D(axes_mesh_x, axes_material_x);
            axes_Model_y = new GeometryModel3D(axes_mesh_y, axes_material_y);
            axes_Model_z = new GeometryModel3D(axes_mesh_z, axes_material_z);
            cube_Model = new GeometryModel3D(cube_mesh, cube_material);
        }

        //Erstellt ein dünnes Rechteck zwischen zwei Punkten
        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            // Vektor zwischen Ursprung und Segment-Endpunkt berechnen
            Vector3D v = point2 - point1;

            // Breite des Segmentes entsprechend Skalieren
            Vector3D n1 = TransformationUtilities.ScaleVector(up, thickness / 2.0);

            // Erstellt einen senkrechten skalierten Vektor zu n1
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = TransformationUtilities.ScaleVector(n2, thickness / 2.0);

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

        //Fügt die Seiten an das Mesh vom Koordinatensystem
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Punktkoordinaten dem Mesh hinzufügen
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Triangles erzeugen
            mesh.TriangleIndices.Add(index++);
            mesh.TriangleIndices.Add(index++);
            mesh.TriangleIndices.Add(index);
        }

        // Beleuchtung für das Koordinatensystem
        private void DefineLights()
        {
            Color color64 = Color.FromArgb(255, 64, 64, 64);
            Color color128 = Color.FromArgb(255, 128, 128, 128);
            Lights.Add(new AmbientLight(color64));
            Lights.Add(new DirectionalLight(color128, new Vector3D(-1.0, -2.0, -3.0)));
            Lights.Add(new DirectionalLight(color128, new Vector3D(1.0, 2.0, 3.0)));

            Lights.Add(new DirectionalLight(color128, new Vector3D(-5000, -5000, -10000)));
            Lights.Add(new DirectionalLight(color128, new Vector3D(5000, 5000, 10000)));

            //füge jedem Visual ein Licht hinzu
            foreach (Light light in Lights)
            {
                _oCoordSystem_ModelGroup.Children.Add(light);
            }
        }

        public void reloadCoordinateSystem()
        {
            _oAxes_Model_X.Transform = new Transform3DGroup();
            _oAxes_Model_Y.Transform = new Transform3DGroup();
            _oAxes_Model_Z.Transform = new Transform3DGroup();
            _oCube_Model.Transform = new Transform3DGroup();
        }
    }
}