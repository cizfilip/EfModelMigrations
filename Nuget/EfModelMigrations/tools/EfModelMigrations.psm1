$InitialModel = '0'

<#
.SYNOPSIS
    Model-EmptyMigration command.

.DESCRIPTION
    Model-EmptyMigration command.

.PARAMETER command
    Model-EmptyMigration command.
#>
function Model-EmptyMigration
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true, HelpMessage="TODO")]
        [string]
        $MigrationName
    )

    Model-ExecuteCommand EfModelMigrations.Commands.EmptyMigrationCommand @( ,$MigrationName )
}

<#
.SYNOPSIS
    Model-AddProperties command.

.DESCRIPTION
    Model-AddProperties command.

.PARAMETER command
    Model-AddProperties command.
#>
function Model-AddProperties
{
    [CmdletBinding()] 
    param (
        #TODO: u vsech povinnych parametru pouzivat HelpMessage
        #TODO: rozmyslet i pouzivani alias atributu viz example zde.
        #TODO: pripadne pouzivat i validacni atributy - viz http://technet.microsoft.com/en-us/library/hh847743.aspx
        #TODO: pro bool parametry pouzit [Switch] jako typ attribut
        [Parameter(Position = 0, Mandatory = $true, HelpMessage="TODO")]
        #[alias("Class","CN")]
        [string]
        $ClassName,
        [Parameter(ValueFromRemainingArguments = $true)][string[]] $Properties
    )

    Model-ExecuteCommand EfModelMigrations.Commands.AddPropertiesCommand @( $ClassName, $Properties )
}

<#
.SYNOPSIS
    Model-CreateClass command.

.DESCRIPTION
    Model-CreateClass command.

.PARAMETER command
    Model-CreateClass command.
#>
function Model-CreateClass
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $ClassName,
        [Parameter(ValueFromRemainingArguments = $true)][string[]] $Properties
    )

    Model-ExecuteCommand EfModelMigrations.Commands.CreateClassCommand @( $ClassName, $Properties )
}

<#
.SYNOPSIS
    Model-RemoveClass command.

.DESCRIPTION
    Model-RemoveClass command.

.PARAMETER command
    Model-RemoveClass command.
#>
function Model-RemoveClass
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $ClassName
    )

    Model-ExecuteCommand EfModelMigrations.Commands.RemoveClassCommand @( ,$ClassName )
}

<#
.SYNOPSIS
    Model-RemoveProperties command.

.DESCRIPTION
    Model-RemoveProperties command.

.PARAMETER command
    Model-RemoveProperties command.
#>
function Model-RemoveProperties
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $ClassName,
        [Parameter(ValueFromRemainingArguments = $true)][string[]] $Properties
    )

    Model-ExecuteCommand EfModelMigrations.Commands.RemovePropertiesCommand @( $ClassName, $Properties )
}

<#
.SYNOPSIS
    Model-RenameClass command.

.DESCRIPTION
    Model-RenameClass command.

.PARAMETER command
    Model-RenameClass command.
#>
function Model-RenameClass
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $OldClassName,
        [Parameter(Position = 1, Mandatory = $true)][string] $NewClassName
    )

    Model-ExecuteCommand EfModelMigrations.Commands.RenameClassCommand @( $OldClassName, $NewClassName )
}

<#
.SYNOPSIS
    Model-RenameProperty command.

.DESCRIPTION
    Model-RenameProperty command.

.PARAMETER command
    Model-RenameProperty command.
#>
function Model-RenameProperty
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $ClassName,
        [Parameter(Position = 1, Mandatory = $true)][string] $OldPropertyName,
        [Parameter(Position = 2, Mandatory = $true)][string] $NewPropertyName
    )

    Model-ExecuteCommand EfModelMigrations.Commands.RenamePropertyCommand @( $ClassName, $OldPropertyName, $NewPropertyName )
}


<#
.SYNOPSIS
    Model-ExtractComplexType command.

.DESCRIPTION
    Model-ExtractComplexType command.

.PARAMETER command
    Model-ExtractComplexType command.
#>
function Model-ExtractComplexType
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $ClassName,
        [Parameter(Position = 1, Mandatory = $true)][string] $ComplexTypeName,
        [Parameter(ValueFromRemainingArguments = $true)][string[]] $Properties
    )

    Model-ExecuteCommand EfModelMigrations.Commands.ExtractComplexTypeCommand @( $ClassName, $ComplexTypeName, $Properties )
}


<#
.SYNOPSIS
    Model-ExecuteCommand command.

.DESCRIPTION
    Model-ExecuteCommand command.

.PARAMETER command
    Model-ExecuteCommand Command.

