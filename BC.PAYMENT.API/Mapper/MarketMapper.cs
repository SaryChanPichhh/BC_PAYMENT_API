using System.Drawing;
using BC.PAYMENT.CORE.Contracts.Request.Market;
using BC.PAYMENT.CORE.Contracts.Response.Market;
using BC.PAYMENT.CORE.Entities.Setting.Preset;

namespace BC.PAYMENT.API.Mapper;

public static class MarketMapper
{
            /// <summary>
            /// Convert a MarketUpdateRequest (API) to a MarketModel (core entity).
            /// </summary>
            public static MarketModel ToMarketModel(this MarketUpdateRequest? request, string dbCode, string username)
            {
                if (request == null) return null!;
                return new MarketModel
                {
                    MarketId   = request.MarketId,
                    MarketName = request.MarketName,
                    MarketNameKhmer = request.MarketNameKhmer,
                    AreaId     = request.AreaId,
                    DistrictId = request.DistrictId,
                    ProvinceId = request.ProvinceId,
                    Image      = request.Image,
                    Other      = request.Other,
                    Map        = request.Map,
                    AnadCode   = request.AnadCode,
                    DbCode     = dbCode,
                    UpdatedBy  = username,
                    UpdatedAt  = DateTime.Now,
                    Status     = request.Status
                };
            }
            /// <summary>
            /// Convert a MarketModel (core) to a MarketResponse (API).
            /// </summary>
            public static MarketResponse ToMarketResponse(this MarketModel model)
            {
                if (model == null) return null!;
                return new MarketResponse
                {
                    MarketId        = int.TryParse(model.MarketId, out var id) ? id : 0,
                    MarketName      = model.MarketName,
                    MarketNameKhmer = model.MarketNameKhmer,
                    AreaId          = model.AreaId,
                    Other           = model.Other,
                    Status          = model.Status,
                };
            }
            /// <summary>
            /// Helper: convert System.Drawing.Image → byte[].
            /// </summary>
            private static byte[] ImageToByteArray(Image image)
            {
                if (image == null) return Array.Empty<byte>();
                using var ms = new MemoryStream();
                // JPEG works for most cases; adjust format if needed.
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
}