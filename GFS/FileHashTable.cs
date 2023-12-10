using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFS
{
    public class FileHashTable : StreamArray
    {
        private int _lastHashIndex = 0;
        public int LastHashIndex
        {
            get { return _lastHashIndex; }
            set
            {
                _lastHashIndex = value;
                _fs.Seek(_dataStart, SeekOrigin.Begin);
                _bw.Write(value);
            }
        }

        private const long OFFSET = sizeof(long);
        private const long ELEMENT_SIZE = sizeof(long) + sizeof(int);

        struct HashSectorIndxPair
        {
            public long Hash;
            public int Index;
        }
        public FileHashTable(long dataStart, long dataEnd, FileStream fs, BinaryWriter bw, BinaryReader br) : base(dataStart, dataEnd, fs, bw, br)
        {
            _fs.Seek(dataStart, SeekOrigin.Begin);
            _lastHashIndex = _br.ReadInt32();
        }
        public int getIdWithSameHash(long hash)
        {
            BinarySearch(hash, 0, LastHashIndex);
            return _br.ReadInt32();
        }
        public void SaveHash(long hash, int index)
        {
            int insertPosition = FindInsertPosition(hash);
            if (insertPosition == -1) {
                return; 
            }
            _fs.Seek(_dataStart + OFFSET + LastHashIndex * ELEMENT_SIZE, SeekOrigin.Begin);
            for (int i = LastHashIndex; i >= insertPosition; i++) {
                long currentHash = _br.ReadInt64();
                int currentIndx = _br.ReadInt32();
                _bw.Write(currentHash);
                _bw.Write(currentIndx);
                //after shigting by one, go back on the previous element before the move
                _fs.Seek(-3 * ELEMENT_SIZE, SeekOrigin.Current);
            }
        }

        private int FindInsertPosition(long hash)
        {
            int start = 0;
            int end = LastHashIndex;
            while (start <= end)
            {
                int mid = (start + end) / 2;
                _fs.Seek(_dataStart + OFFSET + mid * ELEMENT_SIZE,  SeekOrigin.Begin);
                long valAtMid = _br.ReadInt64();

                if (valAtMid == hash)
                {
                    return -1;
                }
                else if (valAtMid < hash)
                {
                    start = mid + 1;
                }
                else
                {
                    end = mid - 1;
                }
            }
            return start;
        }

        public int BinarySearch(long hash, int start, int end)
        {
            if (start > end)
            {
                return -1;
            }
            int mid = (start + end) / 2;
            _fs.Seek(_dataStart + OFFSET + mid * ELEMENT_SIZE,  SeekOrigin.Begin);
            long value = _br.ReadInt64();
            if (value == hash)
            {
                return mid;
            } else if (value > hash)
            {
                return BinarySearch(hash, start, mid - 1); ;
            } else
            {
                return BinarySearch(hash, mid + 1, end);
            }
        }
    }

}
