param($installPath, $toolsPath, $package, $project)

if (Get-Module | ?{ $_.Name -eq 'EfModelMigrations' })
{
    Remove-Module EfModelMigrations
}

Import-Module (Join-Path $toolsPath EfModelMigrations.psd1)