using System.Text.Json;

namespace CountLinesCodeChanged_V3.Logic
{
    public static class CacheManager
    {
        private static readonly string CacheFile = "cache.json";

        public static void SaveCache(Dictionary<string, (DateTime LastUpdate, List<RepositoryStats> Stats)> cache)
        {
            File.WriteAllText(CacheFile, JsonSerializer.Serialize(cache));
        }

        public static Dictionary<string, (DateTime LastUpdate, List<RepositoryStats> Stats)> LoadCache()
        {
            if (!File.Exists(CacheFile)) return new Dictionary<string, (DateTime, List<RepositoryStats>)>();
            return JsonSerializer.Deserialize<Dictionary<string, (DateTime, List<RepositoryStats>)>>(File.ReadAllText(CacheFile));
        }

        public static bool IsCacheValid(string repoPath, DateTime lastCommitTime)
        {
            var cache = LoadCache();
            if (cache.TryGetValue(repoPath, out var data))
            {
                return data.LastUpdate >= lastCommitTime;
            }
            return false;
        }
    }
}