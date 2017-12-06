#load ../scripts/package-json.cake

var foo = new PackageJson(Context, "./package.json");
var version = foo.GetVersion();

Information("Version = " + version);

