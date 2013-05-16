param($installPath, $toolsPath, $package, $project)

if (Get-Module | ?{ $_.Name -eq 'mvc-evolution' })
{
    Remove-Module mvc-evolution
}

Import-Module (Join-Path $toolsPath mvc-evolution.psd1)