using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.data;
using Microsoft.AspNetCore.Authorization;
using API.Entities;

namespace API.Controllers
{
    public class ErrorController : BaseAPIController
    {
        private readonly DataContext context;
        public ErrorController(DataContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSectet()
        {
            return "Secret text";
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var user = context.Users.Find(-1);

            return user.ToString();

        }

        [HttpGet("not-found")]
        public ActionResult<string> GetNotfound()
        {
            return NotFound();
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadrequest()
        {
            return BadRequest("This is a bad request");
        }

    }
}