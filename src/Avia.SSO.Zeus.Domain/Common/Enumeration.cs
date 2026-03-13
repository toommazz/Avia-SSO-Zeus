namespace Avia.SSO.Zeus.Domain.Common;

public abstract class Enumeration(int id, string name) : IComparable
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration other)
            return false;
        return GetType() == other.GetType() && Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public int CompareTo(object? obj) => Id.CompareTo(((Enumeration)obj!).Id);
}
