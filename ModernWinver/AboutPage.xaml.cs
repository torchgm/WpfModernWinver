using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Management;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace ModernWinver
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : Page
    {
        MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public AboutPage()
        {
            InitializeComponent();

            // Actually sets all the labels
            valueCopyright.Content = "© " + mw.vals.CopyrightYear + " Microsoft Corporation. All rights reserved.";
            valueEdition.Content = mw.vals.Edition;
            valueVersion.Content = mw.vals.Version;
            valueBuild.Content = mw.vals.Build;
            valueUser.Content = mw.vals.User;
            if (mw.vals.IsLocal)
            {
                valueWorkgroup.Content = mw.vals.SystemName;
                labelWorkgroup.Content = "Computer";
            }
            else
            {
                valueWorkgroup.Content = mw.vals.Workgroup;
                labelWorkgroup.Content = "Workgroup";
            }
            mw.addLog("Initialised ABOUT");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void buttonLaunchSettings_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:about");
            mw.Close();
        }


    }
}
