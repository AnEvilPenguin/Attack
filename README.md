# Attack

Takes inspiration from L'Attaque and another similar but unnamed strategy board game.

The game is played on a tiled board, where there are a set number of hidden peices (of varying rank and capability). The player will be expected to 'scout' out the pieces placed by the opposition and will need to react to chaging information. The game is won when a player captures the flag of the opposing player.

## Player Experience

The player will be able to play against the computer. This will be a 2d no-frills game.
The player must learn what pieces the computer has played and then maneuver their own in order to best it.
To an extent there is a stone, parchment, shears element and feel to the game.

## Platform
The game is developed to be released on Windows PC.

## Development Software

- Godot
- Asperite/MS Paint
- Visual Studio Community
- SQL Lite

## Genre

Singleplayer, turn based strategy

## Target Audience

The game will be marketed towards fans of the original game, and of similar turn based strategy games.

# Concept

## Gameplay overview

The player controls an army. Each piece of the army will have various values, and/or abilities

| Rank | Piece | Count | Special |
| :---: | --- | :---: | --- |
| 0 | Landmine | 6 | Cannot be moved. Destroys any piece (other than Engineer) attacking it |
| 1 | Spy | 1 | Can capture the Commander-in-chief |
| 2 | Scout | 8 | Can move any contiguous distance in a straight line |
| 3 | Engineer | 5 | Can sweep landmines |
| 4 | Sergeant | 4 | |
| 5 | Lieutenant | 4 | |
| 6 | Captain | 4 | |
| 7 | Commandant | 2 | |
| 8 | Colonel | 2 | |
| 9 | Brigadier General | 1 | |
| 10 | Commander-in-chief | 1 | |
| 11 | Flag | 1 | Cannot be moved. Capturing the flag wins the game |

Players take turns moving one of their pieces vertically or horizontally (usually a single space). A player cannot move their piece into the position of another piece.

If adjacent to one of more pieces the active player may choose to attack one of the adjacent pieces. Both players then reveal their pieces and the one with the lower rank is captured, and removed from play. If the units are of equal rank, both are removed from play.

The game is over when a player has no more movable pieces or a flag is captured

## Mechanics

### Primary Mechanics

| Mechanic | Summary |
| --- | --- |
| Movement | Movement is always in a straight line either horizontally or vertically. Unless stated otherwise movement is 1 space. |
| Capturing | Captuting happens from one tile to an adjacent tile (horizontally or vertically). If a piece has just moved it is the only one allowed to capture. |

### Secondary Mechanics

| Mechanic | Summary |
| --- | --- |
| Blockers | Maps will have some sort of blocking 'terrain' to channel movement between the players.
| Reveal | By attacking a piece it is revealed permanently |

## Art Style

A minumal pixel art style will be implemented. This will mostly be left to the discrecion of the art director (Tom).

Pieces will have art appropriate to the piece - the list of pieces can be adjusted (e.g. Sergeant becomes Infantry, Lieutenant becomes Tank, etc.).
Initially the game will not have any background art, however this can be added later as a stretch goal.

Piece art will need to have resolution defined (based on piece size in game), and a finalized list of pieces to implement.

The game will have a main menu, with some room for basic styling.

The game itself will have a board area, with a heads up display around the board itself to provide game details.

Initially there will be a single board. This may be increased later if desired.

## Audio

There will be no audio. This may be added later at a stretch.

# Game Experience

## UI

Minimalistic, but keeping within the style of the game.

## Controls

Initially the game will be mouse only.

As a stretch goal we can consider keyboard/controller.

## MVP

- Design Document
- Placeholder Art Assets
- Main Menu
- Level and Game Implementation
- Pause menu
- Save Game/Load Game system
- Piece selection and movement
- Basic Piece combat
- Full Piece implementation
- Player Transition
- Computer player
- Full Art Assets

## Stretch

- Computer Difficulty settings
- Capture effects
- In game history (Show previous turns)?
- Full screen toggle
- Additional level designs (Lakes, valley, etc.)
- Audio Assets
