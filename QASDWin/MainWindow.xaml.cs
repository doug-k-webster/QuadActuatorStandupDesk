using QuadActuatorStandupDesk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace QASDWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Progress<Log> progress;

        private readonly Desk desk = new Desk();

        public MainWindow()
        {
            this.LogEntries = new ObservableCollection<Log>();
            progress = new Progress<Log>((log) =>
            {
                this.LogEntries.Insert(0, log);
                int maxLogLength = 100;
                while (this.LogEntries.Count > maxLogLength)
                {
                    this.LogEntries.RemoveAt(maxLogLength);
                }
            });

            InitializeComponent();
           
            this.DataContext = this;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // 
                LogInfo("startup sequence initiated");
                LogInfo("pandemic mode enabled");
                LogInfo("connecting...");
            }
        }

        public ObservableCollection<Log> LogEntries { get; set; }


        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.desk.Initialize(this.progress);
        }

        private void AllUpButton_Click(object sender, RoutedEventArgs e)
        {
            this.desk.Up(this.progress);
        }

        private void AllDownButton_Click(object sender, RoutedEventArgs e)
        {
            this.desk.Down(this.progress);
        }

        private void AllStopButton_Click(object sender, RoutedEventArgs e)
        {
            this.desk.Stop(this.progress);
        }

        private void LogInfo(string text)
        {
            ((IProgress<Log>)this.progress).Report(QuadActuatorStandupDesk.Log.Info(text));
        }
    }
}
