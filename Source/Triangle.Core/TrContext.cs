using System.Collections.Concurrent;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core.Contracts;
using Triangle.Core.Enums;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;

namespace Triangle.Core;

public class TrContext : Disposable
{
    public const int MaxExecutionCount = 10;

    private readonly ConcurrentQueue<Action> _actions = new();
    private readonly bool _isInitialized;

    private bool isColorWrite;
    private bool isDepthTest;
    private bool isDepthWrite;
    private TrDepthFunction depthFunction;
    private bool isStencilTest;
    private bool isStencilWrite;
    private TrStencilFunction stencilFunction;
    private int stencilReference;
    private uint stencilMask;
    private bool isCullFace;
    private TrTriangleFace triangleFace;
    private TrFrontFaceDirection frontFaceDirection;
    private bool isBlend;
    private TrBlendFactor sourceFactor;
    private TrBlendFactor destinationFactor;
    private TrBlendEquationSeparate blendEquationSeparate;
    private TrBlendFuncSeparate blendFuncSeparate;
    private bool isScissorTest;
    private bool isPrimitiveRestart;
    private TrPolygon polygon;
    private bool isMultisample;

    public TrContext(GL gl)
    {
        GL = gl;

        // Set default settings.
        IsColorWrite = true;
        IsDepthTest = true;
        IsDepthWrite = true;
        DepthFunction = TrDepthFunction.Less;
        IsStencilTest = true;
        IsStencilWrite = true;
        StencilFunction = TrStencilFunction.Always;
        StencilReference = 0;
        StencilMask = 0xFF;
        IsCullFace = true;
        TriangleFace = TrTriangleFace.Back;
        FrontFaceDirection = TrFrontFaceDirection.CounterClockwise;
        IsBlend = true;
        SourceFactor = TrBlendFactor.SrcAlpha;
        DestinationFactor = TrBlendFactor.OneMinusSrcAlpha;
        BlendEquationSeparate = TrBlendEquationSeparate.Default;
        BlendFuncSeparate = TrBlendFuncSeparate.Default;
        IsScissorTest = false;
        IsPrimitiveRestart = false;
        Polygon = TrPolygon.Default;
        IsMultisample = true;

        // Other settings, not controlled by the render pipeline.
        GL.Enable(GLEnum.TextureCubeMapSeamless);

        _isInitialized = true;
    }

    public GL GL { get; }

    public bool IsColorWrite
    {
        get => isColorWrite;
        set
        {
            if (isColorWrite != value || !_isInitialized)
            {
                isColorWrite = value;

                GL.ColorMask(value, value, value, value);
            }
        }
    }

    public bool IsDepthTest
    {
        get => isDepthTest;
        set
        {
            if (isDepthTest != value || !_isInitialized)
            {
                isDepthTest = value;

                if (value)
                {
                    GL.Enable(GLEnum.DepthTest);
                }
                else
                {
                    GL.Disable(GLEnum.DepthTest);
                }
            }
        }
    }

    public bool IsDepthWrite
    {
        get => isDepthWrite;
        set
        {
            if (isDepthWrite != value || !_isInitialized)
            {
                isDepthWrite = value;

                GL.DepthMask(value);
            }
        }
    }

    public TrDepthFunction DepthFunction
    {
        get => depthFunction;
        set
        {
            if (depthFunction != value || !_isInitialized)
            {
                depthFunction = value;

                GL.DepthFunc(value.ToGL());
            }
        }
    }

    public bool IsStencilTest
    {
        get => isStencilTest;
        set
        {
            if (isStencilTest != value || !_isInitialized)
            {
                isStencilTest = value;

                if (value)
                {
                    GL.Enable(GLEnum.StencilTest);
                }
                else
                {
                    GL.Disable(GLEnum.StencilTest);
                }
            }
        }
    }

    public bool IsStencilWrite
    {
        get => isStencilWrite;
        set
        {
            if (isStencilWrite != value || !_isInitialized)
            {
                isStencilWrite = value;

                GL.StencilMask((uint)(value ? 0xFF : 0x00));
            }
        }
    }

    public TrStencilFunction StencilFunction
    {
        get => stencilFunction;
        set
        {
            if (stencilFunction != value || !_isInitialized)
            {
                stencilFunction = value;

                GL.StencilFunc(value.ToGL(), StencilReference, StencilMask);
            }
        }
    }

    public int StencilReference
    {
        get => stencilReference;
        set
        {
            if (stencilReference != value || !_isInitialized)
            {
                stencilReference = value;

                GL.StencilFunc(StencilFunction.ToGL(), value, StencilMask);
            }
        }
    }

    public uint StencilMask
    {
        get => stencilMask;
        set
        {
            if (stencilMask != value || !_isInitialized)
            {
                stencilMask = value;

                GL.StencilFunc(StencilFunction.ToGL(), StencilReference, value);
            }
        }
    }

