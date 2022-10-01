#r "../packages/SQLProvider/lib/netstandard2.0/FSharp.Data.SqlProvider.dll"
#r "../packages/Stub.System.Data.SQLite.Core.NetStandard/runtimes/osx-x64/native/SQLite.Interop.dll"

open FSharp.Data.Sql

let [<Literal>] connectionString = "Data Source=../BankAccountDB/BankAccountDB.db; Version=3"
// let [<Literal>] resolutionPath = __SOURCE_DIRECTORY__ + "/../packages/SQLitePCLRaw.lib.e_sqlite3.ios/lib/net6.0-ios14.2/SQLitePCLRaw.lib.e_sqlite3.ios.dll"
let [<Literal>] resolutionPath = __SOURCE_DIRECTORY__ + "/../packages/Stub.System.Data.SQLite.Core.NetStandard/lib/netstandard2.1"

type BankAccountDB = SqlDataProvider<
    Common.DatabaseProviderTypes.SQLITE,
    SQLiteLibrary = Common.SQLiteLibrary.SystemDataSQLite,
    ConnectionString = connectionString,
    ResolutionPath = resolutionPath,
    CaseSensitivityChange = Common.CaseSensitivityChange.ORIGINAL>
type GetAccountId = SqlCommandProvider<"SELECT TOP 1 AccountId FROM Account WHERE Owner = @owner", Conn, SingleRow = true>
