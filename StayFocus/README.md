# FocusGuard - Boostez votre productivité

![FocusGuard Logo](https://img.shields.io/badge/FocusGuard-Productivité-4c5eaf)

FocusGuard est une application complète qui vous aide à bloquer les distractions pendant vos sessions de travail, à suivre votre productivité et à maintenir votre concentration. Développée avec ASP.NET Core pour le backend et une interface utilisateur HTML/CSS/JS simple pour le frontend.

## Table des matières

- [Aperçu](#aperçu)
- [Fonctionnalités](#fonctionnalités)
- [Architecture](#architecture)
- [Installation](#installation)
- [Démarrage](#démarrage)
- [Identifiants de test](#identifiants-de-test)
- [API Documentation](#api-documentation)
- [Captures d'écran](#captures-décran)
- [Développeurs](#développeurs)

## Aperçu

FocusGuard est une solution tout-en-un pour gérer vos sessions de concentration. L'application vous permet de créer des sessions de travail, de bloquer les sites web distrayants et de suivre vos statistiques de productivité. Le système repose sur une API RESTful sécurisée avec JWT et une interface utilisateur réactive.

## Fonctionnalités

- **Gestion des utilisateurs**
  - Inscription et connexion sécurisées
  - Authentification par JWT
  - Profils utilisateurs personnalisables

- **Sessions de concentration**
  - Création de sessions avec durée planifiée
  - Minuteur intégré
  - Prise de notes pour chaque session
  - Historique complet des sessions

- **Liste noire de sites web**
  - Ajout de sites distrayants à bloquer
  - Possibilité d'indiquer une raison pour chaque site
  - Activation/désactivation facile

- **Statistiques de productivité**
  - Suivi du temps total de concentration
  - Analyse des habitudes de travail
  - Visualisation des tendances

- **Motivation**
  - Citations inspirantes
  - Messages d'encouragement personnalisés

## Architecture

- **Backend**: ASP.NET Core API
  - Entity Framework Core
  - JWT pour l'authentification
  - Architecture en couches (Contrôleurs, Services, Modèles)

- **Frontend**: Application web simple
  - HTML, CSS, JavaScript natif
  - Interface réactive
  - Communication API REST

## Installation

### Prérequis

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Navigateur web moderne

### Cloner le repository

```bash
git clone https://github.com/votre-username/FocusGuard.git
cd FocusGuard
```

## Démarrage

### Backend (API)

1. Naviguer vers le dossier de l'API:

```bash
cd /Users/maxmengeringhausen/IPI/LinQ/FocusGuardApi
```

2. Restaurer les packages:

```bash
dotnet restore
```

3. Exécuter l'API:

```bash
dotnet run
```

L'API sera disponible à l'adresse: http://localhost:5114

### Frontend

1. Naviguer vers le dossier du frontend:

```bash
cd /Users/maxmengeringhausen/IPI/LinQ/FocusGuardFront
```

2. Lancer un serveur HTTP simple:

```bash
python3 -m http.server 8080
```

3. Accéder à l'interface utilisateur dans votre navigateur:

```
http://localhost:8080
```

## Identifiants de test

Pour tester l'application sans créer de compte, vous pouvez utiliser les identifiants suivants:

```
Nom d'utilisateur: demo
Mot de passe: Password123!
```

## API Documentation

La documentation Swagger de l'API est disponible à l'adresse:

```
http://localhost:5114/swagger
```

### Endpoints principaux

| Méthode | URL                           | Description                               |
|---------|-------------------------------|-------------------------------------------|
| POST    | /api/auth/register            | Inscription d'un nouvel utilisateur        |
| POST    | /api/auth/login               | Connexion utilisateur                     |
| GET     | /api/session                  | Liste des sessions                        |
| POST    | /api/session                  | Création d'une session                    |
| POST    | /api/session/{id}/start       | Démarrage d'une session                   |
| POST    | /api/session/{id}/end         | Fin d'une session                         |
| GET     | /api/blacklist                | Liste des sites bloqués                   |
| POST    | /api/blacklist                | Ajout d'un site à la liste noire          |
| GET     | /api/stats                    | Statistiques de productivité              |
| GET     | /api/motivation/quote         | Citation aléatoire                        |

## Captures d'écran

### Page de connexion
![Page de connexion](https://via.placeholder.com/800x450.png?text=Page+de+connexion)

### Tableau de bord
![Tableau de bord](https://via.placeholder.com/800x450.png?text=Tableau+de+bord)

### Session de concentration
![Session active](https://via.placeholder.com/800x450.png?text=Session+active)

## Développeurs

- Max Mengeringhausen - Développeur principal

---

© 2025 FocusGuard. Tous droits réservés.
