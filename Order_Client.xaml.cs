using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Order_Client.xaml
    /// </summary>
    public partial class Order_Client : Window
    {
        public static MySqlConnection mySql_cooking;
        private string id_commande;
        public Dictionary<string, int> quantity = new Dictionary<string, int>();
        private Client client = new Client();
        public Order_Client(string id_client)
        {
            InitializeComponent();
            client.ID = id_client;

            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
            id_commande = ID_commande();
            MySqlCommand all_recette = mySql_cooking.CreateCommand();
            all_recette.CommandText = "Select nom_recette From recette";

            MySqlDataReader dr;
            dr = all_recette.ExecuteReader();
            while (dr.Read())
            {
                recettes.Items.Add(dr["nom_recette"]);
            }
            dr.Close();

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

            int total = Convert.ToInt32(total_price.Text);
            MySqlCommand price_recette = mySql_cooking.CreateCommand();
            price_recette.CommandText = "Select price From recette where nom_recette=@nom_recette";
            price_recette.Parameters.AddWithValue("@nom_recette", recettes.SelectedItem);

            string name = Convert.ToString(recettes.SelectedItem);
            int quantity_toAdd = Convert.ToInt32(Quantite.Text);
            if (!(recettes.SelectedItem is null))
            {

                if (quantity.Keys.Contains(name))
                {
                    panier.Items.Remove(($"{recettes.SelectedItem}.(x{quantity[name]})"));
                    quantity[name] += quantity_toAdd;


                    panier.Items.Add($"{recettes.SelectedItem}.(x{quantity[name]})");
                    total += Convert.ToInt32(price_recette.ExecuteScalar());
                    total_price.Text = Convert.ToString(total);
                }
                else
                {
                    quantity.Add(name, quantity_toAdd);
                    panier.Items.Add($"{recettes.SelectedItem}.(x{quantity[name]})");
                    total += quantity_toAdd * Convert.ToInt32(price_recette.ExecuteScalar());
                    total_price.Text = Convert.ToString(total);
                }
            }
            else
            {
                MessageBox.Show("Vous n'avez rien selectionné");
            }


        }

        private void Delete_fromCard(object sender, RoutedEventArgs e)
        {
            
            int total = Convert.ToInt32(total_price.Text);
            object item = panier.SelectedItem;
            MySqlCommand price_recette = mySql_cooking.CreateCommand();
            price_recette.CommandText = "Select price From recette where nom_recette=@nom_recette";
            if (!(item == null))
            {
                if (panier.Items.Count == 1) { item = panier.Items.GetItemAt(panier.SelectedIndex); };
                string recette = Convert.ToString(item).Split('.')[0];
                price_recette.Parameters.AddWithValue("@nom_recette", recette);
                total -= quantity[recette] * Convert.ToInt32(price_recette.ExecuteScalar());
                quantity.Remove(recette);
                total_price.Text = Convert.ToString(total);
                panier.Items.Remove(item);
            }
            else
            {
                MessageBox.Show("Vous n'avez rien selectionné");
            }

        }


        private void Payer(object sender, RoutedEventArgs e)
        {
            
            MySqlCommand solde_client = mySql_cooking.CreateCommand();
            solde_client.CommandText = "select solde from createur_recette where clients_id_client=@id_client;";
            solde_client.Parameters.AddWithValue("@id_client", client.ID);
            int new_solde = Convert.ToInt32(solde_client.ExecuteScalar()) - Convert.ToInt32(total_price.Text);

            bool sucess1 = true;
            foreach (string recette in quantity.Keys)
            {
                List<string> produit = CommandR("SELECT name_product from contient where nom_recette='" + recette + "';");
                foreach (string ingredient in produit)
                {
                    int stock = Convert.ToInt32(CommandR("SELECT stock  from product where name_product='" + ingredient + "';")[0]);
                    int val = Convert.ToInt32(CommandR("SELECT quantite_r  from contient where name_product='" + ingredient + "';")[0]);
                    if ((stock - val * quantity[recette]) > 0)
                    {
                        Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter updatestock = new Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter();
                        updatestock.UpdateQueryStock(Convert.ToInt32(CommandR("SELECT quantite_r  from contient where name_product='" + ingredient + "';")[0]) * quantity[recette]);
                    }
                    else
                    {
                        MessageBox.Show("Nous sommes désolé, il n'a plus assez de " + ingredient + " pour " + recette);
                        sucess1 = false;
                        break;
                    }
                }

            }

            if ((new_solde >= 0)&&(sucess1))
            {
                MySqlCommand up_solde = mySql_cooking.CreateCommand();
                up_solde.Parameters.AddWithValue("@new_solde", new_solde);
                up_solde.Parameters.AddWithValue("@id_client", client.ID);
                up_solde.CommandText = "UPDATE createur_recette SET solde=@new_solde WHERE clients_id_client=@id_client;";
                up_solde.ExecuteNonQuery();

                MySqlCommand newCommande_ref = mySql_cooking.CreateCommand();
                newCommande_ref.Parameters.AddWithValue("@id_client", client.ID);
                newCommande_ref.CommandText = "Insert into commande values(@id_commande,@date,@id_client);";
                newCommande_ref.Parameters.AddWithValue("@id_commande", id_commande);
                DateTime date = new DateTime();
                date = DateTime.Now;
                newCommande_ref.Parameters.AddWithValue("@date", date);
                newCommande_ref.ExecuteNonQuery();





                MySqlCommand newCommande = mySql_cooking.CreateCommand();
                newCommande.CommandText = "Insert into est_composee values(@id_commande,@nom_recette,@quantite);";
                bool sucess = true;
                foreach (string recette in quantity.Keys)
                {
                    newCommande.Parameters.Clear();
                    newCommande.Parameters.AddWithValue("@id_commande", id_commande);
                    newCommande.Parameters.AddWithValue("@nom_recette", recette);
                    newCommande.Parameters.AddWithValue("@quantite", quantity[recette]);
                    newCommande.ExecuteNonQuery();
                    List<string> produit = CommandR("SELECT name_product from contient where nom_recette='" + recette + "';");
                    foreach(string ingredient in produit)
                    {
                        int stock = Convert.ToInt32(CommandR("SELECT stock  from product where name_product='" + ingredient + "';")[0]);
                        int val= Convert.ToInt32(CommandR("SELECT quantite_r  from contient where name_product='" + ingredient + "';")[0]);
                        Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter updatestock = new Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter();
                        updatestock.UpdateQueryStock(Convert.ToInt32(CommandR("SELECT quantite_r  from contient where name_product='" + ingredient + "';")[0]) * quantity[recette]);
                        
                       
                    }

                }
                if (sucess)
                {
                    MessageBox.Show("Votre commmande à bien été enregistée");
                }

                this.Close();


            }
           if (new_solde < 0){MessageBox.Show($"Vous n'avez pas assez de credit, il vous reste {new_solde} Coins"); }
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
        public string ID_commande()
        {
            bool valid = false;
            do
            {

                MySqlCommand nbCommande = mySql_cooking.CreateCommand();
                nbCommande.CommandText = "Select count(*) from commande where id_commande=@id_commande ;";
                id_commande = RandomString(8);
                nbCommande.Parameters.AddWithValue("@id_commande", id_commande);
                if (Convert.ToInt32(nbCommande.ExecuteScalar()) == 1)
                {
                    valid = true;
                }


            } while (valid == true);
            return id_commande;
        }


    }

}