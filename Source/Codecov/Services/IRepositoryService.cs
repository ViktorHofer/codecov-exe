namespace Codecov.Services
{
    internal interface IRepositoryService
    {
        string Branch { get; }

        string Commit { get; }

        string PR { get; }

        string Slug { get; }

        string Tag { get; }
    }
}
