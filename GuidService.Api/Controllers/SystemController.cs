using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

namespace GuidService.Api.Controllers
{
    public class SystemController : Controller
    {
        private readonly TableClient _tableClient;

        public SystemController(TableSettings tableSettings)
        {
            _tableClient = new TableClient(tableSettings.ConnectionString, TableGuidEntity.Table);
        }

        [HttpGet("health")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Health(CancellationToken token = default)
        {
            await _tableClient
                .QueryAsync<TableGuidEntity>(x => x.PartitionKey == TableGuidEntity.Partition, maxPerPage: 1, cancellationToken: token)
                .GetAsyncEnumerator(token)
                .MoveNextAsync();

            return Ok();
        }
    }
}