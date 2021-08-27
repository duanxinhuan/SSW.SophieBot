# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
# 
# Builds and trains LUIS models. Used in conjunction with CICD, performs the following:
#  - Determines what models to build based on the recognizer configured for each dialog and writes a luConfig file with the list
#  - Builds Orchestrator language models (english and multilingual) snapshot files
#  - Creates configuration file used by runtime (orchestrator.settings.json)

function Get-LUModels {
    param 
    (
        [string] $recognizerType,
        [string] $crossTrainedLUDirectory,
        [string] $sourceDirectory
    )

    # Get a list of the cross trained lu models to process
    $crossTrainedLUModels = Get-ChildItem -Path $crossTrainedLUDirectory -Filter "*.lu" -file -name

    # Get a list of all the dialog recognizers (exclude bin and obj just in case)
    $luRecognizerDialogs = Get-ChildItem -Path $sourceDirectory -Filter "*??-??.lu.dialog" -file -name -Recurse | Where-Object { $_ -notmatch '^bin*|^obj*' }
    Write-Host ($luRecognizerDialogs | Format-List | Out-String)

    # Create a list of the models that match the given recognizer
    $luModels = @()
    foreach ($luModel in $crossTrainedLUModels) {
        Write-Host "1-luModel is $($luModel)"
		
        # Load the dialog JSON and find the recognizer kind
        $luDialog = $luRecognizerDialogs | Where-Object { (Split-Path $_ -leaf) -eq "$luModel.dialog" }
        $dialog = Get-Content -Path "$sourceDirectory/$luDialog" | ConvertFrom-Json
        $recognizerKind = ($dialog | Select -ExpandProperty "`$kind")

        # Add it to the list if it is the expected type
        if ( $recognizerKind -eq $recognizerType) {
            $luModels += "$luModel"
        }
    }

    # return the models found
    return $luModels
}

# Creates luConfigFile for a list of lu models
function New-LuConfigFile {
    param
    (
        [string] $luConfig,
        [string[]] $luModels,
        [string[]] $path
    )

    $luConfigLuis = "{
        models:[]
    }" | ConvertFrom-Json
    
    foreach ($model in $models) {
        $luConfigLuis.models += "$path/$model"
    }
    
    $luConfigLuis | ConvertTo-Json | Out-File -FilePath $luConfigFile
}


Param(
    [string] $outputDirectory,
    [string] $sourceDirectory,
    [string] $crossTrainedLUDirectory,
    [string] $authoringKey,
    [string] $luisEndPoint,
    [string] $botName,
    [string] $region
)

if ($PSBoundParameters.Keys.Count -lt 5) {
    Write-Host "Builds, trains and publishes LUIS models" 
    Write-Host "Usage:"
    Write-Host "`t Build-LUIS.ps1 -outputDirectory ./generated -sourceDirectory ./ -crossTrainedLUDirectory ./generated/interruption -authoringKey 12345612345 -botName MyBotName"  
    Write-Host "Parameters: "
    Write-Host "`t  outputDirectory - Directory for processed config file"
    Write-Host "`t  sourceDirectory - Directory containing bot's source code."
    Write-Host "`t  crossTrainedLUDirectory - Directory containing .lu/.qna files to process."
    Write-Host "`t  authoringKey - LUIS Authoring key."
    Write-Host "`t  botName - Bot's name."
    exit 1
}

# Find the lu models for the dialogs configured to use a LUIS recognizer
$models = Get-LUModels -recognizerType "Microsoft.LuisRecognizer" -crossTrainedLUDirectory $crossTrainedLUDirectory -sourceDirectory $sourceDirectory
if ($models.Count -eq 0) {
    Write-Host "No LUIS models found."
    exit 0
}

# Create luConfig file with a list of the LUIS models
$luConfigFile = "$crossTrainedLUDirectory/luConfigLuis.json"
Write-Host "Creating $luConfigFile..."
New-LuConfigFile -luConfig $luConfigFile -luModels $models -path "."

# Output the generated settings
Get-Content $luConfigFile

# Publish and train LUIS models on cloud via bf luis cli
Write-Host "Publish and train LUIS models on cloud via bf luis cli"
Write-Host "luis authoringKey is"$authoringKey
Write-Host "botName is"$botName
Write-Host "luConfigFile is"$luConfigFile

bf luis:build --out $outputDirectory --authoringKey $authoringKey --botName $botName --suffix composer --force --log --luConfig $luConfigFile --endpoint $luisEndPoint --region $region
