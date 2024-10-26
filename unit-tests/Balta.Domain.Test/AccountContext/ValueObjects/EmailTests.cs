using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class EmailTests
{
    private const string ExpectedAddress = "testemail@example.com";
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();

    [Fact]
    public void ShouldLowerCaseEmail()
    {
        var upperCasedAddress = "TestEmail@Example.Com";

        var email = Email.ShouldCreate(upperCasedAddress, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedAddress, email.Address);
    }

    [Fact]
    public void ShouldTrimEmail()
    {
        var untrimmedAddress = " testemail@example.com ";

        var email = Email.ShouldCreate(untrimmedAddress, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedAddress, email.Address);
    }

    [Fact]
    public void ShouldFailIfEmailIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Email.ShouldCreate(null, _dateTimeProviderMock.Object));
    }

    [Fact]
    public void ShouldFailIfEmailIsEmpty()
    {
        var emptyAddress = "";

        Assert.Throws<ArgumentNullException>(() => Email.ShouldCreate(emptyAddress, _dateTimeProviderMock.Object));
    }

    [Fact]
    public void ShouldFailIfEmailIsInvalid()
    {
        var invalidAddress = "testemail@@@example..com";

        Assert.Throws<InvalidEmailException>(() => Email.ShouldCreate(invalidAddress, _dateTimeProviderMock.Object));
    }

    [Fact]
    public void ShouldPassIfEmailIsValid()
    {
        var validAddress = "testemail@example.com";
        var validEmail = Email.ShouldCreate(validAddress, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedAddress, validEmail.Address);
    }

    [Fact]
    public void ShouldHashEmailAddress()
    {
        var expectedHash = ExpectedAddress.ToBase64();

        var email = Email.ShouldCreate(ExpectedAddress, _dateTimeProviderMock.Object);

        Assert.Equal(expectedHash, email.Hash);
    }

    [Fact]
    public void ShouldExplicitConvertFromString()
    {
        var email = Email.FromString(ExpectedAddress, _dateTimeProviderMock.Object);

        Assert.Equal(ExpectedAddress, email.Address);
    }

    [Fact]
    public void ShouldExplicitConvertToString()
    {
        var email = Email.ShouldCreate(ExpectedAddress, _dateTimeProviderMock.Object);
        var result = (string)email;

        Assert.Equal(ExpectedAddress, result);
    }

    [Fact]
    public void ShouldReturnEmailWhenCallToStringMethod()
    {
        var email = Email.ShouldCreate(ExpectedAddress, _dateTimeProviderMock.Object);
        var result = email.ToString();

        Assert.Equal(ExpectedAddress, result);
    }
}