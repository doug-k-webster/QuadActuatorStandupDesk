namespace QuadActuatorStandupDesk
{
    using System;
    using System.Net.Sockets;

    public class TcpConnection : IDisposable
    {
        public delegate void ConnectionEstablishedEventHandler(System.Net.IPEndPoint endPoint);

        private NetworkStream _stream = null;

        bool disposed = false;

        private string ipOrHost;

        private int port;

        private TcpClient tcp = null;

        public bool IsOpened => tcp != null;

        public NetworkStream Stream
        {
            get => _stream;
            set
            {
                _stream = value;
                StreamChanged?.Invoke(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event ConnectionEstablishedEventHandler ConnectionEstablished;

        public event EventHandler StreamChanged;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Release managed objects
                Close(null);
            }

            // Release unmanaged objects

            disposed = true;
        }

        ~TcpConnection()
        {
            Dispose(false);
        }

        public bool Open(string ipOrHost, int port, IProgress<Log> progress)
        {
            if (tcp == null)
            {
                try
                {
                    this.ipOrHost = ipOrHost;
                    this.port = port;

                    tcp = new TcpClient();
                    tcp.BeginConnect(ipOrHost, port, new AsyncCallback(NetConnectCallback), null);

                    progress?.Report(Log.Info($"Connecting to {ipOrHost}:{port}..."));
                }
                catch (Exception ex)
                {
                    progress?.Report(Log.Error($"Connection failed({ex.Message})."));
                    Close(progress);
                    return false;
                }
            }

            return true;
        }

        public void Close(IProgress<Log> progress)
        {
            if (Stream != null)
            {
                // Execute handlers of StreamChanged event, and call Dispose()
                var stream = Stream;
                Stream = null;
                stream.Dispose();
            }

            if (tcp != null)
            {
                tcp.Close();
                tcp = null;

                progress?.Report(Log.Info($"{ipOrHost}:{port} was disconnected."));
            }

            ipOrHost = string.Empty;
            port = 0;
        }

        private void NetConnectCallback(IAsyncResult result)
        {
            if (tcp == null)
                return;

            if (tcp.Connected)
            {
                ConnectionEstablished?.Invoke((System.Net.IPEndPoint) tcp.Client.RemoteEndPoint);

                Console.WriteLine(
                    "Connected to LAN {0}:{1}.",
                    ((System.Net.IPEndPoint) tcp.Client.RemoteEndPoint).Address,
                    ((System.Net.IPEndPoint) tcp.Client.RemoteEndPoint).Port);

                var stream = tcp.GetStream();
                stream.ReadTimeout = 10000;
                stream.WriteTimeout = 10000;
                Stream = stream;
            }
        }
    }
}