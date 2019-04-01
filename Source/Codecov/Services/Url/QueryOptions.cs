namespace Codecov.Services.Url
{
    internal class QueryOptions : IQueryOptions
    {
        public QueryOptions(string branch, string build, string commit, string flags, string name, string pr, string slug, string tag, string token)
        {
            Branch = branch;
            Build = build;
            Commit = commit;
            Flags = flags;
            Name = name;
            PR = pr;
            Slug = slug;
            Tag = tag;
            Token = token;
        }

        public string Branch { get; }

        public string Build { get; }

        public string Commit { get; }

        public string Flags { get; }

        public string Name { get; }

        public string PR { get; }

        public string Slug { get; }

        public string Tag { get; }

        public string Token { get; }
    }
}
