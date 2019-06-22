﻿using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Pinzhi.Identity.Interface;
using Pinzhi.Identity.Model;
using Pinzhi.Identity.Dto.Auth;

using System;
using Bucket.Exceptions;
using Bucket.Utility;
using Bucket.Utility.Helpers;
using Bucket.Redis;
using Bucket.Config;
using System.Linq;
using System.Collections.Generic;
using Bucket.DbContext;
using Bucket.DbContext.SqlSugar;
using SqlSugar;

namespace Pinzhi.Identity.Business.Auth
{
    public class AuthBusiness : IAuthBusiness
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfig _config;
        private readonly ILogger<AuthBusiness> _logger;
        private readonly ISqlSugarDbRepository<UserInfo> _userDbRepository;
        private readonly RedisClient _redisClient;

        public AuthBusiness(IAuthRepository authRepository, IConfig config, ILogger<AuthBusiness> logger, ISqlSugarDbRepository<UserInfo> userDbRepository, RedisClient redisClient)
        {
            _authRepository = authRepository;
            _config = config;
            _logger = logger;
            _userDbRepository = userDbRepository;
            _redisClient = redisClient;
        }


        /// <summary>
        /// 用户密码登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<LoginOutput> LoginAsync(LoginInput input)
        {
            var rediscontect = _config.StringGet("RedisDefaultServer");
            var redis = _redisClient.GetDatabase(rediscontect, 5);
            var kv = await redis.StringGetAsync($"ImgCode:{input.Guid}");
            if (kv.IsNullOrEmpty || kv.ToString().ToLower() != input.ImgCode.ToLower())
                throw new BucketException("GO_2003", "图形验证码错误");
            // 用户验证
            var userInfo = await _userDbRepository.GetFirstAsync(it => it.UserName == input.UserName);
            if (userInfo == null)
                throw new BucketException("GO_0004007", "账号不存在");
            if (userInfo.State != 1)
                throw new BucketException("GO_0004008", "账号状态异常");
            if (userInfo.Password != Encrypt.SHA256(input.Password + userInfo.Salt))
                throw new BucketException("GO_4009", "账号或密码错误");
            // 用户角色
            //_userDbRepository.AsQueryable().Context.Queryable<>()
            var roleList = await _userDbRepository.AsQueryable().Context.Queryable<RoleInfo, UserRoleInfo>((role, urole) => new object[] { JoinType.Inner, role.Id == urole.RoleId })
                 .Where((role, urole) => urole.Uid == userInfo.Id)
                 .Where((role, urole) => role.IsDel == false)
                 .Select((role, urole) => new { role.Key })
                 .ToListAsync();
            // token返回
            var token = _authRepository.CreateAccessToken(new UserTokenDto
            {
                Email = userInfo.Email,
                Id = userInfo.Id,
                Mobile = userInfo.Mobile,
                RealName = userInfo.RealName,
            }, roleList.Select(it => it.Key).ToList());
            return new LoginOutput
            {
                Data = new
                {
                    AccessToken = $"Bearer {token}",
                    Expire = DateTimeOffset.Now.AddHours(4).ToUnixTimeSeconds(),
                    RealName = userInfo.RealName.SafeString(),
                    Mobile = userInfo.Mobile.SafeString(),
                    userInfo.Id
                }
            };
        }
        /// <summary>
        /// 用户短信登陆
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<LoginOutput> LoginBySmsAsync(LoginBySmsInput input)
        {
            // 验证短信验证码
            await _authRepository.VerifySmsCodeAsync(input.Mobile, input.SmsCode);
            // 绑定并登陆
            var token = _authRepository.CreateAccessToken(new UserTokenDto
            {
                Email = string.Empty,
                Id = 10000,
                Mobile = "1888888888",
                RealName = "1888888888"
            }, new List<string>());
            return new LoginOutput
            {
                Data = new
                {
                    AccessToken = $"Bearer {token}",
                    Expire = DateTimeOffset.Now.AddHours(4).ToUnixTimeSeconds(),
                    RealName = "1888888888",
                    Mobile = "1888888888",
                    Id = 10000
                }
            };
        }
        /// <summary>
        /// 发送短信验证码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SendSmsCodeOutput> SendSmsCodeAsync(SendSmsCodeInput input)
        {
            // 账号判断

            // 
            await _authRepository.SendSmsCodeAsync(input.Mobile, input.SmsTemplateName);
            return new SendSmsCodeOutput { Message = "发送成功" };
        }
    }
}
