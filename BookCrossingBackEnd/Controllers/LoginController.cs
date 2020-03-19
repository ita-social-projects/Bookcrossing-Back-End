﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.IServices;
using Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookCrossingBackEnd.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {

        private IUserService serice { get; set; }
        private IConfiguration configuration { get; set; }
        private ITokenService tokenService { get; set; }

        public LoginController(IUserService userService, IConfiguration configuration, ITokenService tokenService)
        {
            this.serice = userService;
            this.configuration = configuration;
            this.tokenService = tokenService;
        }




        /// <summary>
        /// Function for user authentication.
        /// </summary>
        /// <param name="model">This parameter receives email and password from form on client side</param>
        /// <returns>Returns JSON web token or http response code 401(Unauthorized)</returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            IActionResult response = Unauthorized();

            var user = await serice.Validate(model);

            if (user != null)
            {
                var tokenStr = tokenService.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenStr });
            }
            return response;
        }
    }
}
