#load ../src/content/package-json.cake

var packageJson = new PackageJson(Context, "./package.json");
var version = packageJson.GetVersion();

Information("Version = " + version);