    public bool IsCullFace
    {
        get => isCullFace;
        set
        {
            if (isCullFace != value || !_isInitialized)
            {
                isCullFace = value;

                if (value)
                {
                    GL.Enable(GLEnum.CullFace);
                }
                else
                {
                    GL.Disable(GLEnum.CullFace);
                }
            }
        }
    }

    public TrTriangleFace TriangleFace
    {
        get => triangleFace;
        set
        {
            if (triangleFace != value || !_isInitialized)
            {
                triangleFace = value;

                GL.CullFace(value.ToGL());
            }
        }
    }

    public TrFrontFaceDirection FrontFaceDirection
    {
        get => frontFaceDirection;
        set
        {
            if (frontFaceDirection != value || !_isInitialized)
            {
                frontFaceDirection = value;

                GL.FrontFace(value.ToGL());
            }
        }
    }

    public bool IsBlend
    {
        get => isBlend;
        set
        {
            if (isBlend != value || !_isInitialized)
            {
                isBlend = value;

                if (value)
                {
                    GL.Enable(GLEnum.Blend);
                }
                else
                {
                    GL.Disable(GLEnum.Blend);
                }
            }
        }
    }

    public TrBlendFactor SourceFactor
    {
        get => sourceFactor;
        set
        {
            if (sourceFactor != value || !_isInitialized)
            {
                sourceFactor = value;

                GL.BlendFunc(value.ToGL(), DestinationFactor.ToGL());
            }
        }
    }

    public TrBlendFactor DestinationFactor
    {
        get => destinationFactor;
        set
        {
            if (destinationFactor != value || !_isInitialized)
            {
                destinationFactor = value;

                GL.BlendFunc(SourceFactor.ToGL(), value.ToGL());
            }
        }
    }

    public TrBlendEquationSeparate BlendEquationSeparate
    {
        get => blendEquationSeparate;
        set
        {
            if (blendEquationSeparate != value || !_isInitialized)
            {
                blendEquationSeparate = value;

                GL.BlendEquationSeparate(value.ModeRGB.ToGL(), value.ModeAlpha.ToGL());
            }
        }
    }

    public TrBlendFuncSeparate BlendFuncSeparate
    {
        get => blendFuncSeparate;
        set
        {
            if (blendFuncSeparate != value || !_isInitialized)
            {
                blendFuncSeparate = value;

                GL.BlendFuncSeparate(value.SrcRGB.ToGL(), value.DstRGB.ToGL(), value.SrcAlpha.ToGL(), value.DstAlpha.ToGL());
            }
        }
    }

    public bool IsScissorTest
    {
        get => isScissorTest;
        set
        {
            if (isScissorTest != value || !_isInitialized)
            {
                isScissorTest = value;

                if (value)
                {
                    GL.Enable(GLEnum.ScissorTest);
                }
                else
                {
                    GL.Disable(GLEnum.ScissorTest);
                }
            }
        }
    }

    public bool IsPrimitiveRestart
    {
        get => isPrimitiveRestart;
        set
        {
            if (isPrimitiveRestart != value || !_isInitialized)
            {
                isPrimitiveRestart = value;

                if (value)
                {
                    GL.Enable(GLEnum.PrimitiveRestart);
                }
                else
                {
                    GL.Disable(GLEnum.PrimitiveRestart);
                }
            }
        }
    }

    public TrPolygon Polygon
    {
        get => polygon;
        set
        {
            if (polygon != value || !_isInitialized)
            {
                polygon = value;

                GL.PolygonMode(value.Face.ToGL(), value.Mode.ToGL());
                GL.LineWidth(value.LineWidth);
                GL.PointSize(value.PointSize);
            }
        }
    }

    public bool IsMultisample
    {
        get => isMultisample;
        set
        {
            if (isMultisample != value || !_isInitialized)
            {
                isMultisample = value;

                if (value)
                {
                    GL.Enable(GLEnum.Multisample);
                }
                else
                {
                    GL.Disable(GLEnum.Multisample);
                }
            }
        }
    }

    protected override void Destroy(bool disposing = false)
    {
    }

    public void Clear()
    {
        Clear(Vector4D<float>.UnitW);
    }

    public void Clear(Vector4D<float> color)
    {
        GL.ColorMask(true, true, true, true);
        GL.DepthMask(true);
        GL.StencilMask(0xFF);

        GL.ClearColor(color.X, color.Y, color.Z, color.W);
        GL.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);
    }

    public void Enqueue(Action action)
    {
        _actions.Enqueue(action);
    }

    public void Execute()
    {
        for (var i = 0; i < MaxExecutionCount; i++)
        {
            if (_actions.TryDequeue(out Action? action))
            {
                action();
            }
            else
            {
                break;
            }
        }
    }
}