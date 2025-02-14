using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace DiscordAccountManager
{
    public partial class DiscordAccountListWindow : Window
    {
        private string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DiscordAccountManager");
        private string AccountsFile => Path.Combine(AppDataPath, "accounts.json");

        private List<DiscordAccount> discordAccounts = new List<DiscordAccount>();

        public DiscordAccountListWindow()
        {
            InitializeComponent();
            LoadDiscordAccounts();
        }

        private void LoadDiscordAccounts()
        {
            if (File.Exists(AccountsFile))
            {
                string json = File.ReadAllText(AccountsFile);
                discordAccounts = JsonSerializer.Deserialize<List<DiscordAccount>>(json) ?? new List<DiscordAccount>();
                DiscordAccountListBox.ItemsSource = discordAccounts;
            }
        }

        private void DeleteDiscordAccount(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is string email)
            {
                var accountToRemove = discordAccounts.FirstOrDefault(a => a.Email == email);
                if (accountToRemove != null)
                {
                    discordAccounts.Remove(accountToRemove);
                    SaveDiscordAccounts();
                    LoadDiscordAccounts();
                }
            }
        }

        private void SaveDiscordAccounts()
        {
            string json = JsonSerializer.Serialize(discordAccounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AccountsFile, json);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        [STAThread]
        private void DiscordAccountListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DiscordAccountListBox.SelectedItem is DiscordAccount selectedAccount)
            {
                string accountInfo = selectedAccount.Token != null && selectedAccount.Token.Trim() != ""
                    ? $"{selectedAccount.Email}:{selectedAccount.Password}:{selectedAccount.Token}"
                    : $"{selectedAccount.Email}:{selectedAccount.Password}";

                try
                {
                    if (Application.Current.Dispatcher.CheckAccess())
                    {
                        Clipboard.SetText(accountInfo);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Clipboard.SetText(accountInfo);
                        });
                    }

                    MessageBox.Show("Account-Daten wurden in die Zwischenablage kopiert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Copy", "Kopiert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        private void DeleteAllDiscordAccounts(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Möchten Sie wirklich alle Discord-Accounts löschen?", "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                discordAccounts.Clear();
                SaveDiscordAccounts();

                LoadDiscordAccounts();

                MessageBox.Show("Alle Discord-Accounts wurden erfolgreich gelöscht.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }

    public class DiscordAccount
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
