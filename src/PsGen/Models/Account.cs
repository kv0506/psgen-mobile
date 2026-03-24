using System.Text.Json.Serialization;

namespace PsGen.Mobile.Models;

public class AccountDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("includeSpecialCharacter")]
    public bool IncludeSpecialCharacter { get; set; }

    [JsonPropertyName("useCustomSpecialCharacter")]
    public bool UseCustomSpecialCharacter { get; set; }

    [JsonPropertyName("customSpecialCharacter")]
    public string CustomSpecialCharacter { get; set; } = string.Empty;

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }
}

public class CreateAccountRequest
{
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("includeSpecialCharacter")]
    public bool IncludeSpecialCharacter { get; set; }

    [JsonPropertyName("useCustomSpecialCharacter")]
    public bool UseCustomSpecialCharacter { get; set; }

    [JsonPropertyName("customSpecialCharacter")]
    public string CustomSpecialCharacter { get; set; } = string.Empty;

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }
}

public class UpdateAccountRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("pattern")]
    public string Pattern { get; set; } = string.Empty;

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("includeSpecialCharacter")]
    public bool IncludeSpecialCharacter { get; set; }

    [JsonPropertyName("useCustomSpecialCharacter")]
    public bool UseCustomSpecialCharacter { get; set; }

    [JsonPropertyName("customSpecialCharacter")]
    public string CustomSpecialCharacter { get; set; } = string.Empty;

    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }
}

public class DeleteAccountRequest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
