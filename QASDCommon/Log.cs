namespace QuadActuatorStandupDesk
{
    using System;

    public class Log
    {
        public Log(string text, int logLevel = 0)
        {
            this.Text = text;
            this.DateTime = DateTime.Now;
        }

        public DateTime DateTime { get; }

        public string Text { get; }

        public int LogLevel { get; }

        public static Log Debug(string text) => new Log(text, 0);

        public static Log Info(string text) => new Log(text, 1);

        public static Log Warn(string text) => new Log(text, 2);

        public static Log Error(string text) => new Log(text, 3);

        public static Log Fatal(string text) => new Log(text, 5);
    }
}