using Microsoft.AspNetCore.Mvc;
using AALKisAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AALKisShared.Records;
using System;
using System.Collections.Generic;

namespace AALKisAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KeywordListController : ControllerBase
    {
        private readonly ILogger<KeywordListController> _logger;
        private readonly IKeywordsRepository _keywordsRepository;

        public KeywordListController(ILogger<KeywordListController> logger, IKeywordsRepository keywordsRepository)
        {
            _logger = logger;
            _keywordsRepository = keywordsRepository;
        }

        [HttpGet]
        public IEnumerable<Keyword>? GetAllKeywords()
        {
            try
            {
                return _keywordsRepository.GetAllKeywords();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());
                Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            return null;
        }

        [HttpPost]
        public IActionResult CreateKeywordList([FromBody] IEnumerable<string> keywordNames)
        {
            try
            {
                _keywordsRepository.CreateKeywordList(keywordNames);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to create Keyword List: {exception}");
                return BadRequest();
            }
            return new StatusCodeResult(StatusCodes.Status201Created);
        }

        [HttpDelete]
        public IActionResult DeleteKeywordList([FromBody] IEnumerable<string> keywordNames)
        {
            try
            {
                _keywordsRepository.DeleteKeywordList(keywordNames);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Failed to delete Keyword List: {exception}");
                return BadRequest();
            }
            return new StatusCodeResult(StatusCodes.Status204NoContent);
        }
    }
}
