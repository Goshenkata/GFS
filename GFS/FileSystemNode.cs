using GFS.Structures;

namespace GFS
{
    public class FileSystemNode
    {

        public static int ELEMENT_SIZE = sizeof(bool) * 2 + sizeof(char) * 50 + sizeof(int) * (CHILDREN_DATA_SECTORS_LENGTH + 4);
        public const int NAME_LENGTH = 50;
        public const int CHILDREN_DATA_SECTORS_LENGTH = 100;

        //saved
        public bool IsDirectory { get; }
        public bool IsCorrupted { get; set; }
        public int ParentID { get; set; }
        public int Indx { get; set; }
        public int LastDataIndexOfChildrenSector { get; set; }
        public int LastDataIndexOfFile { get; set; }

        public char[] _nameArr;
        private int[] _childrenSectorIds;



        public string Name
        {
            get
            {
                return new string(_nameArr);
            }
            set
            {
                if (value.Length < NAME_LENGTH)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        _nameArr[i] = value[i];
                    }
                    for (int i = value.Length; i < NAME_LENGTH; i++)
                    {
                        _nameArr[i] = '\0';
                    }
                }
            }
        }

        public MyList<int> ChildrenSectorIds
        {
            get
            {
                MyList<int> output = new MyList<int>();
                foreach (int el in _childrenSectorIds)
                {
                    if (el == -1)
                        return output;
                    output.AddLast(el);
                }
                return output;
            }
            set
            {
                if (value.Count < CHILDREN_DATA_SECTORS_LENGTH)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        _childrenSectorIds[i] = value[i];
                    }
                    for (int i = value.Count; i < CHILDREN_DATA_SECTORS_LENGTH; i++)
                    {
                        _childrenSectorIds[i] = -1;
                    }
                }
            }
        }

        public FileSystemNode(bool isDirectory, bool isCorrupted, int parentId, int indx, int lastDataOfChildrenSector, int lastDataIndexOfFile, string name)
        {
            _nameArr = new char[NAME_LENGTH];
            Array.Fill(_nameArr, '\0');

            _childrenSectorIds = new int[100];
            Array.Fill(_childrenSectorIds, -1);

            IsDirectory = isDirectory;
            IsCorrupted = isCorrupted;
            ParentID = parentId;
            Indx = indx;
            LastDataIndexOfChildrenSector = lastDataOfChildrenSector;
            LastDataIndexOfFile = lastDataIndexOfFile;
            Name = name;
            ChildrenSectorIds = new MyList<int>();
        }




        public override string ToString()
        {
            return $"{Name} {IsDirectory} {IsCorrupted}";
        }



        public bool Equals(FileSystemNode node)
        {
            if (node == null)
                return false;

            return Name == node.Name && Indx == node.Indx;
        }
    }
}
