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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour ModeDemo.xaml
    /// </summary>
    public partial class ModeDemo : Window
    {
        private MySqlConnection mySql_cooking;
        public ModeDemo()
        {
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
            InitializeComponent();

        }


        private List<string> CommandR(string command)
        {
            List<string> values = new List<string>();
            MySqlCommand commandR = mySql_cooking.CreateCommand();
            commandR.CommandText = command;
            MySqlDataReader reader;
            reader = commandR.ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
                values.Add(Convert.ToString(reader[i]));
            }
            reader.Close();
            return values;

        }

        private void Nb_Client(object sender, RoutedEventArgs e)
        {
            List_produit.Items.Clear();
            MessageBox.Show("Le nombre de clients est de "+CommandR("select count(*) from clients")[0]);
        }

        private void Cdr(object sender, RoutedEventArgs e)
        {
            List_produit.Items.Clear();
            MessageBox.Show("Le nombre de Cdr est de "+CommandR("select count(*) from createur_recette")[0]);
            List < string >Listnom= CommandR("select nom from clients where createur_recette_id_createur != '';");

            foreach (string nom in Listnom)
            {

                string id = CommandR("select createur_recette_id_createur from clients where nom='" + nom + "';")[0];
                string quantite = CommandR("select sum(c.quantite) from est_composee as c join recette as r on c.nom_recette = r.nom_recette where r.id_createur ='"+id+"';")[0];

                List_produit.Items.Add(nom.ToUpper() + " : " + quantite );

            }
               
               
        }

        private void NbRecettes(object sender, RoutedEventArgs e)
        {
            List_produit.Items.Clear();
            MessageBox.Show("Le nombre de recettes est de " + CommandR("select count(*) from recette")[0]);
        }

        private void Ingredient(object sender, RoutedEventArgs e)
        {
            List_produit.Items.Clear();
            
            
            
            List<string> produit = CommandR("select distinct (name_product) from product where stock<=min_stock*2;");
            foreach (string name in produit)
            {
                List_produit.Items.Add(name);
            }
            
            

        }

        private void Product_recette(object sender, RoutedEventArgs e)
        {
            List_produit.Items.Clear();
            List<string> produit = CommandR("select distinct(nom_recette) from contient where name_product='"+name_product.Text+"';");
            foreach (string name in produit)
            {
                string quantite = CommandR("select quantite_r from contient where name_product='" + name_product.Text + "'and nom_recette='"+name+"';")[0];
                List_produit.Items.Add(name +" : "+quantite  );
            }
        }
    }
}
