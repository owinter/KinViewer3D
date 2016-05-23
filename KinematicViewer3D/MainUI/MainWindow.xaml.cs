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
        private List<Point3D> axisPoints;

        //Model- Dicke (Durchmesser der Linien)
        private double modelThickness;

        private MainViewPortControl mvpControl;
        private CoordSystemSmall cssControl;


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
            axisPoints = new List<Point3D>();
            mvpControl = new MainViewPortControl();
            mvpControl.setTextBlock(statusPane);
            MainUIViewport3D.Content = mvpControl;
            cssControl = new CoordSystemSmall(); 
            MainUICoordSystemSmall.Content = cssControl; 

            create_Button.IsEnabled = false;
        }


        //MENUBAR Elemente
        //Start einer perspektivischen Kamera
        private void perspective_Camera_Click(object sender, RoutedEventArgs e)
        {
            miOrthographic.IsChecked = false;
            miPerspective.IsChecked = true;
            mvpControl.switchToPerspective();
        }

        //Start einer orthographischen Kamera
        private void orthographic_Camera_Click(object sender, RoutedEventArgs e)
        {
            miPerspective.IsChecked = false;
            miOrthographic.IsChecked = true;
            mvpControl.switchToOrthographic();
        }

        //Objekt und Kamera neu laden
        private void reload_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.reloadCameraPositionDefault();
        }

        //Menubar Reset Transformation
        private void reset_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.resetCam();
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
            this.modelThickness = slider_Model_Thickness.Value;
        }

        //Fügt 3D Koordinaten der Liste coordPoints und der TextBox hinzu
        private void add_Button_Click(object sender, RoutedEventArgs e)
        {
            convertUserInput();
            fill_TextBox();
            create_Button.IsEnabled = true;
        }

        //Schreibt die beiden Punkte und die "Thickness" in die Listbox
        private void create_Button_Click(object sender, RoutedEventArgs e)
        {
            //Dem MainGrid den focus übergeben
            //MainGrid.Focus();
            //mvpControl.createCube();
            mvpControl.AxisPoints = axisPoints;
            mvpControl.ModelThickness = modelThickness;
            mvpControl.createModel();
            slider_open_ObjectAngle.Value = 0.0;
        }

        //Button Listener für das Löschen der ListBox
        private void clear_listBox1_Button_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            axisPoints.Clear();
            create_Button.IsEnabled = false;
        }

        //Löscht das 3D Model 
        private void clear_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.clearModel();
        }

        //Listener für Öffnungswinkel SLIDER
        //Ändert den Öffnungswinkel des Objektes anhand des Sliders
        private void change_open_ObjectAngle(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mvpControl.sliderRotate(sender, e);
        }

        //Objekttransformation zurücksetzen
        private void reset_Model_Transformation_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.resetModelTransformation(sender, e);
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

                axisPoints.Add(p1);
                axisPoints.Add(p2);
                axisPoints.Add(p3);
                axisPoints.Add(p4);
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
                                + "Elementbreite in mm: " + modelThickness + "\n"
                                + "\n"
                                + "Antrieb: " + "\n"
                                + "P3(" + x3 + ", "
                                        + y3 + ", "
                                        + z3 + ") \n"
                                + "P4(" + x4 + ", "
                                        + y4 + ", "
                                        + z4 + ") \n");

        }


        //ToolBar Buttons
        private void toolBox_Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.resetCam();
        }

        private void toolBox_ZoomIn_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.zoomIn();
            
        }

        private void toolBox_ZoomOut_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.zoomOut();
           
        }

        private void toolBox_Front_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.viewFrontSide();
        }

        private void toolBox_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.viewBackSide();
        }

        private void toolBox_Right_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.viewRightSide();
        }

        private void toolBox_Left_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.viewLeftSide();
        }

        private void toolBox_Top_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.viewTopSide();
        }

        private void toolBox_Bottom_Button_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.viewBottomSide();
        }

    }
}
