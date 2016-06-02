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
using KinematicViewer;

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
        private Tailgate _oTail;
        private Drive _oDriveLeft;
        private Drive _oDriveRight;

        //Model- Dicke (Durchmesser der Linien)
        private double _dModelThickness;

        private MainViewPortControl _oMvpControl;
        private CoordSystemSmall _oCssControl;


        //Benutzereingaben der Koordinaten zwischen denen eine 3D Linie erzeugt wird
        //Punkt Drehachse
        double x1, y1, z1;

        //Punkt für Haltegriff ( Handangriffspunkt)
        double x2, y2, z2;

        //Fester Punkt für Antrieb an Karrosserie
        double x3, y3, z3;

        //Punkt für Antrieb an Heckklappe
        double x4, y4, z4;
        public MainWindow()
        {
            InitializeComponent();
            AxisPoints = new List<Point3D>();
            //CssControl = new CoordSystemSmall();
            //MainUICoordSystemSmall.Content = CssControl;

            MvpControl = new MainViewPortControl();
            MvpControl.setTextBlock(statusPane);
            MainUIViewport3D.Content = MvpControl;

            

            create_Button.IsEnabled = false;
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
            private set  { _oMvpControl = value; }
        }

        public CoordSystemSmall CssControl
        {
            get { return _oCssControl; }
            private set { _oCssControl = value; }
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
            this.Close();
        }

        //Menubar Anleitung
        private void Manual_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("*  Rechte Maustaste gedrückt halten um die Kamera zu rotieren \n" 
                         + "*  Mausrad für Hinein und HinausZoomen der Kamera \n"
                         + "*  <- -> Pfeiltasten für links und rechts gieren um Y - Achse herum \n" 
                         + "*  oben und unten Pfeiltasten für neigen um die X - Achse herum \n"
                         +"*  + und - Pfeiltasten für Hinein- und HinausZoomen der Kamera \n");
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

            _oDoor = new SideDoor(AxisPoints[0], AxisPoints[1], new Vector3D(0, 1, 0), this.slider_Model_Thickness.Value);
            //_oTail = new Tailgate(AxisPoints[0], AxisPoints[1], this.slider_Model_Thickness.Value);
            _oDriveLeft = new Drive(AxisPoints[2], AxisPoints[3]);
            _oDriveRight = new Drive(AxisPoints[4], AxisPoints[5]);

            //MvpControl.Guide = _oTail;
            MvpControl.Guide = _oDoor;

            MvpControl.AddPassiveElement(_oDriveLeft);
            MvpControl.AddPassiveElement(_oDriveRight);
            //MvpControl.AddActiveElement(_oTail);
            MvpControl.AddActiveElement(_oDoor);


            // trans.resetModelTransformation(groupDriveVisual);

            //Dem MainGrid den focus übergeben
            MvpControl.FocusToViewport(sender, e);

            ////Kamera für Main Viewport updaten
            MvpControl.ViewportCam.updatePositionCamera();   
        }

        //Button Listener für das Löschen der ListBox
        private void clear_listBox1_Button_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            AxisPoints.Clear();
            create_Button.IsEnabled = false;
        }

        //Löscht das 3D Model 
        private void clear_Button_Click(object sender, RoutedEventArgs e)
        {
            MvpControl.resetModelTransformation();
            MvpControl.clearModel();
            
            slider_open_ObjectAngle.Value = 0.0;
        }

        //Listener für Öffnungswinkel SLIDER
        //Ändert den Öffnungswinkel des Objektes anhand des Sliders
        private void change_open_ObjectAngle(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MvpControl.Move(e.NewValue/100);

            //Anzeige des Öffnungswinkel in TextBlock in [°]
           // OpenAngleDegree.Text = Math.Round(_oTail.CurValue, 2).ToString();
            OpenAngleDegree.Text = Math.Round(_oDoor.CurValue, 2).ToString();
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
            /*
            //Benutzereingaben der Koordinaten zwischen denen eine 3D Linie erzeugt wird
            //Punkt Drehachse
            double x1, y1, z1;

            //Punkt für Haltegriff ( Handangriffspunkt)
            double x2, y2, z2;

            //Fester Punkt für Antrieb an Karrosserie
            double x3, y3, z3;

            //Punkt für Antrieb an Heckklappe
            double x4, y4, z4;
            */

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

                x1 = 0;
                y1 = 0;
                z1 = 0;

                x2 = 500;
                y2 = -850;
                z2 = 0;

                x3 = 44.69;
                y3 = -129.16;
                z3 = 550.57;

                x4 = 393.56;
                y4 = -448.26;
                z4 = 626.20;


                Point3D p1 = new Point3D(x1, y1, z1);
                Point3D p2 = new Point3D(x2, y2, z2);
                Point3D p3 = new Point3D(x3, y3, z3);
                Point3D p4 = new Point3D(x4, y4, z4);

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
            listBox1.Items.Add("Objekt: " + "\n"
                                + "P1(" + x1 + ", "
                                        + y1 + ", "
                                        + z1 + ")  \n"
                                + "P2(" + x2 + ", "
                                        + y2 + ", "
                                        + z2 + ") \n"
                                + "Elementbreite in mm: " + ModelThickness + "\n"
                                + "\n"
                                + "Antrieb1: " + "\n"
                                + "P3(" + x3 + ", "
                                        + y3 + ", "
                                        + z3 + ") \n"
                                + "P4(" + x4 + ", "
                                        + y4 + ", "
                                        + z4 + ") \n"
                                + "Antrieb2: " + "\n"
                                + "P5(" + AxisPoints[4].X.ToString() + ", " 
                                        + AxisPoints[4].Y.ToString() + ", "
                                        + AxisPoints[4].Z.ToString() + ") \n"
                                + "P6(" + AxisPoints[5].X.ToString() + ", "
                                        + AxisPoints[5].Y.ToString() + ", "
                                        + AxisPoints[5].Z.ToString() + ") \n");

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
            Vector3D vR = new Vector3D(AxisPoints[0].X, AxisPoints[0].Y, AxisPoints[0].Z);
            Vector3D vAxisToHandE1 = AxisPoints[1] - AxisPoints[0];
            Vector3D vE2 = Vector3D.CrossProduct(vAxisToHandE1, new Vector3D(0,1,0));

            double d = Vector3D.DotProduct(vR, vE2);

            Point3D p1 = TransformationUtilities.reflectPoint(AxisPoints[2], vR, vE2, d);
            Point3D p2 = TransformationUtilities.reflectPoint(AxisPoints[3], vR, vE2, d);

            AxisPoints.Add(p1);
            AxisPoints.Add(p2);
        }

    }
}
