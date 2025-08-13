# The Rise and Fall of the Viking

Submission to [Historically Accurate Game Jam 11](https://itch.io/jam/historically-accurate-game-jam-11)

## Screenshots

<img src="https://img.itch.zone/aW1hZ2UvMzc3ODczMS8yMjQ5MTYyNS5wbmc=/original/gcGjwZ.png">

## Gameplay

### Overview

You assume the role of "Viking Spirit," striving to maximize Viking impact during the Viking Age (793-1066). This era begins with the rise of Viking power and concludes with its decline due to Christianization. Your success is measured by Victory Points (VP), so your goal is to accumulate as many as possible by the game's end.

### How to play

#### Basic

- In card playing phase (current phase is displayed in the game tab of topbar), press on card to open context menu, then check or play it to use its card effect or action point (point is increased and can be used in the action phase).
- Press "Next Phase" in the Game tab of topbar to go to the next phase.
- Click areas to open the context menu to check or used a action point to do an action (conquer, raid, trade, etc. Some actions require to select a target area), until action point is used up.
- Press "Next Phase" to go to the next turn and check housekeeping report and receive new card.

Game end when 1066+ year is reached (28 turns).

#### Advanced

For TTS/Vassal enjoyers who like manipulate board freely without much constrait, you can enable Edit Mode in the "Edit & Debug" tab in the topbar to enable almost editing for every component (read only fields become editable and an edit item appear in area context to open advanced area editor).

### Mechanics

#### Phases

This is a card-driven game where each turn consists of two key phases:  

1. Card Playing Phase
   Play cards to:  
   - Generate Action Points (AP), or  
   - Trigger special event effects (if conditions are met)  

2. Action Phase
   Use AP to activate areas and select actions:  
   - Conquer – Seize control of a target areas.  
   - Raid – Pillage hostile territories to softup foes and strengthen yourself.  
   - Trade – Increase resources (roughly representing economic and military strength) for both involved areas.  
   - Counter Influence Campaign – Resist Christianization in a selected area.  
   - Transfer – Move resources between areas with a lord/vassal relationship.  
   - Colonization – Establish control over uninhabited or vulnerable lands.

Some cards trigger immediate effects, such as altering area stats or launching raids/conquests from a chosen area.  

#### Area

Each area tracks four key metrics:  

1. Viking Resource – Wealth and power under Viking control.  
2. Host Resource – Wealth and power held by "host" (local faction).  
3. Viking Control (%) – Degree of Viking dominance in the area.  
4. Christianization (%) – Measures cultural assimilation or feudalization, representing the decline of traditional Viking identity. (Note: Terminology may vary by area.)

#### Victory Points

- VP are generated each turn based on Viking-controlled resources, but Christianization reduces this output proportionally.  
- VP can also be earned by reducing Host Resources through raids and conquests.  
- High Christianization lowers the risk of "Counterattack Wars" from Host factions but also diminishes VP gains. At 100% Christianization, VP generation stops entirely.

#### Resource Hosekeeping

- Reinvestment: Every resources have a chance to generate another resource.  
- Fixed Growth: Each area receives +1 resource, randomly split between Viking and Host based on Viking Occupation %.  
- Host Decline: Host Resources may randomly drop by 50%, due to external factor (wars, plagues, etc.).  

#### Resource Transfer

Resources cannot be freely moved between Viking-held areas unless they have a lord/vassal relationship (can also be explained as "personal union"). Such relationships can be established via:  

- Conquest or colonization
- Card event effects
- Lord Set Points (a rare resource given by certain card effects, enabling the "Set Lord" Button in the right UI).

#### Action Distance Limit

- Trade: 3
- Conquer: 1
- Raid: 2
- Colonization: 1
- Transfer: Unlimited

## Credits

- BGM: Dream-Protocol's Norse Runes: https://pixabay.com/music/ambient-norse-runes-331151/
- Public Field Historical images: Wikipedia
- [Standalone File Browser](https://github.com/gkngkc/UnityStandaloneFileBrowser) and [Unity Native File Picker](https://github.com/yasirkula/UnityNativeFilePicker) for cross-platform file browser support.
