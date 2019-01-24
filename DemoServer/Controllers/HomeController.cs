﻿using System;
using System.Threading.Tasks;
using DemoParser.Models;
using DemoServer.Models;
using DemoServer.Utils;
using DemoServer.Utils.Database;
using DemoServer.Utils.Filters;
using DemoServer.Utils.UserId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DemoServer.Controllers
{
    [Route("demo")]
    [ServiceFilter(typeof(AddUserIdToResponseHeaderAttribute))]
    public class HomeController : Controller
    {
        private readonly DemoContainer _demoContainer;
        private readonly UserIdContainer _userId;
        private readonly DatabaseLinks _databaseLinks;
        private readonly DatabaseSetup _databaseSetup;
        private readonly Settings _settings;
        private readonly ILogger _logger;

        public HomeController(DemoContainer demoContainer, UserIdContainer userId, DatabaseLinks databaseLinks,
            DatabaseSetup databaseSetup, Settings settings, ILogger<HomeController> logger)
        {
            _demoContainer = demoContainer;
            _userId = userId;
            _databaseSetup = databaseSetup;
            _databaseLinks = databaseLinks;
            _settings = settings;
            _logger = logger;
        }

        private Guid UserId => _userId.Get();

        [HttpGet]
        [Route("get-versions")]
        public IActionResult GetDemoVersions()
        {
            var categories = _demoContainer.GetCategories();

            var dto = new MainPageDto
            {
                Categories = categories,
                ConferenceMode = _settings.ConferenceMode
            };

            return Ok(dto);
        }

        [HttpGet]
        [Route("get/{categoryName}/{demoName}")]
        public IActionResult GetDemo(string categoryName, string demoName)
        {
            try
            {
                var demo = _demoContainer.GetDemo(categoryName, demoName);
                var dto = DemoDto.FromModel(demo);

                dto.StudioUrl = GetStudioUrl(demo);

                return Ok(dto);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "An error occured during demo fetching.");
                return NotFound();
            }
        }

        private string GetStudioUrl(Demo demo)
        {
            if (demo.StudioLinkToMediaDatabase)
                return _databaseLinks.ToMediaDocuments(UserId);

            if (demo.StudioLinkToIndexList)
                return _databaseLinks.ToUserIndexes(UserId);

            return _databaseLinks.ToUserDocuments(UserId);
        }

        [HttpPost]
        [Route("reset-database")]
        public async Task<IActionResult> ResetDatabase()
        {
            await _databaseSetup.ResetUserDatabase(UserId);
            return Ok();
        }
    }
}
