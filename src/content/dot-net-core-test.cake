#load common.cake

public class DotNetCoreTestBuilder
{
  private ICakeContext context;

  private string dotNetCorePath;

  private IEnumerable<string> projects;

  private string configuration;

  private string arguments;

  public DotNetCoreTestBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
  }

  public DotNetCoreTestBuilder WithDotNetCorePath(string path)
  {
    this.dotNetCorePath = path;
    return this;
  }

  public DotNetCoreTestBuilder WithConfiguration(string configuration)
  {
    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    this.configuration = configuration;
    return this;
  }

  public DotNetCoreTestBuilder WithProjectGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.projects = this.context.GetFiles(glob).Select(p => p.ToString());
    return this;
  }

  public DotNetCoreTestBuilder WithProjectGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.projects = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public DotNetCoreTestBuilder WithProjects(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.projects = this.context.GetFiles(glob).Select(p => p.ToString());
    return this;
  }

  public DotNetCoreTestBuilder WithArguments(string arguments)
  {
    if (string.IsNullOrWhiteSpace(arguments))
    {
      throw new ArgumentNullException(nameof(arguments));
    }

    this.arguments = arguments;
    return this;
  }

  public DotNetCoreTestCommand Build()
  {
    return new DotNetCoreTestCommand(
      this.context,
      this.dotNetCorePath,
      this.projects,
      this.configuration,
      this.arguments
    );
  }
}

public class DotNetCoreTestCommand : ICommand
{
  private ICakeContext context;

  private string dotNetCorePath;

  private IEnumerable<string> projects;

  private string configuration;

  private string arguments;

  public DotNetCoreTestCommand(ICakeContext context, string dotNetCorePath, IEnumerable<string> projects, string configuration, string arguments)
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
    this.dotNetCorePath = dotNetCorePath;
    this.projects = projects;
    this.configuration = configuration;
    this.arguments = arguments;
  }

  public void Execute()
  {
    if (this.projects == null)
    {
      return;
    }

    var settings = new DotNetCoreTestSettings()
    {
        Configuration = this.configuration,
        NoBuild = true
    };

    if (this.dotNetCorePath != null)
    {
      settings.ToolPath = this.dotNetCorePath;
    }

    if (this.arguments != null)
    {
      settings.ArgumentCustomization = args => args.Append(this.arguments);
    }

    // TODO: options for test results
    foreach(var project in this.projects)
    {
        this.context.DotNetCoreTest(
            project,
            settings);
    }
  }
}
