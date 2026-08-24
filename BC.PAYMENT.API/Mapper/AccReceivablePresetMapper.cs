using BC.PAYMENT.CORE.DTO.General;
using BC.PAYMENT.CORE.DTO.Prepare.Account;
using BC.PAYMENT.CORE.Entities.Prepare.Account;

namespace BC.PAYMENT.API.Mapper;

public static class AccReceivablePresetMapper
{
    public static AccountReceivablePresetModel FromCreateDtoToModel(this AccountReceivableCreateRequest model,ClaimDTO dto)
    {
        return new AccountReceivablePresetModel
        {
            DbCode = dto.DbCode,
            CreatedBy = dto.Username,
            CreditDebitType = model.CreditDebitType,
            AccountCode = model.AccountCode,
            Description = model.Description,
            Field1 = model.Field1,
            Field2 = model.Field2,
            Field3 = model.Field3,
            Field4 = model.Field4,
            Field5 = model.Field5,
            Field6 = model.Field6,
            Field7 = model.Field7,
            Field8 = model.Field8,
            Field9 = model.Field9,
            CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
    public static AccountReceivablePresetModel ToResponse(AccountReceivableCreateRequest model)
    {
        return new AccountReceivablePresetModel
        {
            CreditDebitType = model.CreditDebitType,
            AccountCode = model.AccountCode,
            Description = model.Description,
            Field1 = model.Field1,
            Field2 = model.Field2,
            Field3 = model.Field3,
            Field4 = model.Field4,
            Field5 = model.Field5,
            Field6 = model.Field6,
            Field7 = model.Field7,
            Field8 = model.Field8,
            Field9 = model.Field9,
            CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
    public static AccountReceivablePresetModel FromUpdateDtoToModel(AccountReceivableUpdateRequest model)
    {
        return new AccountReceivablePresetModel
        {
            CreditDebitType = model.CreditDebitType,
            AccountCode = model.AccountCode,
            Description = model.Description,
            Field1 = model.Field1,
            Field2 = model.Field2,
            Field3 = model.Field3,
            Field4 = model.Field4,
            Field5 = model.Field5,
            Field6 = model.Field6,
            Field7 = model.Field7,
            Field8 = model.Field8,
            Field9 = model.Field9,
            CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }
}