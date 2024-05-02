using GymDomain.Model;

public interface IExportService<TEntity>
   where TEntity : Entity
{
    Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
}
