namespace BC.PAYMENT.CORE.Mappers;

public static class MarketMapper
{
    public static MarketModel ToMarketModel(this MarketCreateRequest request)
    {
        if (request == null) return null!;

        return new MarketModel
        {
            MarketId = request.MarketId,
            MarketName = request.MarketName,
            MarketNameKhmer = request.MarketNameKhmer,
            AreaId = request.AreaId,
            DistrictId = request.DistrictId,
            ProvinceId = request.ProvinceId,
            Image = request.Image,
            Other = request.Other,
            Status = true // Default status for creation
        };
    }

    public static MarketModel ToMarketModel(this MarketUpdateRequest request)
    {
        if (request == null) return null!;

        return new MarketModel
        {
            MarketId = request.MarketId,
            MarketName = request.MarketName,
            MarketNameKhmer = request.MarketNameKhmer,
            AreaId = request.AreaId,
            DistrictId = request.DistrictId,
            ProvinceId = request.ProvinceId,
            Image = request.Image,
            Other = request.Other,
            Status = request.Status
        };
    }

    public static MarketResponse ToMarketResponse(this MarketModel model)
    {
        if (model == null) return null!;

        // Note: Image might require custom conversion depending on your needs.
        return new MarketResponse
        {
            MarketId = model.MarketId,
            MarketName = model.MarketName,
            MarketNameKhmer = model.MarketNameKhmer,
            AreaId = model.AreaId,
            AreaName = model.AreaName,
            District = model.DistrictName,
            Province = model.ProvinceName,
            Status = model.Status,
            Other = model.Other,
            Image = null! // Update this to map to your base64 image or string path
        };
    }
}