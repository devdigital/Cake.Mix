#load ../src/content/assembly-info.cake
#load ../src/content/dot-net-build.cake
#load ../src/content/dot-net-core-build.cake
#load ../src/content/dot-net-core-restore.cake
#load ../src/content/dot-net-core-test.cake
#load ../src/content/dot-net-include-files.cake
#load ../src/content/dot-net-restore.cake
#load ../src/content/package-json.cake
#load ../src/content/xunit2-test.cake
#load ../src/content/yarn.cake

var packageJson = new PackageJson(Context, "./package.json");
var version = packageJson.GetVersion();

Information("Version = " + version);

var assemblyInfoUpdater = new AssemblyInfoUpdaterBuilder(Context)
  .WithFileGlob("**/AssemblyInfo.cs")
  .WithVersion(version)
  .Build();

assemblyInfoUpdater.Execute();

var yarn = new YarnBuilder(Context)
  .WithArguments("build")
  .Build();

yarn.Execute();

var dotNetBuildCommand = new DotNetBuildBuilder(Context)
  .WithSolution("./src/Cake.Mix.Sample.sln")
  .Build();

dotNetBuildCommand.Execute();

var xunit2TestCommand = new XUnit2TestBuilder(Context)
  .WithProjectGlob("./src/**/*Tests.csproj")
  .WithXmlOutput("testoutput")
  .WithXsltTransform("./xunit-to-trx.xslt", "trx")
  .Build();

xunit2TestCommand.Execute();
