namespace GFS;

public class SectorMetadataDTO
{

    public bool IsTaken
    {
        get;
        set;
    }
    public long Hash { get; set; }

    public override string ToString()
    {
        return $"{IsTaken} {Hash}";
    }
}