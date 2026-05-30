namespace MyRpgGame

module Types =
    [<AbstractClass>]
    type Character(name:string, maxHp:float, Ar:float, Mr:float, isFrontline:bool) =
        
        member this.Name = name
        member val Ar = Ar with get, set
        member val Mr = Mr with get, set
        member val HP = maxHp with get, set
        member val MaxHp = maxHp
        member val IsFrontline = isFrontline // 전열인지 아닌지 판단
        member val IsDefending = false with get, set // 방어력 업 버프 썼는지 안썼는지 판단
        member val IsTaunting = false with get, set // 도발 스킬 썼는지 안썼는지 판단
        member val DefenseDebuffTurns = 0 with get, set // 사제의 두번째 스킬 턴이 0인지 판단
        member val DamageMultiplier = 1.0 with get, set
        member this.TakeDamage (basicDamage:float, isMagic: bool) =
            let armor = if isMagic then this.Mr else this.Ar 
            let mutable finalDamage = basicDamage * (1.0 - armor)
            if this.IsDefending then
                finalDamage <- finalDamage * 0.5
            this.HP <- this.HP - finalDamage
            finalDamage

        abstract member UseSkill1 : Character -> string
        abstract member UseSkill2 : Character -> string
        abstract member UseUltimate : Character list -> string
        abstract member GetSpecialStatus : unit -> string
        default this.GetSpecialStatus() = ""
        abstract member Skill1Name : string
        abstract member Skill2Name : string
        abstract member UltimateName : string
        abstract member Skill1TargetType : string
        default this.Skill1TargetType = "Enemy"
        abstract member Skill2TargetType : string
        default this.Skill2TargetType = "Enemy"
        abstract member UltimateTargetType : string
        default this.UltimateTargetType = "Single"
        abstract member IsWideGuarding : bool
        default this.IsWideGuarding = false
   
    /// First party member : Warrior
    type Warrior(name) =
        inherit Character(name, 100.0, 0.4, 0.2, true)

        member val EnhancedSlashCooldown = 0 with get, set
        member val IronWillTurns = 0 with get, set
        member val IronWillCooldown = 0 with get, set

        override this.Skill1Name = "Horizontal Slash"
        override this.Skill2Name = "Enhanced Slash"
        override this.UltimateName = "Iron Will"
        override this.UltimateTargetType = "Self"

        override this.UseSkill1 target = // 횡베기임
            let damageDealt = target.TakeDamage(50.0, false)
            sprintf "%s used 'Horizontal Slash'! Dealt %.1f physical damage to %s." name damageDealt target.Name
        override this.UseSkill2 target = 
            if this.EnhancedSlashCooldown > 0 then
                sprintf "Not ready yet! %d turn(s) remaining." this.EnhancedSlashCooldown
            else
                let damageDealt = target.TakeDamage(150.0, false)
                this.EnhancedSlashCooldown <- 2
                sprintf "%s used'Enhanced Slash'! Powerful triple strikes dealt %.1f damage to %s." name damageDealt target.Name
        override this.UseUltimate _ = 
            if this.IronWillCooldown > 0 then
                sprintf "Not ready yet! %d turn(s) remaining." this.IronWillCooldown
            else
                // 이것도 turn 끝나면 Ar 원래대로 복구되는거 GameLogic에 적어두기
                this.Ar <- 0.9
                this.Mr <- 0.9
                this.IronWillTurns <- 2
                this.IronWillCooldown <- 5
                sprintf "%s used 'Iron Will'! Defense significantly increased for 2 turns." name

    /// Second party member : Tanker
    type Tanker(name) =
        inherit Character(name, 140.0, 0.6, 0.6, true)

        member val TauntTurn = 0 with get, set
        member val TauntCooldown = 0 with get, set
        member val WideGuardActive = false with get, set
        member val WideGuardCooldown = 0 with get, set

        override this.Skill1Name = "Smash"
        override this.Skill2Name = "Taunt"
        override this.UltimateName = "WideGuard"
        override this.Skill2TargetType = "Self"
        override this.UltimateTargetType = "Self"
        override this.IsWideGuarding = this.WideGuardActive

        override this.UseSkill1 target =
            target.HP <- target.HP - 20.0
            sprintf "%s's 'Smash'! Dealt 20 True Damage to %s." name target.Name
        override this.UseSkill2 _ =
            if this.TauntCooldown > 0 then
                sprintf "Not ready yet! %d turn(s) remaining." this.TauntCooldown
            else
                this.IsTaunting <- true
                this.TauntTurn <- 2
                this.TauntCooldown <- 3
                sprintf "%s used 'Taunt'! For the next 2 turns, all enemy attacks are drawn to him. " name
        override this.UseUltimate (_) =
            if this.WideGuardCooldown > 0 then
                sprintf "Not ready yet! %d turn(s) remaining." this.WideGuardCooldown
            else
                this.WideGuardActive <- true
                this.WideGuardCooldown <- 3
                sprintf "%s used 'WideGuard'! All allies are protected from enemy AOE attacks for the next turn." name
        override this.GetSpecialStatus () =
            if this.WideGuardActive then "[WIDE GUARD]" else ""

    /// Third party member : Priest
    type Priest(name) =
        inherit Character(name, 50.0, 0.1, 0.1, false)

        member val UltBuffTurns = 0 with get, set
        member val UltCooldown = 0 with get, set

        override this.Skill1Name = "Heal"
        override this.Skill2Name = "Divine Curse"
        override this.UltimateName = "Celestial Blessing"
        override this.Skill1TargetType = "Ally"
        override this.UltimateTargetType = "AllAllies"

        override this.UseSkill1 target = // heal target (heal)
            target.HP <- min target.MaxHp (target.HP + target.MaxHp * 0.5)
            if target.HP = target.MaxHp then
                sprintf "%s cast 'Heal' on %s! %s's HP has been fully restored." name target.Name target.Name
            else 
                sprintf "%s cast 'Heal' on %s! %s's HP was restored by half of its maximum." name target.Name target.Name
        override this.UseSkill2 target = 
            if target.DefenseDebuffTurns > 0 then
                target.DefenseDebuffTurns <- 3
                sprintf "%s cast 'Divine Curse' on %s again! The curse's duration is refreshed to 2 turns!" name target.Name
            else
                target.Ar <- max 0.0 (target.Ar - 0.2)
                target.Mr <- max 0.0 (target.Mr - 0.2)
                target.DefenseDebuffTurns <- 3
                sprintf "%s cast 'Divine Curse' on %s! %s's Defense resistance decrease by 0.2." name target.Name target.Name
        override this.UseUltimate (targets) = 
            if this.UltCooldown > 0 then
                sprintf "Not ready yet! %d turn(s) remaining." this.UltCooldown
            else
                targets |> List.iter (fun p -> p.DamageMultiplier <- 1.5)
                this.UltBuffTurns <- 3 
                this.UltCooldown <- 5
                sprintf "%s cast 'Celestial Blessing'! All allies' damage increased by 1.5x. " name

    /// Last party member : Mage
    type Mage(name) =
        inherit Character(name, 50.0, 0.1,0.3, false) 
        
        member val UltimateCooldown = 0 with get, set
        member val ManaCharged = false with get, set
        
        override this.Skill1Name = "Fireball"
        override this.Skill2Name = "Focusing Mana"
        override this.UltimateName = "Meteor Strike"
        override this.Skill2TargetType = "Self"
        override this.UltimateTargetType = "AllEnemies"

        override this.UseSkill1 target =
            let basicDamage = if this.ManaCharged then 100.0 else 40.0
            let damageDealt = target.TakeDamage(basicDamage, true)
            this.ManaCharged <- false
            sprintf "%s cast 'Fireball'! Dealt %.1f magic damage to %s." name damageDealt target.Name
        override this.UseSkill2 _ =
            this.ManaCharged <- true
            sprintf "%s cast 'Focusing Mana'! The next attack will be devastating! " name
        override this.UseUltimate targets =
            if this.UltimateCooldown > 0 then
                sprintf "Not ready yet! %d turn(s) remaining." this.UltimateCooldown
            else
                let basicDamage = if this.ManaCharged then 300.0 else 120.0
                targets |> List.iter (fun e -> e.TakeDamage(basicDamage, true) |> ignore)
                this.ManaCharged <- false
                this.UltimateCooldown <- 6
                sprintf "%s cast 'Meteor Strike'! A huge explosion huge damage to all enemies." name
        override this.GetSpecialStatus () =
            if this.ManaCharged then "[MANA CHARGED]" else ""
 
    [<AbstractClass>]
    type Enemy(name, maxHp, Ar, Mr, isFrontline) =
        inherit Character(name, maxHp, Ar, Mr, isFrontline)
        abstract member TakeTurn : players: Character list * enemies : Character list -> string

    /// Stage1(tutorial) & Stage2 enemy : Slime
    type Slime(name) =
        inherit Enemy(name, 30.0, 0.2, 0.0, true)
        override this.Skill1Name = "Acid attack"
        override this.Skill2Name = "Acid attack" // There are no skill 2 and 3.
        override this.UltimateName = "Acid attack"
        override this.UseSkill1 target = // Acid attack(magic attack)
            let damageDealt = target.TakeDamage (10.0,true)
            sprintf "%s's Acid Attack! Dealt %.1f magic damage to %s." name damageDealt target.Name
        override this.UseSkill2 target = this.UseSkill1 target // Slime object only use Skill1 
        override this.UseUltimate targets = this.UseSkill1 targets.[0]
        override this.TakeTurn (players, enemies) = 
            match players with
            |[] -> "It's a total wiped out...."
            | _ -> // Random target attack of the player party
                let rand = System.Random()
                let targetIndex = rand.Next(players.Length)
                let target = players.[targetIndex]
                this.UseSkill1 target
    
    /// Stage2 & Stage3 enemy : Skeleton Warrior
    type SkeletonWarrior(name) =
        inherit Enemy(name, 100.0, 0.6, 0.2, true)

        let mutable turnCount = 0
        override this.Skill1Name = "Rusty Slash"
        override this.Skill2Name = "Bone Shield"
        override this.UltimateName = "Terrifying Roar"
        override this.UseSkill1 target = // Slashed with a rusty sword (physical attack)
            let damageDealt = target.TakeDamage(30.0,false)
            sprintf "%s used 'Rusty Slash'! Dealt %.1f physical damage to %s." name damageDealt target.Name
        override this.UseSkill2 _ = // Boost self Defense greatly for 1 turn (buff)
            this.IsDefending <- true
            sprintf "%s raised a heavy 'Bone Shield', bracing for the impact!" name 
        override this.UseUltimate _ = // Taunt the enemy to draw their attacks (buff)
            this.IsTaunting <- true
            sprintf "%s used 'Terrifying Roar'! All players are now focused on %s." name name
        override this.TakeTurn(players,enemies) =
            this.IsDefending <- false // Reset state
            this.IsTaunting <- false
            turnCount <- turnCount + 1

            let rand = System.Random()
            match players with
            |[] -> "No one left to fight...."
            | _ -> 
                if turnCount % 2 = 1 then 
                    let tauntingTanker = players |> List.filter (fun p -> p.IsTaunting)
                    let finalTarget =
                        if not tauntingTanker.IsEmpty then
                            tauntingTanker.[0]
                        else
                            let frontplayer =
                                players |> List.filter (fun p -> p.IsFrontline)
                            let targetCandidates = if not frontplayer.IsEmpty then frontplayer else players
                            // If there are 2 candidates, the length is 2, and rand.Next(2) returns a 
                            // random integer between 0 and 1.
                            targetCandidates.[rand.Next(targetCandidates.Length)]
                    /// 대상 생존 확인
                    if finalTarget.HP > 0.0 then
                        this.UseSkill1 finalTarget
                    else
                        let alivePlayers = players |> List.filter (fun p -> p.HP > 0.0)
                        if not alivePlayers.IsEmpty then
                            this.UseSkill1 alivePlayers.[rand.Next(alivePlayers.Length)]    
                        else "No one left to fight...."                    
                else
                    let msg2 = this.UseSkill2 players.[0]
                    let msgUlt = this.UseUltimate players
                    sprintf "%s\n%s" msg2 msgUlt

    /// Stage3 & stage4 enemy : Skeleton Archer
    type SkeletonArcher(name) = 
        inherit Enemy(name, 60.0, 0.3, 0.2, false)

        let mutable turnCount = 0

        override this.Skill1Name = "Arrow attack"
        override this.Skill2Name = "Sniping"
        override this.UltimateName = "Arrow attack"

        
        member val TargetLockedOn : Character option = None with get, set

        override this.UseSkill1 target =
            let damageDealt = target.TakeDamage(30.0, false)
            sprintf "%s used 'Arrow attack'! Dealt %.1f physical damage to %s." name damageDealt target.Name
        override this.UseSkill2 target = 
            this.TargetLockedOn <- Some target
            sprintf "%s used 'Sniping'. Targeting our backline..." name 
            /// 얘가 skill2를 쓴 다음에 바로 데미지가 들어가지 않고, 우리 턴으로 돌아와서 우리 턴이 끝난 다음에 데미지가 들어오게 하려면 어떻게 해야 할까?
        override this.UseUltimate target = this.UseSkill1 target.[0]
        override this.TakeTurn(players,enemies) =
            turnCount <- turnCount + 1
            let rand = System.Random()

            let tauntingTanker = players |> List.filter (fun p -> p.IsTaunting)

            match this.TargetLockedOn with
            | Some originaltarget -> 
                let finalTarget =
                    if not tauntingTanker.IsEmpty then
                        tauntingTanker.[0]
                    else
                        originaltarget
                if finalTarget.HP > 0.0 then
                    let damageDealt = finalTarget.TakeDamage(70.0, false)
                    this.TargetLockedOn <- None
                    sprintf "%s fired the sniping shot! Dealt %.1f damage to %s!" name damageDealt finalTarget.Name
                else
                    this.TargetLockedOn <- None
                    this.UseSkill1 players.[rand.Next(players.Length)]
            | None ->
                let currentCandidates = if not tauntingTanker.IsEmpty then tauntingTanker else players 
                
                if turnCount % 3 <> 0 then
                    let target = players.[rand.Next(currentCandidates.Length)]
                    this.UseSkill1 target
                else
                    let backline =
                        currentCandidates |> List.filter (fun p -> not p.IsFrontline)
                    let targetCandidates = if not backline.IsEmpty then backline else currentCandidates
                    // If there are 2 candidates, the length is 2, and rand.Next(2) returns a 
                    // random integer between 0 and 1.
                    let target = targetCandidates.[rand.Next(targetCandidates.Length)]
                    this.UseSkill2 target


    /// Stage4 enemy : Dark Priest
    type DarkPriest(name) = 
        inherit Enemy(name, 40.0, 0.1, 0.5, false)

        let mutable turnCount = 0
        override this.Skill1Name = "Dark Heal"
        override this.Skill2Name = "Touch of Darkness"
        override this.UltimateName = "Dark Heal"

        override this.UseSkill1 target = // heal target (heal)
            target.HP <- min target.MaxHp (target.HP + target.MaxHp * 0.5)
            if target.HP = target.MaxHp then
                sprintf "%s cast 'Dark Heal' on %s! %s's HP has been fully restored." name target.Name target.Name
            else 
                sprintf "%s cast 'Dark Heal' on %s! %s's HP was restored by half of its maximum." name target.Name target.Name
        override this.UseSkill2 target = 
            let damageDealt = target.TakeDamage(20.0,true)
            sprintf "%s cast 'Touch of Darkness'! Dealt %.1f magic damage to %s." name damageDealt target.Name
        override this.UseUltimate targets = this.UseSkill1 targets.[0]
        override this.TakeTurn(players,enemies) =
            let aliveEnemies = enemies |> List.filter (fun e -> e.HP > 0.0) 
            if aliveEnemies.IsEmpty then
                "No dark soldier left to heal...."
            else
                let targetToHeal =
                    aliveEnemies 
                    |> List.reduce (fun best current ->
                        if (current.HP / current.MaxHp) < (best.HP / best.MaxHp) then current else best)
                if targetToHeal.HP < targetToHeal.MaxHp then
                    this.UseSkill1 targetToHeal
                else
                    let rand = System.Random()
                    let tauntingTanker = players |> List.filter (fun p -> p.IsTaunting)
                    let target =
                        if not tauntingTanker.IsEmpty then tauntingTanker.[0]
                        else players.[rand.Next(players.Length)]
                    this.UseSkill2 target
    
    /// last boss
    type DarkLord(name) = 
        inherit Enemy(name, 250.0, 0.5, 0.5, true)

        let mutable turnCount = 0

        member val UltCooldown = 0 with get, set
        // None이면 정상, Some 이면 궁극기 발동 직전
        member val IsChargingUltimate = false with get, set
        
        override this.Skill1Name = "Abyssal Strike"
        override this.Skill2Name = "Mark of Death"
        override this.UltimateName = "APOCALYPSE"
        override this.UseSkill1 target =
            target.Ar <- max 0.0 (target.Ar - 0.1)
            let damageDealt = target.TakeDamage(40.0, false)
            sprintf "%s used 'Abyssal Strike'!. %s's defense decreased and took %.1f damage." name target.Name damageDealt
        override this.UseSkill2 target =
            target.DamageMultiplier <- 2.0
            sprintf "%s cast 'Mark of Death' on %s! Next damage will be doubled!" name target.Name
        override this.UseUltimate _ =
            this.UltCooldown <- 5
            this.IsChargingUltimate <- false
            sprintf "%s's eyes glow red... 'APOCALYPSE' is coming upon everyone!" name
        override this.TakeTurn(players, enemies) =
            turnCount <- turnCount + 1
            let rand = System.Random()

            if this.UltCooldown > 0 then this.UltCooldown <- this.UltCooldown - 1
            match players with
            | [] -> "Everything returns to darkness..."
            | _ ->
                if this.IsChargingUltimate then
                    this.IsChargingUltimate <- false
                    this.UltCooldown <- 5

                    let isProtected = 
                        players |> List.exists (fun p -> p.IsWideGuarding)
                    if isProtected then
                        players |> List.iter (fun p -> p.TakeDamage(20.0, true) |> ignore)
                        "APOCALYPSE RELEASED! ...But the Tanker's [WIDE GUARD] absorbed the impact! Party took zero damage."
                    else
                        players |> List.iter (fun p -> p.TakeDamage(200.0, true) |> ignore)
                        "APOCALYPSE RELEASED! Catastrophic damage to the entire party!"
                
                elif this.UltCooldown = 0 then
                    this.IsChargingUltimate <- true
                    sprintf "%s is gathering catastrophic energy... The sky turns black!" name
                
                elif turnCount % 3 = 0 then
                    let alivePlayers = players |> List.filter (fun p -> p.HP > 0.0)

                    if not alivePlayers.IsEmpty then
                        let weakest =
                            alivePlayers
                            |> List.reduce (fun best current ->
                                if current.HP < best.HP then current else best)
                        this.UseSkill2 weakest
                    else
                        "Everything returns to darkness...."
                else
                    let tauntingTanker = players |> List.filter (fun p -> p.IsTaunting)
                    let target =
                        if not tauntingTanker.IsEmpty then tauntingTanker.[0]
                        else players.[rand.Next(players.Length)]
                    this.UseSkill1 target
        override this.GetSpecialStatus() =
            if this.IsChargingUltimate then "[APOCALYPSE CHARGING!]" else ""



