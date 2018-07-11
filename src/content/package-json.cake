#addin nuget:?package=Cake.FileHelpers&version=3.0.0
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

public class PackageJson 
{  
  private ICakeContext context;
  
  private string path;
  
  private Newtonsoft.Json.Linq.JObject jsonObject;
  
  public PackageJson(ICakeContext context, string path) {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }
    
    if (string.IsNullOrWhiteSpace(path)) {
      throw new ArgumentNullException("path");
    }
    
    this.context = context;
    this.path = path;
  }

  private Newtonsoft.Json.Linq.JObject JsonObject
  {
    get 
    {
      if (this.jsonObject == null)
      {
        this.jsonObject = LoadJsonObject(this.path);    
      }
      
      return this.jsonObject;
    }
  }
  
  private Newtonsoft.Json.Linq.JObject LoadJsonObject(string path)
  {
    // TODO: error handling
    var packageJsonText = this.context.FileReadText(path);
    return Newtonsoft.Json.Linq.JObject.Parse(packageJsonText);    
  }
  
  public string GetVersion()
  {
    return this.JsonObject.Property("version").Value.ToString();
  }
}

