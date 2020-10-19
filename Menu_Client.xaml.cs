using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace Cooking_BDD
{
    /// <summary>
    /// Logique d'interaction pour Menu_Client.xaml
    /// </summary>
    public partial class Menu_Client : Window
    {
        private Client client=new Client();
        public static MySqlConnection mySql_cooking;
        public Menu_Client(string id_client)
        {

            InitializeComponent();
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
            client.ID = id_client;
            MySqlCommand nameCommand = mySql_cooking.CreateCommand();
            nameCommand.CommandText = "Select nom from clients where id_client=@id";
            nameCommand.Parameters.AddWithValue("@id", id_client);
            string nom = Convert.ToString(nameCommand.ExecuteScalar());
            client.Nom = nom;
            MySqlCommand prenomCommand = mySql_cooking.CreateCommand();
            prenomCommand.CommandText = "Select prenom from clients where id_client=@id";
            prenomCommand.Parameters.AddWithValue("@id", id_client);
            string prenom = Convert.ToString(prenomCommand.ExecuteScalar());
            client.Prenom = prenom;
            bienvenue.Text = "Bienvenue " + nom + " " + prenom;
            ActualiserSolde();
            


        }

        private void Commande(object sender, RoutedEventArgs e)
        {
            ActualiserSolde();
            commandeDataGrid.Opacity = 0;
            Order_Client order_Client = new Order_Client(client.ID);
            order_Client.Show();

        }
        public void ActualiserSolde()
        {
            MySqlCommand solde_client = mySql_cooking.CreateCommand();
            solde_client.CommandText = "select solde from createur_recette where clients_id_client=@id_client;";
            solde_client.Parameters.AddWithValue("@id_client", client.ID);
            solde.Text = Convert.ToString(solde_client.ExecuteScalar());
        }


        private void Historique(object sender, RoutedEventArgs e)
        {
            
            ActualiserSolde();
            if (commandeDataGrid.Opacity == 0)
            {
                Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));

                commandeDataGrid.Opacity = 100;
                // Chargez les données dans la table commande. Vous pouvez modifier ce code selon les besoins.
                Cooking_BDD.cookingDataSetTableAdapters.commandeTableAdapter cookingDataSetcommandeTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.commandeTableAdapter();
                cookingDataSetcommandeTableAdapter.FillCommande(cookingDataSet.commande, client.ID);
                System.Windows.Data.CollectionViewSource commandeViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("commandeViewSource")));
                commandeViewSource.View.MoveCurrentToFirst();
            }
            else
            {
                commandeDataGrid.Opacity = 0;
            }


        }

        

        private void CDR(object sender, RoutedEventArgs e)
        {
            ActualiserSolde();
            Menu_CDR cDR = new Menu_CDR(client.ID);
            cDR.Show();
        }
    }
}
