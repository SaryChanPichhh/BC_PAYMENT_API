using BC.PAYMENT.API.MapperHelper.ViewStock;
using BC.PAYMENT.APPLICATION.Interfaces.ViewStock;
using BC.PAYMENT.CORE.DTO.ViewStock;

namespace BC.PAYMENT.API.Controllers.ViewStock;

[Helper.Authorize]
[ApiController]
public sealed class ViewStockController(IViewStockupRepository repository, ILogger<ViewStockController> logger)
    : BaseApiController
{
    private const int ClientClosedRequestStatusCode = 499;

    private readonly IViewStockupRepository _repository =
        repository ?? throw new ArgumentNullException(nameof(repository));

    private readonly ILogger<ViewStockController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("sales-types")]
    public Task<ApiResponse<List<SalesTypeDto>>> GetSalesTypesAsync([FromBody] ViewStockRequestDto request,
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            async () =>
            {
                ArgumentNullException.ThrowIfNull(request);
                var dbCodes = request.NormalizeAndValidate();
                var models = await _repository.GetSalesTypesAsync(dbCodes, cancellationToken);
                return models.ToDtoList();
            },
            "Sales types fetched successfully.",
            "No active sales types found.");
    }

    [HttpGet("branches")]
    public Task<ApiResponse<List<BranchDto>>> GetDatabasesAsync(CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            async () =>
            {
                var models = await _repository.GetDatabasesAsync(cancellationToken);
                return models.ToDtoList();
            },
            "Database list fetched successfully.",
            "No active databases found.");
    }

    [HttpPost("warehouses")]
    public Task<ApiResponse<List<WarehouseDto>>> GetWarehousesAsync([FromBody] ViewStockRequestDto request,
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            async () =>
            {
                ArgumentNullException.ThrowIfNull(request);
                var dbCodes = request.NormalizeAndValidate();
                var models = await _repository.GetWarehousesAsync(dbCodes, cancellationToken);
                return models.ToDtoList();
            },
            "Warehouse list fetched successfully.",
            "No warehouses found.");
    }

    [HttpPost("areas")]
    public Task<ApiResponse<List<AreaDto>>> GetAreasAsync([FromBody] ViewStockRequestDto request,
        CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            async () =>
            {
                ArgumentNullException.ThrowIfNull(request);
                var dbCodes = request.NormalizeAndValidate();
                var models = await _repository.GetAreasAsync(dbCodes, cancellationToken);
                return models.ToDtoList();
            },
            "Area list fetched successfully.", "No areas found.");
    }

    private async Task<ApiResponse<List<TDto>>> ExecuteAsync<TDto>(Func<Task<List<TDto>>> action, string successMessage,
        string emptyMessage)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            var result = await action();

            return CreateResponse(
                true,
                HttpStatusCode.OK,
                result.Count == 0 ? emptyMessage : successMessage,
                result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid view-stock lookup request.");

            return CreateResponse<TDto>(
                false,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error while fetching view-stock lookup data.");

            return CreateResponse<TDto>(
                false,
                HttpStatusCode.InternalServerError,
                "A database error occurred while fetching lookup data.");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "View-stock lookup request was cancelled.");

            return new ApiResponse<List<TDto>>
            {
                Success = false,
                StatusCode = ClientClosedRequestStatusCode,
                Message = "The request was cancelled.",
                Result = []
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching view-stock lookup data.");

            return CreateResponse<TDto>(false, HttpStatusCode.InternalServerError,
                "An unexpected error occurred while fetching lookup data.");
        }
    }

    private static ApiResponse<List<TDto>> CreateResponse<TDto>(bool success, HttpStatusCode statusCode, string message,
        List<TDto>? result = null)
    {
        return new ApiResponse<List<TDto>>
        {
            Success = success,
            StatusCode = (int)statusCode,
            Message = message,
            Result = result ?? []
        };
    }
}