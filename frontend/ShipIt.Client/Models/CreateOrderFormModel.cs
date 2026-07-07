using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ShipIt.Client.Models;

public sealed class CreateOrderFormModel : IValidatableObject
{
    [Required(ErrorMessage = "Укажите город отправителя.")]
    [StringLength(OrderValidationConstants.CityMaxLength, MinimumLength = OrderValidationConstants.CityMinLength,
        ErrorMessage = "Название города должно содержать от 2 до 100 символов.")]
    public string SenderCity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите улицу отправителя.")]
    [StringLength(OrderValidationConstants.StreetMaxLength, MinimumLength = OrderValidationConstants.StreetMinLength,
        ErrorMessage = "Название улицы должно содержать от 2 до 200 символов.")]
    public string SenderStreet { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите дом.")]
    [StringLength(OrderValidationConstants.HouseMaxLength,
        ErrorMessage = "Номер дома должен содержать не более 20 символов.")]
    public string SenderHouse { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Номер квартиры должен быть больше нуля.")]
    public int? SenderApartment { get; set; }

    [Required(ErrorMessage = "Укажите город получателя.")]
    [StringLength(OrderValidationConstants.CityMaxLength, MinimumLength = OrderValidationConstants.CityMinLength,
        ErrorMessage = "Название города должно содержать от 2 до 100 символов.")]
    public string RecipientCity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите улицу получателя.")]
    [StringLength(OrderValidationConstants.StreetMaxLength, MinimumLength = OrderValidationConstants.StreetMinLength,
        ErrorMessage = "Название улицы должно содержать от 2 до 200 символов.")]
    public string RecipientStreet { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите дом.")]
    [StringLength(OrderValidationConstants.HouseMaxLength,
        ErrorMessage = "Номер дома должен содержать не более 20 символов.")]
    public string RecipientHouse { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Номер квартиры должен быть больше нуля.")]
    public int? RecipientApartment { get; set; }

    [Required(ErrorMessage = "Укажите вес груза.")]
    public string CargoWeight { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите дату получения груза.")]
    public DateOnly? PickupDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(CargoWeight))
        {
            return errors;
        }

        if (!TryParseCargoWeight(out var cargoWeight))
        {
            errors.Add(new ValidationResult(
                "Введите корректный вес.",
                new[] { nameof(CargoWeight) }));

            return errors;
        }

        if (cargoWeight <= OrderValidationConstants.CargoWeightMinValue)
        {
            errors.Add(new ValidationResult(
                "Вес груза должен быть больше нуля.",
                new[] { nameof(CargoWeight) }));
        }

        if (PickupDate is not null && PickupDate < DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add(new ValidationResult(
                "Дата получения не может быть раньше текущей даты.",
                new[] { nameof(PickupDate) }));
        }

        return errors;
    }

    public CreateOrderRequest ToRequest()
    {
        TryParseCargoWeight(out var cargoWeight);

        return new CreateOrderRequest(
            SenderCity,
            SenderStreet,
            SenderHouse,
            SenderApartment,
            RecipientCity,
            RecipientStreet,
            RecipientHouse,
            RecipientApartment,
            cargoWeight,
            PickupDate!.Value);
    }

    private bool TryParseCargoWeight(out decimal cargoWeight)
    {
        var normalizedValue = CargoWeight.Trim().Replace(',', '.');

        return decimal.TryParse(
            normalizedValue,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out cargoWeight);
    }
}