#>
function Model-ExecuteCommand
{
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, Mandatory = $true)][string] $CommandFullName,
        [Parameter(Position = 1, Mandatory = $true)][Array] $Parameters
    )

    $runner = New-EfModelMigrationsRunner $ProjectName 

    try
    {	
        Invoke-RunnerCommand $runner EfModelMigrations.Runtime.PowerShell.ExecuteCommand @( $CommandFullName, $Parameters )

        $error = Get-RunnerError $runner                    

        if ($error)
        {
            Write-Verbose $error.StackTrace
            
            throw $error.Message
        }

        #$(Get-VSComponentModel).GetService([NuGetConsole.IPowerConsoleWindow]).Show()           
    }
    finally
    {               
        Remove-Runner $runner       
    }
}

<#
.SYNOPSIS
    Model-Enable command.

.DESCRIPTION
    Model-Enable command.

.PARAMETER command
    Model-Enable Command.

#>
function Model-Enable
{
    [CmdletBinding()] 
    param (
        
    )

    #TODO: $ProjectName predavat ve vsech prikazech jako optional parametr??
    $runner = New-EfModelMigrationsRunner $ProjectName 

    try
    {	
        #TODO: Not Funny ... http://stackoverflow.com/questions/11138288/how-to-create-array-of-arrays-in-powershell
        #magic comma strikes back...
        Invoke-RunnerCommand $runner EfModelMigrations.Runtime.PowerShell.EnableCommand #@( ,$Params )
    
        $error = Get-RunnerError $runner                    
        if ($error)
        {
            Write-Verbose $error.StackTrace
            
            throw $error.Message
        }

        #$(Get-VSComponentModel).GetService([NuGetConsole.IPowerConsoleWindow]).Show()           
    }
    finally
    {               
        Remove-Runner $runner       
    }
}

<#
.SYNOPSIS
   Model-Migrate command.

.DESCRIPTION
   Model-Migrate command.

.PARAMETER command
   Model-Migrate Command.

#>
function Model-Migrate
{
    [CmdletBinding()] 
    param (
        [string] $TargetModelMigration,
        [switch] $Force
    )

    #TODO: $ProjectName predavat ve vsech prikazech jako optional parametr??
    $runner = New-EfModelMigrationsRunner $ProjectName 

    try
    {	
        Invoke-RunnerCommand $runner EfModelMigrations.Runtime.PowerShell.MigrateCommand @( $TargetModelMigration, $Force.IsPresent )
    
        $error = Get-RunnerError $runner                    
        if ($error)
        {
            Write-Verbose $error.StackTrace
            
            throw $error.Message
        }

        #$(Get-VSComponentModel).GetService([NuGetConsole.IPowerConsoleWindow]).Show()           
    }
    finally
    {               
        Remove-Runner $runner       
    }
}

#private

function New-EfModelMigrationsRunner($ProjectName)
{
    $project = Get-ModelMigrationsProject $ProjectName
    #Build-Project $project


    $installPath = Get-EfModelMigrationsInstallPath $project
    $toolsPath = Join-Path $installPath tools

    $info = New-AppDomainSetup $project $installPath


    #$efDllPath = Get-EntityFrameworkDllPath($project)

    $domain = [AppDomain]::CreateDomain('EfModelMigrations', $null, $info)
    $domain.SetData('project', $project)
    #$domain.SetData('startUpProject', $startUpProject)
    #$domain.SetData('configurationTypeName', $ConfigurationTypeName)
    #$domain.SetData('connectionStringName', $ConnectionStringName)
    #$domain.SetData('connectionString', $ConnectionString)
    #$domain.SetData('connectionProviderName', $ConnectionProviderName)
    
    $dispatcher = New-PowerShellDispatcher $toolsPath
    $domain.SetData('dispatcher', $dispatcher)

    return @{
        Domain = $domain;
        ToolsPath = $toolsPath
    }
}

function Get-SingleProject($name)
{
    $project = Get-Project $name

    if ($project -is [array])
    {
        throw "More than one project '$name' was found. Specify the full name of the one to use."
    }

    return $project
}

function Get-ModelMigrationsProject($name)
{
    if ($name)
    {
        return Get-SingleProject $name
    }

    $project = Get-Project
    
    return $project
}

function New-PowerShellDispatcher($ToolsPath)
{
    [AppDomain]::CurrentDomain.SetShadowCopyFiles()
    $utilityAssembly = [System.Reflection.Assembly]::LoadFrom((Join-Path $ToolsPath 'EfModelMigrations.PowerShellDispatcher.dll'))
    $dispatcher = $utilityAssembly.CreateInstance(
        'EfModelMigrations.PowerShellDispatcher.Dispatcher',
        $false,
        [System.Reflection.BindingFlags]::Instance -bor [System.Reflection.BindingFlags]::Public,
        $null,
        $PSCmdlet,
        $null,
        $null)

    return $dispatcher
}

function Remove-Runner($runner)
{
    [AppDomain]::Unload($runner.Domain)
}

function Get-RunnerError($runner)
{
    $domain = $runner.Domain

    if (!$domain.GetData('wasError'))
    {
        return $null
    }

    return @{
            Message = $domain.GetData('error.Message');
            TypeName = $domain.GetData('error.TypeName');
            StackTrace = $domain.GetData('error.StackTrace')
    }
}

