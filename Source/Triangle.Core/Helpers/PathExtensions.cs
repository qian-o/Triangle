namespace Triangle.Core.Helpers;

public static class PathExtensions
{
    public static string Path(this string path)
    {
        // 在Windows平台下，将斜杠替换为反斜杠
        if (System.IO.Path.DirectorySeparatorChar == '\\')
        {
            path = path.Replace('/', '\\');
        }
        // 在其他平台下，将反斜杠替换为斜杠
        else
        {
            path = path.Replace('\\', '/');
        }

        return path;
    }
}
