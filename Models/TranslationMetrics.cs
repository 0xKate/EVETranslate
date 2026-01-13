using System;
using System.Collections.Generic;

namespace EVETranslate.Models
{
    public enum TranslationProvider
    {
        Google,
        Deepl
    }


    public sealed class TranslationMetrics
    {
        // Key: "YYYY-MM" (e.g., "2026-01")
        public Dictionary<string, MonthlyTotals> ByMonth { get; set; } = new();

        public MonthlyTotals GetOrCreateMonth(string monthKey)
        {
            if (!ByMonth.TryGetValue(monthKey, out var totals))
            {
                totals = new MonthlyTotals();
                ByMonth[monthKey] = totals;
            }
            return totals;
        }
    }

    public sealed class MonthlyTotals
    {
        public long Requests { get; set; }
        public long Characters { get; set; }
        public long FailedRequests { get; set; }
    }
}
