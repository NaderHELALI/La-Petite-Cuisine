using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Media;

namespace Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public static MySqlConnection mySql_cooking;
        public MainWindow()
        {
            InitializeComponent();

            mySql_cooking = Database_connexion();



        }
        private void Open_Connexion(object sender, RoutedEventArgs e)
        {
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();

            MySqlCommand pseudo_client = mySql_cooking.CreateCommand();
            pseudo_client.CommandText = "Select count(*) from clients where pseudo=@pseudo and mot_de_passe=@password";
            pseudo_client.Parameters.AddWithValue("@pseudo", pseudo.Text);
            pseudo_client.Parameters.AddWithValue("@password", password.Password);


            int val = Convert.ToInt32(pseudo_client.ExecuteScalar());
            if (val == 1)
            {
                MessageBox.Show("Vous êtes connectés");
                MySqlCommand id_client = mySql_cooking.CreateCommand();
                id_client.CommandText = "Select id_client from clients where pseudo=@pseudo ;";
                id_client.Parameters.AddWithValue("@pseudo", pseudo.Text);
                string id = Convert.ToString(id_client.ExecuteScalar());
                Menu_Client menu_Client = new Menu_Client(id);
                menu_Client.Show();
                this.Close();
            }
            else
            {
                TextStatus.Text = "Le pseudo et/ ou mot de passe est incorrect.";
            }
        }
        public MySqlConnection Database_connexion()
        {
            MySqlConnection mySql = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");

            try
            {
                mySql.Open();
                MessageBox.Show("Connection à la dataBase ");
                database_label.Content = "Connected";
                database_label.Foreground = new SolidColorBrush(Colors.Green); ;

            }
            catch
            {
                MessageBox.Show(mySql.ToString());
                MessageBox.Show("Erreur de Connection à la dataBase");
            }
            return mySql;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Login insription = new Login();
            insription.Show();
        }

        private void Admin(object sender, RoutedEventArgs e)
        {
            Menu_Admin menu_Admin = new Menu_Admin();
            menu_Admin.Show();
        }

        private void Demo(object sender, RoutedEventArgs e)
        {
            ModeDemo modeDemo = new ModeDemo();
            modeDemo.Show();
        }
    }
}
