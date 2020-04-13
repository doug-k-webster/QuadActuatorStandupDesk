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
        private readonly Desk desk = new Desk();

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
    }
}