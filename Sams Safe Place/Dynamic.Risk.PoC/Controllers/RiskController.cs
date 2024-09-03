using Dynamic.Risk.Domain;
using Dynamic.Risk.PoC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace Dynamic.Risk.PoC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RiskController : ControllerBase
    {
        private static JsonSerializerOptions _deserializationOptions = new JsonSerializerOptions
        {
            Converters = { new ParseTypeConverter() },
            PropertyNameCaseInsensitive = true
        };

        private static RiskEnrichmentService _riskEnrichmentService = new RiskEnrichmentService();

        [HttpPost]
        public async Task<IActionResult> ReadBody()
        {
            Request.EnableBuffering();

            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                
                Request.Body.Position = 0;
                var riskDictionary = JsonSerializer.Deserialize<Dictionary<string, RiskEntry>>(body, _deserializationOptions);

                _riskEnrichmentService.Enrich(riskDictionary);
                
                return Content("Cheers!");
            }
        }
    }
}
