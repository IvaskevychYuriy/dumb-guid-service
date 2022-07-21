using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TableClient _tableClient;

        public HomeController(ILogger<HomeController> logger, TableSettings tableSettings)
        {
            _logger = logger;
            _tableClient = new TableClient(tableSettings.ConnectionString, TableGuidEntity.Table);
        }

        public async Task<IActionResult> Index(CancellationToken token = default)
        {
            ViewData["GUID"] = await GenerateGuidAsync(HttpContext, token);
            return View();
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate(CancellationToken token = default)
        {
            var guid = await GenerateGuidAsync(HttpContext, token);
            return Ok(guid);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GenerateGuidAsync(HttpContext httpContext, CancellationToken token)
        {
            var guid = Guid.NewGuid().ToString();
            await WriteTableEntityAsync(httpContext, guid, token);
            return guid;
        }

        private Task WriteTableEntityAsync(HttpContext httpContext, string guid, CancellationToken token)
            => _tableClient.AddEntityAsync(new TableGuidEntity
            {
                PartitionKey = TableGuidEntity.Partition,
                RowKey = guid,
                RemoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString()
            }, token);
    }

    public class TableGuidEntity : ITableEntity
    {
        public const string Table = "guidtable";
        public const string Partition = "GeneratedGuids";

        public string RemoteIpAddress { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; } // holds generated guid
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}