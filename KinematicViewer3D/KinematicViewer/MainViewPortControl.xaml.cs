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
using System.Windows.Markup;
using System.Diagnostics;

namespace KinematicViewer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainViewPortControl : UserControl
    {
        private GeometryModel3D cuboidGeometry, 
                                cylinderGeometry, 
                                sphereGeometry;
        

        private bool mouseDownRight;
        private bool mouseDownLeft;

        //Klasse für Kameraeinstellungen und deren Positionen
        private ViewportCamera viewportCam;

        //Koordinatensystem für rechten Dock Panel erstellen
        private CoordSystemSmall c_SystemSmall;

        //Klasse für Transformationen aller Art
        private Transformation trans;

        public Cuboid cube;
        public Sphere sphere;
        public Cylinder cylinder;

        //Koordinatenpunkte der Benutzereingabe
        private List<Point3D> coordPoints;

        //Mittelpunkt des Objektes
        private Point3D mPoint;

        //Breite bzw Dicke des jeweiligen Models
        private double modelThickness;


        public MainViewPortControl()
        {
            

            InitializeComponent();
            //coordPoints = new List<Point3D>();
            trans = new Transformation();
            c_SystemSmall = new CoordSystemSmall();
            viewportCam = new ViewportCamera(MainGrid, viewport, c_SystemSmall, trans);

            viewportCam.startPerspectiveCamera();
            viewportCam.MyCam = Cam.Perspective;


            //viewportCam.resetCam();
            this.CanMoveCamera = true;
        }

        private void generateCylinder()
        {
            MeshGeometry3D mesh_Cylinder = new MeshGeometry3D();
            cylinder = new Cylinder(mesh_Cylinder, new Point3D(0, 1, 0), new Vector3D(-100, -600, 0), 50, 128);

            cylinderGeometry = new GeometryModel3D(mesh_Cylinder, new DiffuseMaterial(Brushes.Cyan));
            cylinderGeometry.Transform = new Transform3DGroup();
            group.Children.Add(cylinderGeometry);
        }

        private void generateCuboid()
        {
            MeshGeometry3D mesh_Cuboid = new MeshGeometry3D();
            cube = new Cuboid();
            for (int i = 0; i <= coordPoints.Count - 2; i += 2)
            {
                cube.buildCuboid(coordPoints[i], coordPoints[i + 1], mesh_Cuboid, modelThickness);
            }
            cuboidGeometry = new GeometryModel3D(mesh_Cuboid, new DiffuseMaterial(Brushes.Cyan));
            cuboidGeometry.Transform = new Transform3DGroup();
            group.Children.Add(cuboidGeometry);
        }

        private void generateSphere()
        {
            MeshGeometry3D mesh_Sphere = new MeshGeometry3D();
            sphere = new Sphere(coordPoints[0], modelThickness);
            mesh_Sphere = sphere.SphereGeometry;

            sphereGeometry = new GeometryModel3D(mesh_Sphere, new DiffuseMaterial(Brushes.Cyan));
            sphereGeometry.Transform = new Transform3DGroup();
            group.Children.Add(sphereGeometry);
        }

        private void generateModel()
        {
            clearModel();


            generateCylinder();
            generateCuboid();
            generateSphere();

            ////Kamera für Main Viewport updaten
            viewportCam.updatePositionCamera();

            //Kamera im Fenster des Koordinatensystems ändern
            //c_SystemSmall.updatePositionCamera_CoordinateSystem(viewportCam.CameraR, viewportCam.CameraPhi, viewportCam.CameraTheta);

            //Mittelpunkt des Modells berechnen
            calculateMPoint();
        }


        //MAUSSTEUERUNG im MainGrid
        private void MainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.CanMoveCamera)
            {
                viewportCam.viewport_Grid_MouseWheel(sender, e);
            }
        }

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.CanMoveCamera)
            {
                // Wenn Rechte Maustaste "nicht" gedrückt dann passiert auch nichts
                if (!mouseDownRight) return;

                viewportCam.rotateCam();
            }
        }

        private void MainGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CanMoveCamera)
            {
                if (e.RightButton != MouseButtonState.Pressed) return;

                mouseDownRight = true;
                viewportCam.setMouseToCenter();

                // Curser unsichtbar machen
                this.Cursor = Cursors.None;
            }
        }

        private void MainGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Maustaste ist nicht länger gedrückt, also wird Curser wieder sichtbar
            mouseDownRight = false;
            this.Cursor = Cursors.Arrow;
        }

        private void MainGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDownLeft = true;
            viewport_MouseDown(sender, e);
        }

        private void MainGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDownLeft = false;
        }

        //TASTATURSTEUERUNG für KEY Down
        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            viewportCam.viewport_KeyDown(sender, e);
        }

        //Dem Viewport bzw. dem MainGrid den Focus übergeben
        private void viewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainGrid.Focus();
            e.Handled = true;
        }

        public void switchToPerspective()
        {
            viewportCam.startPerspectiveCamera();
            viewportCam.MyCam = Cam.Perspective;
        }

        public void switchToOrthographic()
        {
            viewportCam.startOrthographicCamera();
            viewportCam.MyCam = Cam.Orthographic;
        }

        public void resetCam()
        {
            viewportCam.resetCam();
        }

        public void reloadCameraPositionDefault()
        {
            viewportCam.reloadCameraPositionDefault();
        }

        public void clearModel()
        {
            group.Children.Remove(cuboidGeometry);
            group.Children.Remove(cylinderGeometry);
            group.Children.Remove(sphereGeometry);
        }

        public void createModel()
        {
            generateModel();   
        }

        public void viewFrontSide()
        {
            viewportCam.viewFront();
        }

        public void viewBackSide()
        {
            viewportCam.viewBack();
        }

        public void viewRightSide()
        {
            viewportCam.viewRight();
        }

        public void viewLeftSide()
        {
            viewportCam.viewLeft();
        }

        public void viewTopSide()
        {
            viewportCam.viewTop();
        }

        public void viewBottomSide()
        {
            viewportCam.viewBottom();
        }

        public void zoomIn()
        {
            viewportCam.zoomIn();
        }

        public void zoomOut()
        {
            viewportCam.zoomOut();
        }

        private void calculateMPoint()
        {
            double count = coordPoints.Count;
            double x = 0;
            double y = 0;
            double z = 0;
            for (int i = 0; i < count; i++)
            {
                x += coordPoints[i].X / count;
                y += coordPoints[i].Y / count;
                z += coordPoints[i].Z / count;
            }
            mPoint = new Point3D(x, y, z);
        }



        //Öffentliche Getter & Setter Methoden
        public bool CanMoveCamera { get; set; }

        public double ModelThickness
        {
            get { return modelThickness; }
            set { modelThickness = value; }
        }

        public List<Point3D> CoordPoints
        {
            get { return coordPoints; }
            set { coordPoints = value; }
        }


    }
}
