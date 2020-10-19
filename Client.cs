namespace Cooking_BDD
{
    class Client
    {
        private string id_client;
        private string pseudo;
        private string mdp;
        private string nom;
        private string prenom;
        private string telephone;
        private string email;
        private string id_cdr;

        public Client(string id_client, string pseudo, string mdp, string nom, string prenom, string telephone, string email, string id_cdr)
        {
            this.id_client = id_client;
            this.pseudo = pseudo;
            this.mdp = mdp;
            this.prenom = prenom;
            this.nom = nom;
            this.telephone = telephone;
            this.email = email;
            this.id_cdr = id_cdr;
        }
        public Client(string id_client)
        {
            this.id_client = id_client;
        }
        public Client()
        {
            
        }

        public string ID
        {
            get { return id_client; }
            set { id_client = value; }
        }
        public string Pseudo
        {
            get { return pseudo; }
            set { pseudo = value; }
        }
        public string Mdp
        {
            get { return mdp; }
            set { mdp = value; }
        }
        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }
        public string Prenom
        {
            get { return prenom; }
            set { prenom = value; }
        }
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string ID_cdr
        {
            get { return email; }
            set { email = value; }
        }



    }
}
