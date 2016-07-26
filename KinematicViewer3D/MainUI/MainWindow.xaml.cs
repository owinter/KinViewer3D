using KinematicViewer.Geometry.Guides;
using KinematicViewer.Geometry.GuidedElements;
using KinematicViewer.Transformation;
using KinematicViewer.UserControlLibrary;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MainUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Koordinatenpunkte der Benutzereingabe
        private List<Point3D> _oListAxisPoints;

        //Visuals im Viewport
        private SideDoor _oDoor;

        private SideDoor _oDoorMinAngle;
        private SideDoor _oDoorMaxAngle;

        private Tailgate _oTail;
        private Tailgate _oTailMinAngle;
        private Tailgate _oTailMaxAngle;

        private Drive _oDriveLeft;
        private Drive _oDriveRight;
        private Drive _oDriveDoor;

        private LineOfAction _oLineOfAction;
        private LineOfAction _oLineOfAction2;

        private TrackPoint _oTrackPoint;
        private TrackPoint _oTrackPoint2;

        private Vector3D _oVAxisOfRotation;

        //Model- Dicke (Durchmesser der Linien)
        private double _dModelThickness;

        private MainViewPortControl _oMvpControl;

        private bool _bTailgate = true;

        //Punkt Drehachse
        private double _dX1, _dY1, _dZ1;

        //Punkt für Haltegriff ( Handangriffspunkt)
        private double _dX2, _dY2, _dZ2;

        //Fester Punkt für Antrieb an Karrosserie
        private double _dX3, _dY3, _dZ3;

        //Punkt für Antrieb an Heckklappe
        private double _dX4, _dY4, _dZ4;

        public MainWindow()
        {
            InitializeComponent();
            AxisPoints = new List<Point3D>();

            MvpControl = new MainViewPortControl();
            MvpControl.setTextBlock(statusPane);
            MainUIViewport3D.Content = MvpControl;

            if (_bTailgate)
            {
                AxisOfRotation = new Vector3D(0, 0, 1);
            }
            else
            {
                //Rechte Tür / Beifahrer
                AxisOfRotation = new Vector3D(-13.94, 399.21, 20.94);

                ////Linke  Tür / Fahrertür
                ////AxisOfRotation = new Vector3D(13.94, -399.21, -20.94);
            }

            create_Button.IsEnabled = false;
            slider_open_ObjectAngle.IsEnabled = false;
            slider_open_ObjectAngle_TextBox.IsEnabled = false;

            //toolBox_TransparentMinMax.IsChecked = false;
            //toolBox_TrackPoint.IsChecked = false;
            //toolBox_LineOfAction.IsChecked = false;

            toolBox_TransparentMinMax.IsEnabled = false;
            toolBox_TrackPoint.IsEnabled = false;
            toolBox_LineOfAction.IsEnabled = false;
        }

        public List<Point3D> AxisPoints
        {
            get { return _oListAxisPoints; }
            set { _oListAxisPoints = value; }
        }

        public double ModelThickness
        {
            get { return _dModelThickness; }
            set { _dModelThickness = value; }
        }

        public MainViewPortControl MvpControl
        {
            get { return _oMvpControl; }
            private set { _oMvpControl = value; }
        }

        public Vector3D AxisOfRotation
        {
            get { return _oVAxisOfRotation; }
            set { _oVAxisOfRotation = value; }
        }

        public double X1
        {
            get { return _dX1; }
            set { _dX1 = value; }
        }

        public double Y1
        {
            get { return _dY1; }
            set { _dY1 = value; }
        }

        public double Z1
        {
            get { return _dZ1; }
            set { _dZ1 = value; }
        }

        public double X2
        {
            get { return _dX2; }
            set { _dX2 = value; }
        }

        public double Y2
        {
            get { return _dY2; }
            set { _dY2 = value; }
        }

        public double Z2
        {
            get { return _dZ2; }
            set { _dZ2 = value; }
        }

        public double X3
        {
            get { return _dX3; }
            set { _dX3 = value; }
        }

        public double Y3
        {
            get { return _dY3; }
            set { _dY3 = value; }
        }

        public double Z3
        {
            get { return _dZ3; }
            set { _dZ3 = value; }
        }

        public double X4
        {
            get { return _dX4; }
            set { _dX4 = value; }
        }

        public double Y4
        {
            get { return _dY4; }
            set { _dY4 = value; }
        }

        public double Z4
        {
            get { return _dZ4; }
            set { _dZ4 = value; }
        }

        //MENUBAR Elemente
        //Start einer perspektivischen Kamera
        private void perspective_Camera_Click(object sender, RoutedEventArgs e)
        {
            miOrthographic.IsChecked = false;
            miPerspective.IsChecked = true;
            MvpControl.switchToPerspective();
        }

        //Start einer orthographischen Kamera
        private void orthographic_Camera_Click(object sender, RoutedEventArgs e)
        {
            miPerspective.IsChecked = false;
            miOrthographic.IsChecked = true;
            MvpControl.switchToOrthographic();
        }

        //Objekt und Kamera neu laden
        private void reload_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.reloadCameraPositionDefault();
        }

        //Menubar Reset Transformation
        private void reset_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.resetCam();
        }

        //Menubar Session Beenden & schließen
        private void CommandBinding_Executed(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Menubar Anleitung
        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("* Mittlere und Rechte Maustaste gedrückt halten um die Kamera zu rotieren \n"
                         + "* Mittlere Maustaste gedrückt halten für Verschieben der Kamera auf der XY Ebene \n"
                         + "* Mittlere Maustaste und Linke Maustaste gedrückt halten zum Umschauen mit der Kamera \n"
                         + "*  Mausrad für Hinein und HinausZoomen der Kamera \n"
                         + "* Mittlere Maustaste gedrückt halten und einmal Rechte Maustaste drücken für Zoomen per Mausbewegung \n"
                         + "*  <- -> Pfeiltasten für links und rechts gieren um Y - Achse herum \n"
                         + "*  oben und unten Pfeiltasten für neigen um die X - Achse herum \n"
                         + "*  + und - Pfeiltasten für Hinein- und HinausZoomen der Kamera \n");
        }

        //STEUERUNG Elemente
        private void setSliderModelThickness(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ModelThickness = slider_Model_Thickness.Value;
        }

        //Fügt 3D Koordinaten der Liste coordPoints und der TextBox hinzu
        private void add_Button_Click(object sender, RoutedEventArgs e)
        {
            convertUserInput();

            if (_bTailgate)
                generateReflectedDrive();

            fill_TextBox();
            create_Button.IsEnabled = true;
        }

        private void create_Button_Click(object sender, RoutedEventArgs e)
        {
            slider_open_ObjectAngle.Value = 0.0;
            MvpControl.resetModelTransformation();
            MvpControl.clearModel();

            MvpControl.ModelThickness = ModelThickness;

            if (_bTailgate)
            {
                MvpControl.AxisOfRotation = AxisOfRotation;
                MvpControl.AxisPoint = AxisPoints[0];

                double tailWidth = TransformationUtilities.calculateTailWidth(AxisPoints[0], AxisPoints[1], AxisPoints[3]);
                _oTail = new Tailgate(AxisPoints[0], AxisPoints[1], AxisOfRotation, slider_Model_Thickness.Value, false, tailWidth);

                _oDriveLeft = new Drive(AxisPoints[2], AxisPoints[3], "DriveLeft");
                _oDriveRight = new Drive(AxisPoints[4], AxisPoints[5], "DriveRight");

                MvpControl.Guide = _oTail;

                MvpControl.AddPassiveElement(_oDriveLeft);
                MvpControl.AddPassiveElement(_oDriveRight);
                MvpControl.AddActiveElement(_oTail);
                //createLineOfAction();
                //createTrackPoint();
                //createStaticElementsTailgate();
            }
            else
            {
                MvpControl.AxisOfRotation = AxisOfRotation;
                MvpControl.AxisPoint = AxisPoints[0];
                _oDoor = new SideDoor(AxisPoints[0], AxisPoints[1], AxisOfRotation, slider_Model_Thickness.Value, false);
                _oDriveDoor = new Drive(AxisPoints[2], AxisPoints[3]);

                MvpControl.Guide = _oDoor;

                MvpControl.AddPassiveElement(_oDriveDoor);
                MvpControl.AddActiveElement(_oDoor);
                //createLineOfAction();
                //createTrackPoint();
                //createStaticElementsSideDoor();
            }

            //Dem MainGrid den focus übergeben
            MvpControl.FocusToViewport(sender, e);

            slider_open_ObjectAngle.IsEnabled = true;
            slider_open_ObjectAngle_TextBox.IsEnabled = true;

            toolBox_TransparentMinMax.IsEnabled = true;
            toolBox_TrackPoint.IsEnabled = true;
            toolBox_LineOfAction.IsEnabled = true;
        }

        //Button Listener für das Löschen der ListBox
        private void clear_listBox1_Button_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            AxisPoints.Clear();
            create_Button.IsEnabled = false;
        }

        //Löscht alle visuellen 3D Modele
        private void clear_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.resetModelTransformation();
            MvpControl.clearModel();

            slider_open_ObjectAngle.Value = 0.0;
            slider_open_ObjectAngle.IsEnabled = false;
            slider_open_ObjectAngle_TextBox.IsEnabled = false;

            toolBox_LineOfAction.IsChecked = false;
            toolBox_TrackPoint.IsChecked = false;
            toolBox_TransparentMinMax.IsChecked = false;

            toolBox_TransparentMinMax.IsEnabled = false;
            toolBox_TrackPoint.IsEnabled = false;
            toolBox_LineOfAction.IsEnabled = false;
        }

        //Listener für Öffnungswinkel SLIDER
        //Ändert den Öffnungswinkel des Objektes anhand des Sliders
        private void change_open_ObjectAngle(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MvpControl.Move(e.NewValue / 100);

            //Anzeige des Öffnungswinkel in TextBlock in [°]
            if (_bTailgate)
                OpenAngleDegree.Text = Math.Round(_oTail.CurValue, 2).ToString();
            else
                OpenAngleDegree.Text = Math.Round(_oDoor.CurValue, 2).ToString();

            if (toolBox_LineOfAction.IsChecked == true)
            {
                string hebelarm = " Hebelarm in [mm]: " + Math.Round(_oLineOfAction.DistancePerpendicular, 3).ToString();
                statusPane.Text = hebelarm;
            }
        }

        //Objekttransformation zurücksetzen
        private void reset_Model_Transformation_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.resetModelTransformation();
            slider_open_ObjectAngle.Value = 0.0;
        }

        //Koordinateneingabe in 3D Punkte umwandeln
        private void convertUserInput()
        {
            ////Benutzereingaben der Koordinaten zwischen denen eine 3D Linie erzeugt wird
            ////Punkt Drehachse
            //double x1, y1, z1;

            ////Punkt für Haltegriff ( Handangriffspunkt)
            //double x2, y2, z2;

            ////Fester Punkt für Antrieb an Karrosserie
            //double x3, y3, z3;

            ////Punkt für Antrieb an Heckklappe
            //double x4, y4, z4;

            try
            {
                /*
                // Koordinaten der Drehachse
                x1 = Convert.ToDouble(value_X1.Text);
                y1 = Convert.ToDouble(value_Y1.Text);
                z1 = Convert.ToDouble(value_Z1.Text);

                //Koordinaten des Handangriffspunktes
                x2 = Convert.ToDouble(value_X2.Text);
                y2 = Convert.ToDouble(value_Y2.Text);
                z2 = Convert.ToDouble(value_Z2.Text);

                //Koordinaten für Antrieb an Karrosserie
                x3 = Convert.ToDouble(value_X3.Text);
                y3 = Convert.ToDouble(value_Y3.Text);
                z3 = Convert.ToDouble(value_Z3.Text);

                //Koordinaten für Antrieb an Heckklappe
                x4 = Convert.ToDouble(value_X4.Text);
                y4 = Convert.ToDouble(value_Y4.Text);
                z4 = Convert.ToDouble(value_Z4.Text);*/

                if (_bTailgate)
                {
                    X1 = 0;
                    Y1 = 0;
                    Z1 = 0;

                    X2 = 500;
                    Y2 = -850;
                    Z2 = 0;

                    X3 = 44.69;
                    Y3 = -129.16;
                    Z3 = 550.57;

                    X4 = 393.56;
                    Y4 = -448.26;
                    Z4 = 626.20;
                }
                else
                {
                    //// OriginalKoordinaten
                    //X1 = 1472.41;
                    //Y1 = 780;
                    //Z1 = -934.46;

                    //X2 = 2565;
                    //Y2 = 1125;
                    //Z2 = -875;

                    //X3 = 1515.0;
                    //Y3 = 497.0;
                    //Z3 = -910.0;

                    //X4 = 1920.0;
                    //Y4 = 505.0;
                    //Z4 = -875.0;

                    //Auf den Urpsrung umgerechnete Koordinaten
                    X1 = 1472.41 - 1472.41;
                    Y1 = 780.525 - 780.525;
                    Z1 = -934.46 - (-934.46);

                    X2 = 2565 - 1472.41;
                    Y2 = 1125 - 780.525;
                    Z2 = -875 - (-934.4);

                    X3 = 1515 - 1472.41;
                    Y3 = 497 - 780.525;
                    Z3 = -910 - (-934.46);

                    X4 = 1920 - 1472.41;
                    Y4 = 505 - 780.525;
                    Z4 = -875 - (-934.46);
                }

                Point3D p1 = new Point3D(X1, Y1, Z1);
                Point3D p2 = new Point3D(X2, Y2, Z2);
                Point3D p3 = new Point3D(X3, Y3, Z3);
                Point3D p4 = new Point3D(X4, Y4, Z4);

                AxisPoints.Add(p1);
                AxisPoints.Add(p2);
                AxisPoints.Add(p3);
                AxisPoints.Add(p4);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Zuerst Koordinaten eingeben, dann erst Viewport benutzen. \n"
                    + "Zum Fortfahren den Button Clear betätigen und daraufhin die Koordinaten eingeben."
                    + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //Erstellen des TextBox Inhaltes
        private void fill_TextBox()
        {
            //CheckBox checkBox1 = new CheckBox();
            //checkBox1.Focusable = true;
            //checkBox1.Content
            /*
            listBox1.Items.Add( "Objekt: " + "\n"
                                + "P1(" + value_X1.Text + ", "
                                        + value_Y1.Text + ", "
                                        + value_Z1.Text + ")  \n"
                                + "P2(" + value_X2.Text + ", "
                                        + value_Y2.Text + ", "
                                        + value_Z2.Text + ") \n"
                                + "Elementbreite in mm: " + modelThickness + "\n"
                                + "\n"
                                +"Antrieb: " +"\n"
                                + "P3(" + value_X3.Text + ", "
                                        + value_Y3.Text + ", "
                                        + value_Z3.Text + ") \n"
                                + "P4(" + value_X4.Text + ", "
                                        + value_Y4.Text + ", "
                                        + value_Z4.Text + ") \n");*/
            //listBox1.Items.Add(checkBox1);
            if (_bTailgate)
            {
                listBox1.Items.Add("Objekt: " + "\n"
                                + "P1(" + X1 + ", "
                                        + Y1 + ", "
                                        + Z1 + ")  \n"
                                + "P2(" + X2 + ", "
                                        + Y2 + ", "
                                        + Z2 + ") \n"
                                + "Elementbreite in mm: " + ModelThickness + "\n"
                                + "\n"
                                + "Antrieb1: " + "\n"
                                + "P3(" + X3 + ", "
                                        + Y3 + ", "
                                        + Z3 + ") \n"
                                + "P4(" + X4 + ", "
                                        + Y4 + ", "
                                        + Z4 + ") \n"
                                + "\n"
                                + "Antrieb2: " + "\n"
                                + "P5(" + AxisPoints[4].X.ToString() + ", "
                                        + AxisPoints[4].Y.ToString() + ", "
                                        + AxisPoints[4].Z.ToString() + ") \n"
                                + "P6(" + AxisPoints[5].X.ToString() + ", "
                                        + AxisPoints[5].Y.ToString() + ", "
                                        + AxisPoints[5].Z.ToString() + ") \n");
            }
            else
            {
                listBox1.Items.Add("Objekt: " + "\n"
                                + "P1(" + X1 + ", "
                                        + Y1 + ", "
                                        + Z1 + ")  \n"
                                + "P2(" + X2 + ", "
                                        + Y2 + ", "
                                        + Z2 + ") \n"
                                + "Elementbreite in mm: " + ModelThickness + "\n"
                                + "\n"
                                + "Antrieb1: " + "\n"
                                + "P3(" + X3 + ", "
                                        + Y3 + ", "
                                        + Z3 + ") \n"
                                + "P4(" + X4 + ", "
                                        + Y4 + ", "
                                        + Z4 + ") \n"
                                + "\n");
            }
        }

        //ToolBar Buttons
        private void toolBox_Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.resetCam();
        }

        private void toolBox_ZoomIn_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.zoomIn();
        }

        private void toolBox_ZoomOut_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.zoomOut();
        }

        private void toolBox_TransparentMinMax_Click(object sender, RoutedEventArgs e)
        {
            if (toolBox_TransparentMinMax.IsChecked == false)
            {
                MvpControl.RemoveAllStaticElementsMin();
                MvpControl.RemoveAllStaticElementsMax();
            }
            else
            {
                if (_bTailgate)
                {
                    createStaticElementsTailgate();
                }
                else
                {
                    createStaticElementsSideDoor();
                }
            }
        }

        private void toolBox_LineOfAction_Click(object sender, RoutedEventArgs e)
        {
            if (toolBox_LineOfAction.IsChecked == false)
            {
                MvpControl.RemoveAllLineOfActionElements();
            }
            else
            {
                MvpControl.RemoveAllLineOfActionElements();
                createLineOfAction();

                string hebelarm = " Hebelarm in [mm]:  " + Math.Round(_oLineOfAction.DistancePerpendicular, 2).ToString();
                statusPane.Text = hebelarm;
            }
        }

        private void toolBox_TrackPoint_Click(object sender, RoutedEventArgs e)
        {
            if (toolBox_TrackPoint.IsChecked == false)
            {
                MvpControl.RemoveAllTrackPointElements();
            }
            else
            {
                createTrackPoint();
            }
        }

        private void toolBox_CoordSystem_Click(object sender, RoutedEventArgs e)
        {
            if (toolBox_CoordSystem.IsChecked == true)
            {
                MvpControl.showCoordSystem();
            }
            else
            {
                MvpControl.removeCoordSystem();
            }
        }

        private void toolBox_Front_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.viewFrontSide();
        }

        private void toolBox_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.viewBackSide();
        }

        private void toolBox_Right_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.viewRightSide();
        }

        private void toolBox_Left_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.viewLeftSide();
        }

        private void toolBox_Top_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.viewTopSide();
        }

        private void toolBox_Bottom_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.viewBottomSide();
        }

        private void generateReflectedDrive()
        {
            Point3D p1 = TransformationUtilities.reflectPoint(AxisPoints[0], AxisPoints[1], AxisPoints[2]);
            Point3D p2 = TransformationUtilities.reflectPoint(AxisPoints[0], AxisPoints[1], AxisPoints[3]);

            AxisPoints.Add(p1);
            AxisPoints.Add(p2);
        }

        private void createStaticElementsTailgate()
        {
            double tailWidth = TransformationUtilities.calculateTailWidth(AxisPoints[0], AxisPoints[1], AxisPoints[3]);

            _oTailMinAngle = new Tailgate(AxisPoints[0], AxisPoints[1], AxisOfRotation, slider_Model_Thickness.Value, true, tailWidth, getTransparentMaterial());
            _oTailMaxAngle = new Tailgate(AxisPoints[0], AxisPoints[1], AxisOfRotation, slider_Model_Thickness.Value, true, tailWidth, getTransparentMaterial());

            MvpControl.AddStaticElementMinAngle(_oTailMinAngle);
            MvpControl.AddStaticElementMaxAngle(_oTailMaxAngle);

            MvpControl.ShowStaticElementMin(_oTailMinAngle.MinValue);
            MvpControl.ShowStaticElementMax(_oTailMaxAngle.MaxValue);
        }

        private void createStaticElementsSideDoor()
        {
            _oDoorMinAngle = new SideDoor(AxisPoints[0], AxisPoints[1], AxisOfRotation, slider_Model_Thickness.Value, true, getTransparentMaterial());
            _oDoorMaxAngle = new SideDoor(AxisPoints[0], AxisPoints[1], AxisOfRotation, slider_Model_Thickness.Value, true, getTransparentMaterial());

            MvpControl.AddStaticElementMinAngle(_oDoorMinAngle);
            MvpControl.AddStaticElementMaxAngle(_oDoorMaxAngle);

            MvpControl.ShowStaticElementMin(_oDoorMinAngle.MinValue);
            MvpControl.ShowStaticElementMax(_oDoorMaxAngle.MaxValue);
        }

        private void createLineOfAction()
        {
            if (_bTailgate)
            {
                _oLineOfAction = new LineOfAction(AxisPoints[0], AxisPoints[2], AxisPoints[3], AxisOfRotation);
                _oLineOfAction2 = new LineOfAction(AxisPoints[0], AxisPoints[4], AxisPoints[5], AxisOfRotation);
                MvpControl.AddLineOfActionElement(_oLineOfAction);
                MvpControl.AddLineOfActionElement(_oLineOfAction2);
            }
            else
            {
                _oLineOfAction = new LineOfAction(AxisPoints[0], AxisPoints[2], AxisPoints[3], AxisOfRotation);
                MvpControl.AddLineOfActionElement(_oLineOfAction);
            }
        }

        private void createTrackPoint()
        {
            if (_bTailgate)
            {
                _oTrackPoint = new TrackPoint(AxisPoints[0], AxisPoints[3], AxisOfRotation);
                _oTrackPoint2 = new TrackPoint(AxisPoints[0], AxisPoints[5], AxisOfRotation);
                MvpControl.AddTrackPointElement(_oTrackPoint);
                MvpControl.AddTrackPointElement(_oTrackPoint2);
            }
            else
            {
                _oTrackPoint = new TrackPoint(AxisPoints[0], AxisPoints[3], AxisOfRotation);
                MvpControl.AddTrackPointElement(_oTrackPoint);
            }
        }

        //private Material getTransparentMaterial()
        //{
        //    Color c = new Color();
        //    c.A = 128;
        //    c.R = Colors.LightCyan.R;
        //    c.G = Colors.LightCyan.G;
        //    c.B = Colors.LightCyan.B;

        //    SolidColorBrush scBrush = new SolidColorBrush(c);
        //    scBrush.Opacity = 0.25;

        //    Material mat = new DiffuseMaterial(scBrush);
        //    //Material mat = new DiffuseMaterial((SolidColorBrush)(new BrushConverter().ConvertFrom("#8000FFFF")));

        //    return mat;
        //}

        private Material getTransparentMaterial()
        {
            Color c = new Color();
            c.A = 15;
            c.R = 192;
            c.G = 192;
            c.B = 192;

            SolidColorBrush scBrush = new SolidColorBrush(c);
            scBrush.Opacity = 0.95;
            Material mat = new EmissiveMaterial(scBrush);

            return mat;
        }
    }
}