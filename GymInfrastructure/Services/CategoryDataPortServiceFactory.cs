using GymDomain.Model;

namespace GymInfrastructure.Services
{
    public class CategoryDataPortServiceFactory
        : IDataPortServiceFactory<Gym>
    {
        private readonly DbgymContext _context;
        public CategoryDataPortServiceFactory(DbgymContext context)
        {
            _context = context;
        }
        public IImportService<Gym> GetImportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CategoryImportService(_context);
            }
            throw new NotImplementedException($"No import service implemented for movies with content type {contentType}");
        }
        public IExportService<Gym> GetExportService(string contentType)
        {
            if (contentType is "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CategoryExportService(_context);
            }
            throw new NotImplementedException($"No export service implemented for movies with content type {contentType}");
        }
    }

}
