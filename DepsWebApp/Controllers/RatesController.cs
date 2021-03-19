using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DepsWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace DepsWebApp.Controllers
{
    /// <summary>
    /// Currency exchanger
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RatesController : ControllerBase
    {
        private readonly ILogger<RatesController> _logger;
        private readonly IRatesService _rates;

#pragma warning disable CS1591
        public RatesController(
            IRatesService rates,
            ILogger<RatesController> logger)
        {
            _rates = rates;
            _logger = logger;
        }
#pragma warning restore CS1591

        /// <summary>
        /// Currency exchange method. Returns converted amount of money or exchange rate between currencies if money amount wasn't provided
        /// </summary>
        /// <param name="srcCurrency">Source currency code</param>
        /// <param name="dstCurrency">Desired currency code</param>
        /// <param name="amount">Amount of money in source currency</param>
        /// <returns>Returns converted amount of money or exchange rate between currencies if money amount wasn't provided</returns>
        [HttpGet("{srcCurrency}/{dstCurrency}")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<decimal>> Get(string srcCurrency, string dstCurrency, decimal? amount)
        {
            var exchange =  await _rates.ExchangeAsync(srcCurrency, dstCurrency, amount ?? decimal.One);
            if (!exchange.HasValue)
            {
                _logger.LogDebug($"Can't exchange from '{srcCurrency}' to '{dstCurrency}'");
                return BadRequest("Invalid currency code");
            }
            return exchange.Value.DestinationAmount;
        }
    }
}
