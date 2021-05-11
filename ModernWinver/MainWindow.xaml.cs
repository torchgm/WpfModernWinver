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
using Microsoft.VisualBasic.Devices;
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
        ThemePage LoadedThemePage;
        AdvancedPage LoadedAdvancedPage;
        public Values vals = new Values();
        DateTime current = DateTime.Now;
        public string ValuesPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "winver.json");
        string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "winver.log");
        bool UpToDate = true;
        string m = "";
        public void addLog(string message)
        {
            m = $"[{DateTime.Now}] {message}\n";
            File.AppendAllText(filePath, m);
        }

        public MainWindow()
        {
            File.Create(filePath).Close();
            addLog("Started!");
            if (!File.Exists(ValuesPath))
            {
                File.Create(ValuesPath).Close();
                UpToDate = false;
                addLog("Created winver.json");
            }
            else
            {
                addLog("winver.json already exists");
                string JsonList = File.ReadAllText(ValuesPath);
                
                if (JsonList == "")
                {
                    UpToDate = false;

                }
                else
                {
                    vals = JsonConvert.DeserializeObject<Values>(JsonList);
                    double check = current.DayOfYear / 7;
                    int checkInt = Convert.ToInt32(Math.Round(check));
                    if (vals.WeekOfYear != checkInt || vals.WeekOfYear == -1 || vals.RAMSpeed == null)
                    {
                        UpToDate = false;
                    }
                }
            }

            // ////////////////////////////// //
            // Gets new values for everything //
            // ////////////////////////////// //
            if (UpToDate == false)
            {
                buttonRefresh_Click(null, null);
                addLog("Initial loading complete");
            }
            else
            {
                addLog("Creating pages");
                LoadedAboutPage = new AboutPage();
                LoadedSystemPage = new SystemPage();
                LoadedThemePage = new ThemePage();
                LoadedAdvancedPage = new AdvancedPage();
                addLog("Page creation complete");
            }
            
            InitializeComponent();
            addLog("Initialised MAIN");
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            addLog("Closing");
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
            addLog("NavView loaded successfully");
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // Unused atm because NavigationView hates me
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

                        case "ThemePage":
                            ContentFrame.Navigate(LoadedThemePage);
                            break;

                        case "AdvancedPage":
                            ContentFrame.Navigate(LoadedAdvancedPage);

                            break;
                    }
                }
            }
        }

        public struct Values
        {
            public int WeekOfYear;
            public string CopyrightYear;
            public string Edition;
            public string Version;
            public string Build;
            public string User;
            public bool IsLocal;
            public string SystemName;
            public string Workgroup;
            
            public string CPU;
            public string Arch;
            public double RAM;
            public string RAMType;
            public string RAMSpeed;
            public string Path;
            public double Storage;
            public double FreeSpace;

        }

        static int GetTotalMemoryInGibibytes()
        {
            return Convert.ToInt32(new ComputerInfo().TotalPhysicalMemory / 1073741824) + 1;
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

        private long GetTotalFreeSpace()
        {
            long tfs = 0;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    tfs = tfs + drive.TotalFreeSpace / 1073741824;
                }
            }
            return tfs;
        }

        private long GetTotalSpace()
        {
            long tfs = 0;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    tfs = tfs + drive.TotalSize / 1073741824;
                }
            }
            return tfs;
        }

        // Magically absorbs different types of RAM
        public static string RamType()
        {
                int type = 0;

                ConnectionOptions connection = new ConnectionOptions();
                connection.Impersonation = ImpersonationLevel.Impersonate;
                ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
                scope.Connect();
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    type = Convert.ToInt32(queryObj["MemoryType"]);
                }

                return TypeString(type);
        }

        // Details different types of RAM
        private static string TypeString(int type)
        {
            string outValue;
            switch (type)
            {
                case 0x0: outValue = "Probably DDR4"; break;
                case 0x1: outValue = "Other"; break;
                case 0x2: outValue = "DRAM"; break;
                case 0x3: outValue = "Synchronous DRAM"; break;
                case 0x4: outValue = "Cache DRAM"; break;
                case 0x5: outValue = "EDO"; break;
                case 0x6: outValue = "EDRAM"; break;
                case 0x7: outValue = "VRAM"; break;
                case 0x8: outValue = "SRAM"; break;
                case 0x9: outValue = "RAM"; break;
                case 0xa: outValue = "ROM"; break;
                case 0xb: outValue = "Flash"; break;
                case 0xc: outValue = "EEPROM"; break;
                case 0xd: outValue = "FEPROM"; break;
                case 0xe: outValue = "EPROM"; break;
                case 0xf: outValue = "CDRAM"; break;
                case 0x10: outValue = "3DRAM"; break;
                case 0x11: outValue = "SDRAM"; break;
                case 0x12: outValue = "SGRAM"; break;
                case 0x13: outValue = "RDRAM"; break;
                case 0x14: outValue = "DDR"; break;
                case 0x15: outValue = "DDR2"; break;
                case 0x16: outValue = "DDR2 FB-DIMM"; break;
                case 0x17: outValue = "Undefined 23"; break;
                case 0x18: outValue = "DDR3"; break;
                case 0x19: outValue = "FBD2"; break;
                case 0x1a: outValue = "DDR4"; break;
                default: outValue = "Undefined"; break;
            }

            return outValue;
        }

        public static string SysInfo(string loc, string request)
        {
            string info = "";

            ConnectionOptions connection = new ConnectionOptions();
            connection.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope scope = new ManagementScope("\\\\.\\root\\CIMV2", connection);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM " + loc);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (queryObj[request] != null)
                {
                    info = queryObj[request].ToString();
                }
                else
                {
                    info = "[Unknown]";
                }
            }
            return info;
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            addLog("Refreshing!");
            //refreshProgress.IsActive = true;
            RegistryKey CurrentVersionKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            RegistryKey CentralProcessorKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            addLog("RegKeys open");

            // Easy stuff
            vals.WeekOfYear = current.DayOfYear / 7;
            vals.CopyrightYear = current.Year.ToString();
            vals.Version = (string)CurrentVersionKey.GetValue("DisplayVersion");
            if (vals.Version == "")
            {
                vals.Version = (string)CurrentVersionKey.GetValue("ReleaseId");
                if (vals.Version == "2009")
                {
                    vals.Version = "20H2";
                }
            }
            addLog("ES set");

            vals.Build = (string)CurrentVersionKey.GetValue("CurrentBuild") + "." + CurrentVersionKey.GetValue("UBR").ToString();
            vals.User = (string)CurrentVersionKey.GetValue("RegisteredOwner");
            vals.SystemName = ExecuteCommandSync("hostname").Replace("\r\n", "");
            vals.Workgroup = (string)CurrentVersionKey.GetValue("RegisteredOrganization");
            vals.CPU = (string)CentralProcessorKey.GetValue("ProcessorNameString");
            vals.Arch = SysInfo("Win32_Processor", "Architecture");
            vals.RAM = GetTotalMemoryInGibibytes();
            vals.RAMType = RamType();
            vals.RAMSpeed = SysInfo("Win32_PhysicalMemory", "Speed");
            vals.Path = @"C:\Windows";  //(string)CurrentVersionKey.GetValue("PathName"); TODO: find a proper way to get this because im lazy
            vals.FreeSpace = GetTotalFreeSpace();
            vals.Storage = GetTotalSpace();
            addLog("CS set");

            switch (SysInfo("Win32_Processor", "Architecture"))
            {
                case null:
                    vals.Arch = "Real"; break;
                case "0":
                    vals.Arch = "x86"; break;
                case "1":
                    vals.Arch = "MIPS"; break;
                case "2":
                    vals.Arch = "Alpha"; break;
                case "3":
                    vals.Arch = "PowerPC"; break;
                case "5":
                    vals.Arch = "ARM"; break;
                case "6":
                    vals.Arch = "ia64"; break;
                case "9":
                    vals.Arch = "AMD64"; break;
                case "12":
                    vals.Arch = "ARM64"; break;
                default:
                    vals.Arch = "Unknown"; break;
            }

            addLog("Arch set");


            // Get edition of Windows 10 because apparently that's bloody impossible any other way and the registry returns me wrong values
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    vals.Edition = ((string)queryObj["Caption"]).Replace("Microsoft ", "");
                    if (vals.Edition.Contains("Insider Preview") && vals.Version == "")
                    {
                        vals.Version = "vNext";
                    }
                    vals.Edition = vals.Edition.Replace("Insider Preview", "");

                }
                addLog("WMI success");
            }
            catch (ManagementException ex)
            {
                addLog("WMI failure");
                MessageBox.Show("An error occurred while querying for WMI data: " + ex.Message);
            }

            // This just prevents you from having a blank username
            if (vals.User == "" || vals.User == "user name")
            {
                vals.User = "(Unknown user)";
            }
            addLog("UN set");

            // If in org, show org name, else show hostname
            if (vals.Workgroup == "" || vals.Workgroup == "org name")
            {
                vals.IsLocal = true;
            }
            else
            {
                vals.IsLocal = false;
            }
            addLog("WG set");

            // Writes files
            File.Create(ValuesPath).Close();
            File.WriteAllText(ValuesPath, JsonConvert.SerializeObject(vals, Formatting.Indented));
            addLog("winver.json written");

            // Reloads pages
            LoadedAboutPage = new AboutPage();
            LoadedSystemPage = new SystemPage();
            LoadedThemePage = new ThemePage();
            LoadedAdvancedPage = new AdvancedPage();
            if (ContentFrame != null)
            {
                ContentFrame.Navigate(LoadedAboutPage);
                NavView.SelectedItem = NavView.MenuItems[0];
            }
            addLog("Pages regenerated");
            //refreshProgress.IsActive = false;
        }

        private void WindowsLogo_Click(object sender, MouseButtonEventArgs e)
        {

            if (System.Windows.Input.Keyboard.IsKeyDown(Key.F1))
            {
                if (DebugTag.Visibility == Visibility.Hidden)
                {
                    DebugTag.Visibility = Visibility.Visible;
                }
                else
                {
                    DebugTag.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
