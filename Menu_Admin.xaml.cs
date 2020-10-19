using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing.Printing;
using System.IO;
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
    /// Logique d'interaction pour Menu_Admin.xaml
    /// </summary>

    public class top5week
    {
        public string nom_recette { get; set; }
        public string type { get; set; }
        public string id_createur { get; set; }
        public string nb_fois { get; set; }

    }
    public partial class Menu_Admin : Window
    {

        public static MySqlConnection mySql_cooking;
        public Menu_Admin()
        {
            InitializeComponent();
            mySql_cooking = new MySqlConnection("database=cooking;server=localhost; user id = root ;  pwd=tKyD4bRJ");
            mySql_cooking.Open();
        }

        private void Top_Week(object sender, RoutedEventArgs e)
        {
            Gestion_Produit_grid.Opacity = 0;
            Gestion_fournisseur.Opacity = 0;
            Gestion_Client.Opacity = 0;
            Gestion_recettegrid.Opacity = 0;
            Admingrid.Opacity = 0;
            topDataGrid.Items.Clear();
            best_recette.Items.Clear();


            if (top_weekgrid.Opacity == 0)
            {
                top_weekgrid.Opacity = 100;
                TopWeek();
                Top5Week_Grid();
                TopCdr();
                Top5Cdr_recette();

            }
            else { top_weekgrid.Opacity = 0;
            }
        }

        private void Top5Week_Grid()
        {
            MySqlCommand top_week = mySql_cooking.CreateCommand();
            top_week.CommandText = "select  c.nom_recette,r.type,r.id_createur,sum(c.quantite) as 'nb_fois' from commande, est_composee as c , recette as r where commande.id_commande = c.id_commande " +
                "and c.nom_recette = r.nom_recette  and(adddate(commande.date, 7) >= @datenow) group by  c.nom_recette order by sum(c.quantite) desc limit 5";
            top_week.Parameters.AddWithValue("@datenow", DateTime.Now);
            MySqlDataReader dr;
            dr = top_week.ExecuteReader();
            while (dr.Read())
            {
                var data = new top5week { nom_recette = Convert.ToString(dr["nom_recette"]), type = Convert.ToString(dr["type"]), id_createur = Convert.ToString(dr["id_createur"]), nb_fois = Convert.ToString(dr["nb_fois"]) };

                topDataGrid.Items.Add(data);
            }

            dr.Close();
        }
        private void TopWeek()
        {
            MySqlCommand top_week = mySql_cooking.CreateCommand();
            top_week.CommandText = "select  c.nom_recette,sum(c.quantite)as 'nb_fois' from commande, est_composee as c where commande.id_commande = c.id_commande and(adddate(commande.date, 7) >= @datenow) " +
                                    "group by  c.nom_recette order by sum(c.quantite) desc limit 1; ";
            top_week.Parameters.AddWithValue("@datenow", DateTime.Now);
            MySqlDataReader dr;
            dr = top_week.ExecuteReader();
            while (dr.Read())
            {
                nom_recette.Text = Convert.ToString(dr["nom_recette"]) + " :  " + Convert.ToString(dr["nb_fois"]) + " fois.";


            }

            dr.Close();
        }

        private void TopCdr()
        {

            MySqlCommand top_cdr = mySql_cooking.CreateCommand();
            top_cdr.CommandText = "select c.nom,c.prenom from clients as c Join createur_recette as cdr on c.createur_recette_id_createur = cdr.id_createur where cdr.id_createur = (select  r.id_createur from commande , est_composee as c , recette as r where commande.id_commande = c.id_commande and c.nom_recette = r.nom_recette  group by  id_createur order by sum(c.quantite) desc limit 1)";

            MySqlDataReader dr;
            dr = top_cdr.ExecuteReader();
            while (dr.Read())
            {
                Best_cdr.Text = "Top Créateur : " + Convert.ToString(dr["nom"]).ToUpper() + " " + Convert.ToString(dr["prenom"]);
            }

            dr.Close();
        }
        private void Top5Cdr_recette()
        {

            MySqlCommand top_cdr = mySql_cooking.CreateCommand();
            top_cdr.CommandText = "select r.nom_recette from recette as r  join est_composee as c on c.nom_recette=r.nom_recette where r.id_createur = (select  r.id_createur from commande , est_composee as c , recette as r where commande.id_commande=c.id_commande and c.nom_recette=r.nom_recette  group  by  id_createur  order by sum(c.quantite) desc limit 1) group by nom_recette  order by sum(c.quantite) desc limit 5;";

            MySqlDataReader dr;
            dr = top_cdr.ExecuteReader();
            while (dr.Read())
            {
                var data = new top5week { nom_recette = Convert.ToString(dr["nom_recette"]) };
                best_recette.Items.Add(data);
            }

            dr.Close();
        }



        private void AcualiserFournisseur()
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
            // Impossible de générer ce code, car la méthode cookingDataSetfournisseurTableAdapter.Fill est manquante ou a des paramètres non reconnus.
            Cooking_BDD.cookingDataSetTableAdapters.fournisseurTableAdapter cookingDataSetfournisseurTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.fournisseurTableAdapter();
            cookingDataSetfournisseurTableAdapter.Fill(cookingDataSet.fournisseur);
            System.Windows.Data.CollectionViewSource fournisseurViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fournisseurViewSource")));
            fournisseurViewSource.View.MoveCurrentToFirst();

        }
        private void Gestion_Fournisseur(object sender, RoutedEventArgs e)
        {
            Gestion_Produit_grid.Opacity = 0;
            top_weekgrid.Opacity = 0;
            Gestion_Client.Opacity = 0;
            Gestion_recettegrid.Opacity = 0;
            Admingrid.Opacity = 0;

            if (Gestion_fournisseur.Opacity == 0)
            {
                AcualiserFournisseur();
                Gestion_fournisseur.Opacity = 100;

            }
            else { Gestion_fournisseur.Opacity = 0; }
        }
        private void Ajouter_fournisseur(object sender, RoutedEventArgs e)
        {
            
            // Impossible de générer ce code, car la méthode cookingDataSetfournisseurTableAdapter.Fill est manquante ou a des paramètres non reconnus.
            Cooking_BDD.cookingDataSetTableAdapters.fournisseurTableAdapter cookingDataSetfournisseurTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.fournisseurTableAdapter();
            cookingDataSetfournisseurTableAdapter.InsertFournisseur(ID_Fournisseur(), name_fournisseur.Text, tel_fournisseur.Text);
            AcualiserFournisseur();
            name_fournisseur.Text = "";
            tel_fournisseur.Text = "";

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
        public string ID_Fournisseur()
        {
            string id_fournisseur;
            bool valid = false;
            do
            {

                MySqlCommand nbCommande = mySql_cooking.CreateCommand();
                nbCommande.CommandText = "Select count(*) from fournisseur where id_fournisseur=@id_fournisseur ;";
                id_fournisseur = RandomString(8);
                nbCommande.Parameters.AddWithValue("@id_fournisseur", id_fournisseur);
                if (Convert.ToInt32(nbCommande.ExecuteScalar()) == 1)
                {
                    valid = true;
                }


            } while (valid == true);
            return id_fournisseur;
        }



        private void Gestion_Produit(object sender, RoutedEventArgs e)
        {
            top_weekgrid.Opacity = 0;
            Gestion_fournisseur.Opacity = 0;
            Gestion_Client.Opacity = 0;
            Gestion_recettegrid.Opacity = 0;
            Admingrid.Opacity = 0;
            if (Gestion_Produit_grid.Opacity == 0)
            {
                Actualiser_Produit();
                Gestion_Produit_grid.Opacity = 100;
            }
            else { Gestion_Produit_grid.Opacity = 0; }
        }
        private void Actualiser_Produit()
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
            Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter cookingDataSetproductTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter();
            cookingDataSetproductTableAdapter.FillByProduct(cookingDataSet.product);
            System.Windows.Data.CollectionViewSource productViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("productViewSource")));
            productViewSource.View.MoveCurrentToFirst();
        }
        private void Add_product(object sender, RoutedEventArgs e)
        {
            
            Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter cookingDataSetproductTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.productTableAdapter();
            cookingDataSetproductTableAdapter.InsertProduct(name_produit.Text, cat_produit.Text, unit_produit.Text, Convert.ToInt32(stock.Text), Convert.ToInt32(max_stock.Text), Convert.ToInt32(min_stock.Text));
            Actualiser_Produit();
            

            Cooking_BDD.cookingDataSetTableAdapters.vendTableAdapter vendDataSetproductTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.vendTableAdapter();
            string id = Convert.ToString(CommandR("select id_fournisseur from fournisseur where name_fournisseur='" + Name_fournisseur.Text + "';")[0]);
            vendDataSetproductTableAdapter.InsertQuery(name_produit.Text,id);
            name_produit.Text = "";
            cat_produit.Text = "";
            unit_produit.Text = "";
            stock.Text = "";
            min_stock.Text = "";
            max_stock.Text = "";

        }



        private void gestion_client(object sender, RoutedEventArgs e)
        {
            top_weekgrid.Opacity = 0;
            Gestion_Produit_grid.Opacity = 0;
            Gestion_fournisseur.Opacity = 0;
            Gestion_recettegrid.Opacity = 0;
            Admingrid.Opacity = 0;
            if (Gestion_Client.Opacity == 0)
            {
                Gestion_Client.Opacity = 100;
                ActualiseClient();
            }
            else { Gestion_Client.Opacity = 0; }
        }
        private void ActualiseClient()
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
            Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter cookingDataSetclientsTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter();
            cookingDataSetclientsTableAdapter.Fill(cookingDataSet.clients);
            System.Windows.Data.CollectionViewSource clientsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientsViewSource")));
            clientsViewSource.View.MoveCurrentToFirst();
        }
        private void Delete_Client(object sender, RoutedEventArgs e)
        {


            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
            DataRow data = cookingDataSet.clients[clientsDataGrid.SelectedIndex];
            string items = Convert.ToString(data["createur_recette_id_createur"]);
            string id = Convert.ToString(data["id_client"]);
            if ((items == "") && (id == ""))
            {
                MessageBox.Show("Vous n'avez rien selectionné . ");
            }
            else
            {
                if (items != "")
                {
                    Delete_Cdr(sender, e);
                    Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter cookingDataSetclientsTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter();
                    cookingDataSetclientsTableAdapter.DeleteQuery(id);

                }
                else
                {

                    Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter cookingDataSetclientsTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter();
                    cookingDataSetclientsTableAdapter.DeleteQuery(id);
                }
            }
            ActualiseClient();
        }

        private void Delete_Cdr(object sender, RoutedEventArgs e)
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));

            DataRow data = cookingDataSet.clients[clientsDataGrid.SelectedIndex];
            string items = Convert.ToString(data["createur_recette_id_createur"]);

            

            MySqlCommand my_recette = mySql_cooking.CreateCommand();
            my_recette.CommandText = "select nom_recette from recette where id_createur=@cdr";
            my_recette.Parameters.AddWithValue("@cdr", items);
            MySqlDataReader dr;
            dr = my_recette.ExecuteReader();
            while (dr.Read())
            {

                string nom_recette = Convert.ToString(dr["nom_recette"]);

                Cooking_BDD.cookingDataSetTableAdapters.contientTableAdapter cookingDataSetingredientTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.contientTableAdapter();
                cookingDataSetingredientTableAdapter.DeleteRecetteIngredient(nom_recette);

                Cooking_BDD.cookingDataSetTableAdapters.est_composeeTableAdapter cookingDataSetcontientTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.est_composeeTableAdapter();
                cookingDataSetcontientTableAdapter.DeleteContientRecette(nom_recette);

            } dr.Close();


            if (items == "")
            {
                MessageBox.Show("Vous n'avez rien selectionné et/ou le client n'est pas un CDR.");
            }
            else
            {


                Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter cookingDataSetrecetteTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter();
                cookingDataSetrecetteTableAdapter.DeleteRecette(items);

                Cooking_BDD.cookingDataSetTableAdapters.createur_recetteTableAdapter cookingDataSetcdrTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.createur_recetteTableAdapter();
                cookingDataSetcdrTableAdapter.DeleteQuery(items);

                Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter cookingDataSetclientsTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.clientsTableAdapter();
                cookingDataSetclientsTableAdapter.UpdateCDR_ID(items);

                ActualiseClient();


            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // TODO: ajoutez du code ici pour charger les données dans la table recette.
            // Impossible de générer ce code, car la méthode cookingDataSetrecetteTableAdapter.Fill est manquante ou a des paramètres non reconnus.

        }

        private void Gestion_recette(object sender, RoutedEventArgs e)
        {
            Gestion_Produit_grid.Opacity = 0;
            Gestion_fournisseur.Opacity = 0;
            Gestion_Client.Opacity = 0;
            top_weekgrid.Opacity = 0;
            Admingrid.Opacity = 0;
            if (Gestion_recettegrid.Opacity == 0)
            {
                ActualiseRecette();
                Gestion_recettegrid.Opacity = 100;
            }
            else { Gestion_recettegrid.Opacity = 0; }

        }
        private void ActualiseRecette()
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));
            Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter cookingDataSetrecetteTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter();
            cookingDataSetrecetteTableAdapter.FillBy(cookingDataSet.recette);
            System.Windows.Data.CollectionViewSource recetteViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("recetteViewSource")));
            recetteViewSource.View.MoveCurrentToFirst();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cooking_BDD.cookingDataSet cookingDataSet = ((Cooking_BDD.cookingDataSet)(this.FindResource("cookingDataSet")));

            DataRow data = cookingDataSet.recette[recetteDataGrid.SelectedIndex];
            string items = Convert.ToString(data["nom_recette"]);
            Cooking_BDD.cookingDataSetTableAdapters.contientTableAdapter cookingDataSetingredientTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.contientTableAdapter();
            cookingDataSetingredientTableAdapter.DeleteRecetteIngredient(items);

            Cooking_BDD.cookingDataSetTableAdapters.est_composeeTableAdapter cookingDataSetcontientTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.est_composeeTableAdapter();
            cookingDataSetcontientTableAdapter.DeleteContientRecette(items);

            Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter cookingDataSetrecetteTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.recetteTableAdapter();
            cookingDataSetrecetteTableAdapter.DeleteQuery(items);

            ActualiseRecette();

        }

        private void add_Admin(object sender, RoutedEventArgs e)
        {
            Gestion_Produit_grid.Opacity = 0;
            Gestion_fournisseur.Opacity = 0;
            Gestion_Client.Opacity = 0;
            top_weekgrid.Opacity = 0;
            Gestion_recettegrid.Opacity = 0;
            if (Admingrid.Opacity == 0)
            {
                Admingrid.Opacity = 100;



            }
            else { Admingrid.Opacity = 0; }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if ((id_admin.Text != "") && (pass_admin.Password != ""))
            {
                // Impossible de générer ce code, car la méthode cookingDataSetfournisseurTableAdapter.Fill est manquante ou a des paramètres non reconnus.
                Cooking_BDD.cookingDataSetTableAdapters.cookingTableAdapter cookingDataSetfournisseurTableAdapter = new Cooking_BDD.cookingDataSetTableAdapters.cookingTableAdapter();
                cookingDataSetfournisseurTableAdapter.InsertQuery(id_admin.Text, pass_admin.Password);
                MessageBox.Show("Votre nouvelle admin a bien été ajouté.");
            }
            else { MessageBox.Show("Il manque des éléments."); }
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

        private void Réapprovisionner(object sender, RoutedEventArgs e)
        {
            List<string> fournisseur = CommandR("select name_fournisseur from fournisseur ;");// recuperation des nom des fournisseurs trier 

            try
            {
                try
                {
                    // Instanciation du StreamWriter avec passage du nom du fichier 
                    StreamWriter monStreamWriter = new StreamWriter("XML.txt");
                    monStreamWriter.WriteLine("<Reaprovisionnement>\n\n");
                    for (int i = 0; i < fournisseur.Count; i++)
                    {
                        monStreamWriter.WriteLine("  <Fournisseur>\n");
                        monStreamWriter.WriteLine("   <Nom>" + fournisseur[i] + "</Nom>\n");
                        List<string> aliment = CommandR("select p.name_product from product as p join vend as v on v.name_product=p.name_product join fournisseur as f on v.id_fournisseur=f.id_fournisseur where f.name_fournisseur ='" + fournisseur[i] + "' order by p.name_product");// recuperation des produits qu'on doit realimenter triée
                        for (int j = 0; j < aliment.Count; j++)
                        {
                            monStreamWriter.WriteLine("   <Produit>");
                            monStreamWriter.WriteLine("    <nom>" + aliment[j] + "</nom>");
                            int quantite = Convert.ToInt32(CommandR("select max_stock from product where name_product='" + aliment[j] + "' ;")[0]) - Convert.ToInt32(CommandR("select stock from product where name_product='" + aliment[j] + "' ;")[0]);
                            monStreamWriter.WriteLine("    <quantite>" + quantite + "</quanite>");

                            monStreamWriter.WriteLine("   </Produit>\n");
                        }
                        monStreamWriter.WriteLine("  </Fournisseur>\n\n");
                    }
                    monStreamWriter.WriteLine("</Reaprovisionnement>");
                    //Ecriture du texte dans votre fichier 



                    // Fermeture du StreamWriter (Très important) 
                    monStreamWriter.Close();
                    Console.WriteLine("XML COMPLETE");
                    MessageBox.Show("Le dossier a bien été crée");
                }
                catch (Exception ex)
                {
                    // Code exécuté en cas d'exception 
                    Console.WriteLine(ex);
                }

            }
            catch { Console.WriteLine("Pas de produit a reaprovisionner"); }
        } 
        
    }
}

    
