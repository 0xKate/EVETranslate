using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EVETranslate.Models;
using EVETranslate.Parsing;          // EveChatLogParser
using EVETranslate.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace EVETranslate.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<object> Tabs { get; } = new();

        [ObservableProperty] private object? selectedTab;
        [ObservableProperty] private string outboundInput = string.Empty;
        [ObservableProperty] private string outboundTranslatedPreview = string.Empty;
        [ObservableProperty] private TargetLanguage selectedTargetLanguage = TargetLanguage.ZH;

        private readonly ChatLogSubscriptionManager _subs;
        private readonly SettingsService _settings;

        public MainViewModel(SettingsService settings)
        {
            _settings = settings;
            _subs = new ChatLogSubscriptionManager(new PollingLogTailer());

            Tabs.Add(new AddTabPlaceholder());
        }


        [RelayCommand]
        private void AddTab()
        {
            TryAddTabFromDialog();
        }

        private bool TryAddTabFromDialog()
        {
            var dlg = new OpenFileDialog
            {
                Title = "Select an EVE chat log file",
                Filter = "Chat logs (*.txt)|*.txt|All files (*.*)|*.*",
                CheckFileExists = true
            };

            if (dlg.ShowDialog() != true || string.IsNullOrWhiteSpace(dlg.FileName))
                return false;

            var path = dlg.FileName;

            string headerText;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                char[] buffer = new char[16_384];
                int read = sr.Read(buffer, 0, buffer.Length);
                headerText = new string(buffer, 0, read);
            }

            var tabName = "New Channel";
            if (EveChatLogParser.TryParseLogHeader(headerText, out var header))
                tabName = header.ChannelName;

            var newTab = new ChannelTab
            {
                Name = tabName,
                LogFilePath = path
            };

            Tabs.Insert(Tabs.Count - 1, newTab);
            SelectedTab = newTab;

            _subs.Start(newTab);

            return true;
        }

        [RelayCommand]
        private void CloseSelectedTab()
        {
            if (SelectedTab is not ChannelTab tab)
                return;

            _subs.Stop(tab);
            Tabs.Remove(tab);

            // Pick something sane if you closed the active tab
            if (Tabs.Count > 0 && SelectedTab == tab)
                SelectedTab = Tabs[0];
        }

        [RelayCommand]
        private void TranslateAndCopy()
        {
            var toCopy = string.IsNullOrWhiteSpace(OutboundTranslatedPreview)
                ? OutboundInput
                : OutboundTranslatedPreview;

            if (!string.IsNullOrWhiteSpace(toCopy))
                Clipboard.SetText(toCopy);
        }


    [RelayCommand]
    private void OpenSettings()
    {
        var win = new SettingsWindow
        {
            Owner = Application.Current.MainWindow,
            DataContext = new SettingsViewModel(_settings) // if you use the settings service approach
        };

        win.ShowDialog();
    }

    [RelayCommand]
    private void ExitApp()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand]
    private void OpenAbout()
    {
        MessageBox.Show("EVETranslate\nv0.1", "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }


}
}
