using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
namespace Android_Sensor_Test
{
    
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            AndroidSensorMonitor monitor = new AndroidSensorMonitor();
            AndroidSensorMonitor.StartADBForwarding(@"C:\Users\19VD5\Downloads\tools_r29.0.2-windows\adb", 65000, 65000);
            monitor.sensorDataCallback += Monitor_sensorDataCallback;
            monitor.StartMonitoring(65000);
            monitor.Wait();
        }

        private static void Monitor_sensorDataCallback(SensorData data)
        {
            Console.Write(data.type+", values:");
            for (int i = 0; i < data.values.Length; i++)
            {
                Console.Write(data.values[i]);
                if (i < data.values.Length - 1)
                    Console.Write(", ");
            }
            Console.WriteLine();
        }
    }
}
