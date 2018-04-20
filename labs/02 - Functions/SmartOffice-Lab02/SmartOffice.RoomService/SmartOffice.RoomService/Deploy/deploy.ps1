Param(
    [string]$resourceGroupName,
    [string]$cosmosDbAccountName,    
    [string]$cosmosDbDatabaseName,
    [string]$cosmosDbCollectionName,
	[int]$ruLimit,
	[bool]$isPartitioned,
	[string]$partitionKey
)

$cosmosDBApiVersion = "2017-02-22"
$cosmosDbEndpoint = "https://$CosmosDbAccountName.documents.azure.com:443/"

# generate authorization key
Function Generate-MasterKeyAuthorizationSignature
{
  [CmdletBinding()]
  Param
  (
    [Parameter(Mandatory=$true)][String]$verb,
    [Parameter(Mandatory=$false)][String]$resourceLink,
    [Parameter(Mandatory=$false)][String]$resourceType,
    [Parameter(Mandatory=$true)][String]$dateTime,
    [Parameter(Mandatory=$true)][String]$key,
    [Parameter(Mandatory=$true)][String]$keyType,
    [Parameter(Mandatory=$true)][String]$tokenVersion
  )
  $hmacSha256 = New-Object System.Security.Cryptography.HMACSHA256
  $hmacSha256.Key = [System.Convert]::FromBase64String($key)
  If ($resourceLink -eq $resourceType) {
    $resourceLink = ""
  }
  $payLoad = "$($verb.ToLowerInvariant())`n$($resourceType.ToLowerInvariant())`n$resourceLink`n$($dateTime.ToLowerInvariant())`n`n"
  $hashPayLoad = $hmacSha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($payLoad))
  $signature = [System.Convert]::ToBase64String($hashPayLoad);
  [System.Web.HttpUtility]::UrlEncode("type=$keyType&ver=$tokenVersion&sig=$signature")
}
Function Get-CosmosDb
{
  [CmdletBinding()]
  Param
  (
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
    [Parameter(Mandatory=$true)][String]$EndPoint,
    [Parameter(Mandatory=$true)][String]$MasterKey
  )
  $Verb = "GET"
  $ResourceType = "dbs";
  $ResourceLink = "dbs"
  $dateTime = [DateTime]::UtcNow.ToString("r")
  $authHeader = Generate-MasterKeyAuthorizationSignature -verb $Verb -resourceLink $ResourceLink -resourceType $ResourceType -key $MasterKey -keyType "master" -tokenVersion "1.0" -dateTime $dateTime
  $header = @{authorization=$authHeader;"x-ms-documentdb-isquery"="True";"x-ms-version"=$DocumentDBApi;"x-ms-date"=$dateTime}
  $contentType= "application/query+json"
  $queryUri = "$EndPoint$ResourceLink"
  $result = Invoke-RestMethod -Method $Verb -ContentType $contentType -Uri $queryUri -Headers $header
  return $result.Databases
}
Function Create-CosmosDb
{
  [CmdletBinding()]
  Param
  (
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
    [Parameter(Mandatory=$true)][String]$EndPoint,
    [Parameter(Mandatory=$true)][String]$MasterKey,
        [Parameter(Mandatory=$true)][String]$DatabaseName
  )
  $Verb = "POST"
  $ResourceType = "dbs";
  $ResourceLink = "dbs"
    $body = '{
                "id":"'+$DatabaseName+'"
             }'
  $dateTime = [DateTime]::UtcNow.ToString("r")
  $authHeader = Generate-MasterKeyAuthorizationSignature -verb $Verb -resourceLink $ResourceLink -resourceType $ResourceType -key $MasterKey -keyType "master" -tokenVersion "1.0" -dateTime $dateTime
  $header = @{authorization=$authHeader;"x-ms-version"=$DocumentDBApi;"x-ms-date"=$dateTime}
  $contentType= "application/json"
  $queryUri = "$EndPoint$ResourceLink"
  
    try 
    {
        $result = Invoke-RestMethod -Method $Verb -ContentType $contentType -Uri $queryUri -Headers $header -Body $body
        $result 
    } 
    catch 
    {
        Write-Host "ErrorStatusCode:" $_.Exception.Response.StatusCode.value__ 
        Write-Host "ErrorStatusDescription:" $_.Exception.Response.StatusDescription
    }
}
Function Get-PrimaryKey
{
    [CmdletBinding()]
  Param
  (
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
    [Parameter(Mandatory=$true)][String]$ResourceGroupName,
        [Parameter(Mandatory=$true)][String]$CosmosdbAccountName
    )
    try
    {
  
        $keys=Invoke-AzureRmResourceAction -Action listKeys -ResourceType "Microsoft.DocumentDb/databaseAccounts" -ApiVersion $DocumentDBApi -ResourceGroupName $ResourceGroupName -Name $CosmosdbAccountName -Force
        $connectionKey=$keys[0].primaryMasterKey
        return $connectionKey
    }
    catch 
    {
        Write-Host "ErrorStatusDescription:" $_
    }
}
Function New-ProvisionCosmosDb
{
    Param
  (
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
    [Parameter(Mandatory=$true)][String]$cosmosDbEndpoint,
        [Parameter(Mandatory=$true)][String]$DatabaseName,
        [Parameter(Mandatory=$true)][String]$ResourceGroupName,
        [Parameter(Mandatory=$true)][String]$CosmosdbAccountName
  )
    try
    {
        $MasterKey = Get-PrimaryKey -DocumentDBApi "2016-03-31" -ResourceGroupName $ResourceGroupName -CosmosdbAccountName $CosmosdbAccountName 
        IF ($MasterKey -eq "" )
        {
            throw "Unable to Retrieve MasterKey data for [$CosmosdbAccountName]."
        }
    }
    catch
    {
        Write-Host ($_)
        throw $_
    }
    try
    {
        $DBExistence =  Get-CosmosDb -EndPoint $cosmosDbEndpoint -MasterKey $MasterKey -DocumentDBApi $DocumentDBApi | WHERE id -EQ $DatabaseName
       
    if ($DBExistence.id -eq $DatabaseName) 
    {
      Write-Host "Cosmos database [$DatabaseName] already exists in [$CosmosdbAccountName] account"
    } 
    
    }
    catch
    {
        Write-Host ($_)
        throw $_
    }
    
    try
    {
        if ($DBExistence.id -ne $DatabaseName) 
        {
            Create-CosmosDb -EndPoint $cosmosDbEndpoint -MasterKey $MasterKey -DatabaseName $DatabaseName -DocumentDBApi $DocumentDBApi
        }
    }
    catch
    {
        Write-Host ($_)
        throw $_
    }
}

