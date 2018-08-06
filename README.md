# Cake.Mix

Set of utility Cake build scripts.

## Getting Started

Adding the following gives you access to all Cake.Mix commands:

```
#load "nuget:?package=Cake.Mix"
```

However, it is **strongly recommended** that you specify the version of the package too, e.g:

```
#load "nuget:?package=Cake.Mix&version=0.13.1"
```

This ensures any breaking changes in future versions of the package do not break your build script. It also ensures that the local `tools` folder isn't using a cached version of the package on updates.

The latest version number of the package can be checked in the [Change Log](/CHANGELOG.md).

## Pin Cake version

You may need to [pin the version of Cake](https://cakebuild.net/docs/tutorials/pinning-cake-version) to avoid warnings, for example to version 0.25. First modify `tools/packages.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
    <package id="Cake" version="0.25.0" />
</packages>
```

Next, update your `.gitignore` file to ensure the `tools/packages.config` file is committed to version control:

```
/tools/*
!/tools/xunit.runner.console.2.3.1
!/tools/packages.config
```

## Commands

### Reading package.json file

```csharp
var packageJson = new PackageJson(Context, "./package.json");
var version = packageJson.GetVersion();
```

### Yarn

```csharp
var command = new YarnBuilder(Context)
  .WithWorkingDirectory("./src")
  .WithArguments("install")
  .Build();
  
command.Execute();
```

You can execute any command with the `WithArguments` method, e.g. `build` or `test` etc.
Cake.Mix also provides an `AggregateCommand`, so if you wish to perform a Yarn install, build, and test in a single command you can:

```csharp
var yarnBuilder = new YarnBuilder(Context)
  .WithWorkingDirectory("./src");

var command = new AggregateCommand(
  yarnBuilder.WithArguments("install").Build(),
  yarnBuilder.WithArguments("build").Build(),
  yarnBuilder.WithArguments("test").Build()
);

command.Execute();
```

The global yarn installation will be used by default. If you wish to use a release version of Yarn instead, then you can [download](https://github.com/yarnpkg/yarn/releases) the appropriate release and set the Yarn path with the builder:

```csharp
var yarnBuilder = new YarnBuilder(Context)
  .WithWorkingDirectory("./src")
  .WithYarnPath("./yarn-1.3.2.js");
```

### .NET 4.x

#### Update AssemblyInfo Files

```csharp
var command = new AssemblyInfoUpdaterBuilder(Context)
  .WithFileGlob("./Source/**/AssemblyInfo.cs")
  .WithVersion("1.0.0.0")
  .Build();

command.Execute();
```

#### Restore

```csharp
var command = new DotNetRestoreBuilder(Context)
  .WithSolutions(new[] { "./MySolution.sln" })
  .Build();

command.Execute();
```

#### Build

```csharp
var command = new DotNetBuildBuilder(Context)
  .WithSolution("./MySolution.sln")
  .Build();
  
command.Execute();
```

#### Test with XUnit 2

```csharp
var command = new XUnit2TestBuilder(Context)
  .WithProjectGlob("./Source/**/*UnitTests.csproj")
  .Build();
  
command.Execute();
```

> Note that if you receive an `Unknown test framework: could not find xunit.dll (v1) or xunit.execution.*.dll (v2) ...` error then ensure that your test projects have a reference to the `xunit.core` NuGet package and that `xunit.execution.desktop.dll` gets copied to your bin output folder.

#### Test with XUnit 2, XML test result output, and XSLT transformation

```csharp
var command = new XUnit2TestBuilder(Context)
  .WithProjectGlob("./Source/**/*UnitTests.csproj")
  .WithXmlOutput("testresults")
  .WithXsltTransform("./xunit-to-trx.xslt", "trx")
  .Build();
  
command.Execute();
```

#### Publish

```csharp
var command = new DotNetBuildBuilder(Context)
  .WithSolution("./MySolution.sln")
  .WithTarget("Build")
  .WithParameter("DeployOnBuild", "true")
  .WithParameter("AutoParameterizationWebConfigConnectionStrings", "false")
  .Build();

command.Execute();
```

#### Include Files in csproj for WebDeploy

```csharp
var command = new DotNetIncludeFilesBuilder(Context)
  .WithProject("./MyProject.csproj")
  .WithFilesGlob(@"Content\*")
  .WithDestinationFolder("Content")
  .Build();

command.Execute();
```

### .NET Core

#### Restore

```csharp
var command = new DotNetCoreRestoreBuilder(Context)
  .WithProjects(new[] { "./MyProject.csproj" })
  .Build();

command.Execute();
```

#### Build

```csharp
var command = new DotNetCoreBuildBuilder(Context)
  .WithSolutionGlob("./Source/**/*.sln")
  .WithParameter("DeployOnBuild", "true")
  .WithParameter("AssemblyVersion", version)
  ...
  .Build();

command.Execute();
```

#### Test

```csharp
var command = new DotNetCoreTestBuilder(Context)
  .WithProjectGlob("./Source/**/*UnitTests.csproj")
  .WithArguments("--logger \"trx;LogFileName=testresults.trx\"")
  .Build();

command.Execute();
```

For all of the .NET core commands, you can also specify the path to the .NET Core SDK with `WithDotNetCorePath`, e.g:

```csharp
var command = new DotNetCoreRestoreBuilder(Context)
  .WithDotNetCorePath(dotNetCorePath)
  ...
```

### TFS

#### Set Build Number

```csharp
var command = new TfsBuildNumberBuilder(Context)
  .WithBuildNumber("1.0.0.1")
  .Build();

command.Execute();
```

#### Publish Artifacts

```csharp
var command = new TfsPublishArtifactBuilder(Context)
  .WithContainerFolder("foo")
  .WithArtifactName("foo")
  .WithArtifactLocation(EnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY"))
  .Build();
  
command.Execute();
```
   
