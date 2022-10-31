using Amazon.DynamoDBv2;
using Gomsle.Api.Features.Account;
using Gomsle.Api.Features.Email;
using Gomsle.Api.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Gomsle.Api.Tests.Features.Account;

public class InviteCommandTests : TestBase
{
    [Fact]
    public async Task Should_InviteUser_When_RequestIsValid() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var user = await CreateAndLoginValidUser(services);
            var accountName = "Microsoft";
            var email = "test@gomsle.com";
            var url = "https://gomsle.com/microsoft/invite";
            await mediator.Send(new CreateCommand.Command
            {
                Name = accountName,
            });
            var command = new InviteCommand.Command
            {
                AccountName = accountName,
                Email = email,
                InvitationUrl = url,
                Role = AccountRole.Administrator,
            };

            // Act
            var response = await mediator.Send(command);

            // Assert
            Assert.True(response.IsValid);

            var mock = GetMock<IEmailSender>();
            mock!.Verify(x => 
                x.Send(email, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once());
        });

    [Fact]
    public async Task Should_NotBeValid_When_AccountNameIsNotSet() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                Email = "test@gomsle.com",
                InvitationUrl = "https://gomsle.com/microsoft/invite",
                Role = AccountRole.Administrator,
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "NotEmptyValidator" &&
                x.PropertyName == nameof(InviteCommand.Command.AccountName));
        });

    [Fact]
    public async Task Should_NotBeValid_When_AccountDoesntExist() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                AccountName = "Mycrosöft",
                Email = "test@gomsle.com",
                InvitationUrl = "https://gomsle.com/microsoft/invite",
                Role = AccountRole.Administrator,
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "AccountNotFound" &&
                x.PropertyName == nameof(InviteCommand.Command.AccountName));
        });

    [Fact]
    public async Task Should_NotBeValid_When_EmailIsNotSet() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                AccountName = "Microsoft",
                InvitationUrl = "https://gomsle.com/microsoft/invite",
                Role = AccountRole.Administrator,
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "NotEmptyValidator" &&
                x.PropertyName == nameof(InviteCommand.Command.Email));
        });

    [Fact]
    public async Task Should_NotBeValid_When_EmailIsNotAnEmail() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                Email = "not-an-email",
                AccountName = "Microsoft",
                InvitationUrl = "https://gomsle.com/microsoft/invite",
                Role = AccountRole.Administrator,
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "EmailValidator" &&
                x.PropertyName == nameof(InviteCommand.Command.Email));
        });

    [Fact]
    public async Task Should_NotBeValid_When_InvitationUrlIsNotSet() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                Email = "test@gomsle.com",
                AccountName = "Microsoft",
                Role = AccountRole.Administrator,
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "NotEmptyValidator" &&
                x.PropertyName == nameof(InviteCommand.Command.InvitationUrl));
        });

    [Fact]
    public async Task Should_NotBeValid_When_RoleIsNotSet() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                Email = "test@gomsle.com",
                AccountName = "Microsoft",
                InvitationUrl = "https://gomsle.com/microsoft/invite",
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "NotEmptyValidator" &&
                x.PropertyName == nameof(InviteCommand.Command.Role));
        });

    [Fact]
    public async Task Should_NotBeValid_When_RoleIsOwner() =>
        await MediatorTest(async (mediator, services) =>
        {
            // Arrange
            var command = new InviteCommand.Command
            {
                Email = "test@gomsle.com",
                AccountName = "Microsoft",
                InvitationUrl = "https://gomsle.com/microsoft/invite",
                Role = AccountRole.Owner,
            };
            var database = services.GetRequiredService<IAmazonDynamoDB>();
            var validator = new InviteCommand.CommandValidator(database);

            // Act
            var response = await validator.ValidateAsync(command);

            // Assert
            Assert.False(response.IsValid);
            Assert.Contains(response.Errors, x => x.ErrorCode == "OnlyOneOwner" &&
                x.PropertyName == nameof(InviteCommand.Command.Role));
        });
}