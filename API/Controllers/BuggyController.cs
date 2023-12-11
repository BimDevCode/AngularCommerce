using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly StoreContext _storeContext;

        public BuggyController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [Authorize]
        [HttpGet("testauth")]
        public ActionResult<string> GetSecretText(){
            return "Secret Text";
        }
        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest(){
            var thing = _storeContext.Products.Find(42);
            if (thing is not null){
                return NotFound(new ApiResponse(404));
            }
            return Ok();
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError(){
            var thing = _storeContext.Products.Find(42);
            var stringThing = thing!.ToString();
            return Ok();
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest(){
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id){
            return Ok();
        }
    }
}