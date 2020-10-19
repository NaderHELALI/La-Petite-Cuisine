using System;

namespace Cooking_BDD
{
    class Commande
    {
        private string id;
        private string id_client;
        private DateTime date;

        public Commande(string id, string id_client, DateTime date)
        {
            this.date = date;
            this.id = id;
            this.id_client = id_client;

        }
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        public string ID_Client
        {
            get { return id_client; }
            set { id_client = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
    }
}
