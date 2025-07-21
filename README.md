# Inventory Management

## Description
Inventory Management is a robust system built with C# and .NET 8.0 designed to streamline the management of products, suppliers, and customers. This application provides comprehensive functionalities to add, update, delete, and list entities, ensuring efficient inventory tracking and control.

## Diagramme de Classes
![Diagramme de Classes](https://github.com/user-attachments/assets/304b7b78-51a4-4630-a128-f22b89dd9473)

## Fonctionnalités Principales
- **Gestion des Produits** : Ajout, modification, suppression, et affichage des produits.
- **Suivi des Mouvements de Stock** : Gestion des entrées et sorties de stock avec historique détaillé.
- **Alertes de Stock Faible** : Notifications automatiques pour les produits en rupture de stock.
- **Recherche et Filtrage** : Recherche avancée et filtrage des produits par catégories, fournisseurs, etc.
- **Rapports d’Inventaire** : Génération de rapports détaillés pour une analyse approfondie.
- **Interface Utilisateur Conviviale** : Design intuitif et facile à utiliser.

## Technologies Utilisées
- **Langage** : C#
- **Framework** : .NET 8.0
- **Base de Données** : MySQL
- **Frontend** : Razor Pages
- **Outils de Développement** : Visual Studio, Git

## Installation

### Prérequis
- .NET SDK 8.0 installé
- Serveur MySQL en cours d'exécution
- Git installé

### Étapes d'Installation
1. Clonez le dépôt :git clone https://github.com/d-sar/InventoryManagement.git2. Accédez au dossier du projet :cd InventoryManagement3. Configurez la chaîne de connexion MySQL dans `appsettings.json` (exemple fourni) :"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=StockDB;user=root;password=;"
   }4. Appliquez les migrations pour initialiser la base de données :dotnet ef database update5. Lancez l'application :dotnet run
## Contribution
Les contributions sont les bienvenues ! Veuillez soumettre une pull request ou ouvrir une issue pour discuter des modifications.

## Licence
Ce projet est sous licence MIT. Consultez le fichier `LICENSE` pour plus de détails.

## Contact
Pour toute question ou assistance, veuillez contacter l'équipe de développement à [support@inventorymanagement.com](mailto:support@inventorymanagement.com).

