using System;
using System.Collections.Generic;
using Sage.Active.Models.GraphQL;

namespace Sage.Active.Exceptions
{
    /// <summary>
    /// Base exception for all Sage Active API operations.
    /// </summary>
    public class SageApiException : Exception
    {
        public int? StatusCode { get; }
        public List<GraphQLError>? Errors { get; }

        public SageApiException(string message, int? statusCode = null, List<GraphQLError>? errors = null, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }

    /// <summary>
    /// Exception thrown when Sage Business Cloud Auth fails or credentials are invalid.
    /// </summary>
    public class SageAuthException : SageApiException
    {
        public SageAuthException(string message, int? statusCode = null, Exception? innerException = null)
            : base(message, statusCode, null, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when Sage Active returns HTTP 429 Too Many Requests.
    /// </summary>
    public class SageRateLimitException : SageApiException
    {
        public TimeSpan? RetryAfter { get; }

        public SageRateLimitException(string message, TimeSpan? retryAfter = null)
            : base(message, 429)
        {
            RetryAfter = retryAfter;
        }
    }

    /// <summary>
    /// Exception thrown when validation fails on an input model or business rule.
    /// </summary>
    public class SageValidationException : SageApiException
    {
        public SageValidationException(string message, List<GraphQLError>? errors = null)
            : base(message, 400, errors)
        {
        }
    }
}
