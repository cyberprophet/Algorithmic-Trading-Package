namespace ShareInvest.Models;

readonly struct File
{
    internal string? DirectoryName
    {
        get;
    }
    internal string? Version
    {
        get;
    }
    internal int MajorPart
    {
        get;
    }
    internal int MinorPart
    {
        get;
    }
    internal int BuildPart
    {
        get;
    }
    internal int PrivatePart
    {
        get;
    }
    internal File(string? directoryName,
                  string? fileVersion,
                  int majorPart,
                  int minorPart,
                  int buildPart,
                  int privatePart)
    {
        DirectoryName = directoryName;
        Version = fileVersion;
        MajorPart = majorPart;
        MinorPart = minorPart;
        BuildPart = buildPart;
        PrivatePart = privatePart;
    }
}