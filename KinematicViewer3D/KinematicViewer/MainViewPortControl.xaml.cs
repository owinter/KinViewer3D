﻿using System;
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
        private GeometryModel3D modelGeometry;
        private MeshGeometry3D mesh;

        private bool mouseDownRight;
        private bool mouseDownLeft;

        //Klasse für Kameraeinstellungen und deren Positionen
        private ViewportCamera viewportCam;

        //Koordinatensystem für rechten Dock Panel erstellen
        private CoordSystemSmall c_SystemSmall;

        //Klasse für Transformationen aller Art
        private Transformation trans;

        public Cuboid cube;

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

            //buildSolid();

            //viewportCam.resetCam();
            this.CanMoveCamera = true;
        }

        private void generateModel()
        {
            for (int i = 0; i <= coordPoints.Count - 2; i += 2)
            {
                cube = new Cuboid();
                cube.buildSolid(coordPoints[i], coordPoints[i + 1], mesh, modelThickness);
                
            }
            group.Children.Remove(modelGeometry);

            // Geometry Model erstellen
            modelGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Cyan));

            // Geometry Modell Transformieren
            modelGeometry.Transform = new Transform3DGroup();

            // Geometry Model dem Main Viewport hinzufügen
            group.Children.Add(modelGeometry);

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
            group.Children.Remove(modelGeometry);
        }

        public void createModel()
        {
            mesh = new MeshGeometry3D();
            generateModel();
            
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
