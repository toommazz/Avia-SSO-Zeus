namespace Avia.SSO.Zeus.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    private readonly List<DomainEvent> _domainEvents = [];
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    protected void RaiseDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
