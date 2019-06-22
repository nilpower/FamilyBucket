﻿
using Bucket.HostedService;
using Bucket.HostedService.AspNetCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bucket.ErrorCode.HostedService
{
    public static class ServiceCollectionExtensions
    {
        public static IHostedServiceBuilder AddErrorCode(this IHostedServiceBuilder builder)
        {
            builder.Services.AddSingleton<IExecutionService, BucketErrorCodeHostedService>();
            return builder;
        }
    }
}
