using LibreHardwareMonitor.Hardware;
using System;
using System.Linq;

public class PerformanceMonitor
{
    private readonly Computer _computer;

    // Constructor to initialize the hardware monitoring
    public PerformanceMonitor()
    {
        // Initialize the Computer object with the necessary hardware components enabled
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true
        };
        _computer.Open(); // Open the computer to start monitoring
    }

    // Method to get the CPU usage percentage
    public int GetCpuUsage()
    {
        // Find the first hardware component of type CPU
        var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
        if (cpu == null) return 0; // Return 0 if no CPU found

        cpu.Update(); // Update the CPU data
        // Find the first sensor of type Load in the CPU component
        var cpuLoad = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
        // Return the rounded value of CPU load, or 0 if no load sensor found
        return RoundValue(cpuLoad?.Value ?? 0);
    }

    // Method to get the RAM usage percentage
    public int GetRamUsage()
    {
        // Find the first hardware component of type Memory
        var memory = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Memory);
        if (memory == null) return 0; // Return 0 if no Memory found

        memory.Update(); // Update the Memory data
        // Find the first sensor of type Load in the Memory component
        var memoryLoad = memory.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
        // Return the rounded value of Memory load, or 0 if no load sensor found
        return RoundValue(memoryLoad?.Value ?? 0);
    }

    // Method to get the GPU usage percentage
    public int GetGpuUsage()
    {
        // Array of possible GPU hardware types
        var gpuTypes = new[] { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
        foreach (var gpuType in gpuTypes)
        {
            // Find the first hardware component of the current GPU type
            var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == gpuType);
            if (gpu != null)
            {
                gpu.Update(); // Update the GPU data
                // Find the first sensor of type Load in the GPU component
                var gpuLoad = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Load);
                if (gpuLoad != null)
                {
                    // Return the rounded value of GPU load
                    return RoundValue(gpuLoad.Value ?? 0);
                }
            }
        }
        return 0; // Return 0 if no GPU load sensor found
    }

    // Method to get the CPU temperature
    public int GetCpuTemperature()
    {
        // Find the first hardware component of type CPU
        var cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
        if (cpu == null) return 0; // Return 0 if no CPU found

        cpu.Update(); // Update the CPU data
        // Find the first sensor of type Temperature in the CPU component
        var cpuTemp = cpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
        // Return the rounded value of CPU temperature, or 0 if no temperature sensor found
        return RoundValue(cpuTemp?.Value ?? 0);
    }

    // Method to get the GPU temperature
    public int GetGpuTemperature()
    {
        // Array of possible GPU hardware types
        var gpuTypes = new[] { HardwareType.GpuNvidia, HardwareType.GpuAmd, HardwareType.GpuIntel };
        foreach (var gpuType in gpuTypes)
        {
            // Find the first hardware component of the current GPU type
            var gpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == gpuType);
            if (gpu != null)
            {
                gpu.Update(); // Update the GPU data
                // Find the first sensor of type Temperature in the GPU component
                var gpuTemp = gpu.Sensors.FirstOrDefault(s => s.SensorType == SensorType.Temperature);
                if (gpuTemp != null)
                {
                    // Return the rounded value of GPU temperature
                    return RoundValue(gpuTemp.Value ?? 0);
                }
            }
        }
        return 0; // Return 0 if no GPU temperature sensor found
    }

    // Helper method to round float values to the nearest integer
    private int RoundValue(float value)
    {
        return (int)Math.Round(value); // Ensure rounding to the nearest whole number
    }
}
