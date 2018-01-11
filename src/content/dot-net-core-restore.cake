#load common.cake

public class DotNetCoreRestoreBuilder
{
  private ICakeContext context;

  private string dotNetCorePath;

  private IEnumerable<string> projects;

  public DotNetCoreRestoreBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
  }

  public DotNetCoreRestoreBuilder WithDotNetCorePath(string path)
  {
    this.dotNetCorePath = path;
    return this;
  }

  public DotNetCoreRestoreBuilder WithProjectGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.projects = this.context.GetFiles(glob).Select(p => p.ToString());
    return this;
  }

  public DotNetCoreRestoreBuilder WithProjectGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.projects = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public DotNetCoreRestoreBuilder WithProjects(IEnumerable<string> projects)
  {
    if (projects == null)
    {
      throw new ArgumentNullException(nameof(projects));
    }

    this.projects = projects;
    return this;
  }

  public DotNetCoreRestoreCommand Build()
  {
    return new DotNetCoreRestoreCommand(
      this.context,
      this.dotNetCorePath,
      this.projects
    );
  }
}

public class DotNetCoreRestoreCommand : ICommand
{
  private ICakeContext context;

  private string dotNetCorePath;

  private IEnumerable<string> projects;

  public DotNetCoreRestoreCommand(ICakeContext context, string dotNetCorePath, IEnumerable<string> projects)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.dotNetCorePath = dotNetCorePath;
    this.projects = projects;
  }

  public void Execute()
  {
    if (this.projects == null)
    {
      return;
    }

    foreach (var project in this.projects)
    {
      if (string.IsNullOrWhiteSpace(this.dotNetCorePath))
      {
        this.context.DotNetCoreRestore(project);
        return;
      }

      this.context.DotNetCoreRestore(project, new DotNetCoreRestoreSettings
      {
        ToolPath = this.dotNetCorePath
      });
    }
  }
}
