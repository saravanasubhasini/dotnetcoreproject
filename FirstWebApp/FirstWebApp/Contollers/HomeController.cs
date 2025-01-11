using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts.Enums;
using Services;

namespace FirstWebApp.Contollers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        // private readonly IConfiguration _configuration;

        private readonly WeatherApiOption _configuration;

        private readonly StockService _stockService;

        private readonly IOptions<StocksOption> _stocksOption;

        public HomeController(IWebHostEnvironment webHostEnvironment, 
            IOptions<WeatherApiOption> configuration, StockService stockService,
          IOptions<StocksOption> stocksOption)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration.Value;
            _stockService = stockService;
            _stocksOption = stocksOption;
        }

        //[Route("/")]

        public async Task<IActionResult> Index()
        {
            //ViewBag.EnvironmentName = _webHostEnvironment.EnvironmentName;

            //ViewBag.myKeyConfiguration =  _configuration.GetValue<string>("MyKey");

            //Multiple Hierarchi

            //IConfigurationSection weatherpi = _configuration.GetSection("weatherapi");

            //ViewBag.ID = weatherpi["ClientID"];

            //ViewBag.Secret = weatherpi["ClientSecret"];



            ////Option Pattern

            //WeatherApiOption? weatherpi = _configuration.GetSection("weatherapi").Get<WeatherApiOption>();
            //ViewBag.ID = weatherpi?.ClientID;
            //ViewBag.Secret = weatherpi?.ClientSecret;


            //Configuration as Service
            //ViewBag.ID = _configuration.ClientID;
            //ViewBag.Secret = _configuration.ClientSecret;

            if (_stocksOption.Value.DefaultSymbolValue == null)
            {
                _stocksOption.Value.DefaultSymbolValue = "MSFT";
            }

            Dictionary<string,object>? response =  await _stockService.StockServiceResquestAndResponse
                (_stocksOption.Value.DefaultSymbolValue);
            if (response == null)
                throw new InvalidOperationException("Invalid operation");

            Stock stock = new Stock()
            {
                SymbolValue = _stocksOption.Value.DefaultSymbolValue,
                CurrentPrice = Convert.ToDouble(response["c"].ToString()),
                OpenPrice = Convert.ToDouble(response["o"].ToString()),
                HighestPrice = Convert.ToDouble(response["h"].ToString()),
                LowestPrice = Convert.ToDouble(response["l"].ToString()),


            };



            return View(stock);
        }

        [Route("Others")]
        public IActionResult Test()
        {
            return View();
        }
    }
}
