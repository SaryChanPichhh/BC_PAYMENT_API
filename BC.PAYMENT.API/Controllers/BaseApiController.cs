using AuthorizeAttribute = BC.PAYMENT.API.Helper.AuthorizeAttribute;

namespace BC.PAYMENT.API.Controllers;

[Route("api/v2/[controller]")]
[TypeFilter(typeof(AuthorizeAttribute))]
[ApiController]
public class BaseApiController : ControllerBase
{
}