﻿using Bucket.Utility.Files;
using Bucket.WebApi.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;

namespace Bucket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] IBucketFileProvider fileProvider, [FromQuery] LoginInput input)
        {
            var sb = new StringBuilder();
            foreach(var info in fileProvider.GetDirectoryContents(""))
            {
                sb.Append(info.Name);
            }
            return new string[] { "value1", "value2", sb.ToString() };
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