Function Get-CosmosCollections
{
  [CmdletBinding()]
  Param
  (
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
    [Parameter(Mandatory=$true)][String]$EndPoint,
    [Parameter(Mandatory=$true)][String]$DatabaseName,
    [Parameter(Mandatory=$true)][String]$MasterKey
  )
  $Verb = "GET"
  $ResourceType = "colls";
  $ResourceLink = "dbs/$DatabaseName"
  $dateTime = [DateTime]::UtcNow.ToString("r")
  $authHeader = Generate-MasterKeyAuthorizationSignature -verb $Verb -resourceLink $ResourceLink -resourceType $ResourceType -key $MasterKey -keyType "master" -tokenVersion "1.0" -dateTime $dateTime
  $header = @{authorization=$authHeader;"x-ms-documentdb-isquery"="True";"x-ms-version"=$DocumentDBApi;"x-ms-date"=$dateTime}
  $contentType= "application/json"
  $queryUri = "$EndPoint$ResourceLink/colls"
    $result = Invoke-RestMethod -Method $Verb -ContentType $contentType -Uri $queryUri -Headers $header
  $result.DocumentCollections
}

Function Create-CosmosCollections
{
  [CmdletBinding()]
  Param
  (
        [Parameter(Mandatory=$true)][String]$DatabaseName,
    [Parameter(Mandatory=$true)][String]$EndPoint,
        [Parameter(Mandatory=$true)][String]$CollectionName,
        [Parameter(Mandatory=$true)][String]$MaxRU,
    [Parameter(Mandatory=$true)][String]$MasterKey,
        [Parameter(Mandatory=$true)][Boolean]$IsPartitioned,
        [Parameter(Mandatory=$false)][String]$PartitionKey,
        [Parameter(Mandatory=$false)][String]$CustomIndex,
        [Parameter(Mandatory=$true)][String]$DocumentDBApi
  )
  $Verb = "POST"
  $ResourceType = "colls";
  $ResourceLink = "dbs/$DatabaseName"
    
    #Check JSON structure is valid
    try {
    $powershellRepresentation = ConvertFrom-Json $CustomIndex -ErrorAction Stop;
    $validJson = $true;
    } catch {
    $validJson = $false;
    }
        IF($validJson -eq $false)
        {
            throw "Invalid JSON index format"
            Write-Error "Invalid JSON index format"
        }
        if ($IsPartitioned -eq $true -and ($PartitionKey -eq "" -or $PartitionKey -eq $null))
        {
            throw "IsPartitioned is set to true but no parition key is provided. Update the variable and redeploy."
        }
        
    $partitionschema = IF ($IsPartitioned -eq $true) { ',"partitionKey": {  
                "paths": [  
                  "/'+$PartitionKey+'"  
                ],  
                "kind": "Hash"  
              }'} else {""}
    $indexschema = IF ($validJson -eq $true -and ($CustomIndex.Length -ne 0 )) { ',"indexingPolicy":'+$CustomIndex} else {""}
    $body = '{  
              "id": "'+$CollectionName+'"'+$indexschema+$partitionschema+'}' 
            
  $dateTime = [DateTime]::UtcNow.ToString("r")
  $authHeader = Generate-MasterKeyAuthorizationSignature -verb $Verb -resourceLink $ResourceLink -resourceType $ResourceType -key $MasterKey -keyType "master" -tokenVersion "1.0" -dateTime $dateTime
  $header = @{authorization=$authHeader;"x-ms-version"=$DocumentDBApi;"x-ms-date"=$dateTime; "Accept" =  "application/json";"x-ms-offer-throughput"=$MaxRU }
  $contentType= "application/json"
  $queryUri = "$EndPoint$ResourceLink/colls"
  $result = Invoke-RestMethod -Method $Verb -ContentType $contentType -Uri $queryUri -Headers $header  -Body $body
  $result 
}

