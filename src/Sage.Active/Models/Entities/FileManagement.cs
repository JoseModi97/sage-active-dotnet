using System;
using System.Text.Json.Serialization;

namespace Sage.Active.Models.Entities
{
    public class UploadedFile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("previewPath")]
        public string? PreviewPath { get; set; }

        [JsonPropertyName("downloadPath")]
        public string? DownloadPath { get; set; }

        [JsonPropertyName("businessDate")]
        public DateTimeOffset? BusinessDate { get; set; }
    }

    public class FileExportResult
    {
        [JsonPropertyName("downloadPath")]
        public string? DownloadPath { get; set; }

        [JsonPropertyName("indexPath")]
        public string? IndexPath { get; set; }
    }
}
