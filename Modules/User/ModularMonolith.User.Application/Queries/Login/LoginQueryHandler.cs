﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ModularMonolith.User.Application.Exceptions;
using ModularMonolith.User.Application.Queries.Login;

namespace ModularMonolith.User.Application.Commands.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtService _jwtService;

        public LoginQueryHandler(UserManager<IdentityUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }
        public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new LoginException();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role,"User")
            };

            return new LoginResponse(_jwtService.GenerateJwt(claims));
        }
    }
}