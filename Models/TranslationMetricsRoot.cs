using System;
using System.Collections.Generic;

namespace EVETranslate.Models
{
    public sealed class TranslationMetricsRoot
    {
        // keys like "deepl", "google"
        public Dictionary<string, TranslationMetrics> Providers { get; set; }
            = new(StringComparer.OrdinalIgnoreCase);

        public TranslationMetrics GetOrCreate(string providerKey)
        {
            providerKey = string.IsNullOrWhiteSpace(providerKey) ? "unknown" : providerKey.Trim();

            if (!Providers.TryGetValue(providerKey, out var metrics))
            {
                metrics = new TranslationMetrics();
                Providers[providerKey] = metrics;
            }

            return metrics;
        }
    }
}
