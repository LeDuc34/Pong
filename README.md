# Pong versus IA

## Objectif du jeu

Affrontez une IA entraînée et soyez le premier à marquer **7 points** dans ce classique revisité du Pong !

---

##  Installation

Téléchargez le projet directement utilisable depuis Unity [ici](https://www.swisstransfer.com/d/5e233a0b-1982-4b9c-b2bb-e80dae75867f) 

---

##  Description du projet

### Architecture des Scripts

Le projet contient **4 scripts principaux** situés dans le dossier `Scripts` :

- **`BallController`** : Gère le comportement de la balle (maintien au sol, vitesse constante)
- **`PongAgent`** : Contrôle la raquette de l'IA et gère l'entraînement de l'agent
- **`GameManager`** : Orchestre la logique globale du jeu
- **`UIManager`** : Gère l'interface utilisateur et les interactions

### ML-Agents Unity

Ce projet utilise **Unity ML-Agents** pour créer un adversaire redoutable.

#### Stratégie d'entraînement

L'entraînement repose sur des **affrontements entre deux agents IA** qui s'améliorent mutuellement. 

**Objectifs d'un épisode** :
- Rattraper la balle
- **Marquer des points** (récompense élevée)
- **Éviter d'encaisser des points** (récompense prioritaire)

Le système de récompenses privilégie la défense et la précision offensive pour développer un jeu équilibré.

### La Scène

Un **stade futuriste** sous un ciel étoilé, deux raquettes et une balle : l'essentiel pour une expérience immersive !

### Interface Utilisateur

L'UI offre toutes les fonctionnalités essentielles :
- Lancement de partie
- Affichage du score en temps réel
- Chronomètre de jeu
- Système de pause
- Option de relance après une défaite

---

## Difficultés rencontrées

### Configuration ML-Agents

**Problème** : Installation complexe du setup ML-Agents avec des problèmes de compatibilité CUDA notamment.

### Design de l'entraînement

Le principal défi a été de concevoir un épisode d'entraînement efficace :

- **Comment récompenser/pénaliser l'agent ?**
- **Quelle stratégie adopter ?**

#### Approches testées :

1. **Apprentissage mutuel** : Deux agents apprenant l'un de l'autre
2. **Agent coach** : Une raquette heuristique servant de référence
3. **Curriculum Learning** : Apprentissage progressif
   - Phase 1 : Apprendre à renvoyer la balle
   - Phase 2 : Optimiser les marquages de points

**Résultat** : Faute de temps et de puissance de calcul, le modèle actuel utilise des **heuristiques par défaut** (aucun poids chargé), qui offrent déjà un défi conséquent.

### Gestion de la physique

**Problème initial** : Tentative de gestion manuelle des déplacements et rebonds de la balle.

**Solution adoptée** : Utilisation du moteur physique natif d'Unity 3D, offrant :
- Une mise en place simplifiée
- Un rendu de meilleure qualité
- Des collisions réalistes

---

## 🔧 Améliorations futures

- Entraînement d'un modèle ML-Agents plus performant avec davantage de ressources
- Ajout de niveaux de difficulté
---

