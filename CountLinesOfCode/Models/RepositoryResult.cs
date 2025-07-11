using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountLinesOfCode.Models
{
    public class RepositoryResult
    {
        public string Path { get; set; }
        public int AddedLines { get; set; }
        public int RemovedLines { get; set; }
        public int TotalChangedLines { get; set; }
        public int CommitCount { get; set; }
        public List<CommitDetail> Commits { get; set; }
    }
}
