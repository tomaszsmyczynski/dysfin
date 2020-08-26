$appPoolName = "AspNetCore"
$s = New-PSSession -ComputerName ABC123
Invoke-Command -Session $s {Start-WebAppPool -Name $using:appPoolName}