namespace QASDWin
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microsoft.AspNetCore.SignalR.Client;

    using QASDCommon;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly HubConnection connection;

        private readonly DeskClient desk = new DeskClient();

        private readonly Progress<Log> progress;

        private float deskHeight;

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

            connection = new HubConnectionBuilder().WithUrl("http://192.168.1.10:9999/deskhub")
                .WithAutomaticReconnect()
                .Build();

            connection.Closed += async (error) =>
            {
                LogInfo("connection to signalr was lost, reconnecting to signalr hub...");
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
            connection.Reconnecting += error =>
            {
                LogInfo("connection to signalr was lost, reconnecting to signalr hub...");
                return Task.CompletedTask;
            };
            connection.Reconnected += connectionId =>
            {
                if (connection.State == HubConnectionState.Connected) ;
                {
                    LogInfo("reconnected to signalr.");
                }

                // Notify users the connection was reestablished.
                // Start dequeuing messages queued while reconnecting if any.

                return Task.CompletedTask;
            };
            connection.On<DeskStatus>(
                "ReceiveDeskStatus",
                (deskStatus) =>
                {
                    this.DeskHeight = deskStatus.Height;

                    // toto: set actuator heights
                });

            this.DataContext = this;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                // 
                LogInfo("startup sequence initiated");
                LogInfo("pandemic mode enabled");
                LogInfo("connecting...");
            }

            //FL
            this.FrontLeftUpButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(FrontLeftUp_Click), handledEventsToo: true);
            this.FrontLeftUpButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);
            this.FrontLeftDownButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(FrontLeftDown_Click), handledEventsToo: true);
            this.FrontLeftDownButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);

            //BL
            this.BackLeftUpButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(BackLeftUp_Click), handledEventsToo: true);
            this.BackLeftUpButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);
            this.BackLeftDownButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(BackLeftDown_Click), handledEventsToo: true);
            this.BackLeftDownButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);

            //BR
            this.BackRightUpButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(BackRightUp_Click), handledEventsToo: true);
            this.BackRightUpButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);
            this.BackRightDownButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(BackRightDown_Click), handledEventsToo: true);
            this.BackRightDownButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);

            //FR
            this.FrontRightUpButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(FrontRightUp_Click), handledEventsToo: true);
            this.FrontRightUpButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);
            this.FrontRightDownButton.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(FrontRightDown_Click), handledEventsToo: true);
            this.FrontRightDownButton.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Button_MouseLeftButtonUp), handledEventsToo: true);
        }

        private void MyMouseEventHandler(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public float DeskHeight
        {
            get => deskHeight;
            set
            {
                deskHeight = value;
                OnPropertyChanged(nameof(DeskHeight));
            }
        }

        public ObservableCollection<Log> LogEntries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogInfo("connecting to signalr hub...");
            await ConnectWithRetryAsync(connection, CancellationToken.None);
            await this.desk.Initialize(this.progress);
        }

        private async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    LogInfo("Connected to signalr.");
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    LogInfo("Failed to connect, trying again in 5000 ms.");
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000);
                }
            }
        }

        private async void AllUpButton_Click(object sender, RoutedEventArgs e) => await this.desk.Up(this.progress);

        private async void AllDownButton_Click(object sender, RoutedEventArgs e) => await this.desk.Down(this.progress);

        private async void AllStopButton_Click(object sender, RoutedEventArgs e) => await this.desk.Stop(this.progress);

        private async void FrontLeftUp_Click(object sender, MouseButtonEventArgs e) => await this.desk.FrontLeftActuatorExtend(this.progress);

        private async void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => await this.desk.Stop(this.progress);

        private async void FrontLeftDown_Click(object sender, MouseButtonEventArgs e) => await this.desk.FrontLeftActuatorRetract(this.progress);

        private async void BackLeftUp_Click(object sender, MouseButtonEventArgs e) => await this.desk.BackLeftActuatorExtend(this.progress);

        private async void BackLeftDown_Click(object sender, MouseButtonEventArgs e) => await this.desk.BackLeftActuatorRetract(this.progress);

        private async void FrontRightUp_Click(object sender, MouseButtonEventArgs e) => await this.desk.FrontRightActuatorExtend(this.progress);

        private async void FrontRightDown_Click(object sender, MouseButtonEventArgs e) =>
            await this.desk.FrontRightActuatorRetract(this.progress);

        private async void BackRightUp_Click(object sender, MouseButtonEventArgs e) => await this.desk.BackRightActuatorExtend(this.progress);

        private async void BackRightDown_Click(object sender, MouseButtonEventArgs e) => await this.desk.BackRightActuatorRetract(this.progress);

        private void LogInfo(string text) => ((IProgress<Log>) this.progress).Report(Log.Info(text));

        private async void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                return;
            }

            var commandTextBox = ((TextBox) sender);
            var commandText = commandTextBox.Text;
            commandTextBox.Text = string.Empty;

            await this.desk.ExecuteCommand(commandText, this.progress);
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            await this.desk.Stop(this.progress);
            await this.connection.DisposeAsync();
        }
    }
}