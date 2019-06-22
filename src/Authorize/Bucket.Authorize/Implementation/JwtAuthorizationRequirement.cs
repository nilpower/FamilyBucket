﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Bucket.Authorize.Implementation
{
    /// <summary>
    /// jwt authorizationrequirement
    /// </summary>
    public class JwtAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// issuer
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// signing credentials
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }


        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="claimType">claim type</param>
        /// <param name="issuer">issuer</param>
        /// <param name="audience">audience</param>
        /// <param name="signingCredentials">signing credentials</param>
        /// <param name="expiration">expiration</param>
        public JwtAuthorizationRequirement(string issuer, string audience, SigningCredentials signingCredentials)
        {
            Issuer = issuer;
            Audience = audience;
            SigningCredentials = signingCredentials;
        }
    }
}
