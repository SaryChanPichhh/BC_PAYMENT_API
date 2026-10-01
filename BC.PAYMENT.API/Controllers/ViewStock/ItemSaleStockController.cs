using BC.PAYMENT.CORE.DTO.ViewStock;
using BC.PAYMENT.CORE.Mappers;

namespace BC.PAYMENT.API.Controllers.ViewStock;

[Helper.Authorize]
public sealed class ItemSaleStockController : BaseApiController
{
    private const int ClientClosedRequestStatusCode = 499;
    private const string DefaultImageBaseUrl = "https://tdstorage.tonairedigital.net/MSE/BC/item";
    private readonly IItemSaleStockRepository _repository;
    private readonly ILogger<ItemSaleStockController> _logger;
    private readonly string _imageBaseUrl;

    public ItemSaleStockController(IItemSaleStockRepository repository, ILogger<ItemSaleStockController> logger,
        IOptions<AppSettings> appSettings)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ArgumentNullException.ThrowIfNull(appSettings);
        _imageBaseUrl = string.IsNullOrWhiteSpace(appSettings.Value.ItemImageBaseUrl)
            ? DefaultImageBaseUrl
            : appSettings.Value.ItemImageBaseUrl.Trim().TrimEnd('/');
    }

    [HttpPost("search")]
    public async Task<ApiResponse<List<ItemSaleStockDto>>> GetItemSaleStockAsync(
        [FromBody] ItemSaleStockRequestDto request, CancellationToken cancellationToken)
    {
        var response = new ApiResponse<List<ItemSaleStockDto>>
        {
            Success = false,
            Result = []
        };

        try
        {
            ArgumentNullException.ThrowIfNull(request);

            var credential = Common.DecodeJwt(HttpContext.User);

            if (credential is null ||
                string.IsNullOrWhiteSpace(credential.DbCode))
            {
                response.Message = "The authenticated database code is missing.";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return response;
            }

            request.ApplyDefaultCurrentMonth(GetCurrentBusinessDate());
            request.NormalizeAndValidate(credential.DbCode.Trim().ToUpperInvariant());

            var models = await _repository.GetItemSaleStockAsync(request, _imageBaseUrl, cancellationToken);

            response.Result = models.Select(x => x.ToListDto()).ToList();
            response.Success = true;
            response.StatusCode = (int)HttpStatusCode.OK;

            response.Message = response.Result.Count == 0
                ? "No item sale and stock data found."
                : "Item sale and stock data fetched successfully.";
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid item sale stock request.");

            response.Message = ex.Message;
            response.StatusCode =
                (int)HttpStatusCode.BadRequest;
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error while fetching item sale stock data.");
            response.Message = "A database error occurred while fetching item sale and stock data.";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
        catch (OperationCanceledException)
        {
            response.Message = "The request was cancelled.";
            response.StatusCode = ClientClosedRequestStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching item sale stock data.");
            response.Message = "An unexpected error occurred while fetching item sale and stock data.";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        return response;
    }

    private DateTime GetCurrentBusinessDate()
    {
        var currentDateValue = HttpContext.User.Claims
            .FirstOrDefault(claim => claim.Type.Equals("CurrentDate", StringComparison.OrdinalIgnoreCase))
            ?.Value;

        if (!string.IsNullOrWhiteSpace(currentDateValue))
        {
            string[] acceptedFormats = ["MM/dd/yyyy", "M/d/yyyy", "yyyy-MM-dd", "yyyyMMdd"];

            if (DateTime.TryParseExact(currentDateValue.Trim(), acceptedFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var currentDate))
                return currentDate.Date;
        }

        return DateTime.Today;
    }
}