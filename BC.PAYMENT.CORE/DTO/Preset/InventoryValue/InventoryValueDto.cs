

using BC.PAYMENT.CORE.DTO.General;

namespace BC.PAYMENT.CORE.DTO.Preset.InventoryValue
{
    public class InventoryValueDto
    {
        public List<BranchDTO> BranchDtos { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
