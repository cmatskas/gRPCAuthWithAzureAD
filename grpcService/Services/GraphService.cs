using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;

namespace grpcWithAuth
{
    public class GraphAPIService : GraphService.GraphServiceBase
    {
        private static readonly string[] requiredScope = new string[] {"access_as_user"};
        private readonly ILogger<GraphAPIService> _logger;
        private readonly GraphServiceClient _graphServiceClient;
        public GraphAPIService(ILogger<GraphAPIService> logger, GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
        }

        [Authorize]
        public override async Task<CalendarReply> GetCalendar(CalendarRequest request, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            httpContext.VerifyUserHasAnyAcceptedScope(requiredScope);

            if (!request.Name.Equals("Christos", StringComparison.OrdinalIgnoreCase))
            {
                // search the Graph for the user's calendar data
                // to-do
                return new CalendarReply();
            }

            var data = await _graphServiceClient.Me.Events
                .Request()
                .Select("subject,bodyPreview,start,end")
                .GetAsync();

            return new CalendarReply
            {
                Subject = data.First().Subject,
                BodyPreview = data.First().BodyPreview,
                Start = data.First().Start.DateTime,
                End = data.First().End.DateTime
            };
        }
    }
}