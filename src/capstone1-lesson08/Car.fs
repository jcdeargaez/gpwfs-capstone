module Car

let getDistance destination =
    match destination with
    | "Office" -> Ok 50
    | "Home" -> Ok 25
    | "Stadium" -> Ok 25
    | "Gas" -> Ok 10
    | _ -> Error "Unknown destination!"

let calculateRemainingPetrol currentPetrol distance =
    if currentPetrol >= distance then
        Ok (currentPetrol - distance)
    else
        Error "Ooops! You've ran out of petrol!"

let driveTo petrol destination =
    let refuel currentPetrol =
        let addition = if destination = "Gas" then 50 else 0
        currentPetrol + addition
    
    getDistance destination
    |> Result.bind (calculateRemainingPetrol petrol)
    |> Result.map refuel
