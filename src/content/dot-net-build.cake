#load common.cake

public class DotNetBuildBuilder
{
  private ICakeContext context;

  private IEnumerable<string> solutions;

  private string configuration;

  public DotNetBuildBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
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
      this.solutions
    );
  }
}

public class DotNetBuildCommand : ICommand
{
  private ICakeContext context;

  private string configuration;

  private IEnumerable<string> solutions;

  public DotNetBuildCommand(ICakeContext context, string configuration, IEnumerable<string> solutions)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    this.context = context;
    this.configuration = configuration;
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
      ToolVersion = MSBuildToolVersion.VS2015,
      Configuration = this.configuration
    };

    foreach (var solution in this.solutions)
    {
      this.context.MSBuild(solution, settings);
    }
  }
}