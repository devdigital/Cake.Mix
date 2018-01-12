#load common.cake

public class DotNetIncludeFilesBuilder
{
  private ICakeContext context;

  private string project;

  private string files;

  private string destinationFolder;

  private string @namespace;

  public DotNetIncludeFilesBuilder(ICakeContext context)
  {
   if (context == null)
   {
     throw new ArgumentNullException(nameof(context));
   }

   this.context = context;
   this.@namespace = "http://schemas.microsoft.com/developer/msbuild/2003";
  }

  public DotNetIncludeFilesBuilder WithProject(string project)
  {
    if (string.IsNullOrWhiteSpace(project))
    {
      throw new ArgumentNullException(nameof(project));
    }

    this.project = project;
    return this;
  }

  public DotNetIncludeFilesBuilder WithFilesGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.files = glob;
    return this;
  }

  public DotNetIncludeFilesBuilder WithDestinationFolder(string folder)
  {
    if (string.IsNullOrWhiteSpace(folder))
    {
      throw new ArgumentNullException(nameof(folder));
    }

    this.destinationFolder = folder;
    return this;
  }

  public DotNetIncludeFilesBuilder WithNamespace(string @namespace)
  {
    if (string.IsNullOrWhiteSpace(@namespace))
    {
      throw new ArgumentNullException(nameof(@namespace));
    }

    this.@namespace = @namespace;
    return this;
  }

  public DotNetIncludeFilesCommand Build()
  {
    return new DotNetIncludeFilesCommand(
      this.context,
      this.project,
      this.files,
      this.destinationFolder,
      this.@namespace
    );
  }
}

public class DotNetIncludeFilesCommand : ICommand
{
  private ICakeContext context;

  private string project;

  private string files;

  private string destinationFolder;

  private string @namespace;

  public DotNetIncludeFilesCommand(
    ICakeContext context,
    string project,
    string files,
    string destinationFolder,
    string @namespace)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(project))
    {
      throw new ArgumentNullException(nameof(project));
    }

    if (string.IsNullOrWhiteSpace(files))
    {
      throw new ArgumentNullException(nameof(files));
    }

    if (string.IsNullOrWhiteSpace(destinationFolder))
    {
      throw new ArgumentNullException(nameof(destinationFolder));
    }

    if (string.IsNullOrWhiteSpace(@namespace))
    {
      throw new ArgumentNullException(nameof(@namespace));
    }

    this.context = context;
    this.project = project;
    this.files = files;
    this.destinationFolder = destinationFolder;
    this.@namespace = @namespace;
  }

  public void Execute()
  {
    var copyAllNode = System.Xml.Linq.XDocument.Parse(@"
      <PropertyGroup>
          <CopyAllFilesToSingleFolderForPackageDependsOn>
              $(CopyAllFilesToSingleFolderForPackageDependsOn);
              CustomCollectFiles;
          </CopyAllFilesToSingleFolderForPackageDependsOn>
          <CopyAllFilesToSingleFolderForMsdeployDependsOn>
              $(CopyAllFilesToSingleFolderForMsdeployDependsOn);
              CustomCollectFiles;
          </CopyAllFilesToSingleFolderForMsdeployDependsOn>
      </PropertyGroup>");

    this.AddNamespace(copyAllNode.Root, this.@namespace);

    var customCollectNode = System.Xml.Linq.XDocument.Parse($@"
      <Target Name=""CustomCollectFiles"">
        <ItemGroup>
          <_CustomFiles Include=""{this.files}"" />
          <FilesForPackagingFromProject Include=""%(_CustomFiles.Identity)"">
            <DestinationRelativePath>{this.destinationFolder}\%(RecursiveDir)%(Filename)%(Extension)</DestinationRelativePath>
          </FilesForPackagingFromProject>
        </ItemGroup>
      </Target>");

    this.AddNamespace(customCollectNode.Root, this.@namespace);

    var document = System.Xml.Linq.XDocument.Load(this.project);
    document.Root.Add(copyAllNode.Root);
    document.Root.Add(customCollectNode.Root);

    document.Save(this.project);
  }

  private void AddNamespace(System.Xml.Linq.XElement element, string @namespace)
  {
    System.Xml.Linq.XNamespace ns = @namespace;

    foreach (var descendant in element.DescendantsAndSelf())
    {
      descendant.Name = ns + descendant.Name.LocalName;
    }
  }
}
