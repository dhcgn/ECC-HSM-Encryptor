$targetFolder = 'C:\DEV\ECC-HSM-Encryptor\libs'

Write-Progress -Activity 'Delete old files'
$targetFolder | Get-ChildItem | ?{$_.Fullname -notlike '*.ps1'} | Remove-Item

$path = Resolve-Path C:\DEV\Encryption-Suite\src\*\bin\Release\* | ?{$_.Path -notlike '*TEST*'}

Write-Progress -Activity 'Copy new files'
$path | %{ Copy-Item  $path -Destination $targetFolder}