using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Management;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using ModernWpf.Controls;
using ColorHelper;
using PInvoke;


namespace ModernWinver
{
    /// <summary>
    /// Interaction logic for ThemePage.xaml
    /// </summary>
    /// 
    
    public partial class ThemePage
    {
        public static MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public string wallpaperPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Microsoft\Windows\Themes\TranscodedWallpaper";
        public string wallpaperReal = "";

        public ThemePage()
        {
            try
            {
                mw.addLog("Starting init of theme page because apparently this breaks??");
                try
                {
                    mw.addLog("Trying to initialise page...");
                    InitializeComponent();
                    mw.addLog("Success!!");
                }
                catch (Exception aaaaaa)
                {
                    mw.addLog(aaaaaa.ToString());
                    throw;
                }
                try
                {
                    valueWallpaper.ImageSource = new BitmapImage(new Uri(wallpaperPath, UriKind.Absolute));
                }
                catch (Exception)
                {
                    mw.addLog("EVERYTHING IS ON FIRE AAAAAAAAAAAAAAAAAAAAAAA");
                }
                //SystemParametersInfo(0x0073, )
                updateSecondaryColourRect();
                mw.addLog("Initialised THEME");
            }
            catch (Exception AAAAAAAAAA)
            {
                mw.addLog(AAAAAAAAAA.ToString());
                
                throw;
            }
            
        }

        private void updateSecondaryColourRect()
        {
            try
            {
                RegistryKey colourKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors");
                string[] secondaryColour = ((string)colourKey.GetValue("Hilight")).Split(' ');
                colourKey.Close();
                RGB rgb = new RGB(Convert.ToByte(secondaryColour[0]), Convert.ToByte(secondaryColour[1]), Convert.ToByte(secondaryColour[2]));
                textBoxSecondaryAccent.Text = $"#{ColorHelper.ColorConverter.RgbToHex(rgb)}";
                secondaryRectColour.Color = Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
                mw.addLog("USCR complete");
            }
            catch (Exception aaaa)
            {
                mw.addLog(aaaa.ToString());
                throw;
            }
        }

        private void buttonUpdateSecondaryAccent_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Input.Keyboard.IsKeyDown(Key.LeftShift) || System.Windows.Input.Keyboard.IsKeyDown(Key.RightShift))
            {
                RegistryKey colourKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true);
                colourKey.SetValue("Hilight", "0 120 215");
                colourKey.SetValue("MenuHilight", $"0 120 215");
                colourKey.SetValue("HotTrackingColor", "0 102 204");
                colourKey.Close();
                updateSecondaryColourRect();
                //User32.SendMessage(User32.HWND_BROADCAST, User32.WindowMessage.WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);
                // This *should* make colours update automagically but instead it just hangs MWV
            }
            else
            {
                try
                {
                    RGB rgb = ColorHelper.ColorConverter.HexToRgb(new HEX(textBoxSecondaryAccent.Text.Replace("#", "")));
                    RegistryKey colourKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true);
                    colourKey.SetValue("Hilight", $"{rgb.R} {rgb.G} {rgb.B}");
                    colourKey.SetValue("MenuHilight", $"{rgb.R} {rgb.G} {rgb.B}");
                    colourKey.SetValue("HotTrackingColor", $"{rgb.R} {rgb.G} {rgb.B}");
                    colourKey.Close();
                    updateSecondaryColourRect();
                }
                catch (FormatException)
                {
                    updateSecondaryColourRect();
                }

                //User32.SendMessage(User32.HWND_BROADCAST, User32.WindowMessage.WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);
                // This *should* make colours update automagically but instead it just hangs MWV
            }

        }

        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWubUbu);

        private void buttonPersonalisation_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:personalization");
            mw.Close();
        }

        private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists($"{wallpaperPath}.jpg"))
            {
                File.Copy(wallpaperPath, $"{wallpaperPath}.jpg");
            }
            Process.Start($"{wallpaperPath}.jpg");
            mw.Close();
        }

        private void buttonUpdatePrimaryAccent_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("ms-settings:colors");
            mw.Close();
        }
    }
}
