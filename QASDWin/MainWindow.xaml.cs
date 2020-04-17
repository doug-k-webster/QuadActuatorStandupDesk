namespace QASDWin
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using QuadActuatorStandupDesk;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DeskClient desk = new DeskClient();

        private readonly Progress<Log> progress;

        public MainWindow()
        {
            this.LogEntries = new ObservableCollection<Log>();
            progress = new Progress<Log>(
                (log) =>
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

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void Window_Loaded(object sender, RoutedEventArgs e) => this.desk.Initialize(this.progress);

        private void AllUpButton_Click(object sender, RoutedEventArgs e) => this.desk.Up(this.progress);

        private void AllDownButton_Click(object sender, RoutedEventArgs e) => this.desk.Down(this.progress);

        private void AllStopButton_Click(object sender, RoutedEventArgs e) => this.desk.Stop(this.progress);

        private void FrontLeftUp_Click(object sender, RoutedEventArgs e) => this.desk.FrontLeftActuatorExtend(this.progress);

        private void FrontLeftDown_Click(object sender, RoutedEventArgs e) => this.desk.FrontLeftActuatorRetract(this.progress);

        private void BackLeftUp_Click(object sender, RoutedEventArgs e) => this.desk.BackLeftActuatorExtend(this.progress);

        private void BackLeftDown_Click(object sender, RoutedEventArgs e) => this.desk.BackLeftActuatorRetract(this.progress);

        private void FrontRightUp_Click(object sender, RoutedEventArgs e) => this.desk.FrontRightActuatorExtend(this.progress);

        private void FrontRightDown_Click(object sender, RoutedEventArgs e) => this.desk.FrontRightActuatorRetract(this.progress);

        private void BackRightUp_Click(object sender, RoutedEventArgs e) => this.desk.BackRightActuatorExtend(this.progress);

        private void BackRightDown_Click(object sender, RoutedEventArgs e) => this.desk.BackRightActuatorRetract(this.progress);

        private void LogInfo(string text) => ((IProgress<Log>) this.progress).Report(Log.Info(text));

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            var commandTextBox = ((TextBox) sender);
            var commandText = commandTextBox.Text;
            commandTextBox.Text = string.Empty;

            this.desk.ExecuteCommand(commandText, this.progress);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.desk.Stop(this.progress);
        }
    }
}