using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Triangle.Core.Structs;

public readonly struct TrDescriptor(IList<TrDescriptorBinding> attrib, IList<TrDescriptorBinding> uniform) : IEquatable<TrDescriptor>
{
    public ReadOnlyCollection<TrDescriptorBinding> Attrib { get; } = new ReadOnlyCollection<TrDescriptorBinding>(attrib);

    public ReadOnlyCollection<TrDescriptorBinding> Uniform { get; } = new ReadOnlyCollection<TrDescriptorBinding>(uniform);

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Attrib, Uniform);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is TrDescriptor layout && Equals(layout);
    }

    public readonly bool Equals(TrDescriptor other)
    {
        return Attrib == other.Attrib && Uniform == other.Uniform;
    }

    public static bool operator ==(TrDescriptor value1, TrDescriptor value2)
    {
        return value1.Equals(value2);
    }

    public static bool operator !=(TrDescriptor value1, TrDescriptor value2)
    {
        return !value1.Equals(value2);
    }
}
