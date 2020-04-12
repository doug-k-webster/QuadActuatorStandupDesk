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

            var delay = 100;

            while (true)
            {
                pigpiodIf.gpio_write(26, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(13, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(21, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(27, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(22, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(19, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(20, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(12, PigpiodIf.PI_HIGH);
                Thread.Sleep(delay);               
                pigpiodIf.gpio_write(26, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(13, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(21, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(27, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(22, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(19, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(20, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
                pigpiodIf.gpio_write(12, PigpiodIf.PI_LOW);
                Thread.Sleep(delay);
            }
        }
    }
}
