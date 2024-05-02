using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymInfrastructure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private readonly DbgymContext _context;
        public ChartController(DbgymContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var categories = _context.Gyms.Include(gc => gc.Members).ToList();
            List<object> gymMember = new List<object>();
            gymMember.Add(new[] { "Адреса залу", "Кількість клієнтів" });

            foreach (var c in categories)
            {
                gymMember.Add(new object[] { c.Adress, c.Members.Count() });
            }
            return new JsonResult(gymMember);
        }
    }

}
