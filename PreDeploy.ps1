# Imports
#--------------------------
#Import-Module WebAdministration

# Settings
#--------------------------
#$TargetDirectory = $OctopusPackageDirectoryPath
#$ErrorActionPreference = "Continue"

# Execution
#-------------------------
#Write-Host "Stopping App Pool"
#Stop-WebAppPool $ApplicationPoolName -ErrorAction Continue

#Write-Host "Stopping Site"
#Stop-Website $ApplicationPoolName -ErrorAction Continue

#Write-Host "Deleting everything in $TargetDirectory"
#Remove-Item $TargetDirectory\* -recurse