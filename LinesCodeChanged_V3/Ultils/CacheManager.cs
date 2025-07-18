﻿using System.Text.Json;

namespace CountLinesCodeChanged_V3.Logic
{
    public static class CacheManager
    {
        private static readonly string CacheFile = "cache.json";

        public static void SaveCache(Dictionary<string, SerializableCacheEntry> cache)
        {
            File.WriteAllText(CacheFile, JsonSerializer.Serialize(cache));
        }

        // Make SerializableCacheEntry public to fix CS0050
        public class SerializableCacheEntry
        {
            public string BranchName { get; set; }
            public DateTime LastCheck { get; set; }
            public DateTime DateTimeStart { get; set; }
            public DateTime DateTimeEnd { get; set; }
            public List<RepositoryStats> Stats { get; set; }
        }

        public static Dictionary<string, SerializableCacheEntry> LoadCache()
        {
            if (!File.Exists(CacheFile)) return new Dictionary<string, SerializableCacheEntry>();
            var result = JsonSerializer.Deserialize<Dictionary<string, SerializableCacheEntry>>(File.ReadAllText(CacheFile));
            return result ?? new Dictionary<string, SerializableCacheEntry>();
        }

        public static bool IsCacheValid(string repoPath,
            DateTime lastCommit,
            DateTime dateStart,
            DateTime dateEnd,
            string branch)
        {
            var cache = LoadCache();

            if (lastCommit > dateEnd || lastCommit < dateStart)
            {
                return false; // If the last commit is outside the requested date range, cache is invalid
            }
            if (cache.TryGetValue(repoPath, out var data))
            {
                if (branch != data.BranchName)
                {
                    return false;
                }
                if (data.LastCheck.AddHours(1) < DateTime.Now)
                {
                    return false; // refresh cache if last check was more than a day ago
                }
                if (data.Stats.SelectMany(p => p.Commits).OrderByDescending(p => p.Date).FirstOrDefault()?.Date >= lastCommit)
                {
                    return false;
                }
                if (data.DateTimeStart <= dateStart && data.DateTimeEnd >= dateEnd)
                {
                    return true; // Cache is valid if the date range matches
                }
            }
            return false;
        }
    }
}