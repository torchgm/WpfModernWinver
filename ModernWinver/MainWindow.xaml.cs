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
using ModernWpf.Controls;

namespace ModernWinver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        AboutPage LoadedAboutPage;
        SystemPage LoadedSystemPage;
        AdvancedPage LoadedAdvancedPage;
        public MainWindow()
        {

            LoadedAboutPage = new AboutPage();
            LoadedSystemPage = new SystemPage();
            LoadedAdvancedPage = new AdvancedPage();
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            // Set the initial SelectedItem
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "Page_Settings")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(LoadedAboutPage);
        }
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Process.Start("ms-settings:");
                Close();
            }
            else
            {
                if (args.InvokedItem is TextBlock ItemContent)
                {
                    switch (ItemContent.Tag)
                    {
                        case "AboutPage":
                            ContentFrame.Navigate(LoadedAboutPage);
                            break;

                        case "SystemPage":
                            ContentFrame.Navigate(LoadedSystemPage);
                            break;

                        case "AdvancedPage":
                            ContentFrame.Navigate(LoadedAdvancedPage);

                            break;
                    }
                }
            }
        }

        public string ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                ProcessStartInfo procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                Process proc = new Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                return result;
            }
            catch (Exception e)
            {
                // Log the exception
                return e.ToString();
            }
        }
    }
}