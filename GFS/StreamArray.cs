namespace GFS;

public abstract class StreamArray
{
    protected FileSystemNode _root;
    protected BinaryWriter _bw;
    protected Stream _fs;
    protected BinaryReader _br;
    protected long _dataStart, _dataEnd;

    public StreamArray(long dataStart, long dataEnd,
        Stream fs, BinaryWriter bw, BinaryReader br)
    {
        _fs = fs;
        _fs.Seek(dataStart, SeekOrigin.Begin);
        _bw = bw;
        _br = br;
        _root = new FileSystemNode("/", "root", true);
        this._dataStart = dataStart;
        this._dataEnd = dataEnd;
    }
}