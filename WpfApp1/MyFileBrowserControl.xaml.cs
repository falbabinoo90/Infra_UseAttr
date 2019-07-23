using DataModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MyFileBrowserControl.xaml
    /// </summary>
    public partial class MyFileBrowserControl : UserControl
    {
        PFileAttribute _FileAttribute;
        IPAttributes _Attributes;
        public MyFileBrowserControl(PFileAttribute i_pFileAttribute, IPAttributes i_Attributes)
        {
            _FileAttribute = i_pFileAttribute;
            _Attributes = i_Attributes;
            if (_FileAttribute != null)
            {
                
                InitializeComponent();

                label.Content = _FileAttribute.AttrID;
                string displayText = "";
                _FileAttribute.GetValueAsStringForDisplay(_Attributes, ref displayText);
                textBox.Text = displayText;

                UpdateLayout();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = _FileAttribute.Filter;

            // Show open file dialog box
            Nullable<bool> result = fileDialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = fileDialog.FileName;
                textBox.Text = filename;
                _FileAttribute.SetValueFromStringDisplay(_Attributes, filename);
            }
        }
        
    }
}
