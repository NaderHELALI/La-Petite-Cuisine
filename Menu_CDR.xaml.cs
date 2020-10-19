using System;
using System.Collections.Generic;
using System.Data;
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
    /// Logique d'interaction pour Menu_CDR.xaml
    /// </summary>
    public partial class Menu_CDR : Window
    {
       
        private Client client = new Client();
        public static MySqlConnection mySql_cooking;
        private bool success = false;
        public Dictionary<string, float> quantity = new Dictionary<string, float>();
        public Menu_CDR(string id_client)
        {
            InitializeComponent();
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
            client.ID = id_client;
            AcualiserSolde();
            

            VerifieCDR(client);


        }
        private void VerifieCDR(Client client)
        {
            MySqlCommand nameCommand = mySql_cooking.CreateCommand();
            nameCommand.CommandText = "Select nom from clients where id_client=@id";
            nameCommand.Parameters.AddWithValue("@id", client.ID);
            string nom = Convert.ToString(nameCommand.ExecuteScalar());
            client.Nom = nom;
            MySqlCommand prenomCommand = mySql_cooking.CreateCommand();
            prenomCommand.CommandText = "Select prenom from clients where id_client=@id";
            prenomCommand.Parameters.AddWithValue("@id", client.ID);
            string prenom = Convert.ToString(prenomCommand.ExecuteScalar());
            client.Prenom = prenom;
            MySqlCommand commandcdr = mySql_cooking.CreateCommand();
            commandcdr.CommandText = "Select createur_recette_id_createur from clients where id_client=@id";
            commandcdr.Parameters.AddWithValue("@id", client.ID);
            string cdr = Convert.ToString(commandcdr.ExecuteScalar());
            
            if (cdr== "")
            {
                client.ID_cdr = Id_cdr();

                MySqlCommand cdrtable = mySql_cooking.CreateCommand();
                cdrtable.CommandText = "Insert into createur_recette values(@id_cdr,0,@id);";
                cdrtable.Parameters.AddWithValue("@id", client.ID);
                cdrtable.Parameters.AddWithValue("@id_cdr", client.ID_cdr);
                cdrtable.ExecuteNonQuery();


                MySqlCommand newcdr = mySql_cooking.CreateCommand();
                newcdr.CommandText = "Update clients SET createur_recette_id_createur=@cdr where id_client=@id";
                newcdr.Parameters.AddWithValue("@cdr", client.ID_cdr);
                newcdr.Parameters.AddWithValue("@id", client.ID);
                newcdr.ExecuteNonQuery();

                MessageBox.Show("Vous êtes maintenants inscrit en tant que créateur de recette");
               
            }
            else { client.ID_cdr = cdr; }
            
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
        public string Id_cdr()
        {
            string cdr ;
            bool valid = false;
            do
            {

                MySqlCommand nbCommande = mySql_cooking.CreateCommand();
                nbCommande.CommandText = "Select count(*) from clients where createur_recette_id_createur=@id_cdr ;";
                cdr = RandomString(5);
                nbCommande.Parameters.AddWithValue("@id_cdr", client.ID_cdr);
                if (Convert.ToInt32(nbCommande.ExecuteScalar()) == 1)
                {
                    valid = true;
                }


            } while (valid == true);
            return cdr.ToLower();
        }

        public void AcualiserSolde()
        {
            MySqlCommand solde_client = mySql_cooking.CreateCommand();
            solde_client.CommandText = "select solde from createur_recette where clients_id_client=@id_client;";
            solde_client.Parameters.AddWithValue("@id_client", client.ID);
            solde.Text = Convert.ToString(solde_client.ExecuteScalar());
        }
        private void Buy_Cookin(object sender, RoutedEventArgs e)
        {
            ADD_recette.Opacity = 0;
            recetteDataGrid.Opacity = 0;
                if (Buy_cook.Opacity == 0)
           {
                Buy_cook.Opacity = 100;
                

            }
            else 
            { Buy_cook.Opacity = 0;
                prix.Text = "";
            }
        }
        private void pay_cooks(object sender, RoutedEventArgs e)
        {
            if (success)
            {
                MySqlCommand solde_client = mySql_cooking.CreateCommand();
                solde_client.CommandText = "update createur_recette set solde =solde+ @value where clients_id_client=@id_client;";
                solde_client.Parameters.AddWithValue("@id_client", client.ID);
                solde_client.Parameters.AddWithValue("@value", Int32.Parse(nb_cook.Text));
                solde_client.ExecuteNonQuery();
                AcualiserSolde();
                MessageBox.Show("Votre achat a été finaliser avec succès.");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            success = Int32.TryParse(nb_cook.Text, out int res);
            if (success) { prix.Text = Convert.ToString(res * 0.25)+ " €"; }

        }

        private void MesRecettes(object sender, RoutedEventArgs e)
        {
            Buy_cook.Opacity = 0;
            ADD_recette.Opacity = 0;
            if (recetteDataGrid.Opacity == 0)
            {


                recetteDataGrid.Opacity = 100;
                Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
                // Chargez les données dans la table recette. Vous pouvez modifier ce code selon les besoins.
                Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter cookingDataSetrecetteTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter();
                cookingDataSetrecetteTableAdapter.Fill_MyRecette(cookingDataSet.recette, client.ID_cdr);
                System.Windows.Data.CollectionViewSource recetteViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("recetteViewSource")));
                recetteViewSource.View.MoveCurrentToFirst();
            }
            else { recetteDataGrid.Opacity = 0; }
        }

        private void Add_recette(object sender, RoutedEventArgs e)
        {
            Buy_cook.Opacity = 0;
            recetteDataGrid.Opacity = 0;
            if (ADD_recette.Opacity == 0)
            {
                
                Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
                // TODO: ajoutez du code ici pour charger les données dans la table product.
                // Impossible de générer ce code, car la méthode cookingDataSetproductTableAdapter.Fill est manquante ou a des paramètres non reconnus.
                Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter cookingDataSetproductTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter();
                cookingDataSetproductTableAdapter.FillByName(cookingDataSet.product);
                
                System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
                productViewSource.View.MoveCurrentToFirst();
                ADD_recette.Opacity = 100;


            }
            else { ADD_recette.Opacity = 0; }
        }

        private void Ajouter_produit(object sender, RoutedEventArgs e)
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
            DataRow data = cookingDataSet.product[name_productComboBox.SelectedIndex];
            
            string name_product = Convert.ToString(data["name_product"]);

            
            if ((name_product !="")&&(Convert.ToInt32(quantity_product.Text)>0))
            {

                if (quantity.Keys.Contains(name_product))
                {
                    ens_produit.Items.Remove(($"{name_product}.(x{quantity[name_product]})"));

                    quantity[name_product] += Convert.ToSingle(quantity_product.Text);


                    ens_produit.Items.Add($"{name_product}.(x{quantity[name_product]})");
                    
                }
                else
                {
                    quantity.Add(name_product, Convert.ToSingle(quantity_product.Text));
                    ens_produit.Items.Add($"{name_product}.(x{quantity[name_product]})");
                    
                }
            }
            else
            {
                MessageBox.Show("Vous n'avez rien selectionné et/ou la quantité est mauvaise.");
            }
        }

        private void Creer(object sender, RoutedEventArgs e)
        {
            if (recette_name.Text != "")
            {

                MySqlCommand add_recette = mySql_cooking.CreateCommand();
                add_recette.CommandText = "insert into recette values(@nom,@desc,@type,@price,2,@id_cdr );";
                add_recette.Parameters.AddWithValue("@nom", recette_name.Text);
                add_recette.Parameters.AddWithValue("@desc", description.Text);
                add_recette.Parameters.AddWithValue("@type", unit.Text);
                add_recette.Parameters.AddWithValue("@price", Convert.ToInt32(cook_price.Text));
                add_recette.Parameters.AddWithValue("@id_cdr", client.ID_cdr);
                add_recette.ExecuteNonQuery();

                MySqlCommand add_recette_produit = mySql_cooking.CreateCommand();
                add_recette_produit.CommandText = "insert into contient values(@nom_recette,@nom_product,@quantite)";
                
                foreach (string produit in quantity.Keys)
                {

                    add_recette_produit.Parameters.Clear();
                    add_recette_produit.Parameters.AddWithValue("@nom_recette", recette_name.Text);
                    add_recette_produit.Parameters.AddWithValue("@nom_product", produit);
                    add_recette_produit.Parameters.AddWithValue("@quantite", (quantity[produit]));
                    add_recette_produit.ExecuteNonQuery();

                }
                MessageBox.Show("Votre Recette a bien été ajouté.");
                this.Close();


            }
        }
    }
}
