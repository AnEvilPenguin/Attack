# Attack

Takes inspiration from L'Attaque and another similar but unnamed strategy board game.

The game is played on a tiled board, where there are a set number of hidden peices (of varying rank and capability). The player will be expected to 'scout' out the pieces placed by the opposition and will need to react to changing information. The game is won when a player captures the flag of the opposing player.

## Player Experience

The player will be able to play against the computer. This will be a 2d no-frills game.
The player must learn what pieces the computer has played and then maneuver their own in order to best it.
To an extent there is a quartz, parchment, shears element and feel to the game.

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
| 4 | Private | 4 | |
| 5 | Lance Corporal | 4 | |
| 6 | Corporal | 4 | |
| 7 | Sergeant | 2 | |
| 8 | Lieutenant | 2 | |
| 9 | Captain | 1 | |
| 10 | Colonel | 1 | |
| 11 | General | 1 | Cannot be moved. Capturing the General wins the game |

Players take turns moving one of their pieces vertically or horizontally (usually a single space). A player cannot move their piece into the position of another piece.

If adjacent to one of more pieces the active player may choose to attack one of the adjacent pieces. Both players then reveal their pieces and the one with the lower rank is captured, and removed from play. If the units are of equal rank, both are removed from play.

The game is over when a player has no more movable pieces or a General is captured

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

Left click is to select/move/attack with a piece. Right click cancels the last order (where possible).

## MVP

- [X] Design Document
- [X] Placeholder Art Assets
- [X] Main Menu
- [X] Level and Game Implementation
- [X] Pause menu
- [X] Save Game/Load Game system
- [X] Piece selection and movement
- [X] Basic Piece combat
- [X] Full Piece implementation
- [X] Player Transition
- [X] Computer player
- [X] Full Art Assets

## Stretch

- [ ] Computer Difficulty settings
- [ ] Capture effects
- [ ] In game history (Show previous turns)?
- [X] Full screen toggle
- [ ] Additional level designs (Lakes, valley, etc.)
- [ ] Audio Assets

# Closing Thoughts

Overall I'm mostly pleased with how this worked out. Where I planned things out, things generally went well. However there are a number of places that I didn't plan, generally because of inexperience in the area. These things tended not to go too well. I think this was probably 30% 40% planned really. A lot of this was due to wanting to experiment, so I don't regret this too much. Ultimately this was a quick turn around experience piece first and foremost. Still not too bad for my third c# project from scratch!

I set out to work with a tilemap instead of using a standalone grid as I did in Snek. This made a number of things easier and a number of things harder. Placing and updating Visual assets was significantly easier. Working out where spaces were occupied was reasonably easy. However actions relative to each other were way more dificult. Using a linked list system in combination with the grid would have worked out a lot better. Simply being able to ask a tile for it's neigbours rather than having to go back to the board would have made life a lot easier. The Tile map wound up doing a lot more than it probably should have done due to the complexity of finding tiles relative to another one. That said the vector maths worked out better than expected in this area.

The idea of a state machine tracking what was going on was a good one, though I'm not sure the implementation is as good as it could have been.

I went into the AI a little bit blind. I had some very basic tactics in mind, but these wound up being way too simplistic, and the difficulties with the grid led to the more complex logic becoming a mess. There were also issues with indicating what had happened as a part of the AI turn. I did not consider how the AI was going to play it's turn and did not use any animations that could have helped show what was going on. Even a simple slide into postion would have made things a lot better, had I not tied my hands.

In addition, I leaned on relationships between classes a lot more heavily than I should have done. This was semi-intentional compared to the heavily event driven nature of Snek. Like in a lot of these cases a blend of both would have made things easier.

Logging with Serilog went very well. My implementation was simplistic, however I can really see why people like it. I will definitely be including this in future. SQLLite was also interesting, however I feel this was very much overkill for what I really needed. I'm glad I chose to pick this up and include it in the project, however I think I will be more cautious with picking it in the future.

In my next project I feel I need to learn more about composition. Due to partner preference for the next game jam I will also be trying to pick up Unity with a view to use that there.
