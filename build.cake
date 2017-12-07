#addin "Cake.FileHelpers"
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

var target = Argument("Target", "Default");
var configuration = Argument("Configuration", "Release");

Information("Running target " + target + " in configuration " + configuration);

var packageJsonText = FileReadText("./package.json");
var packageJson = Newtonsoft.Json.Linq.JObject.Parse(packageJsonText);
var buildNumber = packageJson.Property("version").Value;

var artifactsDirectory = Directory("./artifacts");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDirectory);
    });

Task("Pack")
    .IsDependentOn("Clean")
    .Does(() => {
      var version = buildNumber.ToString();
      var nuspecPath = "./src/Cake.Mix.nuspec";
      NuGetPack(
        nuspecPath,
        new NuGetPackSettings()
        {
          Version = version,
          OutputDirectory = artifactsDirectory
        }
      );
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);
