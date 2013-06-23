<#
.SYNOPSIS
    Add dummy class to project.

.DESCRIPTION
    Add dummy class to project.

.PARAMETER ClassName
    Name of the dummy class

#>
function Add-DummyClass
{
    [CmdletBinding()] 
    param (
        [parameter(Position = 0, Mandatory = $true)][string] $ClassName,
		[Parameter(ValueFromRemainingArguments = $true)][string[]] $Properties
    )

    $runner = New-MVCEvolutionRunner $ProjectName 

    try
    {
        Invoke-RunnerCommand $runner mvc_evolution.PowerShell.Commands.CreateEntityCommand @( $ClassName, $Properties )

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

function New-MVCEvolutionRunner($ProjectName)
{
    $project = Get-MVCEvolutionProject $ProjectName
    #Build-Project $project


    $installPath = Get-MVCEvolutionInstallPath $project
    $toolsPath = Join-Path $installPath tools

    $info = New-AppDomainSetup $project $installPath


	$efDllPath = Get-EntityFrameworkDllPath($project)

    $domain = [AppDomain]::CreateDomain('MVCEvolution', $null, $info)
    $domain.SetData('project', $project)
    $domain.SetData('efDllPath', $efDllPath)
    #$domain.SetData('startUpProject', $startUpProject)
    #$domain.SetData('configurationTypeName', $ConfigurationTypeName)
    #$domain.SetData('connectionStringName', $ConnectionStringName)
    #$domain.SetData('connectionString', $ConnectionString)
    #$domain.SetData('connectionProviderName', $ConnectionProviderName)
    
    $dispatcher = New-DomainDispatcher $toolsPath
    $domain.SetData('dispatcher', $dispatcher)

    return @{
        Domain = $domain;
        ToolsPath = $toolsPath
    }
}

function New-DomainDispatcher($ToolsPath)
{
    [AppDomain]::CurrentDomain.SetShadowCopyFiles()
    $utilityAssembly = [System.Reflection.Assembly]::LoadFrom((Join-Path $ToolsPath 'mvc-evolution.PowerShell.Dispatcher.dll'))
    $dispatcher = $utilityAssembly.CreateInstance(
        'mvc_evolution.PowerShell.Dispatcher.DomainDispatcher',
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

function Get-MVCEvolutionProject($name)
{
    if ($name)
    {
        return Get-SingleProject $name
    }

    $project = Get-Project
    
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

function Get-MVCEvolutionInstallPath($project)
{
    $package = Get-Package -ProjectName $project.FullName | ?{ $_.Id -eq 'MVCEvolution' }
    if (!$package)
    {
        $projectName = $project.Name
        throw "The MVCEvolution package is not installed on project '$projectName'."
    }
    
    return Get-PackageInstallPath $package
}

# function Get-MVCEvolutionInstallPath($project)
# {
#     # Solution level vs project level package - prozatim solution level, pro project viz vyse
#     $package = Get-Package | ?{ $_.Id -eq 'MVCEvolution' }
# 
#     if (!$package)
#     {
# 
#         throw "The MVCEvolution package is not installed."
#     }
#     
#     return Get-PackageInstallPath $package
# }

function Get-PackageInstallPath($package)
{
    $componentModel = Get-VsComponentModel
    $packageInstallerServices = $componentModel.GetService([NuGet.VisualStudio.IVsPackageInstallerServices])

    $vsPackage = $packageInstallerServices.GetInstalledPackages() | ?{ $_.Id -eq $package.Id -and $_.Version -eq $package.Version }
    
    return $vsPackage.InstallPath
}

function New-AppDomainSetup($Project, $InstallPath)
{
    $info = New-Object System.AppDomainSetup -Property @{
            ShadowCopyFiles = 'true';
            ApplicationBase = $InstallPath;
            PrivateBinPath = 'tools';
            ConfigurationFile = ([AppDomain]::CurrentDomain.SetupInformation.ConfigurationFile)
        }
    
    $targetFrameworkVersion = (New-Object System.Runtime.Versioning.FrameworkName ($Project.Properties.Item('TargetFrameworkMoniker').Value)).Version

    if ($targetFrameworkVersion -lt (New-Object Version @( 4, 5 )))
    {
        $info.PrivateBinPath += ';lib\net40'
    }
    else
    {
        $info.PrivateBinPath += ';lib\net45'
    }


    return $info
}

function Invoke-RunnerCommand($runner, $command, $parameters, $anonymousArguments)
{
    $domain = $runner.Domain

    if ($anonymousArguments)
    {
        $anonymousArguments.GetEnumerator() | %{
            $domain.SetData($_.Name, $_.Value)
        }
    }

    $domain.CreateInstanceFrom(
        (Join-Path $runner.ToolsPath 'mvc-evolution.PowerShell.dll'),
        $command,
        $false,
        0,
        $null,
        $parameters,
        $null,
        $null) | Out-Null
}





# EXPORT ----------------------
Export-ModuleMember @( 'Add-DummyClass' )