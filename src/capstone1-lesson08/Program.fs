open System
open Car

let getDestination () =
    printf "Enter destination: "
    Console.ReadLine()

let rec inputLoop petrol =
    let destination = getDestination ()
    printfn "Trying to drive to %s" destination
    let nextPetrolAmount =
        match driveTo petrol destination with
        | Ok petrolAfterDriving ->
            printfn "Made it to %s! You have %d petrol left" destination petrolAfterDriving
            petrolAfterDriving
        | Error err ->
            printfn "ERROR: %s" err
            petrol
    inputLoop nextPetrolAmount

// Start with 100 units of petrol
inputLoop 100
