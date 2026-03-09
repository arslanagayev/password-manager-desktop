using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ConnectionString = "Data Source=passwords.db";
        private ObservableCollection<PasswordEntry> passwords;

        public MainWindow()
        {
            InitializeComponent();
            passwords = new ObservableCollection<PasswordEntry>();
            PasswordsListView.ItemsSource = passwords;
            CreateTable();
            LoadPasswords();
        }

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Passwords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Site TEXT NOT NULL,
                        Username TEXT NOT NULL,
                        Password TEXT NOT NULL
                    )";
                command.ExecuteNonQuery();
            }
        }

        private void SavePassword()
        {
            string site = SiteNameTextBox.Text.Trim();
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(site) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO Passwords (Site, Username, Password)
                    VALUES ($site, $username, $password)";
                command.Parameters.AddWithValue("$site", site);
                command.Parameters.AddWithValue("$username", username);
                command.Parameters.AddWithValue("$password", password);
                command.ExecuteNonQuery();
            }

            SiteNameTextBox.Clear();
            UsernameTextBox.Clear();
            PasswordBox.Clear();
            SiteNameTextBox.Focus();

            LoadPasswords();
            MessageBox.Show("Password saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadPasswords()
        {
            passwords.Clear();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Site, Username, Password FROM Passwords";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        passwords.Add(new PasswordEntry
                        {
                            Id = reader.GetInt32(0),
                            Site = reader.GetString(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3)
                        });
                    }
                }
            }
        }

        private void DeletePassword(int id)
        {
            var result = MessageBox.Show("Are you sure you want to delete this password?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "DELETE FROM Passwords WHERE Id = $id";
                    command.Parameters.AddWithValue("$id", id);
                    command.ExecuteNonQuery();
                }

                LoadPasswords();
                MessageBox.Show("Password deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SavePassword();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                DeletePassword(id);
            }
        }
    }
}