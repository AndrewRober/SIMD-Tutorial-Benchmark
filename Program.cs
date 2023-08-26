using System.Numerics;

namespace SIMDTutorial
{
    class Program
    {
        const int COUNT = 1_000_000;
        const int REPT = 1_000;

        static void Main()
        {
            var testCases = new List<(string Name, Type Type, dynamic Operation, dynamic SimdOperation)>
            {
                #region int
		        ("Add", typeof(int), new Func<int, int, int>((a, b) => a + b), new Func<Vector<int>, Vector<int>, Vector<int>>(Vector.Add)),
                ("Subtract", typeof(int), new Func<int, int, int>((a, b) => a - b), new Func<Vector<int>, Vector<int>, Vector<int>>(Vector.Subtract)),
                ("Multiply", typeof(int), new Func<int, int, int>((a, b) => a * b), new Func<Vector<int>, Vector<int>, Vector<int>>(Vector.Multiply)), 
	            #endregion
                #region byte
		        ("Add", typeof(byte), new Func<byte, byte, byte>((a, b) => (byte)(a + b)), new Func<Vector<byte>, Vector<byte>, Vector<byte>>(Vector.Add)),
                ("Subtract", typeof(byte), new Func<byte, byte, byte>((a, b) => (byte)(a - b)), new Func<Vector<byte>, Vector<byte>, Vector<byte>>(Vector.Subtract)),
                ("Multiply", typeof(byte), new Func<byte, byte, byte>((a, b) => (byte)(a * b)), new Func<Vector<byte>, Vector<byte>, Vector<byte>>(Vector.Multiply)), 
	            #endregion
                #region sbyte
		        ("Add", typeof(sbyte), new Func<sbyte, sbyte, sbyte>((a, b) => (sbyte)(a + b)), new Func<Vector<sbyte>, Vector<sbyte>, Vector<sbyte>>(Vector.Add)),
                ("Subtract", typeof(sbyte), new Func<sbyte, sbyte, sbyte>((a, b) => (sbyte)(a - b)), new Func<Vector<sbyte>, Vector<sbyte>, Vector<sbyte>>(Vector.Subtract)),
                ("Multiply", typeof(sbyte), new Func<sbyte, sbyte, sbyte>((a, b) => (sbyte)(a * b)), new Func<Vector<sbyte>, Vector<sbyte>, Vector<sbyte>>(Vector.Multiply)), 
	            #endregion
                #region short
		        ("Add", typeof(short), new Func<short, short, short>((a, b) => (short)(a + b)), new Func<Vector<short>, Vector<short>, Vector<short>>(Vector.Add)),
                ("Subtract", typeof(short), new Func<short, short, short>((a, b) => (short)(a - b)), new Func<Vector<short>, Vector<short>, Vector<short>>(Vector.Subtract)),
                ("Multiply", typeof(short), new Func<short, short, short>((a, b) => (short)(a * b)), new Func<Vector<short>, Vector<short>, Vector<short>>(Vector.Multiply)), 
	            #endregion
                #region ushort
		        ("Add", typeof(ushort), new Func<ushort, ushort, ushort>((a, b) => (ushort)(a + b)), new Func<Vector<ushort>, Vector<ushort>, Vector<ushort>>(Vector.Add)),
                ("Subtract", typeof(ushort), new Func<ushort, ushort, ushort>((a, b) => (ushort)(a - b)), new Func<Vector<ushort>, Vector<ushort>, Vector<ushort>>(Vector.Subtract)),
                ("Multiply", typeof(ushort), new Func<ushort, ushort, ushort>((a, b) => (ushort)(a * b)), new Func<Vector<ushort>, Vector<ushort>, Vector<ushort>>(Vector.Multiply)), 
	            #endregion
                #region uint
		        ("Add", typeof(uint), new Func<uint, uint, uint>((a, b) => a + b), new Func<Vector<uint>, Vector<uint>, Vector<uint>>(Vector.Add)),
                ("Subtract", typeof(uint), new Func<uint, uint, uint>((a, b) => a - b), new Func<Vector<uint>, Vector<uint>, Vector<uint>>(Vector.Subtract)),
                ("Multiply", typeof(uint), new Func<uint, uint, uint>((a, b) => a * b), new Func<Vector<uint>, Vector<uint>, Vector<uint>>(Vector.Multiply)), 
	            #endregion
                #region long
		        ("Add", typeof(long), new Func<long, long, long>((a, b) => a + b), new Func<Vector<long>, Vector<long>, Vector<long>>(Vector.Add)),
                ("Subtract", typeof(long), new Func<long, long, long>((a, b) => a - b), new Func<Vector<long>, Vector<long>, Vector<long>>(Vector.Subtract)),
                ("Multiply", typeof(long), new Func<long, long, long>((a, b) => a * b), new Func<Vector<long>, Vector<long>, Vector<long>>(Vector.Multiply)), 
	            #endregion
                #region ulong
                ("Add", typeof(ulong), new Func<ulong, ulong, ulong>((a, b) => a + b), new Func<Vector<ulong>, Vector<ulong>, Vector<ulong>>(Vector.Add)),
                ("Subtract", typeof(ulong), new Func<ulong, ulong, ulong>((a, b) => a - b), new Func<Vector<ulong>, Vector<ulong>, Vector<ulong>>(Vector.Subtract)),
                ("Multiply", typeof(ulong), new Func<ulong, ulong, ulong>((a, b) => a * b), new Func<Vector<ulong>, Vector<ulong>, Vector<ulong>>(Vector.Multiply)), 
                #endregion
                #region double
                ("Add", typeof(double), new Func<double, double, double>((a, b) => a + b), new Func<Vector<double>, Vector<double>, Vector<double>>(Vector.Add)),
                ("Subtract", typeof(double), new Func<double, double, double>((a, b) => a - b), new Func<Vector<double>, Vector<double>, Vector<double>>(Vector.Subtract)),
                ("Multiply", typeof(double), new Func<double, double, double>((a, b) => a * b), new Func<Vector<double>, Vector<double>, Vector<double>>(Vector.Multiply)),
                ("Divide", typeof(double), new Func<double, double, double>((a, b) => a / b), new Func<Vector<double>, Vector<double>, Vector<double>>(Vector.Divide))
                #endregion
            };

            SystemSpecs.Print();
            Console.WriteLine(new string('=', 80));
            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine(new string('-', 72));
            Console.WriteLine($"| {"Type".Center(10)} | {"Operation".Center(10)} | {"Vector Size".Center(12)} | {"Traditional".Center(12)} | {"SIMD".Center(12)} |");
            Console.WriteLine(new string('-', 72));
            string previousTypeName = null;
            foreach (var testCase in testCases)
            {
                var vectorSize = (int)(typeof(Vector<>).MakeGenericType(testCase.Type).GetProperty("Count")?.GetValue(null) ?? throw new InvalidOperationException());

                // Use reflection to create a SimdBenchmark instance with the correct type parameters
                var benchmarkType = typeof(SIMDBenchmark<>).MakeGenericType(testCase.Type);
                var benchmark = Activator.CreateInstance(benchmarkType, COUNT, REPT, testCase.Operation, testCase.SimdOperation);

                // Invoke the Run method
                var (Traditional, SIMD) = (ValueTuple<long, long>)(benchmarkType.GetMethod("Run")?.Invoke(benchmark, null) ?? throw new InvalidOperationException());

                if (previousTypeName != null && previousTypeName != testCase.Type.Name)
                    Console.WriteLine(new string('-', 72));  // Print separator between different types

                Console.WriteLine($"| {testCase.Type.Name.Center(10)} | {testCase.Name.Center(10)} | {vectorSize.ToString().Center(12)} | {Traditional.FormatTime().Center(12)} | {SIMD.FormatTime().Center(12)} |");

                previousTypeName = testCase.Type.Name;
            }

            Console.WriteLine(new string('-', 72));  // Print separator
        }

    }

    public static class StringExtensions
    {
        /// <summary>
        /// Centers a string within a specified width by padding it with spaces on either side.
        /// </summary>
        /// <param name="text">The string to center.</param>
        /// <param name="width">The width within which to center the string.</param>
        /// <returns>The input string centered within the specified width.</returns>
        public static string Center(this string text, int width)
        {
            if (text.Length >= width)
            {
                return text;
            }
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;

            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
        }
    }
}