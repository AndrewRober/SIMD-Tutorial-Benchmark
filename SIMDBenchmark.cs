using System.Diagnostics;
using System.Numerics;

namespace SIMDTutorial;

/// <summary>
/// A class for benchmarking traditional and SIMD operations.
/// </summary>
/// <typeparam name="T">The type of data to operate on.</typeparam>
public class SIMDBenchmark<T> where T : struct
{
    private readonly int _count;
    private readonly int _repetitions;
    private readonly Func<T, T, T> _operation;
    private readonly Func<Vector<T>, Vector<T>, Vector<T>> _simdOperation;

    /// <summary>
    /// Initializes a new instance of the SimdBenchmark class.
    /// </summary>
    /// <param name="count">The number of data elements to operate on.</param>
    /// <param name="repetitions">The number of times to repeat the operations for the benchmark.</param>
    /// <param name="operation">The traditional operation to benchmark.</param>
    /// <param name="simdOperation">The SIMD operation to benchmark.</param>
    public SIMDBenchmark(int count, int repetitions,
        Func<T, T, T> operation, Func<Vector<T>, Vector<T>, Vector<T>> simdOperation)
    {
        _count = count;
        _repetitions = repetitions;
        _operation = operation;
        _simdOperation = simdOperation;
    }

    /// <summary>
    /// Runs the benchmark and returns the elapsed time for the traditional and SIMD operations.
    /// </summary>
    /// <returns>A tuple containing the elapsed time for the traditional operation (in ms) and the SIMD operation (in ms).</returns>
    /// <exception cref="Exception">Thrown when the results of the traditional and SIMD operations are not the same.</exception>
    public (long Traditional, long SIMD) Run()
    {
        // Calculate the number of elements we need to add to make the length 
        // of the arrays a multiple of Vector<T>.Count
        var padCount = _count % Vector<T>.Count == 0 ? 0 : Vector<T>.Count - _count % Vector<T>.Count;

        // Create the arrays with the padded length
        var numbers1 = new T[_count + padCount];
        var numbers2 = new T[_count + padCount];

        // Create a new Random instance with a fixed seed
        var random = new Random(42);


        //using half range to avoid overflowing and underflowing as 
        //the result of the operations is not the same with the 
        //traditional and the SIMD operations, one will overflow and the other will not
        for (var i = 0; i < _count; i++)
        {
            // Generate a random number and cast it to the appropriate type
            if (typeof(T) == typeof(int))
            {
                numbers1[i] = (T)(object)random.Next(int.MinValue / 2, int.MaxValue / 2);
                numbers2[i] = (T)(object)random.Next(int.MinValue / 2, int.MaxValue / 2);
            }
            else if (typeof(T) == typeof(byte))
            {
                numbers1[i] = (T)(object)(byte)random.Next(byte.MinValue, byte.MaxValue / 2);
                numbers2[i] = (T)(object)(byte)random.Next(byte.MinValue, byte.MaxValue / 2);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                numbers1[i] = (T)(object)(sbyte)random.Next(sbyte.MinValue / 2, sbyte.MaxValue / 2);
                numbers2[i] = (T)(object)(sbyte)random.Next(sbyte.MinValue / 2, sbyte.MaxValue / 2);
            }
            else if (typeof(T) == typeof(short))
            {
                numbers1[i] = (T)(object)(short)random.Next(short.MinValue / 2, short.MaxValue / 2);
                numbers2[i] = (T)(object)(short)random.Next(short.MinValue / 2, short.MaxValue / 2);
            }
            else if (typeof(T) == typeof(ushort))
            {
                numbers1[i] = (T)(object)(ushort)random.Next(ushort.MinValue, ushort.MaxValue / 2);
                numbers2[i] = (T)(object)(ushort)random.Next(ushort.MinValue, ushort.MaxValue / 2);
            }
            else if (typeof(T) == typeof(uint))
            {
                numbers1[i] = (T)(object)(uint)random.Next(0, int.MaxValue / 2);
                numbers2[i] = (T)(object)(uint)random.Next(0, int.MaxValue / 2);
            }
            else if (typeof(T) == typeof(long))
            {
                var nextLong = ((long)random.Next(int.MinValue / 2, int.MaxValue / 2) << 32) | (long)random.Next(int.MinValue / 2, int.MaxValue / 2);
                numbers1[i] = (T)(object)nextLong;
                numbers2[i] = (T)(object)nextLong;
            }
            else if (typeof(T) == typeof(ulong))
            {
                var nextULong = ((ulong)random.Next(0, int.MaxValue / 2) << 32) | (uint)random.Next(0, int.MaxValue / 2);
                numbers1[i] = (T)(object)nextULong;
                numbers2[i] = (T)(object)nextULong;
            }
            else if (typeof(T) == typeof(float))
            {
                numbers1[i] = (T)(object)(float)(random.NextDouble() / 2);
                numbers2[i] = (T)(object)(float)(random.NextDouble() / 2);
            }
            else if (typeof(T) == typeof(double))
            {
                numbers1[i] = (T)(object)(random.NextDouble() / 2);
                numbers2[i] = (T)(object)(random.NextDouble() / 2);
            }
        }

        // Calculate the number of vectors we can fit into the padded arrays
        var vecSize = Vector<T>.Count;
        var vecCount = numbers1.Length / vecSize;

        // Create the vectors
        var vectors1 = new Vector<T>[vecCount];
        var vectors2 = new Vector<T>[vecCount];

        for (var i = 0; i < vecCount; i++)
        {
            vectors1[i] = new Vector<T>(numbers1, i * vecSize);
            vectors2[i] = new Vector<T>(numbers2, i * vecSize);
        }

        // Create the result arrays with the padded length
        var traditionalResult = new T[_count + padCount];
        var simdResult = new T[_count + padCount];

        var stopwatch = Stopwatch.StartNew();

        for (var r = 0; r < _repetitions; r++)
        {
            for (var i = 0; i < _count; i++)
                traditionalResult[i] = _operation(numbers1[i], numbers2[i]);
        }

        stopwatch.Stop();
        var traditional = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();

        // SIMD operation
        var result = new Vector<T>[vecCount];
        for (var r = 0; r < _repetitions; r++)
        for (var i = 0; i < vecCount; i++)
            result[i] = _simdOperation(vectors1[i], vectors2[i]);

        stopwatch.Stop();
        var simd = stopwatch.ElapsedMilliseconds;


        // Convert SIMD result back to array for comparison (not benchmarked)
        for (var i = 0; i < vecCount; i++)
        {
            var v = result[i];
            for (var j = 0; j < vecSize; j++)
                simdResult[i * vecSize + j] = v[j];
        }

        // Assert that the results are the same
        for (var i = 0; i < _count; i++)  // Only compare the first _count elements
            if (!traditionalResult[i].Equals(simdResult[i]))
                throw new Exception($"Results are not the same at index {i}: {traditionalResult[i]} vs {simdResult[i]}");

        return (traditional, simd);
    }
}