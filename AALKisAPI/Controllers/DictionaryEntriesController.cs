using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AALKisAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryEntriesController : ControllerBase
    {
        private readonly ILogger<DictionaryEntriesController> _logger;

        public DictionaryEntriesController(ILogger<DictionaryEntriesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Simulate reading entries from a database or another data source.
                List<string> entries = new List<string>
                {
                    "Word1;Meaning1",
                    "Word2;Meaning2",
                    "Word3;Meaning3"
                    // Add more entries as needed.
                };

                return Ok(entries);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while retrieving dictionary entries.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
