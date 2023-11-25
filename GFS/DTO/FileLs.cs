using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFS.DTO
{
    public struct FileLs
    {
        public string Name;
        public bool IsDirectory;

        public override string? ToString()
        {
            var type = IsDirectory ? "D" : "F";
            return $"{Name} :{type}";
        }
    }
}
