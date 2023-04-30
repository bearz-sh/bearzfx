namespace Cocoa.Configuration.Sections;

// todo: #2565 retrofit other command configs this way
[Serializable]
public sealed class ListCommandConfiguration
{
    public ListCommandConfiguration()
    {
        this.PageSize = 25;
    }

    // list
    public bool LocalOnly { get; set; }

    public bool IdOnly { get; set; }

    public bool IncludeRegistryPrograms { get; set; }

    public int? Page { get; set; }

    public int PageSize { get; set; }

    public bool Exact { get; set; }

    public bool ByIdOnly { get; set; }

    public bool ByTagOnly { get; set; }

    public bool IdStartsWith { get; set; }

    public bool OrderByPopularity { get; set; }

    public bool ApprovedOnly { get; set; }

    public bool DownloadCacheAvailable { get; set; }

    public bool NotBroken { get; set; }

    public bool IncludeVersionOverrides { get; set; }

    public bool ExplicitPageSize { get; set; }
}