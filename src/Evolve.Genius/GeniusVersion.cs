namespace Evolve.Genius
{
    public record GeniusVersion(long Major1, long Major2, long Minor)
    {
        public override string ToString() => $"{Major1}.{Major2}.{Minor}";
    }

    public record GeniusFullVersion(GeniusVersion Version, long Patch)
    {
        public override string ToString() => $"{Version}.{Patch}";
    }
}
