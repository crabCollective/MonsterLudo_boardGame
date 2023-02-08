# MonsterLUDO
MonsterLUDO - board game made for the purposes of code demonstration

### About
This game is variation of a popular LUDO game. Two teams represented by box-like cute monsters compete with each other. The team which first occupies all the final slots wins!
You can find the game here: https://collectiveofcrab.com/monsterludo/

I formerly created this as a test task for the company I was interviewing with and now I am showing this publicly for the people to get better understanding of my coding abilities and project structuring.
This game is bare working minimum and it definitely shouldn't be taken as a final product. Actually, it would need quite a lot polish to be considered as a proper game. 

**Rules:**
- Each turn consists from 3 actions - figure selection, dice roll and figure movement
- Each team can use either dice or superdice to turn. Superdice is charged once per three rounds and when used, the team can choose certain number of tiles being walked
- If you step on the tile with another monster, this monster is sent back to the start. But be careful - friendly kick is on!
- Two bonuses on the way - spikes will send you back to the start, the dice tile will make you turn again

**Controls:**
- Meant to be controlled by keyboard.
- WD/LR or arrows to choose figures/dice number, SPACE/ENTER for confirmation actions
- F1 to restart, ESC to quit
- On the first screen, when the game title is being shown, please press SPACE/ENTER and you will start playing

### Used assets
Multiple assets has been used for creation of this game. Because of the legal issues I am not permitted to put some these assets to this GIT repository, so you won't be able to run it in the Unity editor as long as you don't have FEEL and Simple Waypoint System assets added to the project by your own.

**Main graphics and animations:**
- Meshtint Free Boximon Cyclopes and Fiery (https://assetstore.unity.com/packages/3d/characters/meshtint-free-boximon-fiery-mega-toon-series-153958)
- Meshtint Free Tile Map (https://assetstore.unity.com/packages/3d/environments/meshtint-free-tile-map-mega-toon-series-153619)
- Sweet Land GUI https://assetstore.unity.com/packages/2d/gui/sweet-land-gui-208285
- Few elements used from Polygon prototype and Archanor sci-fi pack

**Coding and design:**
- FEEL framework (https://assetstore.unity.com/packages/tools/particles-effects/feel-183370)
- Simple Waypoint system (https://assetstore.unity.com/packages/tools/animation/simple-waypoint-system-2506)

### Known issues:
- after restart, no music is playing - problem with MM_Music manager from FEEL framework
- problems with round text after restart - again FEEL-related problem, partially fixed
- figures sometimes jump when they shouldn't

### Unity editor compatibility:
Tested in Unity 2021.3.17f1
