# Inventory Management

## Description
Inventory Management is a robust system built with C# and .NET 8.0 designed to streamline the management of products, suppliers, and customers. This application provides comprehensive functionalities to add, update, delete, and list entities, ensuring efficient inventory tracking and control.

## Diagramme de Classes
![Diagramme de Classes](https://github.com/user-attachments/assets/304b7b78-51a4-4630-a128-f22b89dd9473)

## Fonctionnalit�s Principales
- **Gestion des Produits** : Ajout, modification, suppression, et affichage des produits.
- **Suivi des Mouvements de Stock** : Gestion des entr�es et sorties de stock avec historique d�taill�.
- **Alertes de Stock Faible** : Notifications automatiques pour les produits en rupture de stock.
- **Recherche et Filtrage** : Recherche avanc�e et filtrage des produits par cat�gories, fournisseurs, etc.
- **Rapports d�Inventaire** : G�n�ration de rapports d�taill�s pour une analyse approfondie.
- **Interface Utilisateur Conviviale** : Design intuitif et facile � utiliser.

## Technologies Utilis�es
- **Langage** : C#
- **Framework** : .NET 8.0
- **Base de Donn�es** : MySQL
- **Frontend** : Razor Pages
- **Outils de D�veloppement** : Visual Studio, Git

## Installation

### Pr�requis
- .NET SDK 8.0 install�
- Serveur MySQL en cours d'ex�cution
- Git install�

### �tapes d'Installation
1. Clonez le d�p�t :git clone https://github.com/d-sar/InventoryManagement.git2. Acc�dez au dossier du projet :cd InventoryManagement3. Configurez la cha�ne de connexion MySQL dans `appsettings.json` (exemple fourni) :"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=StockDB;user=root;password=;"
   }4. Appliquez les migrations pour initialiser la base de donn�es :dotnet ef database update5. Lancez l'application :dotnet run
## Contribution
Les contributions sont les bienvenues ! Veuillez soumettre une pull request ou ouvrir une issue pour discuter des modifications.

## Licence
Ce projet est sous licence MIT. Consultez le fichier `LICENSE` pour plus de d�tails.

## Contact
Pour toute question ou assistance, veuillez contacter l'�quipe de d�veloppement � [support@inventorymanagement.com](mailto:support@inventorymanagement.com).

