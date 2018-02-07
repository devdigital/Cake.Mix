#load common.cake

public class TfsBuildNumberBuilder
{
  private ICakeContext context;

  private string buildNumber;

  public TfsBuildNumberBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
  }

  public TfsBuildNumberBuilder WithBuildNumber(string buildNumber)
  {
    if (string.IsNullOrWhiteSpace(buildNumber))
    {
      throw new ArgumentNullException(nameof(buildNumber));
    }

    this.buildNumber = buildNumber;
    return this;
  }

  public TfsBuildNumberCommand Build()
  {
    return new TfsBuildNumberCommand(
      this.context,
      this.buildNumber
    );
  }
}

public class TfsBuildNumberCommand : ICommand
{
  private ICakeContext context;

  private string buildNumber;

  public TfsBuildNumberCommand(ICakeContext context, string buildNumber)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    if (string.IsNullOrWhiteSpace(buildNumber))
    {
      throw new ArgumentNullException(nameof(buildNumber));
    }

    this.context = context;
    this.buildNumber = buildNumber;
  }

  public void Execute()
  {
    this.context.Information($"##vso[build.updatebuildnumber]{this.buildNumber}");
  }
}
