namespace ShipIt.Client.Models;

public static class OrderValidationConstants
{
    public const int CityMinLength = 2;
    public const int CityMaxLength = 100;
    public const int StreetMinLength = 2;
    public const int StreetMaxLength = 200;
    public const int HouseMaxLength = 20;
    public const decimal CargoWeightMinValue = 0;
}
