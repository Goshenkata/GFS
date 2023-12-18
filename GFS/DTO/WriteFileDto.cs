namespace GFS.DTO;

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
    public override string ToString()
    {
        var sector = "";
        foreach (var item in Sectors)
        {
            sector += item + " ";
        }
        return $"lastDataIndx: {LastDataIndex}\n sectors: {sector}\n is corrupter: {IsCorrupted}";
    }
}