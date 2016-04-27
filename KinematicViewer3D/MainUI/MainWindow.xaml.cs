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
        private List<Point3D> coordPoints;

        //Model- Dicke (Durchmesser der Linien)
        private double modelThickness;

        private MainViewPortControl mvpControl;
        private CoordSystemSmall cssControl;

        public MainWindow()
        {
            InitializeComponent();
            coordPoints = new List<Point3D>();
            mvpControl = new MainViewPortControl();
            cssControl = new CoordSystemSmall();
            MainUIViewport3D.Content = mvpControl;
            MainUICoordSystemSmall.Content = cssControl; 
            create_Button.IsEnabled = false;
        }

        //MENUBAR Elemente
        //Start einer perspektivischen Kamera
        private void perspective_Camera_Click(object sender, RoutedEventArgs e)
        {
            mvpControl.switchToPerspective();
        }

        //Start einer orthographischen Kamera
        private void orthographic_Camera_Click(object sender, RoutedEventArgs e)
        {
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
            mvpControl.createCube();
        }

        //Button Listener für das Löschen der ListBox
        private void clear_listBox1_Button_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            coordPoints.Clear();
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

        }


        //Koordinateneingabe in 3D Punkte umwandeln
        private void convertUserInput()
        {
            //Benutzereingaben der Koordinaten zwischen denen eine 3D Linie erzeugt wird
            //Punkt Drehachse
            double x1, y1, z1;

            //Punkt für Haltegriff ( Handangriffspunkt)
            double x2, y2, z2;
            try
            {
                // Koordinaten der Drehachse
                x1 = Convert.ToDouble(value_X1.Text);
                y1 = Convert.ToDouble(value_Y1.Text);
                z1 = Convert.ToDouble(value_Z1.Text);

                //Koordinaten des Handangriffspunktes
                x2 = Convert.ToDouble(value_X2.Text);
                y2 = Convert.ToDouble(value_Y2.Text);
                z2 = Convert.ToDouble(value_Z2.Text);

                Point3D p1 = new Point3D(x1, y1, z1);
                Point3D p2 = new Point3D(x2, y2, z2);

                coordPoints.Add(p1);
                coordPoints.Add(p2);
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
            CheckBox checkBox1 = new CheckBox();
            checkBox1.Focusable = true;
            checkBox1.Content = "Objekt: " + "\n"
                                + "P1(" + value_X1.Text + ", "
                                        + value_Y1.Text + ", "
                                        + value_Z1.Text + ")  "
                                + "P2(" + value_X2.Text + ", "
                                        + value_Y2.Text + ", "
                                        + value_Z2.Text + ") \n"
                                        + "Element Dicke: " + modelThickness + "\n";
            listBox1.Items.Add(checkBox1);

        }
    }
}
