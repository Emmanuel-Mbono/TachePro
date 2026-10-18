# TâchePro – application .NET MAUI

Gestion de tâches d'équipe : création, assignation, échéance, statut, filtre par membre et tableau d'avancement.
Projet de l'Atelier de développement Mobile (Activité N°5 → implémentation des maquettes).

## Lancer le projet
1. Visual Studio Community 2026 avec la charge de travail **Développement d'applications mobiles .NET (MAUI)**.
2. Ouvrir `TachePro.sln`, laisser NuGet restaurer `CommunityToolkit.Mvvm`.
3. Choisir la cible **Windows Machine** (test rapide) ou un **émulateur Android**, puis F5.

Premier lancement : *S'inscrire*, puis sur l'écran « Aucune tâche » appuyer sur **Charger des données d'exemple** pour la démonstration.

## Correspondance maquettes → code
| Écran | Fichiers |
|---|---|
| 1 Connexion | `Views/LoginPage` · `ViewModels/LoginViewModel` |
| 2 Inscription | `Views/RegisterPage` · `RegisterViewModel` |
| 3 Tableau d'avancement | `Views/DashboardPage` · `DashboardViewModel` |
| 4 Liste + filtre par membre | `Views/TasksPage` · `TasksViewModel` |
| 5 Création de tâche (et modification) | `Views/TaskEditPage` · `TaskEditViewModel` |
| 6 Détail et statut | `Views/TaskDetailPage` · `TaskDetailViewModel` |
| 7 Équipe | `Views/TeamPage` · `TeamViewModel` |
| 8 État vide | `TasksPage` (affiché quand il n'y a aucune tâche) |
| 9 Profil | `Views/ProfilePage` · `ProfileViewModel` |
| 10 Chargement | `TasksPage` (squelettes affichés pendant la lecture des données) |

## Navigation (`AppShell.xaml` / `AppShell.xaml.cs`)
- `login` → `//dashboard` après connexion ou inscription ; `register` s'ouvre depuis la connexion.
- `TabBar` de 4 onglets : Accueil, Tâches, Équipe, Profil.
- Routes empilées : `taskdetail?id=…` et `taskedit` / `taskedit?id=…` (modification).
- Déconnexion (Profil) → `//login`.

## Architecture
MVVM avec `CommunityToolkit.Mvvm` et injection de dépendances (`MauiProgram.cs`).
`Models/` données · `Services/` (`DataService` : stockage JSON local, `AuthService` : comptes et session) · `ViewModels/` · `Views/` · `Resources/Styles/` (charte, styles, modèle de carte partagé).

## Écoconception appliquée
Aucune image bitmap ni police embarquée, icônes SVG minuscules, palette sobre, thème sombre, stockage local sans réseau, listes virtualisées (`CollectionView`), chargement des données uniquement à l'ouverture d'un écran.

## Limites connues
- Les données sont **locales à l'appareil** (une équipe par installation). Une vraie équipe multi-appareils demanderait une API et une base distante.
- Le réglage « Notifications » enregistre la préférence, mais aucune notification n'est encore émise.
- Le mot de passe oublié n'est pas disponible hors ligne (message explicatif affiché).
- Cibles : Android et Windows. iOS et Mac Catalyst peuvent être ajoutés dans `TachePro.csproj` (build sur Mac requis).

## En cas de problème
Si Visual Studio signale une erreur de configuration (version de MAUI, icône, manifeste) : créer un projet vierge **Application .NET MAUI** nommé `TachePro`, ajouter `CommunityToolkit.Mvvm`, puis copier dans ce projet les dossiers `Models`, `Services`, `ViewModels`, `Views`, `Resources`, ainsi que `App.xaml(.cs)`, `AppShell.xaml(.cs)` et `MauiProgram.cs` (en remplaçant les fichiers existants).
Si le contenu passe sous la barre d'état Android, définir `SafeAreaEdges="Container"` sur les `ContentPage`.
