using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Common;

namespace RegisterDevice
{
    class Program
    {
        private static RegistryManager _registryManager;
        private static string _iotHubConnectionString = ConfigurationManager.AppSettings["iotHubConnectionString"];

        static void Main(string[] args)
        {
            _registryManager = RegistryManager.CreateFromConnectionString(_iotHubConnectionString);

            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        private static async Task AddDeviceAsync()
        {
            var simulatedDeviceIds = new string [] 
            {
                Constants.Device1Id,
                Constants.Device2Id,
                Constants.Device3Id
            };
            var devices = new List<Device>();

            foreach (string deviceId in simulatedDeviceIds)
            {
                try
                {
                    devices.Add(await _registryManager.AddDeviceAsync(new Device(deviceId)));
                }
                catch (DeviceAlreadyExistsException)
                {
                    devices.Add(await _registryManager.GetDeviceAsync(deviceId));
                }
            }            

            foreach (Device d in devices)
            {
                Console.WriteLine("Generated device key: {0}", d.Authentication.SymmetricKey.PrimaryKey);
            }
        }
    }
}
