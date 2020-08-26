$appPoolName = "AspNetCore"
$s = New-PSSession -ComputerName ABC123
Invoke-Command -Session $s {Stop-WebAppPool -Name $using:appPoolName}