function Get-SingleProject($name)
{
    $project = Get-Project $name

    if ($project -is [array])
    {
        throw "More than one project '$name' was found. Specify the full name of the one to use."
    }

    return $project
}


function Build-Project($project)
{
    $configuration = $DTE.Solution.SolutionBuild.ActiveConfiguration.Name

    $DTE.Solution.SolutionBuild.BuildProject($configuration, $project.UniqueName, $true)

    if ($DTE.Solution.SolutionBuild.LastBuildInfo)
    {
        $projectName = $project.Name

        throw "The project '$projectName' failed to build."
    }
}


function Get-EntityFrameworkDllPath($project)
{
    $efInstallPath = Get-EntityFrameworkInstallPath($project)
    
    $targetFrameworkVersion = (New-Object System.Runtime.Versioning.FrameworkName ($project.Properties.Item('TargetFrameworkMoniker').Value)).Version

    if ($targetFrameworkVersion -lt (New-Object Version @( 4, 5 )))
    {
        $efInstallPath += '\lib\net40'
    }
    else
    {
        $efInstallPath += '\lib\net45'
    }
    return $efInstallPath += '\EntityFramework.dll'
    
    
}

function Get-EntityFrameworkInstallPath($project)
{
    $package = Get-Package -ProjectName $project.FullName | ?{ $_.Id -eq 'EntityFramework' }

    if (!$package)
    {
        $projectName = $project.Name

        throw "The EntityFramework package is not installed on project '$projectName'."
    }
    
    return Get-PackageInstallPath $package
}

function Get-EfModelMigrationsInstallPath($project)
{
    $package = Get-Package -ProjectName $project.FullName | ?{ $_.Id -eq 'EfModelMigrations' }
    if (!$package)
    {
        $projectName = $project.Name
        throw "The EfModelMigrations package is not installed on project '$projectName'."
    }
    
    return Get-PackageInstallPath $package
}

function Get-PackageInstallPath($package)
{
    $componentModel = Get-VsComponentModel
    $packageInstallerServices = $componentModel.GetService([NuGet.VisualStudio.IVsPackageInstallerServices])

    $vsPackage = $packageInstallerServices.GetInstalledPackages() | ?{ $_.Id -eq $package.Id -and $_.Version -eq $package.Version }
    
    return $vsPackage.InstallPath
}

function New-AppDomainSetup($Project, $InstallPath)
{
    $packageRootPath = Split-Path $InstallPath
    
    $packageName = Split-Path $InstallPath -Leaf
    
    $info = New-Object System.AppDomainSetup -Property @{
            ShadowCopyFiles = 'true';
            ApplicationBase = $packageRootPath; # package root
            PrivateBinPath = $packageName + '\tools';
            ConfigurationFile = ([AppDomain]::CurrentDomain.SetupInformation.ConfigurationFile)
        }
    
    $targetFrameworkVersion = (New-Object System.Runtime.Versioning.FrameworkName ($Project.Properties.Item('TargetFrameworkMoniker').Value)).Version
    $efPath = Get-EntityFrameworkInstallPath($Project)
    $efPackageName = Split-Path $efPath -Leaf

    if ($targetFrameworkVersion -lt (New-Object Version @( 4, 5 )))
    {
        $info.PrivateBinPath += ';' + $efPackageName + '\lib\net40'
        $info.PrivateBinPath += ';' + $packageName + '\lib\net40'
    }
    else
    {
    
        $info.PrivateBinPath += ';' + $efPackageName + '\lib\net45'
        $info.PrivateBinPath += ';' + $packageName + '\lib\net45'
    }

    #TODO: Remove next line. Dll with framework in release must be in net40 and net45 subfolders and they are added above.
    $info.PrivateBinPath += ';' + $packageName + '\lib'
    #Write-Host $info.ApplicationBase
    #Write-Host $info.PrivateBinPath
    
    return $info
}

function Invoke-RunnerCommand($runner, $command, $parameters)
{
    $domain = $runner.Domain

    #if ($anonymousArguments)
    #{
    #    $anonymousArguments.GetEnumerator() | %{
    #        $domain.SetData($_.Name, $_.Value)
    #    }
    #}

    $domain.CreateInstanceFrom(
        (Join-Path $runner.ToolsPath 'EfModelMigrations.Runtime.dll'),
        $command,
        $false,
        0,
        $null,
        $parameters,
        $null,
        $null) | Out-Null
}





# EXPORT ----------------------
Export-ModuleMember @( 	'Model-ExtractComplexType', 'Model-AddProperties', 'Model-CreateClass', 'Model-RemoveClass', 
                        'Model-RemoveProperties', 'Model-RenameClass', 'Model-RenameProperty', 
                        'Model-ExecuteCommand', 'Model-Enable', 'Model-Migrate', 'Model-EmptyMigration' ) -Variable InitialModel