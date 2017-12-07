#load ../src/content/package-json.cake
#load ../src/content/assembly-info.cake
#load ../src/content/yarn.cake

var packageJson = new PackageJson(Context, "./package.json");
var version = packageJson.GetVersion();

Information("Version = " + version);

var assemblyInfoUpdater = new AssemblyInfoUpdaterBuilder(Context)
  .WithFiles("**/AssemblyInfo.cs")
  .WithVersion(version)
  .Build();
  
assemblyInfoUpdater.Execute();

var yarn = new YarnBuilder(Context)
  .WithArguments("build")
  .Build();

yarn.Execute();