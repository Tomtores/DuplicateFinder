namespace DuplicateFinder
{
    internal class FindParameters
    {
        public FindParameters(string[] paths, string[] ignored, FinderSettings settings)
        {
            Paths = paths;
            Ignored = ignored;
            Settings = settings;
        }

        public string[] Paths { get; private set; }
        public string[] Ignored { get; private set; }
        public FinderSettings Settings { get; private set; }
    }
}