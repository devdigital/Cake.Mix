# Cake.Mix

Set of utility Cake build scripts.

## Getting Started

Adding the following gives you access to all Cake.Mix commands:

```
#load "nuget:?package=Cake.Mix"
```

In fact, it's recommended that you specify the version of the package too, e.g:

```
#load "nuget:?package=Cake.Mix&version=0.4.0"
```

This ensures any breaking changes in future versions of the package do not break your build script. It also ensures that the local `tools` folder isn't using a cached version on the package on updates.

The latest version number of the package can be checked in the [Change Log](/CHANGELOG.md).

## Commands

### Reading package.json file

```csharp
var packageJson = new PackageJson(Context, "./package.json");
var version = packageJson.GetVersion();
```

### Yarn

```csharp
var yarn = new YarnBuilder(Context)
  .WithWorkingDirectory("./src")
  .WithArguments("install")
  .Build();
  
yarn.Execute();
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
