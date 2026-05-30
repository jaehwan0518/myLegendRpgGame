namespace MyRpgGame

open System
open MyRpgGame.Types
open MyRpgGame.GameLogic
open System.Threading

module Program =

    [<EntryPoint>]
    let main argv =
        Console.Clear()
        let miniSwordArt = """
                             |
                             |
                           --+--
                             |
                             |
                         ____|____
                        /_________\
                        """
        let titleScreenArt = """
================================================================================
|                                                                              |
|  TTTTTTT  H   H  EEEEE      L       EEEEE  GGGGG  EEEEE  N   N  DDDD         |
|     T     H   H  E          L       E      G      E      NN  N  D   D        |
|     T     HHHHH  EEEEE      L       EEEEE  G  GG  EEEEE  N  N N  D   D       |
|     T     H   H  E          L       E      G   G  E      N  NN  D   D        |
|     T     H   H  EEEEE      LLLLL   EEEEE  GGGGG  EEEEE  N   N  DDDD         |
|                                                                              |
|               O O O      FFFFF     H   H  EEEEE  RRRR   OOO  EEEEE  SSSS     |
|              O     O     F         H   H  E      R   R O   O E     S         |
|              O     O     FFFFF     HHHHH  EEEEE  RRRR  O   O EEEEE  SSS      |
|              O     O     F         H   H  E      R R   O   O E         S     |
|               O O O      F         H   H  EEEEE  R  RR  OOO  EEEEE SSSS      |
|                                                                              |
================================================================================
|                                                                              |
|                                     |>>>                                     |
|                                     |                                        |
|                       |>>>      _  _|_  _         |>>>                       |
|                       |        |;| |;| |;|        |                          |
|                   _  _|_  _    \\.    .  /    _  _|_  _                      |
|                  |;|_|;|_|;|    \\:  .  /    |;|_|;|_|;|                     |
|                  \\..      /    ||:   . |    \\..      /                     |
|                   \\.  ,  /     ||:  .  |     \\.    ./                      |
|                    ||:   |_   _ ||_ . _ | _   _||:   |                       |
|                    ||:  .|||_|;|_|;|_|;|_|;|_|;||:.  |                       |
|                    ||:   ||.    .     .      . ||:  .|                       |
|                    ||: . || .     . .   .  ,   ||:   |                       |
|                    ||:   ||:  ,  _______   .   ||: , |                       |
|                    ||:   || .   /+++++++\    . ||:   |                       |
|                    ||:   ||.    |+++++++| .    ||: . |                       |
|                 __ ||: . ||: ,  |+++++++|  .   ||:   |__                     |
|        ____--`~    '--~~__|.    |+++++__|----~    ~`---,                     |
|       -~--~                   ~---__|,--~'                  ~~----_____-~'   |
|                                                                              |
================================================================================
|                                                                              |
|                          -> PRESS ENTER TO START <-                          |
|                                                                              |
================================================================================
"""
        printfn "%s" titleScreenArt
        printfn "Press Enter to start your journey..."
        Console.ReadLine() |> ignore

        let rec startGame () =
            let players = initializeparty ()

            let stages =
                [ 
                    (1,[ Slime("Slime A") :> Enemy; Slime("Slime B") :> Enemy ])
                    (2,[ Slime("Slime A") :> Enemy; Slime("Slime B") :> Enemy; SkeletonWarrior("SkeletonWarrior A") :> Enemy ])
                    (3,[ SkeletonWarrior("SkeletonWarrior A") :> Enemy; SkeletonWarrior("SkeletonWarrior B") :> Enemy; SkeletonArcher("SkeletonArcher A") :> Enemy ])
                    (4,[ SkeletonWarrior("SkeletonWarrior A") :> Enemy; SkeletonWarrior("SkeletonWarrior B") :> Enemy; SkeletonArcher("SkeletonArcher A") :> Enemy; DarkPriest("DarkPriest A") :> Enemy ])
                    (5,[ DarkLord("DARK LORD") :> Enemy ])
                ]
            let rec playGame stageList party =
                match stageList with
                | [] ->
                    Console.Clear()
                    printfn "============================================================"
                    printfn "         CONGRATULATIONS! YOU DEAFTED THE DARK LORD!        "
                    printfn "              Peace has returned to the world.              "
                    printfn "                             %s                             " miniSwordArt
                    printfn "============================================================"
            
                | (stageNum, enemies) :: nextStages ->
                    Console.Clear()
                    printfn ">>> Entering Stage %d...<<<" stageNum
                    Thread.Sleep(1000)

                    /// 전투 시작
                    let isVictory = battleLoop party enemies stageNum 1

                    if isVictory then
                        if List.isEmpty nextStages then
                            playGame nextStages party
                        else
                            printfn "\n[ Rest at Campfire ] You're filled with DETERMINATION... "
                            printfn "The party rests and recovers 50%% of Max HP!"
                            printfn "But beware... your skill cooldowns and buffs carry over the next stage."
                            Thread.Sleep(1000)
                            party |> List.iter (fun p -> 
                                if p.HP <= 0.0 then
                                    printfn " - %s has been revived! " p.Name
                                p.HP <- min p.MaxHp (p.HP + p.MaxHp * 0.5)
                            )

                            printfn "Press Enter to advance to the next Stage."
                            Console.ReadLine() |> ignore

                            playGame nextStages party
                    else
                        printfn "============================================================"
                        printfn "                        GAME OVER...                        "
                        printfn "             The world is consumed by darkness.             "
                        printfn "============================================================"
                        printfn "\nThe world need you once more. Please Enter to try again."

                        Console.ReadLine() |> ignore
                        Console.Clear()
                    
                        startGame ()
            playGame stages players
        startGame ()
        0
            