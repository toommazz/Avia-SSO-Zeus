using Avia.SSO.Zeus.Domain.Shared;

namespace Avia.SSO.Zeus.Tests.Domain.Shared;

[TestClass]
public sealed class ResultTests
{
    [TestMethod]
    public void Success_ShouldReturnSuccessResult()
    {
        var result = Result.Success();
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);
        Assert.AreEqual(Error.None, result.Error);
    }

    [TestMethod]
    public void Failure_ShouldReturnFailureResult()
    {
        var error = new Error("Test.Error", "Test error", ErrorType.Validation);
        var result = Result.Failure(error);
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(error, result.Error);
    }

    [TestMethod]
    public void SuccessGeneric_ShouldReturnValueAndSuccess()
    {
        var result = Result.Success(42);
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(42, result.Value);
    }

    [TestMethod]
    public void FailureGeneric_ShouldReturnError()
    {
        var error = new Error("Test.Error", "Test error", ErrorType.NotFound);
        var result = Result.Failure<int>(error);
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(error, result.Error);
    }
}
