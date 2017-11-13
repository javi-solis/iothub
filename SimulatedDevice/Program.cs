using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using Common;
using System.Configuration;

namespace SimulatedDevice
{
    class Program
    {
        private static string _iotHubUri = ConfigurationManager.AppSettings["iotHubUri"];
        private static Hashtable _deviceClients = new Hashtable();

        internal class DeviceInfo
        {
            public DeviceInfo (string id, string key)
            {
                this.id = id;
                this.key = key;
            }

            public string id { get; set; }
            public string key { get; set; }
        }
   
        static List<DeviceInfo> GetDevicesList()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();

            devices.Add(new DeviceInfo(Constants.Device1Id, "RegistrationDeviceKey1"));
            devices.Add(new DeviceInfo(Constants.Device2Id, "RegistrationDeviceKey2"));
            devices.Add(new DeviceInfo(Constants.Device3Id, "RegistrationDeviceKey3"));
          
            return devices;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");

            foreach (DeviceInfo device in GetDevicesList())
            {
                _deviceClients.Add(device.id, DeviceClient.Create(_iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(device.id, device.key), TransportType.Mqtt));
            }

            SendDeviceToCloudMessagesAsync();

            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                foreach (DeviceInfo device in GetDevicesList())
                {
                    var telemetryDataPoint = new
                    {
                        deviceId = device.id,
                        temperature = currentTemperature,
                        humidity = currentHumidity
                    };
                    var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                    var message = new Message(Encoding.ASCII.GetBytes(messageString));
                    message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");

                    DeviceClient deviceClient = (DeviceClient)_deviceClients[device.id];
                    
                    await deviceClient.SendEventAsync(message);

                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                    await Task.Delay(1000);
                }

                await Task.Delay(5000);
            }
        }
    }
}
