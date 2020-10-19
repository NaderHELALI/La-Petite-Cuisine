using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Commande_Client.xaml
    /// </summary>
    public partial class Commande_Client : Page
    {
        public static MySqlConnection mySql_cooking;
        public Commande_Client()
        {
            InitializeComponent();
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();

            MySqlCommand all_recette = mySql_cooking.CreateCommand();
            all_recette.CommandText = "Select nom_recette From recette";

            MySqlDataReader dr;
            dr=all_recette.ExecuteReader();
            while (dr.Read())
            {
                recettes.Items.Add(dr["nom_recette"]);
            }
        }

        private void Description(object sender, RoutedEventArgs e)
        {
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
            MySqlCommand description_recette = mySql_cooking.CreateCommand();
            description_recette.CommandText = "Select description From recette where nom_recette=@name";
            description_recette.Parameters.AddWithValue("@name", recettes.SelectedItem);
            string desc = Convert.ToString(description_recette.ExecuteScalar());
            MessageBox.Show(desc);
        }

        private void Add_toCard(object sender, RoutedEventArgs e)
        {
            recettes.Items.Remove(recettes.SelectedItem);
            panier.Items.Add(recettes.SelectedItem);
        }
    }
}
