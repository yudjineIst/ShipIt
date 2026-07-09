using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShipIt.Application.Common.Models;
using ShipIt.Application.Features;
using ShipIt.Tests.Infrastructure;

namespace ShipIt.Tests.Orders;

public sealed class OrdersControllerTests
{
    private static readonly DateOnly PickupDate = new(2026, 7, 10);

    [Fact]
    public async Task CreateOrder_ShouldCreateOrderWithGeneratedNumber_WhenRequestIsValid()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/orders", CreateValidCommand());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
        order.Should().NotBeNull();
        order!.Id.Should().NotBeEmpty();
        order.OrderNumber.Should().NotBeNullOrWhiteSpace();
        order.OrderNumber.Should().NotBe(order.Id.ToString());
        order.OrderNumber.Should().MatchRegex("^ORD-20260704-[A-F0-9]{6}$");
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.PathAndQuery.Should().Be($"/api/orders/{order.Id}");
    }

    [Fact]
    public async Task CreateOrder_ShouldRejectOrder_WhenSenderCityIsEmpty()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        var command = CreateValidCommand() with { SenderCity = string.Empty };

        var response = await client.PostAsJsonAsync("/api/orders", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_ShouldRejectOrder_WhenCargoWeightIsNotPositive()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        var command = CreateValidCommand() with { CargoWeight = 0m };

        var response = await client.PostAsJsonAsync("/api/orders", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateOrder_ShouldRejectOrder_WhenPickupDateIsInThePast()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        var pastDate = DateOnly.FromDateTime(factory.DateTimeProvider.UtcNow).AddDays(-1);
        var command = CreateValidCommand() with { PickupDate = pastDate };

        var response = await client.PostAsJsonAsync("/api/orders", command);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        var created = await CreateOrderAsync(client, CreateValidCommand());

        var response = await client.GetAsync($"/api/orders/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDetailsResponse>();
        order.Should().NotBeNull();
        order!.Id.Should().Be(created.Id);
        order.SenderCity.Should().Be("Москва");
        order.SenderStreet.Should().Be("Тверская улица");
        order.SenderHouse.Should().Be("10");
        order.SenderApartment.Should().Be(5);
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnOrdersOrderedByCreatedDateDescending()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        var first = await CreateOrderAsync(client, CreateValidCommand());
        factory.DateTimeProvider.UtcNow = factory.DateTimeProvider.UtcNow.AddMinutes(1);
        var second = await CreateOrderAsync(client, CreateValidCommand() with { SenderCity = "Казань" });

        var ordersPage = await client.GetFromJsonAsync<PagedResponse<OrderListItemResponse>>("/api/orders");

        ordersPage.Should().NotBeNull();
        ordersPage!.Items.Select(order => order.Id).Should().ContainInOrder(second.Id, first.Id);
        ordersPage.Items.Should().BeInDescendingOrder(order => order.CreatedAtUtc);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnPagedResponse_WithDefaultPagination()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        await CreateOrderAsync(client, CreateValidCommand());
        factory.DateTimeProvider.UtcNow = factory.DateTimeProvider.UtcNow.AddMinutes(1);
        await CreateOrderAsync(client, CreateValidCommand() with { SenderCity = "Казань" });

        var response = await client.GetAsync("/api/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var ordersPage = await response.Content.ReadFromJsonAsync<PagedResponse<OrderListItemResponse>>();
        ordersPage.Should().NotBeNull();
        ordersPage!.PageNumber.Should().Be(1);
        ordersPage.PageSize.Should().Be(20);
        ordersPage.TotalCount.Should().Be(2);
        ordersPage.TotalPages.Should().Be(1);
        ordersPage.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetOrders_ShouldRejectRequest_WhenPageNumberIsLessThanOne()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders?pageNumber=0&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrders_ShouldRejectRequest_WhenPageSizeIsLessThanOne()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders?pageNumber=1&pageSize=0");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrders_ShouldRejectRequest_WhenPageSizeIsGreaterThanMax()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders?pageNumber=1&pageSize=101");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnRequestedPage()
    {
        using var factory = await CreateFactoryAsync();
        using var client = factory.CreateClient();
        var first = await CreateOrderAsync(client, CreateValidCommand() with { SenderCity = "Москва" });
        factory.DateTimeProvider.UtcNow = factory.DateTimeProvider.UtcNow.AddMinutes(1);
        await CreateOrderAsync(client, CreateValidCommand() with { SenderCity = "Казань" });
        factory.DateTimeProvider.UtcNow = factory.DateTimeProvider.UtcNow.AddMinutes(1);
        await CreateOrderAsync(client, CreateValidCommand() with { SenderCity = "Самара" });

        var ordersPage = await client.GetFromJsonAsync<PagedResponse<OrderListItemResponse>>(
            "/api/orders?pageNumber=2&pageSize=2");

        ordersPage.Should().NotBeNull();
        ordersPage!.PageNumber.Should().Be(2);
        ordersPage.PageSize.Should().Be(2);
        ordersPage.TotalCount.Should().Be(3);
        ordersPage.TotalPages.Should().Be(2);
        ordersPage.Items.Should().ContainSingle();
        ordersPage.Items[0].Id.Should().Be(first.Id);
    }

    private static async Task<TestApplicationFactory> CreateFactoryAsync()
    {
        var factory = new TestApplicationFactory();
        await factory.InitializeDatabaseAsync();
        return factory;
    }

    private static CreateOrderCommand CreateValidCommand() => new(
        "Москва",
        "Тверская улица",
        "10",
        5,
        "Тула",
        "Советская улица",
        "12",
        null,
        3.5m,
        PickupDate);

    private static async Task<CreateOrderResponse> CreateOrderAsync(HttpClient client, CreateOrderCommand command)
    {
        var response = await client.PostAsJsonAsync("/api/orders", command);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CreateOrderResponse>())!;
    }
}
