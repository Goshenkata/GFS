using System.Text;

namespace GFS;

public class SectorNode
{
    public bool IsTaken;
    public long hash;
    public byte[] data;

    public override string ToString()
    {
        return $"{IsTaken} {hash} {Encoding.UTF8.GetString(data)}";
    }
}