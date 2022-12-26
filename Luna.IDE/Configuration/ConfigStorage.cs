using System.IO;
using System.Reflection;
using SimpleDB;

namespace Luna.IDE.Configuration;

public interface IConfigStorage
{
    TEntity? GetById<TEntity>(object id);
    void Save<TEntity>(TEntity poco);
}

public class ConfigStorage : IConfigStorage
{
    private readonly IDBEngine _engine;

    public ConfigStorage()
    {
        var builder = DBEngineBuilder.Make();

        builder.DatabaseFilePath(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ?? "", "config"));

        builder.Map<LastOpenedProjectFilesPoco>()
            .PrimaryKey(x => x.ProjectFullPath)
            .Field(1, x => x.FilesRelativePathes);

        _engine = builder.BuildEngine();
    }

    public TEntity? GetById<TEntity>(object id)
    {
        return _engine.GetCollection<TEntity>().Get(id);
    }

    public void Save<TEntity>(TEntity poco)
    {
        _engine.GetCollection<TEntity>().InsertOrUpdate(poco);
    }
}
