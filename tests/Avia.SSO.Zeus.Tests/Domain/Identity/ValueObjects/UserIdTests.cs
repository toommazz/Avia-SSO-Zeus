using Avia.SSO.Zeus.Domain.Identity.ValueObjects;

namespace Avia.SSO.Zeus.Tests.Domain.Identity.ValueObjects;

[TestClass]
public sealed class UserIdTests
{
    [TestMethod]
    public void Create_WithValidGuid_ShouldSucceed()
    {
        var guid = Guid.NewGuid();
        var result = UserId.Create(guid);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(guid, result.Value.Value);
    }

    [TestMethod]
    public void Create_WithEmptyGuid_ShouldFail()
    {
        var result = UserId.Create(Guid.Empty);
        Assert.IsTrue(result.IsFailure);
    }

    [TestMethod]
    public void New_ShouldGenerateUniqueIds()
    {
        var id1 = UserId.New();
        var id2 = UserId.New();
        Assert.AreNotEqual(id1, id2);
    }
}
