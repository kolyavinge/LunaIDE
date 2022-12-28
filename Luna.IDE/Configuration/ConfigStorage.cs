using System.IO;
using System.Reflection;
using SimpleDB;

namespace Luna.IDE.Configuration;

public interface IConfigStorage
{
    TConfigStoragePoco? GetById<TConfigStoragePoco>(object id) where TConfigStoragePoco : IConfigStoragePoco;
    void Save<TConfigStoragePoco>(TConfigStoragePoco poco) where TConfigStoragePoco : IConfigStoragePoco;
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

        builder.Map<RecentProjectPoco>()
            .PrimaryKey(x => x.Id)
            .Field(1, x => x.ProjectFullPath)
            .Field(2, x => x.LastAccess);

        _engine = builder.BuildEngine();
    }

    public TConfigStoragePoco? GetById<TConfigStoragePoco>(object id) where TConfigStoragePoco : IConfigStoragePoco
    {
        return _engine.GetCollection<TConfigStoragePoco>().Get(id);
    }

    public void Save<TConfigStoragePoco>(TConfigStoragePoco poco) where TConfigStoragePoco : IConfigStoragePoco
    {
        _engine.GetCollection<TConfigStoragePoco>().InsertOrUpdate(poco);
    }
}
