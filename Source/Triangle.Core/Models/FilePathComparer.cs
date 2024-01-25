namespace Triangle.Core.Models;

public class FilePathComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null || y == null)
        {
            return 0;
        }

        string[] xParts = x.Split(Path.DirectorySeparatorChar);
        string[] yParts = y.Split(Path.DirectorySeparatorChar);

        int xLength = xParts.Length;
        int yLength = yParts.Length;

        int length = Math.Min(xLength, yLength);

        for (int i = 0; i < length; i++)
        {
            string s1 = xParts[i];
            string s2 = yParts[i];

            if (s1.Contains('.') && !s2.Contains('.'))
            {
                return -1;
            }

            if (!s1.Contains('.') && s2.Contains('.'))
            {
                return 1;
            }

            int result = string.Compare(xParts[i], yParts[i], StringComparison.OrdinalIgnoreCase);
            if (result != 0)
            {
                return result;
            }
        }

        return xLength.CompareTo(yLength);
    }
}
