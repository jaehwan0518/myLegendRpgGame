namespace MyRpgGame

open MyRpgGame.Types

open System
open System.Threading
module GameLogic =

/// 초기 파티 구성
    let initializeparty () = [Warrior("Warrior") :> Character ; Tanker("Tanker") :> Character ; Priest("Priest") :> Character ; Mage("Mage") :> Character]

/// user에게 보일 디스플레이 화면
    let displayStatus (players : Character list) (enemies: Character list) =
        printfn "\n=================== ENEMY ==================="
        enemies |> List.iter (fun e ->
            let tauntStatus = if e.IsTaunting then "[TAUNT]" else ""
            let defendStatus = if e.IsDefending then "[DEFENDING]" else ""
            let classStatus = e.GetSpecialStatus() 
            printfn "[%s] %s%s%s" e.Name tauntStatus defendStatus classStatus
            printfn "HP: %.2f/%.2f | Ar: %.1f | Mr: %.1f" e.HP e.MaxHp e.Ar e.Mr)
        printfn "\n=================== PLAYER ==================="
        players |> List.iter (fun p ->
            let postStatus = if p.IsFrontline then "[FRONT]" else "[BACK]"
            let tauntStatus = if p.IsTaunting then "[TAUNT]" else ""
            let classStatus = p.GetSpecialStatus()
            printfn "[%s] %s%s%s" p.Name postStatus tauntStatus classStatus
            printfn "HP: %.2f/%.2f | Ar: %.1f | Mr: %.1f" p.HP p.MaxHp p.Ar p.Mr)
        printfn "================================================"

/// player의 skillchoice
    let getSkillChoice (character : Character) =
        printfn "\n[%s]'s turn. Choose a skill: (write 1 or 2 or 3 and push enter.)" character.Name
        printfn "1. %s (Skill 1)" character.Skill1Name
        printfn "2. %s (Skill 2)" character.Skill2Name
        printfn "3. %s (Ultimate)" character.UltimateName

        let rec askForSkill () =
            let choice = Console.ReadLine()
            match choice with
            | "1" -> "Skill1"
            | "2" -> "Skill2"
            | "3" -> "Ultimate"
            | _ -> 
                printfn "Invalid input. Please enter 1, 2, or 3."
                askForSkill ()
        askForSkill ()

/// 아군에게 스킬 사용할때 어떤 아군을 타겟으로 할지 설정      
    let getAllyTargetChoice (players: Character list) =
        printfn "\nSelect an ally to target:"

        let mutable number = 1
        for p in players do
            printfn "%d. %s (HP: %.2f / %.2f)" number p.Name p.HP p.MaxHp
            number <- number + 1
        let rec askForInput () =
            let choice = Console.ReadLine()

            if choice = "" then
                printfn "Incorrect entry. Please enter the right number and press Enter."
                askForInput ()             
            else
                try
                    let index = (int choice) - 1
                    if index >=0 && index < players.Length then
                        players.[index]
                    else
                        printfn "Incorrect entry. Please enter the right number and press Enter."
                        askForInput ()
                with
                | _ -> 
                    printfn "Please input the correct number and press Enter."
                    askForInput ()
        askForInput ()

/// enemies list 중 스킬을 사용할 대상을 정한다.
    let getTargetChoice (enemies: Character list) =
        let taunters = enemies |> List.filter (fun e -> e.IsTaunting)
        let candidates = if not taunters.IsEmpty then taunters else enemies
        
        printfn "\nSelect a target to Attack."
        
        let mutable number = 1
        for e in candidates do 
            printfn "%d. %s (HP: %.2f)" number e.Name e.HP
            number <- number + 1

        let rec askForInput () =
            let choice = Console.ReadLine()

            if choice = "" then
                printfn "Incorrect entry. Please enter the right number and press Enter."
                askForInput ()             
            else
                try
                    let index = (int choice) - 1
                    if index >=0 && index < candidates.Length then
                        candidates.[index]
                    else
                        printfn "Incorrect entry. Please enter the right number and press Enter."
                        askForInput ()
                with
                | _ -> 
                    printfn "Please input the correct number and press Enter."
                    askForInput ()
        askForInput ()

