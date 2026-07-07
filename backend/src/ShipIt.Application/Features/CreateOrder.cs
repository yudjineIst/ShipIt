using FluentValidation;
using Microsoft.Extensions.Logging;
using ShipIt.Application.Common.Abstractions;
using ShipIt.Domain.Common;
using ShipIt.Domain.Orders;
using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Application.Features;

public sealed record CreateOrderCommand(
    string SenderCity,
    string SenderStreet,
    string SenderHouse,
    int? SenderApartment,
    string RecipientCity,
    string RecipientStreet,
    string RecipientHouse,
    int? RecipientApartment,
    decimal CargoWeight,
    DateOnly PickupDate);

public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.SenderCity).NotEmpty().MinimumLength(City.MinLength).MaximumLength(City.MaxLength);
        RuleFor(x => x.SenderStreet)
            .NotEmpty()
            .MinimumLength(Address.StreetMinLength)
            .MaximumLength(Address.StreetMaxLength);
        RuleFor(x => x.SenderHouse)
            .NotEmpty()
            .MaximumLength(Address.HouseMaxLength);
        RuleFor(x => x.SenderApartment)
            .GreaterThan(0)
            .When(x => x.SenderApartment.HasValue);
        RuleFor(x => x.RecipientCity).NotEmpty().MinimumLength(City.MinLength).MaximumLength(City.MaxLength);
        RuleFor(x => x.RecipientStreet)
            .NotEmpty()
            .MinimumLength(Address.StreetMinLength)
            .MaximumLength(Address.StreetMaxLength);
        RuleFor(x => x.RecipientHouse)
            .NotEmpty()
            .MaximumLength(Address.HouseMaxLength);
        RuleFor(x => x.RecipientApartment)
            .GreaterThan(0)
            .When(x => x.RecipientApartment.HasValue);
        RuleFor(x => x.CargoWeight).GreaterThan(CargoWeight.MinValue);
        RuleFor(x => x.PickupDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(dateTimeProvider.UtcNow))
            .WithMessage("Дата получения не может быть раньше текущей даты.");
    }
}

public sealed record CreateOrderResponse(Guid Id, string OrderNumber);

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IValidator<CreateOrderCommand> validator,
    IOrderNumberGenerator orderNumberGenerator,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateOrderHandler> logger)
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            var message = string.Join(" ", validation.Errors.Select(error => error.ErrorMessage));
            logger.LogWarning("Order creation validation failed: {ValidationErrors}", message);
            return Result<CreateOrderResponse>.Failure(new Error(
                OrderErrors.ValidationFailed.Code, message, ErrorType.Validation));
        }

        var order = DeliveryOrder.Create(
            Guid.NewGuid(),
            orderNumberGenerator.Generate(),
            command.SenderCity,
            command.SenderStreet,
            command.SenderHouse,
            command.SenderApartment,
            command.RecipientCity,
            command.RecipientStreet,
            command.RecipientHouse,
            command.RecipientApartment,
            command.CargoWeight,
            command.PickupDate,
            dateTimeProvider.UtcNow);
        if (order.IsFailure)
        {
            logger.LogWarning("Order domain validation failed: {ErrorCode}", order.Error.Code);
            return Result<CreateOrderResponse>.Failure(order.Error);
        }

        var createdOrder = order.Value;

        var persistenceResult = await orderRepository.AddAsync(
            createdOrder,
            cancellationToken);

        if (persistenceResult.IsFailure)
            return Result<CreateOrderResponse>.Failure(persistenceResult.Error);

        logger.LogInformation(
            "Order {OrderId} with number {OrderNumber} was created",
            createdOrder.Id,
            createdOrder.OrderNumber.Value);

        var response = new CreateOrderResponse(
            createdOrder.Id,
            createdOrder.OrderNumber.Value);

        return Result<CreateOrderResponse>.Success(response);
    }
}
