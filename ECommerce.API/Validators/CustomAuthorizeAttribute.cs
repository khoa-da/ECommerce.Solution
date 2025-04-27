﻿using ECommerce.Infrastructure.Utils;
using ECommerce.Shared.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ECommerce.API.Validators
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute(params RoleEnum[] roleEnums)
        {
            var allowedRolesAsString = roleEnums.Select(x => x.GetDescriptionFromEnum());
            Roles = string.Join(",", allowedRolesAsString);
        }
    }
}
