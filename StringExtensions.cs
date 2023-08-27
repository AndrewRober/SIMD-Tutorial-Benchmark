namespace SIMDTutorial;

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