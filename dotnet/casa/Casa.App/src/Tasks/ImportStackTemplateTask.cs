using Bearz.Std;

namespace Bearz.Casa.App.Tasks;

public class ImportStackTemplateTask
{
    public List<string> Templates { get; set; } = new List<string>();

    public bool Overwrite { get; set; }

    public Task<IReadOnlyList<ImportResult>> RunAsync(CancellationToken cancellationToken)
    {
        var results = new List<ImportResult>();
        foreach (var template in this.Templates)
        {
            var result = new ImportResult()
            {
                TemplatePath = template,
            };

            var templateName = FsPath.Basename(template);
            var dest = Path.Join(Paths.TemplatesDirectory, templateName);
            if (!this.Overwrite && Fs.DirectoryExists(dest))
            {
                result.AlreadyImported = true;
                results.Add(result);
                continue;
            }

            Fs.CopyDirectory(template, Paths.TemplatesDirectory, true, true);
            result.Imported = true;
        }

        return Task.FromResult((IReadOnlyList<ImportResult>)results);
    }
}

public class ImportResult
{
    public string TemplatePath { get; set; } = string.Empty;

    public bool AlreadyImported { get; set; }

    public bool Imported { get; set; }
}