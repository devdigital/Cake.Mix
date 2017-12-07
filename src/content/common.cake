public interface ICommand
{
  void Execute();
}

public class AggregateCommand : ICommand
{
  private IEnumerable<ICommand> commands;
  
  public AggregateCommand(params ICommand[] commands)
  {
    if (commands == null)
    {
      throw new ArgumentNullException(nameof(commands));
    }
    
    this.commands = commands;
  }
  
  public void Execute()
  {
    foreach(var command in this.commands)
    {
      command.Execute();
    }
  }
}