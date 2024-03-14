using System.Numerics;
using BepuPhysics;
using BepuUtilities;

namespace Triangle.Core.Physics;

public struct PoseIntegratorCallbacks(Vector3 gravity, float linearDamping = 0.03f, float angularDamping = 0.03f) : IPoseIntegratorCallbacks
{
    private Vector3Wide gravityWideDt;
    private Vector<float> linearDampingDt;
    private Vector<float> angularDampingDt;

    public Vector3 Gravity = gravity;

    public float LinearDamping = linearDamping;

    public float AngularDamping = angularDamping;

    public readonly AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

    public readonly bool AllowSubstepsForUnconstrainedBodies => false;

    public readonly bool IntegrateVelocityForKinematics => false;

    public readonly void Initialize(Simulation simulation)
    {
    }

    public void PrepareForIntegration(float dt)
    {
        linearDampingDt = new Vector<float>(MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt));
        angularDampingDt = new Vector<float>(MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt));
        gravityWideDt = Vector3Wide.Broadcast(Gravity * dt);
    }

    public readonly void IntegrateVelocity(Vector<int> bodyIndices, Vector3Wide position, QuaternionWide orientation, BodyInertiaWide localInertia, Vector<int> integrationMask, int workerIndex, Vector<float> dt, ref BodyVelocityWide velocity)
    {
        velocity.Linear = (velocity.Linear + gravityWideDt) * linearDampingDt;
        velocity.Angular *= angularDampingDt;
    }
}