Function Get-PrimaryKey
{
    [CmdletBinding()]
  Param
  (
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
    [Parameter(Mandatory=$true)][String]$ResourceGroupName,
        [Parameter(Mandatory=$true)][String]$CosmosdbAccountName
    )
    try
    {
  
        $keys=Invoke-AzureRmResourceAction -Action listKeys -ResourceType "Microsoft.DocumentDb/databaseAccounts" -ApiVersion $DocumentDBApi -ResourceGroupName $ResourceGroupName -Name $CosmosdbAccountName -Force
        $connectionKey=$keys[0].primaryMasterKey
        return $connectionKey
    }
    catch 
    {
        Write-Host "ErrorStatusDescription:" $_
    }
}

Function New-ProvisionCosmosCollection
{
    Param
  (
    [Parameter(Mandatory=$true)][String]$ResourceGroupName,
        [Parameter(Mandatory=$true)][String]$CosmosdbAccountName,
    [Parameter(Mandatory=$true)][String]$CosmosDBEndPoint,
    [Parameter(Mandatory=$true)][String]$DatabaseName,
        [Parameter(Mandatory=$true)][String]$CollectionName,
        [Parameter(Mandatory=$true)][Boolean]$IsPartitioned,
        [Parameter(Mandatory=$false)][String]$PartitionKey,
        [Parameter(Mandatory=$false)][String]$CustomIndex,
        [Parameter(Mandatory=$true)][String]$DocumentDBApi,
        [Parameter(Mandatory=$true)][String]$MaxRU
  )
    try
    {
        $MasterKey = Get-PrimaryKey -DocumentDBApi "2015-04-08" -ResourceGroupName $ResourceGroupName -CosmosdbAccountName $CosmosdbAccountName 
        IF ($MasterKey -eq "" -or $MasterKey -eq $null )
        {
            throw "Unable to Retrieve MasterKey data for [$CosmosdbAccountName]."
        }
    }
    catch
    {
        Write-Host ($_)
        throw $_
    }
    
  
    try
    {
        $CollectionExistence =  Get-CosmosCollections -EndPoint $CosmosDBEndPoint -MasterKey $MasterKey -DatabaseName $DatabaseName -DocumentDBApi $DocumentDBApi | WHERE id -eq $CollectionName
                
    if ($CollectionExistence.id -eq $CollectionName) 
    {
      Write-Host "Collection [$CollectionName] already exists in [$DatabaseName] database"
    } 
    
    }
    catch
    {
        Write-Host ($_)
        throw $_
    }
    
    try
    {
        if ($CollectionExistence.id -ne $CollectionName) 
        { Write-Host ($CustomIndex)
            Create-CosmosCollections -DatabaseName $DatabaseName -EndPoint $CosmosDBEndPoint -CollectionName $CollectionName -MaxRU $MaxRU -MasterKey $MasterKey -IsPartitioned $IsPartitioned -PartitionKey $PartitionKey -CustomIndex $CustomIndex -DocumentDBApi $DocumentDBApi
        }
    }
    catch
    {
        Write-Host ($_)
        throw $_
    } 
}
  
New-ProvisionCosmosDb -DocumentDBApi $cosmosDBApiVersion -CosmosDBEndPoint $cosmosDbEndpoint -DatabaseName $CosmosDbDatabaseName  -ResourceGroupName $ResourceGroupName  -CosmosdbAccountName $CosmosDbAccountName

New-ProvisionCosmosCollection -ResourceGroupName $ResourceGroupName -CosmosdbAccountName $CosmosDbAccountName -CosmosDBEndPoint $CosmosDbEndPoint -DatabaseName $CosmosDbDatabaseName -CollectionName $CosmosDbCollectionName -IsPartitioned $isPartitioned -PartitionKey $partitionKey -CustomIndex $CosmosDbIndex -DocumentDBApi $CosmosDBApiVersion -MaxRU $ruLimit