/// 배틀이 돌아갈 코드
    let rec battleLoop (players : Character list) (enemies : Enemy list) stageNum turnCount =
        let alivePlayers = players |> List.filter (fun p -> p.HP > 0.0)
        let aliveEnemies = enemies |> List.filter (fun e -> e.HP > 0.0)
        
        /// 승패 판정
        if alivePlayers.IsEmpty then
            printfn "\n[ GAME OVER ] Party was wiped out..."
            false
        elif aliveEnemies.IsEmpty then
            printfn "\n[ STAGE CLEAR ] Stage %d Cleared!" stageNum
            true
        else
            //2. 턴 시작 알림 및 상태창 출력
            printfn "\n=========== [ STAGE %d - Turn %d ] ============" stageNum turnCount
            let enemyChars = aliveEnemies |> List.map (fun e -> e :> Character)
            displayStatus alivePlayers enemyChars
            
            //적의 턴 시작
            printfn "\n--- [ENEMY PHASE] ---"
            for e in aliveEnemies do
                // 플레이어 살아있는지 확인
                let currentPlayers = alivePlayers |> List.filter (fun p -> p.HP > 0.0)
                if e.HP > 0.0 && not currentPlayers.IsEmpty then
                    let actionMessage = e.TakeTurn(currentPlayers, enemyChars)
                    printfn "-> %s" actionMessage
                    System.Threading.Thread.Sleep(800)
            
            //아군 턴 시작
            let surviveAfterEnemyPhase = players |> List.filter (fun p -> p.HP > 0.0)
            
            /// 적턴 끝났으니 만약 와이드가드가 true 라면 false로 바꿔주기
            players |> List.iter (fun p ->
                match p with
                | :? Tanker as t -> t.WideGuardActive <- false
                | _ -> ()    
            )
            
            if not surviveAfterEnemyPhase.IsEmpty then
                printfn "\n--- [PLAYER PHASE] ---"
                for p in surviveAfterEnemyPhase do
                    let currentEnemies = aliveEnemies |> List.filter (fun e -> e.HP > 0.0)
                    if p.HP > 0.0 && not currentEnemies.IsEmpty then
                        let currentEnemyChars = currentEnemies |> List.map (fun e -> e:> Character)

                        let rec playerActionLoop () =    
                            let chosenSkill = getSkillChoice p // 위의 getSKillChoice에서 "SKill1,2,3" 반환을 pattern matching 시킨다
                            let actionMessage =
                                match chosenSkill with
                                | "Skill1" ->
                                    let target = 
                                        if p.Skill1TargetType = "Ally" then /// priest 가 회복스킬을 썼을때
                                            getAllyTargetChoice surviveAfterEnemyPhase
                                        else
                                            getTargetChoice currentEnemyChars
                                    p.UseSkill1(target)
                                | "Skill2" ->
                                    let target = 
                                        match p.Skill2TargetType with
                                        | "Ally" -> getAllyTargetChoice surviveAfterEnemyPhase
                                        | "Self" -> p
                                        | _ -> getTargetChoice currentEnemyChars
                                    p.UseSkill2(target)
                                | "Ultimate" ->
                                    match p.UltimateTargetType with
                                    | "AllEnemies" ->
                                        p.UseUltimate(currentEnemyChars)
                                    | "AllAllies" ->
                                        p.UseUltimate(surviveAfterEnemyPhase)
                                    | "Self" ->
                                        p.UseUltimate([p])
                                    | _ -> 
                                        let target = getTargetChoice currentEnemyChars
                                        p.UseUltimate([target])
                                | _ -> // 여기에 뭐가 들어오는건 물리적으로 불가능하지만 얘가 자꾸 컴파일 에러를 냄;;
                                    let target = getTargetChoice currentEnemyChars
                                    p.UseSkill1(target)
                            if actionMessage.StartsWith("Not ready") then
                                printfn "-> %s" actionMessage
                                printfn "[ Please choose another skill. ]"
                                playerActionLoop()
                            else
                                printfn "-> %s\n" actionMessage
                        playerActionLoop ()
                /// 버프 & 디버프 쿨 관리
                for p in players do
                    match p with
                    | :? Warrior as w ->
                        if w.EnhancedSlashCooldown > 0 then w.EnhancedSlashCooldown <- w.EnhancedSlashCooldown - 1
                        if w.IronWillCooldown > 0 then w.IronWillCooldown <- w.IronWillCooldown - 1
                        if w.IronWillTurns > 0 then
                            w.IronWillTurns <- w.IronWillTurns - 1
                            if w.IronWillTurns = 0 then
                                w.Ar <- 0.4
                                w.Mr <- 0.2
                    | :? Tanker as t ->
                        if t.TauntCooldown > 0 then t.TauntCooldown <- t.TauntCooldown - 1
                        if t.TauntTurn > 0 then
                            t.TauntTurn <- t.TauntTurn - 1
                            if t.TauntTurn = 0 then t.IsTaunting <- false
                        if t.WideGuardCooldown > 0 then t.WideGuardCooldown <- t.WideGuardCooldown - 1
                    | :? Priest as pr ->
                        if pr.UltCooldown > 0 then pr.UltCooldown <- pr.UltCooldown - 1
                        if pr.UltBuffTurns > 0 then
                            pr.UltBuffTurns <- pr.UltBuffTurns - 1
                            if pr.UltBuffTurns = 0 then
                                players |> List.iter (fun p -> p.DamageMultiplier <- 1.0)    
                    | :? Mage as m ->
                        if m.UltimateCooldown > 0 then m.UltimateCooldown <- m.UltimateCooldown - 1
                    | _ -> ()
                for e in enemies do
                    let c = e :> Character
                    if c.DefenseDebuffTurns > 0 then
                        c.DefenseDebuffTurns <- c.DefenseDebuffTurns - 1
                        if c.DefenseDebuffTurns = 0 then
                            c.Ar <- c.Ar + 0.2
                            c.Mr <- c.Mr + 0.2

            printfn "[Turn Complete. Press Enter to proceed to the next turn.]"
            Thread.Sleep(1000)
            Console.ReadLine() |> ignore
            Console.Clear()
            
            battleLoop players enemies stageNum (turnCount + 1)
                    