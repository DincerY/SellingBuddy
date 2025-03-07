﻿using IdentityService.Api.Application.Models;

namespace IdentityService.Api.Application.Services;

public interface IIdentityService
{
    Task<LoginResponseModel> LoginAsync(LoginRequestModel requestModel);
}