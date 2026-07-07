using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipIt.Domain.Orders;
using ShipIt.Domain.Orders.ValueObjects;

namespace ShipIt.Infrastructure.Database.Configurations;

public sealed class DeliveryOrderConfiguration : IEntityTypeConfiguration<DeliveryOrder>
{
    public void Configure(EntityTypeBuilder<DeliveryOrder> builder)
    {
        builder.ToTable("delivery_orders");
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .HasColumnName("id");

        builder.Property(order => order.OrderNumber)
            .HasConversion(number => number.Value, value => OrderNumber.Create(value).Value)
            .HasColumnName("order_number")
            .HasMaxLength(OrderNumber.MaxLength)
            .IsRequired();

        builder.Property(order => order.SenderCity)
            .HasConversion(city => city.Value, value => City.Create(value).Value)
            .HasColumnName("sender_city")
            .HasMaxLength(City.MaxLength)
            .IsRequired();

        builder.Property(order => order.RecipientCity)
            .HasConversion(city => city.Value, value => City.Create(value).Value)
            .HasColumnName("recipient_city")
            .HasMaxLength(City.MaxLength)
            .IsRequired();

        builder.OwnsOne(order => order.SenderAddress, address =>
        {
            address.Property(value => value.Street)
                .HasColumnName("sender_street")
                .HasMaxLength(Address.StreetMaxLength)
                .IsRequired();

            address.Property(value => value.House)
                .HasColumnName("sender_house")
                .HasMaxLength(Address.HouseMaxLength)
                .IsRequired();

            address.Property(value => value.Apartment)
                .HasColumnName("sender_apartment")
                .IsRequired(false);
        });

        builder.Navigation(order => order.SenderAddress).IsRequired();

        builder.OwnsOne(order => order.RecipientAddress, address =>
        {
            address.Property(value => value.Street)
                .HasColumnName("recipient_street")
                .HasMaxLength(Address.StreetMaxLength)
                .IsRequired();

            address.Property(value => value.House)
                .HasColumnName("recipient_house")
                .HasMaxLength(Address.HouseMaxLength)
                .IsRequired();

            address.Property(value => value.Apartment)
                .HasColumnName("recipient_apartment")
                .IsRequired(false);
        });

        builder.Navigation(order => order.RecipientAddress).IsRequired();

        builder.Property(order => order.CargoWeight)
            .HasConversion(weight => weight.Value, value => CargoWeight.Create(value).Value)
            .HasColumnName("cargo_weight")
            .HasPrecision(CargoWeight.TotalDigits, CargoWeight.FractionDigits)
            .IsRequired();

        builder.Property(order => order.PickupDate)
            .HasColumnName("pickup_date")
            .IsRequired();

        builder.Property(order => order.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(order => order.OrderNumber)
            .HasDatabaseName("ix_delivery_orders_order_number")
            .IsUnique();
    }
}
