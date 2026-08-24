using System.Security.Claims;
using BC.PAYMENT.API.Helper;
using BC.PAYMENT.CORE.DTO.ClosingInventoryAndInvoice.ClosingInventory;
using BC.PAYMENT.CORE.Entities.ClosingInventoryAndInvoice.ClosingInventory;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.API.Mapper
{
    public static class MapperDto
    {
        public static NewItemModel ToNewItemModel(NewItemDto? dto,ClaimsPrincipal User)
        {
            if (dto is not null)
                return new NewItemModel
                {
                    ItemCode = dto.ItemCode,
                    Location = dto.Location,
                    DbCode = dto.DbCode,
                    CreatedBy = Common.DecodeJwt(User).Username
                };
            return new NewItemModel();
        }
        public static List<ClosingStockEntryModel> ToClosingInventoryModel(List<ClosingInventoryDto.ClosingInventoryPostDto>? dto,ClaimsPrincipal User)
        {
            if (dto is not null)
                return dto.Select(x => new ClosingStockEntryModel
                {
                    DbCode = x.DbCode,
                    ClosingDate = x.TransactionDate,
                    Location = x.Location,
                    ItemCode = x.ItemCode,
                    Order = x.Order,
                    Print = x.Print,
                    OpeningBalance = x.OpeningBalance,
                    PurchaseOrder = x.PurchaseOrder,
                    Sale = x.Sale,
                    Transfer = x.Transfer,
                    CreditNote = x.CreditNote,
                    InventoryAdjustment = x.InventoryAdjustment,
                    CreatedBy = Common.DecodeJwt(User).Username,
                    CreatedDate = DateTime.Today,
                    ClosingEntryType = ClosingEntryType.Daily,
                }).ToList();
            return new List<ClosingStockEntryModel>();
        } 
        public static List<ClosingStockEntryModel> ToMonthlyClosingInventoryModel(List<ClosingInventoryDto.MonthlyClosingPostDto>? dto,ClaimsPrincipal User)
        {
            if (dto is not null)
                return dto.Select(x => new ClosingStockEntryModel
                {
                    DbCode = x.DbCode,
                    ClosingDate = x.TransactionDate,
                    Location = x.Location,
                    ItemCode = x.ItemCode,
                    Order = x.Order,
                    Print = x.Print,
                    OpeningBalance = x.OpeningBalance,
                    PurchaseOrder = x.PurchaseOrder,
                    Sale = x.Sale,
                    Transfer = x.Transfer,
                    CreditNote = x.CreditNote,
                    InventoryAdjustment = x.InventoryAdjustment,
                    CreatedBy = Common.DecodeJwt(User).Username,
                    CreatedDate = DateTime.Today,
                    ClosingEntryType = ClosingEntryType.Daily,
                }).ToList();
            return new List<ClosingStockEntryModel>();
        }

    }
}
