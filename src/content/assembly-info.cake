#addin "Cake.FileHelpers"

public class AssemblyInfoUpdaterBuilder
{
  private ICakeContext context;
  
  private string filesGlob;
  
  private string version;
  
  public AssemblyInfoUpdaterBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }
    
    this.context = context;
  }
  
  public AssemblyInfoUpdaterBuilder WithFiles(string filesGlob)
  {
    if (string.IsNullOrWhiteSpace(filesGlob))
    {
      throw new ArgumentNullException("filesGlob");
    }
    
    this.filesGlob = filesGlob;
    return this;
  }
  
  public AssemblyInfoUpdaterBuilder WithVersion(string version)
  {
    if (string.IsNullOrWhiteSpace(version))
    {
      throw new ArgumentNullException("version");
    }
    
    this.version = version;
    return this;
  }
  
  public AssemblyInfoUpdater Build()
  {
    return new AssemblyInfoUpdater(this.context, this.filesGlob, this.version);
  }
}

public class AssemblyInfoUpdater
{
  private ICakeContext context;
  
  private string filesGlob;
  
  private string version;
  
  public AssemblyInfoUpdater(ICakeContext context, string filesGlob, string version)
  {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }
    
    if (string.IsNullOrWhiteSpace(filesGlob))
    {
      throw new ArgumentNullException("filesGlob");
    }
    
    if (string.IsNullOrWhiteSpace(version))
    {
      throw new ArgumentNullException("version");
    }
    
    this.context = context;
    this.filesGlob = filesGlob;
    this.version = version;
  }
  
  public void Run()
  {
    this.context.ReplaceRegexInFiles(
      this.filesGlob, 
      "(?<=AssemblyVersion\\(\")(.+?)(?=\"\\))", 
      this.version);
      
    this.context.ReplaceRegexInFiles(
      this.filesGlob,
      "(?<=AssemblyFileVersion\\(\")(.+?)(?=\"\\))", 
      this.version);
  }
}