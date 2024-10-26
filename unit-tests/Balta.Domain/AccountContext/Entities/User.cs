using Balta.Domain.AccountContext.Errors;
using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Aggregates.Abstractions;
using Balta.Domain.SharedContext.Entities;

namespace Balta.Domain.AccountContext.Entities;

public sealed class User : Entity, IAggregate
{
    #region Constructors

    private User(Guid id, Email email, Password password) : base(id)
    {
        Email = email;
        Password = password;
    }

    #endregion

    #region Factories

    public static User ShouldCreate(string emailAddress, string plainTextPassword, IDateTimeProvider dateTimeProvider)
    {
        var email = Email.ShouldCreate(emailAddress, dateTimeProvider);
        var password = Password.ShouldCreate(plainTextPassword);

        return new User(Guid.NewGuid(), email, password);
    }

    public static User ShouldCreate(Email email, Password password)
        => new(Guid.NewGuid(), email, password);

    #endregion

    #region Properties

    public Email Email { get; }
    public Password Password { get; }
    public LockOut? LockOut { get; private set; }
    public short AccessFailedCount { get; private set; }

    #endregion

    #region Public Methods

    public void ShouldAuthenticate()
    {
        if (LockOut != null)
        {
            return;
        }

        AccessFailedCount++;

        if (AccessFailedCount >= Configuration.Api.MaxAccessFailedCount)
            LockOut = LockOut.ShouldCreate(Configuration.Api.DefaultLockOutTimeInMinutes, UserErrors.LockedOutByLoginAttemptsMessage);
    }

    #endregion
}