namespace Codecov.Services
{
    internal struct ReportFile
    {
        public ReportFile(string file, string content)
        {
            File = file;
            Content = content;
        }

        public string Content { get; }

        public string File { get; }
    }
}
