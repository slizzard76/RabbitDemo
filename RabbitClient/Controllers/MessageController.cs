using Microsoft.AspNetCore.Mvc;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RabbitClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        IRabbitManipulation _rabbitManipulation;
        public MessageController(IRabbitManipulation rabbitManipulation)
        {
            _rabbitManipulation = rabbitManipulation;
        }

        [HttpPost]
        public async Task Post([FromBody] string value)
        {
            await _rabbitManipulation.SendMessage(value);
        }
    }
}
