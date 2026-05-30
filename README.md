# ⚔️ The Legend of Heroes ~CLI RPG~

This is a game where a party of heroes progresses through multiple stages to defeat the Dark Lord and restore peace to the world.

---

### 🎮 What is this Game?
This is a turn-based, text-based RPG where combat alternates between the Enemy Phase and the Player Phase each turn. If the HP of all enemies in a stage reaches 0, you advance to the next stage. If the HP of all allies reaches 0, you fail to clear the game. Ultimately, the game is cleared when you conquer all stages. 

Clearing a stage grants a Campfire rest sequence, which revives fallen allies and recovers 50% of the party's Max HP. Note that skill cooldowns and active buffs/debuffs carry over to the next stage.

---

### 🚀 How to start?
This game is a console-based RPG built to run on the **.NET 10 environment**. You can run the game by downloading the project files using `git clone` and executing the `dotnet run` command in your terminal.

> 🚨 **Important Note**
> When playing the game, please **maximize your terminal window to the full screen**. This ensures optimal gameplay and prevents the ASCII art layouts from breaking.

---

### 🕹️ How to play?
Once the game starts, you can progress toward clearing it by following the text prompts and options displayed in the terminal window (type 1, 2, or 3 and press Enter). In-game attacks are classified into physical attacks, magical attacks, and status modifiers. Damage scales based on the physical defense (Ar) and magical defense (Mr) of each target. You should use your skills strategically while taking cooldowns and durations into account.

⚠️ **CRITICAL WARNING FOR FAST PLAYERS:** 
Please **WAIT until all text and combat animations have fully finished typing out** before entering your next command (1, 2, 3 + Enter). Rapidly spamming inputs or pressing Enter repeatedly before the text completes may cause input buffer bugs, skip crucial logs, or crash the console application! Please play at a steady pace.

---

### ⚔️ Ally Skill Guide

#### 🛡️ Warrior
* **Skill 1 (Horizontal Slash)**: Deals 50.0 physical damage to a single target. No cooldown.
* **Skill 2 (Enhanced Slash)**: Deals three times the damage of Skill 1. Cooldown: 2.
* **Ultimate (Iron Will)**: Increases both Ar and Mr to 0.9. Cooldown: 5, Duration: 2.

#### 🛡️ Tanker
* **Skill 1 (Smash)**: Bypasses enemy defense to deal 20.0 fixed damage. No cooldown.
* **Skill 2 (Taunt)**: Draws enemy attacks to focus entirely on yourself. Cooldown: 3, Duration: 2.
* **Ultimate (WideGuard)**: Reduces the damage of incoming enemy AoE (wide-range) attacks during that turn to 10%. Cooldown: 3.

#### 🤍 Priest
* **Skill 1 (Heal)**: Restores a single ally's HP by half of their maximum health. If the current damage taken is less than half of their maximum health, it heals them back to full health. No cooldown.
* **Skill 2 (Divine Curse)**: Decreases a single enemy's Ar and Mr by 0.2 (Minimum threshold is 0.0). It does not stack permanently; however, using it again while the effect is active extends its duration. No cooldown, Duration: 2.
* **Ultimate (Celestial Blessing)**: Increases the final damage dealt by all allies by 1.5x. Cooldown: 5, Duration: 3.

#### 🧙‍♂️ Mage
* **Skill 1 (Fireball)**: Deals 40.0 magical damage to a single target. No cooldown.
* **Skill 2 (Focusing Mana)**: Concentrates mana to amplify your own final damage by 2.5x on the next turn. No cooldown.
* **Ultimate (Meteor Strike)**: Deals 120.0 magical damage to all enemies. Cooldown: 6.

> 💡 **Enemy Skills & Strategy Advice**
> Enemy skills can be discovered dynamically as you progress through the game.
> 
> *Quick Advice*: When you reach the final stage and encounter the text `"DARK LORD is gathering catastrophic energy... The sky turns black!"`, it means the Dark Lord is preparing a devastating AoE ultimate! Also, constantly checking the active statuses of both allies and enemies via the **"State Table"** displayed every turn will greatly help you clear the game.

---

### 🤖 Use of LLM Attribution
Finding a purely text-based interface somewhat lacking, I initially asked Gemini to generate a text-based ASCII art of a slime to display next to the character states. However, the output did not turn out as expected and felt out of place. 

To capture a grand and atmospheric feeling for the introduction sequence instead, I revised my prompt and asked the LLM to generate an ASCII art of a castle with bats flying around it. Thanks to this adjustment, I successfully applied a wonderful castle imagery to the game's opening title screen.
