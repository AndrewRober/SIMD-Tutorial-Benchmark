namespace SIMDTutorial;

/// <summary>
/// A class that provides helper methods.
/// </summary>
public static class Helpers
{

    /// <summary>
    /// Converts the given millisecond value into a string that represents the time in the most appropriate unit.
    /// </summary>
    /// <param name="milliseconds">The time in milliseconds to format.</param>
    /// <returns>A string that represents the time in the most appropriate unit (ms, s, m, hr, or d).</returns>
    public static string FormatTime(this long milliseconds)
    {
        var units = new[] { "ms", "s", "m", "hr", "d" };
        var conversions = new[] { 1000.0, 60, 60, 24 };

        double time = milliseconds;
        var unitIndex = 0;

        while (unitIndex < conversions.Length && time >= conversions[unitIndex])
        {
            time /= conversions[unitIndex];
            unitIndex++;
        }

        return $"{time:0.##} {units[unitIndex]}";
    }
}