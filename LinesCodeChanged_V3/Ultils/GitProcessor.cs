using LibGit2Sharp;

namespace CountLinesCodeChanged_V3.Logic
{
    public static class GitProcessor
    {
        public static async Task<List<RepositoryStats>> ProcessRepositories(List<string> repoPaths, DateTime startDate, DateTime endDate, string branch)
        {
            var stats = new List<RepositoryStats>();
            var cache = CacheManager.LoadCache();

            foreach (var repoPath in repoPaths)
            {
                using (var repo = new Repository(repoPath))
                {
                    var lastCommit = repo.Commits.FirstOrDefault();
                    // Check if cache is valid for each repository
                    if (lastCommit != null && CacheManager.IsCacheValid(repoPath, lastCommit.Committer.When.DateTime))
                    {
                        // If cache is valid, add stats from cache
                        stats.AddRange(cache[repoPath].Stats);
                        continue;
                    }
                    // If cache is not valid, process repository
                    var repoName = Path.GetFileName(repoPath);
                    var branchCommits = repo.Branches[branch]?.Commits
                        .Where(c => c.Committer.When.DateTime >= startDate && c.Committer.When.DateTime <= endDate)
                        .ToList() ?? new List<Commit>();

                    var authorStats = branchCommits
                        .GroupBy(c => c.Author.Name)
                        .Select(g =>
                        {
                            var authorCommits = g.ToList();
                            var added = 0;
                            var removed = 0;
                            foreach (var commit in authorCommits)
                            {
                                var diff = repo.Diff.Compare<Patch>(commit.Parents.FirstOrDefault()?.Tree, commit.Tree);
                                added += diff.LinesAdded;
                                removed += diff.LinesDeleted;
                            }
                            return new RepositoryStats
                            {
                                Author = g.Key,
                                RepoName = repoName,
                                Branch = branch,
                                AddedLines = added,
                                RemovedLines = removed,
                                CommitCount = authorCommits.Count,
                                Commits = authorCommits.Select(c => new CommitInfo
                                {
                                    Message = c.Message,
                                    Date = c.Committer.When.DateTime
                                }).ToList()
                            };
                        }).ToList();

                    stats.AddRange(authorStats);

                    // Update cache with new stats
                    cache[repoPath] = (lastCommit?.Committer.When.DateTime ?? DateTime.Now, authorStats);
                }
            }
            // Save cache
            CacheManager.SaveCache(cache);
            return stats;
        }

        public static List<(string RepoName, int TotalCommits, int TotalAuthors, int TotalAdded, int TotalRemoved, double AddPerRemove, double AddPerTotal, double RemovePerTotal)> GetSummary(List<RepositoryStats> stats)
        {
            return stats
                .GroupBy(s => s.RepoName)
                .Select(g =>
                {
                    var repoStats = g.ToList();
                    var totalCommits = repoStats.Sum(s => s.CommitCount);
                    var totalAuthors = repoStats.Select(s => s.Author).Distinct().Count();
                    var totalAdded = repoStats.Sum(s => s.AddedLines);
                    var totalRemoved = repoStats.Sum(s => s.RemovedLines);
                    var totalChanged = totalAdded + totalRemoved;
                    var addPerRemove = totalRemoved > 0 ? Math.Round((double)totalAdded / totalRemoved, 2) : 0;
                    var addPerTotal = totalChanged > 0 ? Math.Round((double)totalAdded / totalChanged * 100, 2) : 0;
                    var removePerTotal = totalChanged > 0 ? Math.Round((double)totalRemoved / totalChanged * 100, 2) : 0;

                    return (g.Key, totalCommits, totalAuthors, totalAdded, totalRemoved, addPerRemove, addPerTotal, removePerTotal);
                }).ToList();
        }
    }
}