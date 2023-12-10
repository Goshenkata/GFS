namespace GFS;

public abstract class StreamArray
{
    public BinaryWriter _bw;
    public FileStream _fs;
    public BinaryReader _br;
    protected long _dataStart;
    private long _dataEnd;

    public StreamArray(long dataStart, long dataEnd,
         FileStream fs, BinaryWriter bw, BinaryReader br)
    {
        _fs = fs;
        _bw = bw;
        _br = br;
        _fs.Seek(dataStart, SeekOrigin.Begin);
        this._dataStart = dataStart;
        this._dataEnd = dataEnd;
    }
}