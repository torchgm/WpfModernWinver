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
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace ModernWinver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DateTime current = DateTime.Now;

            valueCopyright.Content = "© " + current.Year + " Microsoft Corporation. All rights reserved.";
            
            valueEdition.Content = ExecuteCommandSync("powershell \"Get-WmiObject -Class Win32_OperatingSystem | % Caption\"").Replace("Microsoft ", "");
            valueVersion.Content = ExecuteCommandSync("powershell \"Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion' | % ReleaseId\"");
            valueBuild.Content = (ExecuteCommandSync("powershell \"Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion' | % CurrentBuild\"") + "." + ExecuteCommandSync("powershell \"Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion' | % UBR\"")).Replace("\n", "").Replace("\r", "");
            valueUser.Content = ExecuteCommandSync("powershell \"Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion' | % RegisteredOwner\"");
            if (ExecuteCommandSync("powershell \"Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion' | % RegisteredOrganization\"").Replace("\n", "").Replace("\r", "") == "")
            {
                labelWorkgroup.Content = "Computer";
                valueWorkgroup.Content = ExecuteCommandSync("hostname");
            }
            else
            {
                valueWorkgroup.Content = ExecuteCommandSync("powershell \"Get-ItemProperty 'HKLM:\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion' | % RegisteredOrganization\"");
            }
            Show();
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.ToString());
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonLaunchSettings_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:about");
            Close();
        }
    }
}
