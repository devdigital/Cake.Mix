#tool "nuget:?package=xunit.runner.console"
#load common.cake

public class DotNetTestBuilder
{
  private ICakeContext context;

  private string configuration;

  private IEnumerable<string> projects;

  public DotNetTestBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
  }

  public DotNetTestBuilder WithConfiguration(string configuration)
  {
    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    this.configuration = configuration;
    return this;
  }

  public DotNetTestBuilder WithProjectGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.projects = this.context.GetFiles(glob).Select(p => p.ToString());
    return this;
  }

  public DotNetTestBuilder WithProjectGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.projects = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public DotNetTestBuilder WithProjects(IEnumerable<string> projects)
  {
    if (projects == null)
    {
      throw new ArgumentNullException(nameof(projects));
    }

    this.projects = projects;
    return this;
  }

  public DotNetTestCommand Build()
  {
    return new DotNetTestCommand(
      this.context,
      this.configuration,
      this.projects
    );
  }
}

public class DotNetTestCommand : ICommand
{
  private ICakeContext context;

  private string configuration;

  private IEnumerable<string> projects;

  public DotNetTestCommand(ICakeContext context, string configuration, IEnumerable<string> projects)
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
    this.projects = projects;
  }

  public void Execute()
  {
    if (this.projects == null)
    {
      return;
    }

    var assemblyPaths = this.projects.Select(project => {
      var projectFilePath = new FilePath(project);
      var projectName = projectFilePath.GetFilename();
      var projectFolder = projectFilePath.GetDirectory();

      var assemblyFolder = $"{projectFolder}/bin/{this.configuration}";

      var assemblyFile = projectName.ToString().Replace(".csproj", ".dll");
      return $"{assemblyFolder}/{assemblyFile}";
    });

    // TODO: make configurable
    var settings = new XUnit2Settings
    {
      ToolPath = "./tools/xunit.runner.console/xunit.runner.console/tools/net452/xunit.console.exe"
    };

    this.context.XUnit2(assemblyPaths, settings);
  }
}
