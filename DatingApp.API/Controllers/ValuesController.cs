using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _db;
        public ValuesController(DataContext db)
        {
            _db = db;
        }
        // GET api/values
        [HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        public async Task<IActionResult> GetValues()
        {
            //return new string[] { "value1", "value2", "value3", "value4" };

            var vals = await _db.Values.ToListAsync();
            return Ok(vals);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValueAsync(int id)
        {
            var val = await _db.Values.FirstOrDefaultAsync(w=>w.Id == id);

            return Ok(val);

            /*
            if(val == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(val);
            }
            */
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
