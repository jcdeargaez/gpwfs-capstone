module Capstone2.Operations

open Capstone2.Domain

/// Deposits an amount into an account
let deposit amount account =
    { account with Balance = account.Balance + amount }

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount account =
    if amount > account.Balance
    then account
    else { account with Balance = account.Balance - amount }

/// Runs some account operation such as withdraw or deposit with auditing
let auditAs operationName audit operation amount account =
    audit account (sprintf "Performing operation '%s' for $%M..." operationName amount)
    let account' = operation amount account
    let resultMsg =
        if account'.Balance <> account.Balance
        then sprintf "Transaction accepted! Balance is now $%M" account'.Balance
        else "Transaction rejected!"
    audit account' resultMsg
    account'
