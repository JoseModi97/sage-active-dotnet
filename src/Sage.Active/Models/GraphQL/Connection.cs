using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.GraphQL
{
    /// <summary>
    /// Relay-compliant cursor connection container.
    /// </summary>
    public class Connection<T>
    {
        [JsonPropertyName("edges")]
        public List<Edge<T>> Edges { get; set; } = new List<Edge<T>>();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("pageInfo")]
        public PageInfo PageInfo { get; set; } = new PageInfo();

        /// <summary>
        /// Flattens the edges to an IEnumerable of nodes for convenient LINQ access.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<T> Nodes
        {
            get
            {
                if (Edges == null) yield break;
                foreach (var edge in Edges)
                {
                    if (edge != null && edge.Node != null)
                    {
                        yield return edge.Node;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Connection Edge containing node and cursor.
    /// </summary>
    public class Edge<T>
    {
        [JsonPropertyName("cursor")]
        public string? Cursor { get; set; }

        [JsonPropertyName("node")]
        public T? Node { get; set; }
    }

    /// <summary>
    /// Connection PageInfo for cursor pagination.
    /// </summary>
    public class PageInfo
    {
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage { get; set; }

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage { get; set; }

        [JsonPropertyName("startCursor")]
        public string? StartCursor { get; set; }

        [JsonPropertyName("endCursor")]
        public string? EndCursor { get; set; }
    }
}
