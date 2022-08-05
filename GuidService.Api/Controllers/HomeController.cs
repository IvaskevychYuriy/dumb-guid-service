using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GuidService.Api.Models;

namespace GuidService.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly TableClient _tableClient;
        private readonly InstanceSettings _instanceSettings;

        public HomeController(
            TableSettings tableSettings, 
            InstanceSettings instanceSettings)
        {
            _tableClient = new TableClient(tableSettings.ConnectionString, TableGuidEntity.Table);
            _instanceSettings = instanceSettings;
        }

        public async Task<IActionResult> Index(CancellationToken token = default)
        {
            ViewData["GUID"] = await GenerateGuidAsync(HttpContext, token);
            ViewData["InstanceId"] = _instanceSettings.InstanceId;
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
                InstanceId = _instanceSettings.InstanceId,
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
        public string InstanceId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}