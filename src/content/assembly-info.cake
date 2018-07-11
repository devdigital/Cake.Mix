#addin nuget:?package=Cake.FileHelpers&version=3.0.0
#load common.cake

public class AssemblyInfoUpdaterBuilder
{
  private ICakeContext context;

  private IEnumerable<string> fileGlobs;

  private string version;

  public AssemblyInfoUpdaterBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
  }

  public AssemblyInfoUpdaterBuilder WithFileGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.fileGlobs = new List<string> { glob };
    return this;
  }

  public AssemblyInfoUpdaterBuilder WithFileGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.fileGlobs = globs;
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
    return new AssemblyInfoUpdater(
      this.context,
      this.fileGlobs,
      this.version);
  }
}

public class AssemblyInfoUpdater : ICommand
{
  private ICakeContext context;

  private IEnumerable<string> fileGlobs;

  private string version;

  public AssemblyInfoUpdater(ICakeContext context, IEnumerable<string> fileGlobs, string version)
  {
    if (context == null)
    {
      throw new ArgumentNullException("context");
    }

    if (string.IsNullOrWhiteSpace(version))
    {
      throw new ArgumentNullException("version");
    }

    this.context = context;
    this.fileGlobs = fileGlobs;
    this.version = version;
  }

  public void Execute()
  {
    if (this.fileGlobs == null)
    {
      return;
    }

    foreach (var fileGlob in this.fileGlobs)
    {
      this.context.ReplaceRegexInFiles(
        fileGlob,
        "(?<=AssemblyVersion\\(\")(.+?)(?=\"\\))",
        this.version);

      this.context.ReplaceRegexInFiles(
        fileGlob,
        "(?<=AssemblyFileVersion\\(\")(.+?)(?=\"\\))",
        this.version);
    }
  }
}
