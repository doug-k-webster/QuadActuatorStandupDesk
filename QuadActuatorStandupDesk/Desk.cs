using System;
using System.Collections.Generic;
using System.Text;

namespace QuadActuatorStandupDesk
{
    public class Desk
    {
        private readonly PigpiodIf pigpiodIf = new PigpiodIf();

        public void Initialize(IProgress<Log> progress)
        {
            pigpiodIf.pigpio_start("192.168.1.10", "8888", progress);
        }

        public void Up(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking all actuators to go up..."));
            pigpiodIf.gpio_write(26, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(13, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(21, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(27, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(22, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(19, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(20, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(12, PigpiodIf.PI_LOW);
            progress?.Report(Log.Debug("all actuators going up"));
        }

        public void Down(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("asking all actuators to go down..."));
            pigpiodIf.gpio_write(26, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(13, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(21, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(27, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(22, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(19, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(20, PigpiodIf.PI_LOW);
            pigpiodIf.gpio_write(12, PigpiodIf.PI_HIGH);
            progress?.Report(Log.Debug("all actuators going down"));
        }

        public void Stop(IProgress<Log> progress)
        {
            progress?.Report(Log.Debug("stopping all actuators..."));
            pigpiodIf.gpio_write(26, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(13, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(21, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(27, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(22, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(19, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(20, PigpiodIf.PI_HIGH);
            pigpiodIf.gpio_write(12, PigpiodIf.PI_HIGH);
            progress?.Report(Log.Debug("all actuators stopped"));
        }
    }
}
