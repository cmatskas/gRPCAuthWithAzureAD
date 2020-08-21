using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

namespace grpcWithAuth
{
    public class GraphAPIService : GraphService.GraphServiceBase
    {
        private static string[] requiredScope = new string[] {"access_as_user"};
        private static string[] scopes = new string[] {"Calendars.Read"};
        private readonly ILogger<GraphAPIService> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        public GraphAPIService(ILogger<GraphAPIService> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        [Authorize]
        public override async Task<CalendarReply> GetCalendar(CalendarRequest request, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            httpContext.VerifyUserHasAnyAcceptedScope(requiredScope);

            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            
            var graphUri = "https://graph.microsoft.com/v1.0/me/events?$top=5&$select=subject,bodyPreview,start,end";

            var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", token);
            var data = await http.GetStringAsync(graphUri);

            return new CalendarReply
            {
                Message = data
            };
        }
    }
}