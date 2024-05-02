using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColumnChartController : ControllerBase
    {
        private readonly DbgymContext _context;

        public ColumnChartController(DbgymContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData2")]
        public JsonResult JsonData2()
        {
            var classes = _context.GroupClasses.Include(gc => gc.Members).ToList();
            List<object> classMembers = new List<object>();
            classMembers.Add(new[] { "Груповий клас", "Кількість клієнтів" });

            foreach (var c in classes)
            {
                classMembers.Add(new object[] { c.Name, c.Members.Count() });
            }

            return new JsonResult(classMembers);
        }
    }
}
