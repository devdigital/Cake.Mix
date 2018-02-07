#load common.cake

public class TfsPublishArtifactBuilder
{
  private ICakeContext context;

  private string containerFolder;

  private string artifactName;

  private string artifactLocation;

  public TfsPublishArtifactBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
  }

  public TfsPublishArtifactBuilder WithContainerFolder(string containerFolder)
  {
    if (string.IsNullOrWhiteSpace(containerFolder))
    {
      throw new ArgumentNullException(nameof(containerFolder));
    }

    this.containerFolder = containerFolder;
    return this;
  }

  public TfsPublishArtifactBuilder WithArtifactName(string artifactName)
  {
    if (string.IsNullOrWhiteSpace(artifactName))
    {
      throw new ArgumentNullException(nameof(artifactName));
    }

    this.artifactName = artifactName;
    return this;
  }

  public TfsPublishArtifactBuilder WithArtifactLocation(string artifactLocation)
  {
    if (string.IsNullOrWhiteSpace(artifactLocation))
    {
      throw new ArgumentNullException(nameof(artifactLocation));
    }

    this.artifactLocation = artifactLocation;
    return this;
  }

  public TfsPublishArtifactCommand Build()
  {
    return new TfsPublishArtifactCommand(
      this.context,
      this.containerFolder,
      this.artifactName,
      this.artifactLocation
    );
  }
}

public class TfsPublishArtifactCommand : ICommand
{
  private ICakeContext context;

  private string containerFolder;

  private string artifactName;

  private string artifactLocation;

  public TfsPublishArtifactCommand(ICakeContext context, string containerFolder, string artifactName, string artifactLocation)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(containerFolder))
    {
      throw new ArgumentNullException(nameof(containerFolder));
    }

    if (string.IsNullOrWhiteSpace(artifactName))
    {
      throw new ArgumentNullException(nameof(artifactName));
    }

    if (string.IsNullOrWhiteSpace(artifactLocation))
    {
      throw new ArgumentNullException(nameof(artifactLocation));
    }

    this.context = context;
    this.containerFolder = containerFolder;
    this.artifactName = artifactName;
    this.artifactLocation = artifactLocation;
  }

  public void Execute()
  {
    this.context.Information($"##vso[artifact.upload containerfolder={this.containerFolder};artifactname={this.artifactName};]{this.artifactLocation}");
  }
}
