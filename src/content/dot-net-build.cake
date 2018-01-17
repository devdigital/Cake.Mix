#load common.cake

public class DotNetBuildBuilder
{
  private ICakeContext context;

  private IEnumerable<string> projects;

  private IEnumerable<string> solutions;

  private string configuration;

  private string target;

  private MSBuildPlatform buildPlatform;

  private PlatformTarget? platformTarget;

  private IDictionary<string, string[]> parameters;

  private string toolPath;

  private MSBuildToolVersion toolVersion;

  public DotNetBuildBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
    this.toolPath = null;
    this.toolVersion = MSBuildToolVersion.VS2017;
    this.target = "Build";
    this.buildPlatform = MSBuildPlatform.x86;
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

  public DotNetBuildBuilder WithToolPath(string path)
  {
    if (string.IsNullOrWhiteSpace(path))
    {
      throw new ArgumentNullException(nameof(path));
    }

    this.toolPath = path;
    return this;
  }

  public DotNetBuildBuilder WithToolVersion(MSBuildToolVersion toolVersion)
  {
    this.toolVersion = toolVersion;
    return this;
  }

  public DotNetBuildBuilder WithTarget(string target)
  {
    if (string.IsNullOrWhiteSpace(target))
    {
      throw new ArgumentNullException(nameof(target));
    }

    this.target = target;
    return this;
  }

  public DotNetBuildBuilder WithBuildPlatform(MSBuildPlatform buildPlatform)
  {
    this.buildPlatform = buildPlatform;
    return this;
  }

  public DotNetBuildBuilder WithPlatformTarget(PlatformTarget platformTarget)
  {
    this.platformTarget = platformTarget;
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

  public DotNetBuildBuilder WithProjectGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.projects = this.context.GetFiles(glob).Select(s => s.ToString());
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

  public DotNetBuildBuilder WithProjectGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.projects = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
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

  public DotNetBuildBuilder WithSolution(string solution)
  {
    return this.WithSolutions(new[] { solution });
  }

  public DotNetBuildBuilder WithProjects(IEnumerable<string> projects)
  {
    if (projects == null)
    {
      throw new ArgumentNullException(nameof(projects));
    }

    this.projects = projects;
    return this;
  }

  public DotNetBuildBuilder WithProject(string project)
  {
    return this.WithProjects(new[] { project });
  }

  public DotNetBuildCommand Build()
  {
    return new DotNetBuildCommand(
      this.context,
      this.configuration,
      this.toolPath,
      this.toolVersion,
      this.target,
      this.buildPlatform,
      this.platformTarget,
      this.parameters,
      this.solutions,
      this.projects
    );
  }
}

public class DotNetBuildCommand : ICommand
{
  private ICakeContext context;

  private string configuration;

  private string toolPath;

  private MSBuildToolVersion toolVersion;

  private string target;

  private MSBuildPlatform buildPlatform;

  private PlatformTarget? platformTarget;

  private IDictionary<string, string[]> parameters;

  private IEnumerable<string> solutions;

  private IEnumerable<string> projects;

  public DotNetBuildCommand(
    ICakeContext context,
    string configuration,
    string toolPath,
    MSBuildToolVersion toolVersion,
    string target,
    MSBuildPlatform buildPlatform,
    PlatformTarget? platformTarget,
    IDictionary<string, string[]> parameters,
    IEnumerable<string> solutions,
    IEnumerable<string> projects)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    if (string.IsNullOrWhiteSpace(target))
    {
      throw new ArgumentNullException(nameof(target));
    }

    if (parameters == null)
    {
      throw new ArgumentNullException(nameof(parameters));
    }

    this.context = context;
    this.configuration = configuration;
    this.toolPath = toolPath;
    this.toolVersion = toolVersion;
    this.target = target;
    this.buildPlatform = buildPlatform;
    this.platformTarget = platformTarget;
    this.parameters = parameters;
    this.solutions = solutions ?? Enumerable.Empty<string>();
    this.projects = projects ?? Enumerable.Empty<string>();
  }

  public void Execute()
  {
    // TODO: make configurable
    var settings = new MSBuildSettings
    {
      Verbosity = Verbosity.Minimal,
      ToolVersion = this.toolVersion,
      Configuration = this.configuration,
      MSBuildPlatform = this.buildPlatform,
    };

    if (!string.IsNullOrWhiteSpace(this.toolPath))
    {
      settings.ToolPath = new FilePath(this.toolPath);
    }

    if (this.platformTarget.HasValue)
    {
      settings.PlatformTarget = this.platformTarget;
    }

    settings = settings.WithTarget(this.target);

    foreach (var parameter in this.parameters)
    {
      settings = settings.WithProperty(parameter.Key, parameter.Value);
    }

    foreach (var solution in this.solutions)
    {
      this.context.MSBuild(solution, settings);
    }

    foreach (var project in this.projects)
    {
      this.context.MSBuild(project, settings);
    }
  }
}
