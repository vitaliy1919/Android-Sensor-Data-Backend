# Android Sensor Data Backend
## Overview

This library gives ability to access the data from sensors send by [this app](https://github.com/vitaliy1919/Android-Sensors-Data-to-PC)

## Prerequisites

- Microsoft Visual Studio 2019
- .NET Core 2.2
- ADB tools downladed on your PC 
- USB debugging turned on your phone.
- Newton.JSON (v12.0.2) library for json object deserialization.

## Usage

At first we need to establish adb connection with the phone (at this moment your phone has to be connected to the PC):
``` csharp
AndroidSensorMonitor.StartADBForwarding(@"<path to adb.exe>", 65000, 65000);
```

Then we need to create monitoring object and assign our callback:

``` csharp
AndroidSensorMonitor monitor = new AndroidSensorMonitor();
monitor.sensorDataCallback += Monitor_sensorDataCallback;
```

sensorDataCallback has the following signature: 
``` csharp
delegate void SensorDataArrived(SensorData data);
```
where `SensorData`:
``` cs
class SensorData
{
    public string type;
    public long timestamp;
    public float[] values;
}
```
For the exact content of values depending on sensor type see [here](https://developer.android.com/reference/android/hardware/SensorEvent.html#values).

Finally, we need to start monitoring:
``` cs
monitor.StartMonitoring(65000);
```

Additionally, if you are using console applications, you have wait, otherwise, the programm will finish preliminary:
``` cs
monitor.Wait();
```

## Important notes

If you want to test latency of the sensors or record sensor data take a look at [this project](https://github.com/vitaliy1919/Android-Sensors-Delay-Test).