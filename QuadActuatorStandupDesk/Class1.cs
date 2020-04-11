using Rapidnack.Net;
using System;
using System.Threading;

namespace QuadActuatorStandupDesk
{
    public class Class1
    {
        public static void Testing()
        {
            var pigpiodIf = new PigpiodIf();
            pigpiodIf.pigpio_start("192.168.1.10", "8888");
            const int GPIO = 5;

            while (true)
            {
                pigpiodIf.gpio_write(GPIO, PigpiodIf.PI_HIGH);
                Thread.Sleep(500);
                pigpiodIf.gpio_write(GPIO, PigpiodIf.PI_LOW);
                Thread.Sleep(500);
            }

        }
    }
}
