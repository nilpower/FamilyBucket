﻿using System.Collections.Generic;
using System.Security.Claims;

namespace Bucket.Core
{
    public interface IUser
    {
        string Id { get; }
        string Ids { get; }
        string Name { get; }
        string MobilePhone { get; }
        IEnumerable<Claim> Claims { get; }
    }
}
