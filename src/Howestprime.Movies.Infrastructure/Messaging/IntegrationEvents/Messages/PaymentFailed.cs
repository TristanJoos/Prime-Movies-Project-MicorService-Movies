using System.Text.Json.Serialization;

public record PaymentFailed(
    [property: JsonPropertyName("BookingId")] Guid BookingId 
);