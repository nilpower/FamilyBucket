﻿using AutoMapper;
using Bucket.Core;
using Bucket.Redis;
using Pinzhi.Platform.Dto;
using Pinzhi.Platform.Model;
using SqlSugar;
using System.Threading.Tasks;
using Pinzhi.Platform.Interface;
using System;
using Bucket.Utility;
using Bucket.DbContext;
using Bucket.DbContext.SqlSugar;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pinzhi.Platform.Business
{
    public class ApiBusiness : IApiBusiness
    {
        /// <summary>
        /// 数据库操作
        /// </summary>
        private readonly BucketSqlSugarClient _dbContext;
        private readonly RedisClient _redisClient;
        private readonly IMapper _mapper;
        private readonly IJsonHelper _jsonHelper;
        private readonly IUser _user;
        public ApiBusiness(BucketSqlSugarClient dbContext,
            IMapper mapper,
            RedisClient redisClient,
            IJsonHelper jsonHelper,
            IUser user)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _redisClient = redisClient;
            _jsonHelper = jsonHelper;
            _user = user;
        }
        /// <summary>
        /// 查询Api资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<QueryApisOutput> QueryApis(QueryApisInput input)
        {
            var pageNumber = 0;
            var query = await _dbContext.Queryable<ApiInfo>()
                                 .WhereIF(!input.ProjectKey.IsEmpty(), it => it.ProjectName == input.ProjectKey)
                                 .ToPageListAsync(input.PageIndex, input.PageSize, pageNumber);
            return new QueryApisOutput { Data = query, CurrentPage = input.PageIndex, Total = query.Count };
        }
        /// <summary>
        /// 设置Api资源
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetApiOutput> SetApi(SetApiInput input)
        {
            var model = _mapper.Map<ApiInfo>(input);
            if (model.Id > 0)
            {
                model.UpdateTime = DateTime.Now;
                await _dbContext.Updateable(model).IgnoreColumns(it => new { it.CreateTime }).ExecuteCommandAsync();
            }
            else
            {
                model.UpdateTime = DateTime.Now;
                model.CreateTime = DateTime.Now;
                await _dbContext.Insertable(model).ExecuteCommandAsync();
            }
            return new SetApiOutput { };
        }

    }
}
