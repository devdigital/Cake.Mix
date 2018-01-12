#load common.cake

public class DotNetBuildBuilder
{
  private ICakeContext context;

  private IEnumerable<string> solutions;

  private string configuration;

  private ICollection<string> targets;

  private IDictionary<string, string[]> parameters;

  private MSBuildToolVersion toolVersion;

  public DotNetBuildBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
    this.toolVersion = MSBuildToolVersion.VS2017;
    this.targets = new List<string>();
    this.parameters = new Dictionary<string, string[]>();
  }

  public DotNetBuildBuilder WithConfiguration(string configuration)
  {
    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    this.configuration = configuration;
    return this;
  }

  public DotNetBuildBuilder WithToolVersion(MSBuildToolVersion toolVersion)
  {
    this.toolVersion = toolVersion;
    return this;
  }

  public DotNetBuildBuilder WithTarget(string value)
  {
    this.targets.Add(value);
    return this;
  }

  public DotNetBuildBuilder WithParameter(string name, string[] value)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentNullException(nameof(name));
    }

    if (value == null)
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

  public DotNetBuildBuilder WithParameter(string name, string value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      throw new ArgumentNullException(nameof(value));
    }

    return this.WithParameter(name, new[] { value });
  }

  public DotNetBuildBuilder WithSolutionGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.solutions = this.context.GetFiles(glob).Select(s => s.ToString());
    return this;
  }

  public DotNetBuildBuilder WithSolutionGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.solutions = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public DotNetBuildBuilder WithSolutions(IEnumerable<string> solutions)
  {
    if (solutions == null)
    {
      throw new ArgumentNullException(nameof(solutions));
    }

    this.solutions = solutions;
    return this;
  }

  public DotNetBuildCommand Build()
  {
    return new DotNetBuildCommand(
      this.context,
      this.configuration,
      this.toolVersion,
      this.targets,
      this.parameters,
      this.solutions
    );
  }
}

public class DotNetBuildCommand : ICommand
{
  private ICakeContext context;

  private string configuration;

  private MSBuildToolVersion toolVersion;

  private IEnumerable<string> targets;

  private IDictionary<string, string[]> parameters;

  private IEnumerable<string> solutions;

  public DotNetBuildCommand(
    ICakeContext context,
    string configuration,
    MSBuildToolVersion toolVersion,
    IEnumerable<string> targets,
    IDictionary<string, string[]> parameters,
    IEnumerable<string> solutions)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    if (targets == null)
    {
      throw new ArgumentNullException(nameof(targets));
    }

    if (parameters == null)
    {
      throw new ArgumentNullException(nameof(parameters));
    }

    this.context = context;
    this.configuration = configuration;
    this.toolVersion = toolVersion;
    this.targets = targets;
    this.parameters = parameters;
    this.solutions = solutions;
  }

  public void Execute()
  {
    if (this.solutions == null)
    {
      return;
    }

    // TODO: make configurable
    var settings = new MSBuildSettings
    {
      Verbosity = Verbosity.Minimal,
      ToolVersion = this.toolVersion,
      Configuration = this.configuration
    };

    foreach (var target in this.targets)
    {
      settings = settings.WithTarget(target);
    }

    foreach (var parameter in this.parameters)
    {
      settings = settings.WithProperty(parameter.Key, parameter.Value);
    }

    foreach (var solution in this.solutions)
    {
      this.context.MSBuild(solution, settings);
    }
  }
}
