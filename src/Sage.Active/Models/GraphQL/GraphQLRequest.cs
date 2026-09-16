using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.GraphQL
{
    /// <summary>
    /// Standard GraphQL payload format for queries and mutations.
    /// </summary>
    public class GraphQLRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;

        [JsonPropertyName("operationName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? OperationName { get; set; }

        [JsonPropertyName("variables")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Variables { get; set; }

        public GraphQLRequest() { }

        public GraphQLRequest(string query, object? variables = null, string? operationName = null)
        {
            Query = query;
            Variables = variables;
            OperationName = operationName;
        }
    }

    /// <summary>
    /// Standard GraphQL response envelope.
    /// </summary>
    public class GraphQLResponse<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<GraphQLError>? Errors { get; set; }

        [JsonIgnore]
        public bool HasErrors => Errors != null && Errors.Count > 0;
    }

    /// <summary>
    /// Hot Chocolate GraphQL Error details.
    /// </summary>
    public class GraphQLError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("locations")]
        public List<GraphQLLocation>? Locations { get; set; }

        [JsonPropertyName("path")]
        public List<object>? Path { get; set; }

        [JsonPropertyName("extensions")]
        public Dictionary<string, object>? Extensions { get; set; }

        public string? Code => Extensions != null && Extensions.TryGetValue("code", out var code) 
            ? code?.ToString() 
            : null;
    }

    public class GraphQLLocation
    {
        [JsonPropertyName("line")]
        public int Line { get; set; }

        [JsonPropertyName("column")]
        public int Column { get; set; }
    }
}
