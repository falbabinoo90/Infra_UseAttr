using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataModel;
//using Utilities;
using Interfaces;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Session createdSession = new Session("C:\\Users\\bfall\\Desktop\\MySession")
            {
                Distance = 10,
                TxtFile = "C:\\Users\\bfall\\Desktop\\MySession\\MyFile_Iwanna_import.txt"
            };

            //Session_2 createdSession = new Session_2("C:\\Users\\bfall\\Desktop\\MySession")
            //{
            //    Temperature = 37,
            //    Surface = 25,
            //    TxtFile = "C:\\Users\\bfall\\Desktop\\MySession\\MyFile_Iwanna_import.txt"
            //};

            InitializeUI(createdSession as IPAttributes);
        }


         double d1 = 20;
         double d2 = 45;
         double d3 = 0;
         double d4 = 0;
           

        private void InitializeUI(IPAttributes iPAttributes)
        {
            
            foreach (var PA in iPAttributes.GetAttributes(typeof(PAttribute)))
            {
                switch (PA)
                {
                    case PParameterAttribute PPA:

                        Label label = new Label();
                        label.Content = PPA.AttrID;
                        label.HorizontalAlignment = HorizontalAlignment.Left;
                        label.VerticalAlignment = VerticalAlignment.Top;
                        label.Margin = new Thickness(d1, d2, d3, d4);
                        
                        Grid1.Children.Add(label);

                        TextBox textBox = new TextBox();
                        textBox.HorizontalAlignment = HorizontalAlignment.Left;
                        textBox.VerticalAlignment = VerticalAlignment.Top;
                        textBox.Margin = new Thickness(d1 + 100, d2, d3, d4);

                        string displayValue = ""; 
                        PPA.GetValueAsStringForDisplay(iPAttributes, ref displayValue);
                        textBox.Text = displayValue;

                        Grid1.Children.Add(textBox);

                        d2 += 50;
                        break;

                    case PFileAttribute PFA:
                        
                        MyFileBrowserControl myFileBrowserControl = new MyFileBrowserControl(PFA, iPAttributes);

                        myFileBrowserControl.HorizontalAlignment = HorizontalAlignment.Left;
                        myFileBrowserControl.VerticalAlignment = VerticalAlignment.Top;
                        myFileBrowserControl.Margin = new Thickness(d1, d2, d3, d4);

                        Grid1.Children.Add(myFileBrowserControl);

                        d2 += 50;
                        break;

  
                    default:
                        break; 

                }
            }

            this.UpdateLayout();
        }

        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
        }
    }
}
