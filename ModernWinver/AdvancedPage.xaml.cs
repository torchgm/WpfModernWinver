using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModernWpf;

namespace ModernWinver
{
    /// <summary>
    /// Interaction logic for AdvancedPage.xaml
    /// </summary>
    public partial class AdvancedPage : Page
    {
        MainWindow mw = (MainWindow)Application.Current.MainWindow;

        public AdvancedPage()
        {
            InitializeComponent();
        }

        private void applyDebugButton_Click(object sender, RoutedEventArgs e)
        {
            if (debugToggle0.IsOn) // ForceThemeOverride
            {
                debugToggle0.IsEnabled = false;
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
            }

            if (debugToggle1.IsOn) // ForceColourOverride
            {
                debugToggle1.IsEnabled = false;
                ThemeManager.Current.AccentColor = Colors.Red;
            }

            if (debugToggle2.IsOn) // DestroyWinverJson
            {
                debugToggle2.IsEnabled = false;
                File.Delete(mw.ValuesPath);
            }

            if (debugToggle3.IsOn) // HangUIThread
            {
                debugToggle3.IsEnabled = false;

                while (true)
                {

                }
            }

            if (debugToggle4.IsOn) // ForceUnmanagedResize
            {
                debugToggle4.IsEnabled = false;
                mw.Width = 450;
                mw.Height = 450;
            }
        }
    }
}
