using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetServer")]
        public async Task<string> Get()
        {


            var b = new Test()
            {
                Name= "amresh",
                Data = 1
            };

            var c = new Test();

            List<Test> a = new List<Test>();
            a.Add(b);
            a.ForEach(x =>
            {
                a.Where(y => !y.Equals(y) && y.Name == "Amresh");
            });

            return "Chat";
        }
    }

    public class Test
    {
        public string Name { get; set; }
        public int Data { get; set; }
    };
}