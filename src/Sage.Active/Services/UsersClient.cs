using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Sage.Active.Models.Entities;
using Sage.Active.Models.GraphQL;
using Sage.Active.Transport;

namespace Sage.Active.Services
{
    public class UsersClient
    {
        private readonly SageGraphQLClient _transport;

        public UsersClient(SageGraphQLClient transport)
        {
            _transport = transport;
        }

        /// <summary>
        /// Retrieves the current authenticated user's profile.
        /// </summary>
        public async Task<UserProfile> GetUserProfileAsync(CancellationToken cancellationToken = default)
        {
            const string query = @"
            query {
                userProfile {
                    applicationLanguageCode
                    authenticationEmail
                    fullName
                    lastName
                    firstName
                }
            }";

            var result = await _transport.SendQueryAsync<UserProfileResponse>(query, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.UserProfile;
        }

        /// <summary>
        /// Retrieves list of users in the current organization.
        /// </summary>
        public async Task<Connection<User>> GetUsersAsync(int first = 50, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($first: Int!) {
                users(first: $first) {
                    edges {
                        node {
                            id
                            fullName
                            firstName
                            lastName
                            authenticationEmail
                            applicationLanguageCode
                            auth0UserId
                        }
                    }
                    totalCount
                }
            }";

            var variables = new { first };
            var result = await _transport.SendQueryAsync<UsersResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.Users;
        }

        /// <summary>
        /// Validates granular permissions for specified actions using the Sage User Access Policy.
        /// </summary>
        public async Task<List<UserAccessPolicyCheckResult>> CheckAccessPolicyAsync(IEnumerable<string> actions, CancellationToken cancellationToken = default)
        {
            const string query = @"
            query ($actions: [String!]!) {
                userAccessPolicyCheck(actions: $actions) {
                    action
                    isAllowed
                }
            }";

            var variables = new { actions };
            var result = await _transport.SendQueryAsync<UserAccessPolicyResponse>(query, variables, cancellationToken: cancellationToken).ConfigureAwait(false);
            return result.UserAccessPolicyCheck;
        }

        private class UserProfileResponse
        {
            [JsonPropertyName("userProfile")]
            public UserProfile UserProfile { get; set; } = new UserProfile();
        }

        private class UsersResponse
        {
            [JsonPropertyName("users")]
            public Connection<User> Users { get; set; } = new Connection<User>();
        }

        private class UserAccessPolicyResponse
        {
            [JsonPropertyName("userAccessPolicyCheck")]
            public List<UserAccessPolicyCheckResult> UserAccessPolicyCheck { get; set; } = new List<UserAccessPolicyCheckResult>();
        }
    }
}
