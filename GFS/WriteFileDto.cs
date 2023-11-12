namespace GFS;

public class WriteFileDto
{
    public int[] Sectors { get; set; }
    public int LastDataIndex { get; set; }

    public WriteFileDto(int[] sectors, int lastDataIndex)
    {
        Sectors = sectors;
        LastDataIndex = lastDataIndex;
    }
}