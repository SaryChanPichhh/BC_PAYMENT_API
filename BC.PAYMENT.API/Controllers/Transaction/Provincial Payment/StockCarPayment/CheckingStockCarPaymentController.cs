using BC.PAYMENT.APPLICATION.Interfaces.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers.Transaction.Provincial_Payment.StockCarPayment
{
    public class CheckingStockCarPaymentController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckingStockCarPaymentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Submitting

        [HttpGet]
        [Route("getsubmittinginvoice")]
            

        #endregion
    }
}
