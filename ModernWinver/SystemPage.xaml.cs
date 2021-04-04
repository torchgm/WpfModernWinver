using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.ComponentModel;

namespace ModernWinver
{
    /// <summary>
    /// Interaction logic for SystemPage.xaml
    /// </summary>
    public partial class SystemPage : Page
    {
        public static MainWindow mw = (MainWindow)Application.Current.MainWindow;
        public SystemPage()
        {
            // RAM Background Worker
            BackgroundWorker bwRAM = new BackgroundWorker();
            bwRAM.WorkerReportsProgress = true;
            bwRAM.DoWork += BwRam_DoWork;
            bwRAM.ProgressChanged += BwRam_ProgressChanged;
            bwRAM.RunWorkerAsync();
            mw.addLog("RAM worker started");

            // CPU Background Worker
            BackgroundWorker bwCPU = new BackgroundWorker();
            bwCPU.WorkerReportsProgress = true;
            bwCPU.DoWork += BwCpu_DoWork;
            bwCPU.ProgressChanged += BwCpu_ProgressChanged;
            bwCPU.RunWorkerAsync();
            mw.addLog("CPU worker started");

            InitializeComponent();
            valueSystemName.Content = mw.vals.SystemName;
            valueCPU.Content = mw.vals.CPU;
            valueArch.Content = mw.vals.Arch;
            mw.addLog("Added some CPU details or something");

            valueRAM.Content = Math.Round(mw.vals.RAM / GetUnits(mw.vals.RAM).Value, 2).ToString() + $" {GetUnits(mw.vals.RAM).Key} " + mw.vals.RAMType + " @ " + mw.vals.RAMSpeed.ToString() + " MT/s";
            valueMaxRAM.Content = $"{mw.vals.RAM} {GetUnits(mw.vals.RAM).Key}";
            valueZeroRAM.Content = $"0 {GetUnits(mw.vals.RAM).Key}";
            mw.addLog("More stuff idk what probably RAM");

            valuePath.Content = mw.vals.Path;
            mw.addLog("oh amazing a path >:c");

            valueFreeStorage.Content = Math.Round(mw.vals.FreeSpace/GetUnits(mw.vals.FreeSpace).Value, 2).ToString() + $" {GetUnits(mw.vals.FreeSpace).Key} free";
            valueStorage.Content = Math.Round((mw.vals.Storage - mw.vals.FreeSpace) / GetUnits(mw.vals.Storage - mw.vals.FreeSpace).Value, 2).ToString() + $" {GetUnits(mw.vals.Storage - mw.vals.FreeSpace).Key} used";
            valueTotalStorage.Content = Math.Round(mw.vals.Storage / GetUnits(mw.vals.Storage).Value, 2).ToString() + $" {GetUnits(mw.vals.Storage).Key}";
            mw.addLog("ugh storage too why bother");

            double storagePercentage = (mw.vals.FreeSpace / mw.vals.Storage) * 100;
            progressStorage.Value = 100 - ((storagePercentage > 100) ? 100 : storagePercentage);
            mw.addLog("oh shut up nobody cares about the percentage of storage they've used");
        }



        private void BwCpu_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressCPU.Value = e.ProgressPercentage;
        }

        private void BwCpu_DoWork(object sender, DoWorkEventArgs e)
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            BackgroundWorker bw = sender as BackgroundWorker;
            while (true)
            {
                dynamic val = cpuCounter.NextValue();
                bw.ReportProgress((int)val);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void BwRam_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressRAM.Value = e.ProgressPercentage;
        }

        private void BwRam_DoWork(object sender, DoWorkEventArgs e)
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            BackgroundWorker bw = sender as BackgroundWorker;
            int totalRAM = Convert.ToInt32(mw.vals.RAM);
            while (true)
            {
                dynamic val = ramCounter.NextValue();
                bw.ReportProgress(100 - (((int)val * 100) / (totalRAM * 1024)));
                System.Threading.Thread.Sleep(1000);
            }
        }

        public KeyValuePair<string, double> GetUnits(double valueInGB)
        {
            if (valueInGB / 1024 > 1)
            {
                if ((valueInGB / 1024) / 1024 > 1)
                {
                    return new KeyValuePair<string, double>("PB", 1048576);
                }
                return new KeyValuePair<string, double>("TB", 1024);
            }
            if (valueInGB <= 1)
            {
                return new KeyValuePair<string, double>("MB", 1/1024);
            }
            mw.addLog("amazing i got some units");
            return new KeyValuePair<string, double>("GB", 1);
        }
    }
}
