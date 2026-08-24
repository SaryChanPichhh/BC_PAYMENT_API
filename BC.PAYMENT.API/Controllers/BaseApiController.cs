using BC.PAYMENT.API.Filter;
using BC.PAYMENT.API.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers
{
    [Route("api/v2/[controller]")]
    [TypeFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
