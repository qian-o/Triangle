using Silk.NET.Shaderc;
using Triangle.Core.Exceptions;

namespace Triangle.Core.Helpers;

public static unsafe class ShadercHelper
{
    public static void CompileSpirv(string folder)
    {
        using Shaderc shaderc = Shaderc.GetApi();
        Compiler* compiler = shaderc.CompilerInitialize();
        CompileOptions* options = shaderc.CompileOptionsInitialize();
        CompilationResult* result;

        shaderc.CompileOptionsSetSourceLanguage(options, SourceLanguage.Glsl);
        shaderc.CompileOptionsSetAutoCombinedImageSampler(options, true);

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
            string outputFolder = Path.GetDirectoryName(file) ?? folder;
            string name = Path.GetFileNameWithoutExtension(file);
            string source = File.ReadAllText(file);

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

                using FileStream fileStream = File.Create(Path.Combine(outputFolder, $"{name}.{extension}.spv"));
                fileStream.Write(bytes);
            }
        }
    }
}
