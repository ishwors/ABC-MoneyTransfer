
# EF Core Migrations
## Steps for running migrations in EF Core in .Net8

## Create Migration File
Generate new migration file when models are created or updated
> dotnet ef -p MoneyTransfer.Data -s MoneyTransfer.Web migrations add {migrationFileName}

## Update Database
Run database migrations based on generated migration files
> dotnet ef -p MoneyTransfer.Data -s MoneyTransfer.Web database update

## Remove a Migration
Remove last migration file
> dotnet ef -p MoneyTransfer.Data -s MoneyTransfer.Web migrations remove

## Reverting migration
Revert latest migration
> dotnet ef -p MoneyTransfer.Data -s MoneyTransfer.Web database update {LastMigrationFileName}