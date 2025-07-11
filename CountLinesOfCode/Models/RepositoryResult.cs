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
        public double AddPerRemovePercentage
        {
            get
            {
                return AddedLines > 0 && RemovedLines > 0
                    ? Math.Round((double)AddedLines / RemovedLines, 2)
                    : 0;
            }
        }
        public double AddPerTotalPercentage
        {
            get
            {
                return TotalChangedLines > 0
                    ? Math.Round((double)AddedLines / TotalChangedLines, 2)
                    : 0;
            }
        }
        public double RemovePerTotalPercentage
        {
            get
            {
                return TotalChangedLines > 0
                    ? Math.Round((double)RemovedLines / TotalChangedLines, 2)
                    : 0;
            }
        }
        public int CommitCount { get; set; }
        public List<CommitDetail> Commits { get; set; }
    }
}
