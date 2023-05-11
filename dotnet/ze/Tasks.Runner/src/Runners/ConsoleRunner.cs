namespace Ze.Tasks.Runners;

public class ConsoleRunner
{
    private readonly IServiceProvider services;

    public ConsoleRunner(IServiceProvider services)
    {
        this.services = services;
    }
}