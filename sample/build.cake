#load ../src/content/package-json.cake
#load ../src/content/assembly-info.cake

var packageJson = new PackageJson(Context, "./package.json");
var version = packageJson.GetVersion();

Information("Version = " + version);

var assemblyInfoUpdater = new AssemblyInfoUpdaterBuilder(Context)
  .WithFiles("**/AssemblyInfo.cs")
  .WithVersion(version)
  .Build();
  
assemblyInfoUpdater.Execute();