using System.Text;

namespace Explory.Options;

public class ExploryOptions
{
    public static readonly string SectionName = "Explory";

    public string RootDir { get; set; } = string.Empty;
}