using System.Text;

namespace GFS;

public class SectorNode
{
    public bool isFree;
    public long hash;
    public byte[] data;

    public override string ToString()
    {
        return $"{isFree} {hash} {Encoding.UTF8.GetString(data)}";
    }
}