using System.Runtime.CompilerServices;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

namespace Triangle.Core.Physics;

public struct NarrowPhaseCallbacks(SpringSettings contactSpringiness, float maximumRecoveryVelocity = 2.0f, float frictionCoefficient = 1.0f) : INarrowPhaseCallbacks
{
    public SpringSettings ContactSpringiness = contactSpringiness;

    public float MaximumRecoveryVelocity = maximumRecoveryVelocity;

    public float FrictionCoefficient = frictionCoefficient;

    public void Initialize(Simulation simulation)
    {
        if (ContactSpringiness.AngularFrequency == 0.0f && ContactSpringiness.TwiceDampingRatio == 0.0f)
        {
            ContactSpringiness = new(30.0f, 1.0f);
            MaximumRecoveryVelocity = 2.0f;
            FrictionCoefficient = 1.0f;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b, ref float speculativeMargin)
    {
        return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
    {
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ConfigureContactManifold<TManifold>(int workerIndex, CollidablePair pair, ref TManifold manifold, out PairMaterialProperties pairMaterial) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        pairMaterial.FrictionCoefficient = FrictionCoefficient;
        pairMaterial.MaximumRecoveryVelocity = MaximumRecoveryVelocity;
        pairMaterial.SpringSettings = ContactSpringiness;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ref ConvexContactManifold manifold)
    {
        return true;
    }

    public readonly void Dispose()
    {
    }
}
