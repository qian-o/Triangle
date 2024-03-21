using System.ComponentModel;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Hexa.NET.ImGui;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Controllers;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Physics;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;
using Triangle.Render.Materials.Chapter7;

namespace Triangle.Render.Tutorials;

[DisplayName("Physics Simulation")]
[Description("Demonstrate how to use BepuPhysics to simulate physics.")]
public class Tutorial08(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    public const float TimestepDuration = 1.0f / 60.0f;

    #region Physics
    private BufferPool bufferPool = null!;
    private ThreadDispatcher dispatcher = null!;
    private Simulation simulation = null!;
    private Dictionary<TrModel, BodyHandle> map = null!;
    #endregion

    #region Meshes
    private TrMesh floorMesh = null!;
    private TrMesh cubeMesh = null!;
    #endregion

    #region Materials
    private SingleTextureMat singleTextureMat = null!;
    private DiffusePixelLevelInstancedMat diffusePixelLevelInstancedMat = null!;
    #endregion

    #region Models
    private TrModel floor = null!;
    private List<TrModel> physicsModels = null!;
    #endregion

    private Random random = null!;

    protected override void Loaded()
    {
        Scene.Camera.Transform.Position = new Vector3D<float>(50.0f, 15.0f, 100.0f);
        Scene.Camera.Transform.EulerAngles = new Vector3D<float>(0.0f, 40.0f, 0.0f);

        bufferPool = new BufferPool();
        dispatcher = new ThreadDispatcher(int.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1));
        simulation = Simulation.Create(bufferPool, new NarrowPhaseCallbacks(new SpringSettings(30.0f, 1.0f)), new PoseIntegratorCallbacks(new Vector3(0.0f, -10.0f, 0.0f)), new SolveDescription(8, 1));

        floorMesh = Context.GetPlane();
        cubeMesh = Context.GetCube();

        singleTextureMat = new(Context)
        {
            Channel0 = TrTextureManager.Texture("Resources/Textures/wood_floor.jpg".Path()),
            Channel0ST = new Vector4D<float>(60.0f, 40.0f, 0.0f, 0.0f)
        };
        diffusePixelLevelInstancedMat = new(Context);

        floor = new("Floor", [floorMesh], singleTextureMat);
        floor.Transform.Scaled(new Vector3D<float>(250.0f, 1.0f, 250.0f));

        SceneController.Add(floor);

        physicsModels = [];

        const int columns = 40;
        const int rows = 20;
        SceneController.BeginGroup("Cubes");
        {
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    int columnCount = rows - j;
                    for (int k = 0; k < columnCount; k++)
                    {
                        TrModel cube = new($"Cube {physicsModels.Count}", [cubeMesh], diffusePixelLevelInstancedMat);
                        cube.Transform.Position = new Vector3D<float>((-columnCount * 0.5f) + k, j + 0.5f, (i - (columns * 0.5f)) * 4.0f);

                        physicsModels.Add(cube);

                        SceneController.Add(cube);
                    }
                }
            }
        }
        SceneController.EndGroup();

        diffusePixelLevelInstancedMat.Diffuse = [.. physicsModels.Select(item => item.ColorId.ToSingle())];

        // Physics scene building
        {
            simulation.Statics.Add(new StaticDescription(new Vector3(0.0f, -0.5f, 0.0f), simulation.Shapes.Add(new Box(floor.Transform.Scale.X, 1.0f, floor.Transform.Scale.Z))));

            Box box = new(1.0f, 1.0f, 1.0f);
            BodyInertia bodyInertia = box.ComputeInertia(1.0f);
            TypedIndex boxIndex = simulation.Shapes.Add(box);

            map = [];

            for (int i = 0; i < physicsModels.Count; i++)
            {
                TrModel cube = physicsModels[i];

                BodyHandle bodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(cube.Transform.Position.ToSystem(),
                                                                                            bodyInertia,
                                                                                            boxIndex,
                                                                                            0.01f));

                map.Add(cube, bodyHandle);
            }
        }

        random = new();
    }

    protected override void SceneDrawContentInWindow()
    {
        Vector2 size = ImGui.GetContentRegionMax();
        size.Y -= 20;

        ImGui.SetCursorPos(new Vector2(10, size.Y));

        ImGui.Text("Press 'Z' to add a sphere.");
    }

    protected override void UpdateScene(double deltaSeconds)
    {
        if (SceneController.IsTransformObject)
        {
            foreach ((TrModel Model, BodyHandle BodyHandle) in map)
            {
                BodyReference body = simulation.Bodies[BodyHandle];

                body.Pose.Position = Model.Transform.Position.ToSystem();
                body.Pose.Orientation = Model.Transform.Rotation.ToSystem();

                body.Awake = true;
                body.UpdateBounds();
            }
        }

        simulation.Timestep(TimestepDuration, dispatcher);

        foreach ((TrModel Model, BodyHandle BodyHandle) in map)
        {
            RigidPose pose = simulation.Bodies[BodyHandle].Pose;

            Model.Transform.Position = pose.Position.ToGeneric();
            Model.Transform.Rotation = pose.Orientation.ToGeneric();
        }

        if (Scene.KeyPressed(Key.Z))
        {
            float radius = 0.5f + 5 * random.NextSingle();

            TrMesh sphereMesh = Context.GetSphere(radius);
            TrModel model = new($"Sphere {physicsModels.Count}", [sphereMesh], diffusePixelLevelInstancedMat);
            model.Transform.Position = new Vector3D<float>(0.0f, 8.0f, 130.0f);

            physicsModels.Add(model);

            SceneController.BeginGroup("Spheres");
            SceneController.Add(model);
            SceneController.EndGroup();

            Sphere shape = new(radius);

            BodyHandle bodyHandle = simulation.Bodies.Add(BodyDescription.CreateConvexDynamic(model.Transform.Position.ToSystem(),
                                                                                              new Vector3(0, 0, -150),
                                                                                              radius * radius * radius,
                                                                                              simulation.Shapes,
                                                                                              shape));
            map.Add(model, bodyHandle);

            diffusePixelLevelInstancedMat.Diffuse = [.. physicsModels.Select(item => item.ColorId.ToSingle())];
        }
    }

    protected override void RenderScene(double deltaSeconds)
    {
        floor.Render(Parameters);

        diffusePixelLevelInstancedMat.Draw([.. physicsModels], Parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        map.Clear();
        simulation.Dispose();
        dispatcher.Dispose();
        bufferPool.Clear();

        singleTextureMat.Dispose();
        diffusePixelLevelInstancedMat.Dispose();
    }
}
