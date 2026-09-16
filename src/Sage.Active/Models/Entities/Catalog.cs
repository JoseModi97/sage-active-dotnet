using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class AggregationCatalogDefinition
    {
        [JsonPropertyName("entityKey")]
        public string EntityKey { get; set; } = string.Empty;

        [JsonPropertyName("context")]
        public string? Context { get; set; }

        [JsonPropertyName("aggregationTypes")]
        public List<string>? AggregationTypes { get; set; }

        [JsonPropertyName("periodTypes")]
        public List<string>? PeriodTypes { get; set; }

        [JsonPropertyName("filters")]
        public List<string>? Filters { get; set; }

        [JsonPropertyName("valueColumns")]
        public List<string>? ValueColumns { get; set; }
    }

    public class AggregationExecuteInput
    {
        [JsonPropertyName("entityKey")]
        public string EntityKey { get; set; } = string.Empty;

        [JsonPropertyName("aggregationType")]
        public string AggregationType { get; set; } = "SUM";

        [JsonPropertyName("periodType")]
        public string? PeriodType { get; set; }

        [JsonPropertyName("dateMin")]
        public string? DateMin { get; set; }

        [JsonPropertyName("dateMax")]
        public string? DateMax { get; set; }

        [JsonPropertyName("groupByName")]
        public string? GroupByName { get; set; }

        [JsonPropertyName("valueColumn1")]
        public string? ValueColumn1 { get; set; }

        [JsonPropertyName("valueColumn2")]
        public string? ValueColumn2 { get; set; }

        [JsonPropertyName("top")]
        public int? Top { get; set; }
    }

    public class AggregationExecutionResult
    {
        [JsonPropertyName("rows")]
        public List<AggregationRow>? Rows { get; set; }
    }

    public class AggregationRow
    {
        [JsonPropertyName("groupValue")]
        public string? GroupValue { get; set; }

        [JsonPropertyName("period")]
        public string? Period { get; set; }

        [JsonPropertyName("value1")]
        public decimal? Value1 { get; set; }

        [JsonPropertyName("value2")]
        public decimal? Value2 { get; set; }

        [JsonPropertyName("deltaPercent")]
        public decimal? DeltaPercent { get; set; }
    }

    public class ListCatalogDefinition
    {
        [JsonPropertyName("entityKey")]
        public string EntityKey { get; set; } = string.Empty;

        [JsonPropertyName("context")]
        public string? Context { get; set; }

        [JsonPropertyName("filters")]
        public List<string>? Filters { get; set; }

        [JsonPropertyName("orderBy")]
        public List<string>? OrderBy { get; set; }
    }
}
