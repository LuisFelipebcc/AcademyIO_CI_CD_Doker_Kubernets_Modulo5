using AcademyIO.WebAPI.Core.User;
using System.Net.Http.Headers;

namespace AcademyIO.Bff.Extensions
{
    /// <summary>
    /// A delegating handler that adds an authorization header to outgoing HTTP requests.
    /// </summary>
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IAspNetUser _aspNetUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientAuthorizationDelegatingHandler"/> class.
        /// </summary>
        /// <param name="aspNetUser">The ASP.NET user.</param>
        public HttpClientAuthorizationDelegatingHandler(IAspNetUser aspNetUser)
        {
            _aspNetUser = aspNetUser;
        }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _aspNetUser.GetHttpContext().Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }

            var token = _aspNetUser.GetUserToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
