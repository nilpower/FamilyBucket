﻿
using Bucket.HostedService;
using Bucket.HostedService.AspNetCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.Authorize.HostedService
{
    public static class ServiceCollectionExtensions
    {
        public static IHostedServiceBuilder AddAuthorize(this IHostedServiceBuilder builder)
        {
            builder.Services.AddSingleton<IExecutionService, BucketAuthorizeHostedService>();
            return builder;
        }
    }
}
