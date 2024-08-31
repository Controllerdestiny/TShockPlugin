#!/usr/bin/env pwsh

foreach ($p in @(ls $PSScriptRoot/../src/*/*.csproj))  {
    if (-not (Test-Path -Path "$($p.DirectoryName)/i18n" -PathType Container)) {
        continue
    }
    
    $pot = "$([System.IO.Path]::Combine($p.DirectoryName, "i18n", "template.pot"))"
    GetText.Extractor -s $p.FullName -t $pot
    
    foreach ($t in @(ls "$($p.DirectoryName)/i18n/*.po"))  {
        msgmerge --previous --update $t $pot
    }
}