using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DiscordAccountManager
{

    public partial class OverlayWindow : Window
    {
        private string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DiscordAccountManager");
        private string AccountsFile => Path.Combine(AppDataPath, "accounts.json");

        private List<Account> accounts = new List<Account>();

        public OverlayWindow()
        {
            InitializeComponent();
            LoadAccounts();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseOverlay(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddAccount(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text;
            string password = PasswordBox.Text;  
            string token = TokenBox.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    MessageBox.Show("Bitte gib entweder E-Mail + Passwort oder einen Token ein!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            accounts.Add(new Account { Email = email, Password = password, Token = token });
            SaveAccounts();
            StatusText.Text = $"Gespeicherte Accounts: {accounts.Count}";

            EmailBox.Clear();
            PasswordBox.Clear();
            TokenBox.Clear();
        }

        private void LoadAccounts()
        {
            if (!Directory.Exists(AppDataPath))
                Directory.CreateDirectory(AppDataPath);

            if (File.Exists(AccountsFile))
            {
                string json = File.ReadAllText(AccountsFile);
                accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
                StatusText.Text = $"Gespeicherte Accounts: {accounts.Count}";
            }
        }

        private void SaveAccounts()
        {
            string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(AccountsFile, json);
        }
        private void Button_Click(object sender, RoutedEventArgs e)

        {
            DiscordAccountListWindow discordAccountListWindow = new DiscordAccountListWindow();
            discordAccountListWindow.Show();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null && tb.Text == "E-Mail" || tb.Text == "Token (optional)" || tb.Text == "Passwort")
            {
                tb.Clear();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                if (tb.Name == "EmailBox")
                    tb.Text = "E-Mail";
                else if (tb.Name == "TokenBox")
                    tb.Text = "Token (optional)";
                else
                    tb.Text = "Passwort";
            }
        }
    }

    public class Account
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}
