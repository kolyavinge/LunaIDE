namespace Luna.IDE.Versioning;

internal interface IVersionControlRepositoryFactory
{
    bool IsRepositoryExist(string projectPath);

    VersionControl.Core.IVersionControlRepository OpenRepository(string projectPath);

    VersionControl.Core.IVersionControlRepository GetDummyRepository();
}

internal class VersionControlRepositoryFactory : IVersionControlRepositoryFactory
{
    public bool IsRepositoryExist(string projectPath)
    {
        return VersionControl.Core.VersionControlRepositoryFactory.IsRepositoryExist(projectPath);
    }

    public VersionControl.Core.IVersionControlRepository OpenRepository(string projectPath)
    {
        return VersionControl.Core.VersionControlRepositoryFactory.OpenRepository(projectPath);
    }

    public VersionControl.Core.IVersionControlRepository GetDummyRepository()
    {
        return VersionControl.Core.DummyVersionControlRepository.Instance;
    }
}
