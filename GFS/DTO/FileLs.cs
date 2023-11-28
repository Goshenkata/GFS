namespace GFS.DTO
{
    public struct FileLs
    {
        public string Name;
        public bool IsDirectory;
        public bool IsCorrupted;

        public override string? ToString()
        {
            var type = IsDirectory ? "D" : "F";
            return $"{Name} :{type}";
        }
    }
}
