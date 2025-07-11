namespace CountLinesCodeChanged_V3
{
    public class RepositoryStats
    {
        public string Author { get; set; }
        public string RepoName { get; set; }
        public string Branch { get; set; }
        public int AddedLines { get; set; }
        public int RemovedLines { get; set; }
        public int TotalChangedLines => AddedLines + RemovedLines;
        public double AddPerRemovePercentage => RemovedLines > 0 ? Math.Round((double)AddedLines / RemovedLines, 2) : 0;
        public double AddPerTotalPercentage => TotalChangedLines > 0 ? Math.Round((double)AddedLines / TotalChangedLines * 100, 2) : 0;
        public double RemovePerTotalPercentage => TotalChangedLines > 0 ? Math.Round((double)RemovedLines / TotalChangedLines * 100, 2) : 0;
        public int CommitCount { get; set; }
        public List<CommitInfo> Commits { get; set; }
    }

    public class CommitInfo
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}