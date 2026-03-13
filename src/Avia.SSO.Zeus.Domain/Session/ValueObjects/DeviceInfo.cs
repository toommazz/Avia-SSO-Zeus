using Avia.SSO.Zeus.Domain.Common;

namespace Avia.SSO.Zeus.Domain.Session.ValueObjects;

public sealed class DeviceInfo : ValueObject
{
    public string UserAgent { get; }
    public string? IpAddress { get; }

    private DeviceInfo(string userAgent, string? ipAddress)
    {
        UserAgent = userAgent;
        IpAddress = ipAddress;
    }

    public static DeviceInfo Create(string userAgent, string? ipAddress = null) =>
        new(userAgent ?? string.Empty, ipAddress);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserAgent;
        yield return IpAddress ?? string.Empty;
    }
}
