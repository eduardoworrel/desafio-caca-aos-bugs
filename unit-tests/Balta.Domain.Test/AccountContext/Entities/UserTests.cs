using Balta.Domain.AccountContext.Entities;
using Balta.Domain.AccountContext.Errors;
using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Moq;

namespace Balta.Domain.Test.AccountContext.Entities;

public class UserTests
{
    private const string EmailAddress = "test@test.com"; 
    private const string PlainTextPassword = "senhabemlouca123";
    
    [Fact]
    public void ShouldCreateUserIfEmailAndPasswordIsValid()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        var email = Email.ShouldCreate(EmailAddress, dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);
        
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);         
        Assert.Equal(password, user.Password);   
        Assert.Null(user.LockOut);               
        Assert.Equal(0, user.AccessFailedCount); 
    }

    [Fact]
    public void ShouldIncrementAccessFailedCountIfAuthFails()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        var email = Email.ShouldCreate(EmailAddress, dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);
        
        user.ShouldAuthenticate();
        
        Assert.Equal(1, user.AccessFailedCount);
    }

    [Fact]
    public void ShouldBLockUserIfMultipleFailAttempts()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        var email = Email.ShouldCreate(EmailAddress, dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);
        
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();

        Assert.NotNull(user.LockOut);
    }
    
    [Fact]
    public void ShouldSendBlockingMessageIfMultipleFailAttempts()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        var email = Email.ShouldCreate(EmailAddress, dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);
        
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();

        Assert.Equal(UserErrors.LockedOutByLoginAttemptsMessage, user.LockOut?.LockOutReason);
    }
    
    [Fact]
    public void ShouldNotIncrementAccessFailedCountAfterBlock()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        
        var email = Email.ShouldCreate(EmailAddress, dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);
        
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();
        user.ShouldAuthenticate();

        Assert.True(user.AccessFailedCount <= 3);
    }
    
}