using System.Runtime.InteropServices;
using Silk.NET.Shaderc;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Helpers;

public static unsafe class ShadercHelper
{
    private static readonly Dictionary<nint, Dictionary<string, string>> _cache = [];

    public static void CompileSpirv(string folder, Dictionary<string, string> includes)
    {
        nint key = Marshal.StringToHGlobalAnsi(folder);

        _cache.Add(key, includes);
        {
            using Shaderc shaderc = Shaderc.GetApi();
            Compiler* compiler = shaderc.CompilerInitialize();
            CompileOptions* options = shaderc.CompileOptionsInitialize();
            CompilationResult* result;

            shaderc.CompileOptionsSetSourceLanguage(options, SourceLanguage.Glsl);
            shaderc.CompileOptionsSetAutoCombinedImageSampler(options, true);
            shaderc.CompileOptionsSetTargetEnv(options, TargetEnv.Opengl, 460);

            foreach (string file in Directory.GetFiles(folder, "*.vert", SearchOption.AllDirectories))
            {
                CompileShader(file, ShaderKind.VertexShader);
            }

            foreach (string file in Directory.GetFiles(folder, "*.frag", SearchOption.AllDirectories))
            {
                CompileShader(file, ShaderKind.FragmentShader);
            }

            void CompileShader(string file, ShaderKind kind)
            {
                string path = Path.GetDirectoryName(file) ?? folder;
                string name = Path.GetFileNameWithoutExtension(file);
                string source = File.ReadAllText(file);

                shaderc.CompileOptionsSetIncludeCallbacks(options,
                                                          PfnIncludeResolveFn.From(IncludeResolver),
                                                          PfnIncludeResultReleaseFn.From(IncludeResultRelease),
                                                          (void*)key);

                result = shaderc.CompileIntoSpv(compiler, source, (uint)source.Length, kind, name, "main", options);

                if (shaderc.ResultGetCompilationStatus(result) != CompilationStatus.Success)
                {
                    throw new TrException(shaderc.ResultGetErrorMessageS(result));
                }
                else
                {
                    string extension = kind switch
                    {
                        ShaderKind.VertexShader => "vert",
                        ShaderKind.FragmentShader => "frag",
                        ShaderKind.ComputeShader => "comp",
                        ShaderKind.GeometryShader => "geom",
                        ShaderKind.TessControlShader => "tesc",
                        ShaderKind.TessEvaluationShader => "tese",
                        _ => throw new TrException("Unknown shader kind")
                    };

                    ReadOnlySpan<byte> bytes = new(shaderc.ResultGetBytes(result), (int)shaderc.ResultGetLength(result));

                    using FileStream fileStream = File.Create(Path.Combine(path, $"{name}.{extension}.spv"));
                    fileStream.Write(bytes);
                }
            }
        }
        _cache.Remove(key);

        Marshal.FreeHGlobal(key);
    }

    private static unsafe IncludeResult* IncludeResolver(void* userData, byte* requestedResource, int type, byte* requestingResource, UIntPtr includePath)
    {
        IncludeResult* result = (IncludeResult*)Marshal.AllocHGlobal(sizeof(IncludeResult));

        string include = Path.GetFileName(Marshal.PtrToStringAnsi((nint)requestedResource)!.Path());

        if (!_cache.TryGetValue((nint)userData, out Dictionary<string, string>? includes) || !includes.TryGetValue(include, out string? path))
        {
            throw new TrException($"Include file '{include}' not found.");
        }

        string name = Path.GetFileNameWithoutExtension(include);
        string source = File.ReadAllText(path);

        result->SourceName = (byte*)Marshal.StringToHGlobalAnsi(name);
        result->SourceNameLength = (nuint)name.Length;
        result->Content = (byte*)Marshal.StringToHGlobalAnsi(source);
        result->ContentLength = (nuint)source.Length;

        return result;
    }

    private static unsafe void IncludeResultRelease(void* userData, IncludeResult* includeResult)
    {
        Marshal.FreeHGlobal((nint)includeResult->Content);
        Marshal.FreeHGlobal((nint)includeResult->SourceName);
        Marshal.FreeHGlobal((nint)includeResult);
    }
}
