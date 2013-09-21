<#
.SYNOPSIS
    Model command.

.DESCRIPTION
    Model command.

.PARAMETER command
    Command.

#>
function Model
{
    [CmdletBinding()] 
    param (
        [parameter(Position = 0, Mandatory = $true)][string] $Command,
		[Parameter(ValueFromRemainingArguments = $true)][string[]] $Params
    )

    $runner = New-EfModelMigrationsRunner $ProjectName 

    try
    {	
		#if([string]::Equals($Command, "Enable", [stringCo]) 
		if($Command -ieq "Enable")
		{
			Invoke-RunnerCommand $runner EfModelMigrations.Runtime.PowerShell.EnableCommand @( $Params )
			
			$defaultModelMigrationsDir = "ModelMigrations"
			$dbMigrationsDir = $defaultModelMigrationsDir + "\DbMigrations"
			Enable-Migrations -MigrationsDirectory $dbMigrationsDir
		}
		else
		{
        	Invoke-RunnerCommand $runner EfModelMigrations.Runtime.PowerShell.ModelCommand @( $Command, $Params )
		}

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
    #$domain.SetData('efDllPath', $efDllPath)
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
Export-ModuleMember @( 'Model' )