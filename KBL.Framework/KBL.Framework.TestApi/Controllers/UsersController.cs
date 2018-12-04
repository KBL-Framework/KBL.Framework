using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KBL.Framework.BAL.Base.Entities;
using KBL.Framework.TestApi.DTOs;
using KBL.Framework.TestApi.Model;
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

        [HttpGet]
        [Route("history/{id}")]
        public ActionResult<IEnumerable<EntityHistoryDto<User>>> History(long id)
        {
            return new ObjectResult(_userServices.GetHistory(id));
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
        public async Task<ActionResult> PostAsync([FromBody] UserDto value)
        {
            var result = await _userServices.CreateAsync(value, "loskubos");
            return result != 0 ? NoContent() : StatusCode(500);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task PutAsync(int id, [FromBody] UserDto value)
        {
           await _userServices.UpdateAsync(value, "testor");
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task DeleteAsync(int id)
        {
            await _userServices.DeleteAsync(id);
        }
    }
}
