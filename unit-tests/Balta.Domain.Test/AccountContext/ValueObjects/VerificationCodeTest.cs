using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class VerificationCodeTest
{
    private readonly DateTime _now;
    private readonly Mock<IDateTimeProvider> _dateTimeProvider;
    public VerificationCodeTest()
    {
        _now = DateTime.UtcNow;
        _dateTimeProvider = new Mock<IDateTimeProvider>();
       
    }

    [Fact]
    public void ShouldGenerateVerificationCode()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.IsType<string>(validCode.Code);
        Assert.IsType<DateTime>(validCode.ExpiresAtUtc);
        Assert.True(validCode.Code.Length == 6);
        Assert.True(validCode.Code.Equals(validCode.Code, StringComparison.CurrentCultureIgnoreCase));
        Assert.NotNull(validCode.ExpiresAtUtc);
        Assert.Equal((validCode.ExpiresAtUtc ?? new DateTime()).ToLongDateString(), _now.ToLongDateString());

    }

    [Fact]
    public void ShouldGenerateExpiresAtInFuture()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.True(validCode.ExpiresAtUtc > _now);
    }

    [Fact]
    public void ShouldGenerateVerifiedAtAsNull()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.Null(validCode.VerifiedAtUtc);
    }

    [Fact]
    public void ShouldBeInactiveWhenCreated()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.False(validCode.IsActive);
    }

    [Fact]
    public void ShouldFailIfExpired()
    {
        var dateTimeProvider = new Mock<IDateTimeProvider>();
        dateTimeProvider.Setup(d => d.UtcNow).Returns(_now);

        var verificationCode = VerificationCode.ShouldCreate(dateTimeProvider.Object);
        
        dateTimeProvider.Setup(d => d.UtcNow).Returns(_now.AddMinutes(6));

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(verificationCode.Code, dateTimeProvider.Object));
    }

    [Fact]
    public void ShouldFailIfCodeIsInvalid()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(null, _dateTimeProvider.Object));
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify("", _dateTimeProvider.Object));
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(" ", _dateTimeProvider.Object));

    }

    [Fact]
    public void ShouldFailIfCodeIsLessThanSixChars()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var invalidCode = validCode.Code.Substring(0, 4);
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(invalidCode, _dateTimeProvider.Object));

    }

    [Fact]
    public void ShouldFailIfCodeIsGreaterThanSixChars()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var invalidCode = validCode.Code + "rs";

        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(invalidCode, _dateTimeProvider.Object));
    }

    [Fact]
    public void ShouldFailIfIsNotActive()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        validCode.ShouldVerify(validCode.Code, _dateTimeProvider.Object);
          
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(validCode.Code, _dateTimeProvider.Object));
    }


    [Fact]
    public void ShouldFailIfIsAlreadyVerified()
    {
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var code = verificationCode.Code;

        verificationCode.ShouldVerify(code, _dateTimeProvider.Object);

        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(code, _dateTimeProvider.Object));
    }

    [Fact]
    public void ShouldFailIfIsVerificationCodeDoesNotMatch()
    {
        var code = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var otherCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
      
        

        Assert.Throws<InvalidVerificationCodeException>(
            () => code.ShouldVerify(otherCode.Code, _dateTimeProvider.Object)
        );
    }

    [Fact]
    public void ShouldVerify()
    {
        var code = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var codeValue = code.Code;

        code.ShouldVerify(codeValue, _dateTimeProvider.Object);

        Assert.NotNull(code.VerifiedAtUtc);
        Assert.Null(code.ExpiresAtUtc);
        Assert.True(code.IsActive);
    }
}