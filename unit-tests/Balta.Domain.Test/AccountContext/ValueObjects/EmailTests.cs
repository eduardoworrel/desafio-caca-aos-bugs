using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class EmailTests
{
    private const string ExpectedAddress = "testemail@example.com";
    
    [Fact]
    public void ShouldLowerCaseEmail()
    {
        var upperCasedAddress = "TestEmail@Example.Com";
        var dateTimeProvider = new Mock<IDateTimeProvider>();
            
        var email = Email.ShouldCreate(upperCasedAddress, dateTimeProvider.Object);
        
        Assert.Equal(ExpectedAddress, email.Address);
    }

    [Fact]
    public void ShouldTrimEmail()
    {
        var untrimmedAddress = " testemail@example.com ";
        var dateTimeProvider = new Mock<IDateTimeProvider>();

        var email = Email.ShouldCreate(untrimmedAddress, dateTimeProvider.Object);
        
        Assert.Equal(ExpectedAddress, email.Address);
    }

    [Fact]
    public void ShouldFailIfEmailIsNull()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();

        Assert.Throws<ArgumentNullException>(() => Email.ShouldCreate(null, dateTimeProvider.Object));
    }

    [Fact]
    public void ShouldFailIfEmailIsEmpty()
    {
        var emptyAddress = "";
        var dateTimeProvider = new Mock<IDateTimeProvider>();

        Assert.Throws<ArgumentNullException>(() => Email.ShouldCreate(emptyAddress, dateTimeProvider.Object));
    }

    [Fact]
    public void ShouldFailIfEmailIsInvalid()
    {
        var invalidAddress = "testemail@@@example..com";
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        Assert.Throws<InvalidEmailException>(() => Email.ShouldCreate(invalidAddress, dateTimeProvider.Object));
    }

    [Fact]
    public void ShouldPassIfEmailIsValid()
    {
        var validAddress = "testemail@example.com";
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        var validEmail = Email.ShouldCreate(validAddress, dateTimeProvider.Object); 
        
        Assert.Equal(ExpectedAddress, validEmail.Address);
    }

    [Fact]
    public void ShouldHashEmailAddress()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();

        var expectedHash = ExpectedAddress.ToBase64();
        
        var email = Email.ShouldCreate(ExpectedAddress, dateTimeProvider.Object);
        
        Assert.Equal(expectedHash, email.Hash);
    }

    [Fact]
    public void ShouldExplicitConvertFromString()
    {
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var email = Email.FromString(ExpectedAddress, dateTimeProviderMock.Object);
        
        Assert.Equal(ExpectedAddress, email.Address);
    }

    [Fact]
    public void ShouldExplicitConvertToString()
    {
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        
        var email = Email.ShouldCreate(ExpectedAddress, dateTimeProviderMock.Object);
        var result = (string)email;
        
        Assert.Equal(ExpectedAddress, result);
    }

    [Fact]
    public void ShouldReturnEmailWhenCallToStringMethod()
    {
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        var email = Email.ShouldCreate(ExpectedAddress, dateTimeProviderMock.Object);
        var result = email.ToString();

        Assert.Equal(ExpectedAddress, result);
    }
}