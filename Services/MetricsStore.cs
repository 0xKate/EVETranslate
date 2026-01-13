using System;
using System.IO;
using System.Text.Json;
using EVETranslate.Models;

namespace EVETranslate.Services
{
    public sealed class MetricsStore
    {
        private static readonly string FolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EVETranslate");

        private static readonly string FilePath =
            Path.Combine(FolderPath, "metrics.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        private readonly object _gate = new();
        private TranslationMetricsRoot _root;

        public MetricsStore()
        {
            _root = LoadInternal();
        }

        public TranslationMetrics Snapshot(string providerKey)
        {
            lock (_gate)
            {
                return _root.GetOrCreate(providerKey);
            }
        }

        public TranslationMetricsRoot SnapshotAll()
        {
            lock (_gate)
            {
                return _root;
            }
        }

        public void AddSuccess(string providerKey, DateTimeOffset when, int characters)
        {
            lock (_gate)
            {
                var metrics = _root.GetOrCreate(providerKey);

                var monthKey = when.ToString("yyyy-MM");
                var month = metrics.GetOrCreateMonth(monthKey);
                month.Requests += 1;
                month.Characters += Math.Max(0, characters);

                SaveInternal(_root);
            }
        }

        public void AddFailure(string providerKey, DateTimeOffset when)
        {
            lock (_gate)
            {
                var metrics = _root.GetOrCreate(providerKey);

                var monthKey = when.ToString("yyyy-MM");
                var month = metrics.GetOrCreateMonth(monthKey);
                month.FailedRequests += 1;

                SaveInternal(_root);
            }
        }

        private static TranslationMetricsRoot LoadInternal()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new TranslationMetricsRoot();

                var json = File.ReadAllText(FilePath);

                // Try new format first
                try
                {
                    var root = JsonSerializer.Deserialize<TranslationMetricsRoot>(json, JsonOptions);
                    if (root != null)
                        return root;
                }
                catch
                {
                    // ignore and try old format
                }

                // Back-compat: old file was just TranslationMetrics
                var old = JsonSerializer.Deserialize<TranslationMetrics>(json, JsonOptions);
                var migrated = new TranslationMetricsRoot();
                if (old != null)
                    migrated.Providers["unknown"] = old;

                return migrated;
            }
            catch
            {
                return new TranslationMetricsRoot();
            }
        }

        private static void SaveInternal(TranslationMetricsRoot root)
        {
            Directory.CreateDirectory(FolderPath);

            var tmp = FilePath + ".tmp";
            var json = JsonSerializer.Serialize(root, JsonOptions);

            File.WriteAllText(tmp, json);

            if (File.Exists(FilePath))
                File.Copy(tmp, FilePath, overwrite: true);
            else
                File.Move(tmp, FilePath);

            try { File.Delete(tmp); } catch { /* ignore */ }
        }
    }
}
