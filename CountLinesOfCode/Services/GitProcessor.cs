using CountLinesOfCode.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CountLinesOfCode.Services
{
    public static class GitProcessor
    {
        public static (int added, int removed, int total, int commitCount, List<CommitDetail> commits) CountChangedLines(string author, DateTime startDate, DateTime endDate, string branch, string repoPath)
        {
            try
            {
                string dateFormat = "yyyy-MM-dd";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = $"-C \"{repoPath}\" log {branch} --author=\"{author}\" --since=\"{startDate.ToString(dateFormat)}\" --until=\"{endDate.ToString(dateFormat)}\" --pretty=%H|%s|%ad",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string commitOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var commits = new List<CommitDetail>();
                if (!string.IsNullOrWhiteSpace(commitOutput))
                {
                    commits = commitOutput.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.Split('|'))
                        .Where(parts => parts.Length >= 2) // Ensure at least 2 parts (hash, message)
                        .Select(parts => new CommitDetail
                        {
                            Hash = parts[0],
                            Message = parts[1]
                        })
                        .Where(p => !p.Message.ToLower().StartsWith("merge branch")) // Exclude merge commits
                        .ToList();
                }

                int totalAdded = 0, totalRemoved = 0;

                foreach (var commit in commits)
                {
                    var diffProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "git",
                            Arguments = $"-C \"{repoPath}\" diff {commit.Hash}^ {commit.Hash}",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    diffProcess.Start();
                    string diffOutput = diffProcess.StandardOutput.ReadToEnd();
                    diffProcess.WaitForExit();

                    var lines = diffOutput.Split('\n');
                    int added = lines.Count(line => line.StartsWith("+") && !line.StartsWith("+++"));
                    int removed = lines.Count(line => line.StartsWith("-") && !line.StartsWith("---"));

                    totalAdded += added;
                    totalRemoved += removed;
                }

                return (totalAdded, totalRemoved, totalAdded + totalRemoved, commits.Count, commits);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in {repoPath}: {ex.Message}");
                return (0, 0, 0, 0, new List<CommitDetail>());
            }
        }
    }
}