using System.Net.Http.Json;
using System.Text.Json;
using ShipIt.Client.Models;

namespace ShipIt.Client.Services;

public sealed class OrdersApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<OrderListItem>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/orders", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<List<OrderListItem>>(cancellationToken) ?? [];
    }

    public async Task<OrderDetails?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/orders/{id}", cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<OrderDetails>(cancellationToken);
    }

    public async Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/orders", request, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<CreateOrderResponse>(cancellationToken);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var message = ReadErrorMessage(body)
            ?? $"Сервер вернул ошибку {(int)response.StatusCode} ({response.ReasonPhrase}).";

        throw new HttpRequestException(message, null, response.StatusCode);
    }

    private static string? ReadErrorMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;

            if (root.TryGetProperty("detail", out var detail))
            {
                return detail.GetString();
            }

            if (root.TryGetProperty("title", out var title))
            {
                return title.GetString();
            }
        }
        catch (JsonException)
        {
            return body;
        }

        return body;
    }
}
