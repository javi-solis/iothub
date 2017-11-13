# Azure IoT Hub C# Implementation

End to End Azure IoT Hub sample implementation

## Device To Cloud Messaging	

ConsoleApp: Creates an EventHubClient to stablish connection with the Azure IoT Hub, reads its partitions and starts an EventHubReceiver for each partition to receive the messages.

## Register Device

ConsoleApp: Uses the RegistryManager to stablish connection with the Azure IoT Hub and register three simulated devices. The console output shows the generated keys for these three devices.

## Simulated Device

ConsoleApp: Uses the RegistryManager to stablish connection with the Azure IoT Hub and register three simulated devices using the generated keys from RegisterDevice project.

