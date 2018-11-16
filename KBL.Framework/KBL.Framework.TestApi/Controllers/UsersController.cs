using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KBL.Framework.TestApi.DTOs;
using KBL.Framework.TestApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KBL.Framework.TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UsersController(IUserServices userservices)
        {
            _userServices = userservices;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> Get()
        {
            return new ObjectResult(_userServices.GetAll());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] UserDto value)
        {
            _userServices.Update(value, "testor");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
