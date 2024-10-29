using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TradePositionSimulator.Services;

namespace TradePositionSimulator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICalculatorService _calculatorService;

        [BindProperty]
        public CalculatorInputModel Input { get; set; } = new()
        {
            InitialBalance = 1000,
            FinalBalance = 3500,
            HighestLiquidationPrice = 0.5,
            MinimumIterations = 0,
            MaximumIterations = 10,
            LowestActualPurchasePrice = 0.75,

            InitialBuyStart = 0.5,
            InitialBuyEnd = 1.0,
            InitialBuyStep = 0.1,

            LeverageStart = 1.0,
            LeverageEnd = 20.0,
            LeverageStep = 0.1,

            BuyPercentageStart = 0.1,
            BuyPercentageEnd = 2.0,
            BuyPercentageStep = 0.1,

            SellPercentageStart = 0.0,
            SellPercentageEnd = 1.0,
            SellPercentageStep = 0.1,

            DrawdownStart = 0.1,
            DrawdownEnd = 0.9,
            DrawdownStep = 0.1,

            InitialPrice = 1,
            FinalPrice = 2
        };

        public string Results { get; private set; } = "";

        public IndexModel(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                Results = _calculatorService.CalculateOptimalSettings(Input);
            }
            catch (Exception ex)
            {
                Results = $"Error: {ex.Message}";
            }

            return Page();
        }

        public IActionResult OnGetRunConfiguration([FromQuery] double initialBuyPercentage,
            [FromQuery] double leverage, [FromQuery] double drawdown,
            [FromQuery] double buyPercentage, [FromQuery] double sellPercentage)
        {
            try
            {
                var logs = _calculatorService.RunSpecificConfiguration(
                    initialBuyPercentage,
                    leverage,
                    drawdown,
                    buyPercentage,
                    sellPercentage,
                    Input);

                return new JsonResult(new { logs });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message }) { StatusCode = 500 };
            }
        }
    }
}