using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class FilesClient
    {
        private readonly SageGraphQLClient _transport;

        public FilesClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Uploads a binary file to an entity (CUSTOMER, SUPPLIER, SALES_INVOICE, PURCHASE_INVOICE) using multipart GraphQL.
        /// </summary>
        public async Task<string> UploadFileToEntityAsync(string entityType, string entityId, Stream fileStream, string fileName, string? comment = null, DateTimeOffset? businessDate = null, CancellationToken cancellationToken = default)
        {
            const string mutation = @"
            mutation ($input: UploadFileInput!) {
                uploadFileToEntity(input: $input) {
                    id
                }
            }";

            var variables = new
            {
                input = new
                {
                    file = (object?)null,
                    entityType = entityType,
                    entityId = entityId,
                    businessDate = (businessDate ?? DateTimeOffset.UtcNow).ToString("yyyy-MM-dd"),
                    comment = comment ?? ""
                }
            };

            var result = await _transport.SendMultipartAsync<UploadFileResponse>(mutation, variables, fileStream, fileName, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.UploadFileToEntity.Id;
        }

        /// <summary>
        /// Retrieves file details and secure download/preview paths.
        /// </summary>
        public async Task<Connection<UploadedFile>> GetFilesAsync(int first = 20, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                files(first: $first) {
                    edges {
                        node {
                            id
                            fileName
                            status
                            previewPath
                            downloadPath
                            businessDate
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<FilesResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Files;
        }

        /// <summary>
        /// Triggers a bulk ZIP export of files and returns download and index paths.
        /// </summary>
        public async Task<FileExportResult> ExportFilesZipAsync(string? customerId = null, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($customerId: UUID) {
                filesExport(customerId: $customerId) {
                    downloadPath
                    indexPath
                }
            }";

            var variables = new { customerId };
            var result = await _transport.SendQueryAsync<ExportResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.FilesExport;
        }

        private class UploadFileResponse
        {
            [JsonPropertyName("uploadFileToEntity")]
            public UploadedFile UploadFileToEntity { get; set; } = new UploadedFile();
        }

        private class FilesResponse
        {
            [JsonPropertyName("files")]
            public Connection<UploadedFile> Files { get; set; } = new Connection<UploadedFile>();
        }

        private class ExportResponse
        {
            [JsonPropertyName("filesExport")]
            public FileExportResult FilesExport { get; set; } = new FileExportResult();
        }
    }
}
