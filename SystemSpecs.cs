using System.Management;
using System.Numerics;

namespace SIMDTutorial;

/// <summary>
/// This class provides functionality to print system specifications.
/// </summary>
public static class SystemSpecs
{

    /// <summary>
    /// Prints system specifications including machine name, processor count, CPU name, CPU speed, total memory, and vector sizes for various data types.
    /// </summary>
    public static void Print()
    {
        // Get info about the machine
        string machineName = Environment.MachineName;
        int processorCount = Environment.ProcessorCount;
        long totalMemory = GC.GetTotalMemory(false);

        // Get CPU info
#pragma warning disable CA1416 // Validate platform compatibility
        var cpu = new ManagementObjectSearcher("select * from Win32_Processor").Get().OfType<ManagementObject>().FirstOrDefault();
        var cpuName = cpu?["Name"].ToString();
        var cpuCurrentClockSpeed = cpu?["CurrentClockSpeed"].ToString();
#pragma warning restore CA1416 // Validate platform compatibility

        // Print machine info
        Console.WriteLine($"Machine Name: {machineName}");
        Console.WriteLine($"Processor Count: {processorCount}");
        Console.WriteLine($"Processor Name: {cpuName}");
        Console.WriteLine($"Processor Speed: {cpuCurrentClockSpeed} Mhz");
        Console.WriteLine($"Total Memory: {totalMemory / 1024 / 1024} MB");

        // Print SIMD info
        Console.WriteLine("Vector<T> Sizes:");
        PrintVectorSize<byte>("byte");
        PrintVectorSize<sbyte>("sbyte");
        PrintVectorSize<int>("Int32");
        PrintVectorSize<uint>("UInt32");
        PrintVectorSize<long>("Int64");
        PrintVectorSize<ulong>("UInt64");
        PrintVectorSize<float>("Single");
        PrintVectorSize<double>("Double");
    }

    /// <summary>
    /// Prints the vector size for a specified data type.
    /// </summary>
    /// <typeparam name="T">The data type for which to print the vector size.</typeparam>
    /// <param name="typeName">The name of the data type.</param>
    private static void PrintVectorSize<T>(string typeName) where T : struct => Console.WriteLine($"  {typeName}: {Vector<T>.Count}");
}