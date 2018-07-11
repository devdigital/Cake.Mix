#addin nuget:?package=Cake.Yarn&version=0.3.7
#load common.cake

public class YarnBuilder
{
  private ICakeContext context;
  
  private string workingDirectory;
  
  private string yarnPath;
  
  private string arguments;
  
  public YarnBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }
    
    this.context = context;
    this.workingDirectory = ".";
    this.yarnPath = null;
    this.arguments = "start";
  }
  
  public YarnBuilder WithWorkingDirectory(string workingDirectory)
  {
    if (string.IsNullOrWhiteSpace(workingDirectory))
    {
      throw new ArgumentNullException(nameof(workingDirectory));
    }
    
    this.workingDirectory = workingDirectory;
    return this;
  }
  
  public YarnBuilder WithYarnPath(string yarnPath)
  {
    if (string.IsNullOrWhiteSpace(yarnPath))
    {
      throw new ArgumentNullException(nameof(yarnPath));
    }
    
    this.yarnPath = yarnPath;
    return this;
  }
  
  public YarnBuilder WithArguments(string arguments)
  {
    if (string.IsNullOrWhiteSpace(arguments))
    {
      throw new ArgumentNullException(nameof(arguments));
    }
    
    this.arguments = arguments;
    return this;
  }
  
  public YarnCommand Build()
  {
    return new YarnCommand(
      this.context,
      this.workingDirectory,
      this.yarnPath,
      this.arguments);
  }
}

public class YarnCommand : ICommand
{
  private readonly ICakeContext context;

  private readonly string workingDirectory;

  private readonly string yarnPath;
  
  private readonly string arguments;

  public YarnCommand(
    ICakeContext context, 
    string workingDirectory,
    string yarnPath, 
    string arguments) 
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }
    
    if (string.IsNullOrWhiteSpace(workingDirectory))
    {
      throw new ArgumentNullException(nameof(workingDirectory));
    }
    
    if (string.IsNullOrWhiteSpace(arguments))
    {
      throw new ArgumentNullException(nameof(arguments));      
    }
    
    this.context = context;
    this.workingDirectory = workingDirectory;
    this.yarnPath = yarnPath;
    this.arguments = arguments;
  }

  public void Execute()
  {
    if (string.IsNullOrWhiteSpace(this.yarnPath))
    {
      this.RunGlobalYarn(this.arguments);
      return;
    }
    
    this.RunYarn(this.arguments);
  }

  private void RunGlobalYarn(string arguments)
  {
    var yarn = this.context.Yarn().FromPath(this.workingDirectory);
    
    if (arguments == "install")
    {
      yarn.Install();
      return;
    }
    
    yarn.RunScript(this.arguments);
  }

  private void RunYarn(string arguments)
  {
    var exitCode = this.context.StartProcess("node", new ProcessSettings {
        Arguments = $"{this.yarnPath} {arguments}",
        WorkingDirectory = this.workingDirectory
      });

    if (exitCode != 0)
    {
      throw new Exception($"yarn '{arguments}' failed with exit code '{exitCode}'.");
    }
  }
}