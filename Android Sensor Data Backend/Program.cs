using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
namespace Android_Sensor_Test
{
    class SensorData
    {
        public string type;
        public long timestamp;
        public float[] values;
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            TcpClient clientSocket = new System.Net.Sockets.TcpClient();
            try
            {
                clientSocket.Connect("127.0.0.1", 65000);
            } catch (Exception e)
            {
                Console.WriteLine("Error happened when connecting to server: "+ e.Message);
                Console.WriteLine("Check that your server is up and that the adb forward command was executed properly");
                Console.ReadKey();
                return;
            }
            var stream = clientSocket.GetStream();
            var bytes = new byte[512];
            System.IO.StreamWriter json = new System.IO.StreamWriter(@"json.txt");
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"log.txt");

            var jsonString = "";
            var offset = 0;
            var start = -1;
            var end = -1;
            var firstLineReceived = false;
            var timer = new Stopwatch();
            var jsons = new List<String>();
            var difference = 0L;
            do
            {
                var len = stream.Read(bytes, 0, bytes.Length);
                var iter = offset;
                while (iter <  len)
                {
                    if (start == -1 && jsonString == "")
                    {
                        if (bytes[iter] == 0)
                            start = iter;
                    } else if (bytes[iter] == 1)
                    {
                        var curJsonString = (iter > start ? Encoding.ASCII.GetString(bytes, start + 1, iter - start - 1) : "");
                        if (jsonString != "")
                        {
                            curJsonString = jsonString + curJsonString;
                            jsonString = "";
                        }
                        //json.Write(curJsonString);
                        Console.WriteLine("JSON: " + curJsonString);
                        var jsonObj = JsonConvert.DeserializeObject<SensorData>(curJsonString);
                        if (jsonObj.type.Contains("Acc"))
                            Console.WriteLine($"{Math.Asin(jsonObj.values[2] / 9.9) * 180 / Math.PI} degree");
                        if (!firstLineReceived)
                        {
                            firstLineReceived = true;
                            difference = jsonObj.timestamp;
                            timer.Start();
                            json.WriteLine();
                        }
                        else
                        {
                            json.WriteLine($"  {(double)timer.ElapsedTicks / Stopwatch.Frequency * 1_000_000_000}ns, {difference}, {jsonObj.timestamp}");

                            //var diff = (double)timer.ElapsedTicks / Stopwatch.Frequency*1_000_000_000 + difference;
                            //json.WriteLine($"  {(diff - jsonObj.timestamp)/ 1_000_000_000d}s");
                            //Console.WriteLine($"  {(diff - jsonObj.timestamp) / 1_000_000_000}s");

                        }
                        //Console.WriteLine(jsonObj.type);
                        jsons.Add(curJsonString);

                        //if (curJsonString.Contains("Gyro"))
                        start = -1;
                    }
                    ++iter;
                }
                if (start == -1)
                {
                    jsonString = "";
                } else
                { 
                    jsonString += Encoding.ASCII.GetString(bytes, start+1, len - start - 1);
                    start = -1;
                }
                //if (bytes[0] != '{')
                //{
                //    offset = 0;
                //    continue;
                //}
                //var str = Encoding.ASCII.GetString(bytes, 0, len);
                //if (str[0] == '{')
                //    Console.WriteLine("--");
                //file.WriteLine("#" + str + "#");
                //Console.WriteLine(str);
            } while (true);
            stream.Close();
            clientSocket.Close();
        }
    }
}
