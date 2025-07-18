# FocusGuard API

FocusGuard est une API ASP.NET Core qui aide à bloquer les distractions pendant les sessions de travail. Cette API permet aux utilisateurs de gérer des sessions de concentration et de bloquer des sites web distrayants pour améliorer leur productivité.

## Fonctionnalités principales

- **Gestion des utilisateurs** : inscription, connexion et authentification via JWT
- **Sessions de concentration** : création, démarrage, arrêt et suivi des sessions de travail
- **Liste noire de sites web** : ajout, suppression et vérification des sites web distractifs
- **Statistiques de productivité** : suivi et analyse des habitudes de travail
- **Messages de motivation** : citations aléatoires et messages motivants basés sur les progrès

## Technologies utilisées

- ASP.NET Core 7.0
- Entity Framework Core (avec base de données en mémoire pour la démo)
- JWT pour l'authentification
- Swagger pour la documentation de l'API
- Middleware personnalisé pour la gestion des erreurs

## Mise en route

### Prérequis

- .NET SDK 7.0 ou supérieur
- Un IDE comme Visual Studio, VS Code ou JetBrains Rider

### Installation

1. Clonez le dépôt :
```
git clone https://github.com/votre-username/FocusGuardApi.git
```

2. Naviguez vers le répertoire du projet :
```
cd FocusGuardApi
```

3. Restaurez les packages :
```
dotnet restore
```

4. Exécutez l'application :
```
dotnet run
```

5. Accédez à l'interface Swagger pour tester l'API :
```
https://localhost:5001/swagger
```

## Structure de l'API

### Endpoints principaux

#### Authentification
- `POST /api/auth/register` - Inscription d'un nouvel utilisateur
- `POST /api/auth/login` - Connexion d'un utilisateur
- `POST /api/auth/refresh-token` - Rafraîchissement du token JWT
- `GET /api/auth/me` - Obtention des informations de l'utilisateur actuel

#### Sessions
- `GET /api/session` - Liste des sessions de l'utilisateur
- `GET /api/session/{id}` - Détails d'une session spécifique
- `POST /api/session` - Création d'une nouvelle session
- `PUT /api/session/{id}` - Mise à jour d'une session
- `DELETE /api/session/{id}` - Suppression d'une session
- `POST /api/session/{id}/start` - Démarrage d'une session
- `POST /api/session/{id}/end` - Arrêt d'une session

#### Liste noire
- `GET /api/blacklist` - Liste des sites bloqués
- `GET /api/blacklist/{id}` - Détails d'un site bloqué
- `POST /api/blacklist` - Ajout d'un site à la liste noire
- `PUT /api/blacklist/{id}` - Mise à jour d'un site bloqué
- `DELETE /api/blacklist/{id}` - Suppression d'un site de la liste noire
- `GET /api/blacklist/check` - Vérification si une URL est bloquée

#### Motivation
- `GET /api/motivation/quote` - Citation motivante aléatoire
- `GET /api/motivation/message` - Message de motivation basé sur le nombre de sessions

#### Statistiques
- `GET /api/stats` - Statistiques générales de productivité
- `GET /api/stats/daterange` - Statistiques pour une période spécifique

## Extensions possibles

- Intégration avec des extensions de navigateur pour bloquer automatiquement les sites
- Implémentation d'une base de données persistante (SQL Server, PostgreSQL)
- Ajout de fonctionnalités sociales (partage de statistiques, défis entre amis)
- Système de récompenses et de badges pour la motivation
- Techniques de concentration comme Pomodoro intégrées

## Licence

Ce projet est sous licence MIT. Voir le fichier LICENSE pour plus d'informations.
