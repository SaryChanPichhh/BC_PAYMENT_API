using BC.PAYMENT.API.Filter;
using BC.PAYMENT.API.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BC.PAYMENT.API.Controllers
{
    [Route("api/[controller]")]
    [TypeFilter(typeof(AuthorizeAttribute))]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
