using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;

namespace grpcWithAuth
{
    public class GreeterService : Greeter.GreeterBase
    {
        static string[] scopeRequiredByAPI = new string[] { "access_as_user" };
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByAPI);
            
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
