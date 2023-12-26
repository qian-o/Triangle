using System.Diagnostics.CodeAnalysis;
using Triangle.Core.Enums;

namespace Triangle.Core.Structs;

public readonly struct TrDescriptorBinding(uint binding, TrDescriptorType descriptorType) : IEquatable<TrDescriptorBinding>
{
    public uint Binding { get; } = binding;

    public TrDescriptorType DescriptorType { get; } = descriptorType;

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Binding, DescriptorType.GetHashCode());
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrDescriptorBinding binding && Equals(binding);
    }

    public readonly bool Equals(TrDescriptorBinding other)
    {
        return Binding == other.Binding && DescriptorType == other.DescriptorType;
    }

    public static bool operator ==(TrDescriptorBinding value1, TrDescriptorBinding value2)
    {
        return value1.Equals(value2);
    }

    public static bool operator !=(TrDescriptorBinding value1, TrDescriptorBinding value2)
    {
        return !value1.Equals(value2);
    }
}
