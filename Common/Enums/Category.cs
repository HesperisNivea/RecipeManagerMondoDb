using System.Text.Json.Serialization;

namespace Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum Category
{
    All,
    Breakfast,
    Lunch,
    Dinner,
    Dessert,
    Brunch,
    Tea
}