# LaPetiteCuisine
RAPPORT BDD La Petite Cuisine
# I - Organisation de la Base de Donnée

##	Tout commence avec le client qui a plusieurs possibilités : 
•	La première correspond au fait de passer une commande. Chaque client peut commander une ou plusieurs commandes. Ainsi on pourra accéder aux commandes des clients à partir de la clé étrangère(id_client) dans la table commande.
•	La seconde est celle de la création d’une recette. Ainsi le client aura accès aux recettes seulement s’il est enregistré dans la table créateur de recette. Ainsi chaque créateur possède 0 ou plusieurs recettes qui lui sera identifier par la clé étrangère(id_createur) dans la table recette.
## Les besoins d’une création de recette : 
•	Pour une recette est associée à plusieurs produits. Cependant les produits peuvent être attribuer à de nombreuses recettes. Ainsi la fonction de la table prend son sens ici. Cette table permet de stocker les associations en recette et produits. De plus, elle permet également de prévoir la quantité de produits dans chaque recette.
•	Ainsi la société aura besoin de se procurer les produits auprès de ses fournisseurs pour que les recettes restent disponibles pour les clients. Ainsi les réapprovisionnements et la quantité de ceux-là sont stocké dans la table Vend.
## Le fonctionnement des commandes
•	Après que le client est passé la commande, celle-ci est inscrit dans la table commande. Chaque commande est composée d’une ou plusieurs recettes. Or chaque recette n’est pas associée à une seule commande. Par conséquent la table Est_composee prend sens et permet d’enregistrer la dépendance entre une commande et une recette. De plus, elle stocke la quantité de recette commandé dans une commande
La table cooking est ici définit comme celui qui a accès au menu administrateur qui permettra de gérer les commandes et leur livraison. Trop peu d’éléments inscrit dans le cahier des charges concernant se procéder. Ainsi les cuisiniers n’ont pas été intégrer dans la modélisation  
 # II – Option de codage :
On a choisi de développer en WPF afin de créer une interface sobre et facile d’utilisation qui permettra d’attirer tous types de clientèles. 
##	Ce WPF a été réalisé avec une implémentation Open Source du module se nommant ‘ModernWpf’ que l’on a pu acquérir sur GitHub.

#	Menu Principal :

•	Lorsque l’application se lance, un message s’affiche pour vous confirmer la connexion à la Base de données. Après ce message, un label ‘Connected ‘ en vert sera affiché sur le Menu Principale. 

•	Permet de lancer le Menu client ou celui de l’administration. Ces menus se lancent à partir des identifiants et des mots de passe stockés dans la base de données.

# Menu Client : 
•	Fonction permettant de réaliser une commande :	
  Utilisation de ListBox pour avoir une vue globale du panier et des recettes
	Le panier est stocké dans un dictionnaire avant son inscription dans la base de données




•	Accès au Menu Créateur de recette :
  Vérifie si le client est un cdr sinon il l’inscrit dans la table créatrice de recettes.
  Fonction permettant d’ajouter une recette
  Achat de Cook 1€ -> 4 cooks
•	Fonction Historique des commandes


 ## Menu Démo : 
•	Fonction permettant de réaliser : 


 ## Menu Administrateur : 
•	Plusieurs fonctions Explicites :				







##	Remarques : 
•	Concernant l’Xml, on a décidé de le généré en brut plutôt qu’utiliser les pour confirmer nos connaissances dans ce nouveau langage.
•	On a décidé de limiter le nombre de classe, car on a trouvé plus juste d’utiliser un maximum la base de données pour stocker les informations. Ainsi on a pu nous 		familiariser pleinement avec les requêtes SQL dans C#
