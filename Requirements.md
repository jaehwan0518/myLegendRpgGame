# Project Title: CLI Turn-Based RPG: The Legend of Heroes

### Overview
This project is a text-based RPG where the user controls a party of four heroes to defeat enemies in each stage.[cite: 2]

---

### Requirements

**1. Party Management**
The system initializes a party consisting of four distinct classes (Warrior, Tanker, Priest, Mage), each with unique attributes (HP, Armor, MR) and skills.

**2. Status Display**
At the beginning of each turn, the system displays the current status (Name, HP/MaxHP, Armor, Magic Resistance, and active Buffs/Debuffs) for all players and enemies in a formatted table.

**3. Turn-Based Flow**
The gameplay follows a cyclical turn structure, where each single turn is defined by the sequential completion of an 'Enemy Phase' followed by a 'Player Phase'.

**4. Skill System**
Each hero provides three skill options (Skill 1, Skill 2, and Ultimate).

**5. Targeting Logic**
The system allows the user to select specific targets for single-target skills. If an enemy is using a 'Taunt' skill, the player's single-target attacks must only target the taunting enemies.

**6. Damage Calculation**
* **Physical & Magic Damage**: Damage is categorized into Physical (reduced by Ar) and Magic (reduced by Mr).
* **Defense Mechanism**: The final damage is calculated by multiplying the skill's base attack power by `(1 - Target's Defense Value (Ar or Mr))`.
* **Execution Logic**: The internal system calculates the damage automatically once the user selects a skill and a target. While the detailed calculation steps are hidden from the user, the final damage dealt is reflected in the action message.
* **Documentation**: Detailed descriptions and base power for each skill will be provided in the README.md file.

**7. Cooldown Enforcement**
If the user selects a skill that is currently on cooldown, the system must display a `"Not ready yet!"` message and prompt the user to choose a different skill without skipping the turn.

**8. Condition-Based Termination**
* **Victory**: If all enemies' HP reaches 0, the system displays a `"STAGE CLEAR"` message and proceeds to the next stage or ends the game.
* **Defeat & Restart**: If all players' HP reaches 0, the system displays a `"GAME OVER"` message and `"Press Enter to try again"` message. Once the user presses Enter, the system clears the console and the user can restart the game.

---

### Example Interaction

* The system displays the current HP and status table for both heroes and enemies.[cite: 2]
* The system executes enemy actions. *(e.g., Slime A’s Acid Attack! Dealt 4.0 magic damage to Tanker.)*
* The turn passes to the first hero. *(e.g., [Warrior]’s turn. Choose a skill: (1, 2, or 3 and push enter))*
* If the user attempts to use a skill currently on cooldown, then system executes a ‘not ready message’. *(e.g., Not ready yet! 2 turn(s) remaining. Please choose another skill.)*
* The user selects a valid skill and target *(e.g., input: 1 (Skill selection) -> input: 2 (Target Selection))*
* The system calculates damage and updates the status. *(e.g., Warrior used ‘Horizontal Slash’! Dealt 40.0 physical damage to Slime B.)*
* Once all heroes have acted, the system prompts the user to move to the next turn. *(e.g., Turn complete. Press Enter to proceed to the next turn.)*
