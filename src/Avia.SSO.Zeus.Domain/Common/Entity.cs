namespace Avia.SSO.Zeus.Domain.Common;

public abstract class Entity : BaseEntity
{
    protected Entity(Guid id) => Id = id;
}
