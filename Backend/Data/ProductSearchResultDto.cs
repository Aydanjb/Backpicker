namespace Backend.Data;

public record ProductSearchResultDto(
    int Id,
    string? Name,
    string? Brand,
    int? PriceCents,
    string? Link,
    string? Image,
    List<string> Weights
);
            