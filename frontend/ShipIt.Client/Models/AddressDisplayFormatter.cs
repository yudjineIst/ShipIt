namespace ShipIt.Client.Models;

public static class AddressDisplayFormatter
{
    public static string Format(string street, string house, int? apartment)
    {
        var address = $"{street}, д. {house}";

        if (apartment.HasValue)
        {
            address += $", кв. {apartment.Value}";
        }

        return address;
    }
}
