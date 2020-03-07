# UnityBuilder

## 〇 Configuration

### ■ Configuration Overview

- Unity definition
  - Assets\UnityBuilder\Plugins\StandardKit\PresettingProcessor.cs
  - Assets\UnityBuilder\Plugins\StandardKit\PresettingProcessor\Configuration.cs
- The items set by PresettingProcessor are mostly defined in PlayerSettings or EditorUserBuildSettings.

### ■ Configuration Example

``` Configuration Example
<?xml version="1.0" encoding="UTF-8"?>
<Configuration Identifier="example">
    <Schemes>
        <Scheme Identifier="development">
            <ApplicationIdentifier>[Your ApplicationIdentifier]]</ApplicationIdentifier>
            <CompanyName>[Your CompanyName]</CompanyName>
            <ProductName>[ProductName]</ProductName>
            <Development>[Is Development Build]</Development>
            <IOS>
                <ScriptingDefineSymbols>[Scripting Define Symbols]</ScriptingDefineSymbols>
                <AppleDeveloperTeamID>[Apple Developer TeamID]</AppleDeveloperTeamID>
            </IOS>
            <Android>
                <ScriptingDefineSymbols>[Scripting Define Symbols]</ScriptingDefineSymbols>
                <UseBuildAppBundle>[Use App Bundle]</UseBuildAppBundle>
                <KeystoreName>[Keystore File Path]</KeystoreName>
                <KeystorePass>[Keystore Password]</KeystorePass>
                <KeyaliasName>[Keyalias Name]</KeyaliasName>
                <KeyaliasPass>[Keyalias Password]</KeyaliasPass>
            </Android>
        </Scheme>
    </Schemes>
</Configuration>
```

## 〇 Batch/AppBuild.sh

### ■ Batch/AppBuild.sh Overview

- First make sure that the manual build succeeds.
- I got the Unity path to use in Setup.sh. This should be changed by the user.
- Operation has been confirmed on the terminal integrated with VSCode.
- On macOS, operation is confirmed with zsh.
- gnu-sed is required for macOS.

``` gnu-sed
$brew install gnu-sed
```

### ■ Batch/AppBuild.sh Usage

``` AppBuild.sh Usage
AppBuild.sh is a tool for ...

Usage:
    AppBuild.sh [<options>]

Options:
    --help      -h  print this
    --platform  -p  Platform [Android | iOS]
    --config    -c  Configuration Identifier in Configuration
    --scheme    -s  Scheme Identifier in Configuration
    --entry     -e  Entry Method Name
```

### ■ Batch/AppBuild.sh Example

``` AppBuild.sh Example
sh Batch/AppBuild.sh -p Android -c example -s development -e Example.DoIt
```

## 〇 Custom Build Process

### ■ Custom Build Example

- Unity definition
  - Assets\UnityBuilder\Example\Editor\Example.cs

## 〇 About output files

### ■ logs

- Two types of logs are output.
  - 1:Logs/[platform]/[ProductName].log
    - Use Assets\UnityBuilder\Example\Editor\MyLogHandler.cs
    - This is a simple log.
    - Only the contents of UnityEngine.Debug.Log during the build process.
  - 2:Logs/[platform]/[Configuration Identifier]/[Scheme Identifier].log
    - All logs output by Unity.
    - The log of 1. is also included.

### ■ Executable file

- It is basically output with the following naming.
  - build/[platform]/[ProductName]
