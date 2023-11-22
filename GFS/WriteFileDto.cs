namespace GFS;

public class WriteFileDto
{
    public int[] Sectors { get; set; }
    public int LastDataIndex { get; set; }
    public bool IsCorrupted { get; set; }

    public WriteFileDto(int[] sectors, int lastDataIndex, bool isCorrupted = false)
    {
        Sectors = sectors;
        LastDataIndex = lastDataIndex;
    }
}