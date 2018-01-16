#tool "nuget:?package=xunit.runner.console"
#load common.cake

public class XUnit2TestBuilder
{
  private ICakeContext context;

  private string configuration;

  private IEnumerable<string> projects;

  private bool xmlOutput;

  private string xmlOutputFolder;

  public XUnit2TestBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.configuration = "Release";
  }

  public XUnit2TestBuilder WithConfiguration(string configuration)
  {
    if (string.IsNullOrWhiteSpace(configuration))
    {
      throw new ArgumentNullException(nameof(configuration));
    }

    this.configuration = configuration;
    return this;
  }

  public XUnit2TestBuilder WithProjectGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.projects = this.context.GetFiles(glob).Select(p => p.ToString());
    return this;
  }

  public XUnit2TestBuilder WithProjectGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.projects = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public XUnit2TestBuilder WithProjects(IEnumerable<string> projects)
  {
    if (projects == null)
    {
      throw new ArgumentNullException(nameof(projects));
    }

    this.projects = projects;
    return this;
  }

  public XUnit2TestBuilder WithXmlOutput()
  {
    this.xmlOutput = true;
    return this;
  }

  public XUnit2TestBuilder WithXmlOutputFolder(string folder)
  {
    if (string.IsNullOrWhiteSpace(folder))
    {
      throw new ArgumentNullException(nameof(folder));
    }

    this.xmlOutputFolder = folder;
    return this;
  }

  public XUnit2TestCommand Build()
  {
    return new XUnit2TestCommand(
      this.context,
      this.configuration,
      this.projects,
      this.xmlOutput,
      this.xmlOutputFolder
    );
  }
}

public class XUnit2TestCommand : ICommand
{
  private ICakeContext context;

  private string configuration;

  private IEnumerable<string> projects;

  private bool xmlOutput;

  private string xmlOutputFolder;

  public XUnit2TestCommand(
    ICakeContext context,
    string configuration,
    IEnumerable<string> projects,
    bool xmlOutput,
    string xmlOutputFolder)
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
    this.xmlOutput = xmlOutput;
    this.xmlOutputFolder = xmlOutputFolder;
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
      ToolPath = "./tools/xunit.runner.console/xunit.runner.console/tools/net452/xunit.console.exe",
      XmlReport = this.xmlOutput
    };

    if (!string.IsNullOrWhiteSpace(this.xmlOutputFolder))
    {
      settings.OutputDirectory = this.xmlOutputFolder;
    }

    this.context.XUnit2(assemblyPaths, settings);
  }
}
