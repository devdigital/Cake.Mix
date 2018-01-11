#load common.cake

public class DotNetRestoreBuilder
{
  private ICakeContext context;

  private IEnumerable<string> solutions;

  public DotNetRestoreBuilder(ICakeContext context)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
  }

  public DotNetRestoreBuilder WithSolutionGlob(string glob)
  {
    if (string.IsNullOrWhiteSpace(glob))
    {
      throw new ArgumentNullException(nameof(glob));
    }

    this.solutions = this.context.GetFiles(glob).Select(p => p.ToString());
    return this;
  }

  public DotNetRestoreBuilder WithSolutionGlobs(IEnumerable<string> globs)
  {
    if (globs == null)
    {
      throw new ArgumentNullException(nameof(globs));
    }

    this.solutions = globs.SelectMany(g => this.context.GetFiles(g).Select(p => p.ToString()));
    return this;
  }

  public DotNetRestoreBuilder WithSolutions(IEnumerable<string> solutions)
  {
    if (solutions == null)
    {
      throw new ArgumentNullException(nameof(solutions));
    }

    this.solutions = solutions;
    return this;
  }

  public DotNetRestoreCommand Build()
  {
    return new DotNetRestoreCommand(
      this.context,
      this.solutions
    );
  }
}

public class DotNetRestoreCommand : ICommand
{
  private ICakeContext context;

  private IEnumerable<string> solutions;

  public DotNetRestoreCommand(ICakeContext context, IEnumerable<string> solutions)
  {
    if (context == null)
    {
      throw new ArgumentNullException(nameof(context));
    }

    this.context = context;
    this.solutions = solutions;
  }

  public void Execute()
  {
    if (this.solutions == null)
    {
      return;
    }

    foreach (var solution in this.solutions)
    {
      this.context.NuGetRestore(solution);
    }
  }
}
