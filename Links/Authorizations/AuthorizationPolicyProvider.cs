﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Links.Authorizations
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IConfiguration _configuration;

        private readonly AuthorizationOptions _options;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
            _options = options.Value;
        }
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // Check static policies first
            AuthorizationPolicy policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                   .AddRequirements(new HasScopeRequirement(policyName, $"{_configuration["AzureAd:Instance"]}/{_configuration["AzureAd:TenantId"]}"))
                   .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                   .Build();
            }
            _options.AddPolicy(policyName, policy);
            return policy;
        }
    }
}
