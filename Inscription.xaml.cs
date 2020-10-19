using MySql.Data.MySqlClient;
using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static MySqlConnection mySql_cooking;
        private string id_client;
        public Login()
        {
            InitializeComponent();
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
            

        }

        private void Inscription(object sender, RoutedEventArgs e)
        {
            
           

            MySqlCommand nb_client = mySql_cooking.CreateCommand();
            nb_client.CommandText = "Select count(*) from clients where pseudo=@pseudo and email=@mail;";
            nb_client.Parameters.AddWithValue("@pseudo", pseudo.Text);
            nb_client.Parameters.AddWithValue("@mail", mail.Text);
            if (Convert.ToInt32(nb_client.ExecuteScalar()) != 0)
            {
                erreur.Opacity = 90;
            }
            else
            {
                if (pseudo.Text !="" ) 
                {

                    MySqlCommand add_clients = mySql_cooking.CreateCommand();
                    add_clients.CommandText = "Insert into clients values(@id,@pseudo,@mdp,@nom,@prenom,@tel,@email,null) ;";
                    add_clients.Parameters.AddWithValue("@id", Id_client());
                    add_clients.Parameters.AddWithValue("@pseudo", pseudo.Text);
                    add_clients.Parameters.AddWithValue("@mdp", mdp.Text);
                    add_clients.Parameters.AddWithValue("@nom", nom.Text);
                    add_clients.Parameters.AddWithValue("@prenom", prenom.Text);
                    add_clients.Parameters.AddWithValue("@tel", tel.Text);
                    add_clients.Parameters.AddWithValue("@email", mail.Text);
                    add_clients.ExecuteNonQuery();
                    MessageBox.Show("Vous êtes maintenant inscrit.");
                    this.Close();
                }
                else {
                    erreur.Text = "Vous n'avez pas remplis tout les champs.";
                    erreur.Opacity = 90;
                }
                

            }
            




        }
        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();

            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public string Id_client()
        {
            bool valid = false;
            do
            {

                MySqlCommand nbCommande = mySql_cooking.CreateCommand();
                nbCommande.CommandText = "Select count(*) from clients where id_client=@id_client ;";
                id_client = RandomString(5);
                nbCommande.Parameters.AddWithValue("@id_client", id_client);
                if (Convert.ToInt32(nbCommande.ExecuteScalar()) == 1)
                {
                    valid = true;
                }


            } while (valid == true);
            return id_client.ToLower();
        }
    }
}
