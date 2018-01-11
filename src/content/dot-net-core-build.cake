#load common.cake

public class DotNetCoreBuildBuilder
{
  private ICakeContext context;

  private string dotNetCorePath;

  private IEnumerable<string> solutions;

  private string configuration;

  private IDictionary<string, string> parameters;

  public DotNetCoreBuildBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
    this.parameters = new Dictionary<string, string>();
  }

  public DotNetCoreBuildBuilder WithDotNetCorePath(string path)
  {
    this.dotNetCorePath = path;
    return this;
  }

  public DotNetCoreBuildBuilder WithConfiguration(string configuration)
  {
    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    this.configuration = configuration;
    return this;
  }

  public DotNetCoreBuildBuilder WithSolutionGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.solutions = this.context.GetFiles(glob).Select(s => s.ToString());
    return this;
  }

  public DotNetCoreBuildBuilder WithSolutionGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.solutions = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public DotNetCoreBuildBuilder WithSolutions(IEnumerable<string> solutions)
  {
    if (solutions == null)
    {
      throw new ArgumentNullException(nameof(solutions));
    }

    this.solutions = solutions;
    return this;
  }

  public DotNetCoreBuildBuilder WithParameter(string name, string value)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentNullException(nameof(name));
    }

    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentNullException(nameof(value));
    }

    if (this.parameters.ContainsKey(name))
    {
      throw new ArgumentException($"Parameter with name '{name}' already added.");
    }

    this.parameters.Add(name, value);
    return this;
  }

  public DotNetCoreBuildCommand Build()
  {
    return new DotNetCoreBuildCommand(
      this.context,
      this.dotNetCorePath,
      this.solutions,
      this.configuration,
      this.parameters
    );
  }
}

public class DotNetCoreBuildCommand : ICommand
{
  private ICakeContext context;

  private string dotNetCorePath;

  private IEnumerable<string> solutions;

  private string configuration;

  private IDictionary<string, string> parameters;

  public DotNetCoreBuildCommand(ICakeContext context, string dotNetCorePath, IEnumerable<string> solutions, string configuration, IDictionary<string, string> parameters)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    if (parameters == null)
    {
      throw new ArgumentNullException(nameof(parameters));
    }

    this.context = context;
    this.dotNetCorePath = dotNetCorePath;
    this.solutions = solutions;
    this.configuration = configuration;
    this.parameters = parameters;
  }

  public void Execute()
  {
    if (this.solutions == null)
    {
      return;
    }

    // TODO: boolean option for nologo param
    var msbuildArgs = "/nologo";

    if (parameters.Any())
    {
      var msbuildParams = parameters.Select(kvp => $"/p:{kvp.Key}={kvp.Value}");
      msbuildArgs = $"{msbuildArgs} {string.Join(" ", msbuildParams)}";
    }

    /*this.context.Information(@"/nologo /p:publishProfile=Release /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:PackageLocation="".\bin\Package"" /p:AssemblyVersion=" + "1.0.0.0");
    this.context.Information(msbuildArgs);*/

    var settings = new DotNetCoreBuildSettings()
    {
        Configuration = this.configuration,
        ArgumentCustomization = args => args.Append(msbuildArgs)
    };

    if (this.dotNetCorePath != null)
    {
       settings.ToolPath = this.dotNetCorePath;
    }

    foreach(var solution in solutions)
    {
        this.context.DotNetCoreBuild(
            solution,
            settings);
    }
  }
}
