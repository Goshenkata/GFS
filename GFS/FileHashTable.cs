using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFS
{
    public class FileHashTable : StreamArray
    {
        private int _lastHashIndex = -1;
        private long _totalNumberOfSectors = 0;
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

        public static  long OFFSET = sizeof(long);
        public static long ELEMENT_SIZE = sizeof(long) + sizeof(int);

        public struct HashSectorIndxPair
        {
            public long Hash;
            public int Index;
        }
        public FileHashTable(long totalNumberOfSectors,long dataStart, long dataEnd, FileStream fs, BinaryWriter bw, BinaryReader br) : base(dataStart, dataEnd, fs, bw, br)
        {
            _fs.Seek(dataStart, SeekOrigin.Begin);
            _lastHashIndex = _br.ReadInt32();
            _totalNumberOfSectors = totalNumberOfSectors; 
        }
        public int getIdWithSameHash(long hash)
        {
            var result = BinarySearch(hash, 0, LastHashIndex);
            if (result == -1)
            {
                return -1;
            }
            return _br.ReadInt32();
        }
        public void RemoveHash(long hash)
        {
            int removePosition = FindInsertPosition(hash);
            if (removePosition == -1) {
                return;
            }


            for (int i = removePosition + 1; i <= LastHashIndex; i++)
            {
                _fs.Seek(_dataStart + OFFSET + i * ELEMENT_SIZE, SeekOrigin.Begin);
                long currentHash = _br.ReadInt64();
                int currentIndx = _br.ReadInt32();
                _fs.Seek( -2 * ELEMENT_SIZE, SeekOrigin.Current);
                _bw.Write(currentHash);
                _bw.Write(currentIndx);
            }
            LastHashIndex = _lastHashIndex - 1;
        }
        public void SaveHash(long hash, int index)
        {
            int insertPosition = FindInsertPosition(hash);

            if (insertPosition == -1)
            {
                return;
            }

            LastHashIndex = _lastHashIndex + 1;
            // Calculate the number of elements to shift
            int elementsToShift = LastHashIndex - insertPosition;

            if (elementsToShift > 0)
            {
            // Read the data to be shifted
            _fs.Seek(_dataStart + OFFSET + insertPosition * ELEMENT_SIZE, SeekOrigin.Begin);
            byte[] dataToShift = new byte[elementsToShift * ELEMENT_SIZE];
            _fs.Read(dataToShift, 0, dataToShift.Length);

            // Write the shifted data
            _fs.Seek(_dataStart + OFFSET + (insertPosition + 1) * ELEMENT_SIZE, SeekOrigin.Begin);
            _fs.Write(dataToShift, 0, dataToShift.Length);
            }

            // Write the new hash at the correct position
            _fs.Seek(_dataStart + OFFSET + insertPosition * ELEMENT_SIZE, SeekOrigin.Begin);
            _bw.Write(hash);
            _bw.Write(index);
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
                    return mid;
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
            } else if (value.CompareTo(hash) > 0)
            {
                return BinarySearch(hash, start, mid - 1); ;
            } else
            {
                return BinarySearch(hash, mid + 1, end);
            }
        }

        public HashSectorIndxPair GetBySectorId(int sectorId)
        {
            _fs.Seek(_dataStart + OFFSET, SeekOrigin.Begin);
            for (int i = 0; i <= LastHashIndex; i++)
            {
                long hash = _br.ReadInt64();
                int curSectorID = _br.ReadInt32();
                if (curSectorID == sectorId)
                {
                    return new HashSectorIndxPair { Hash = hash, Index = curSectorID };
                }
            }
            return new HashSectorIndxPair { Hash = -1, Index = -1 };
        }
        public void InitData()
        {
            _fs.Seek(_dataStart, SeekOrigin.Begin);
            _bw.Write(-1L);
            _lastHashIndex = -1;
            for (int i = 0; i < _totalNumberOfSectors; i++)
            {
                _bw.Write(-1L);
                _bw.Write(-1);
            }
        }
        public void PrintAll()
        {
            _fs.Seek(_dataStart + OFFSET, SeekOrigin.Begin);
            for (int i =0; i<= LastHashIndex; i++) {
                var hash = _br.ReadInt64();
                var indx = _br.ReadInt32();
                Console.WriteLine($"{i}: {hash} {indx}");
            }
        }
    }
}
