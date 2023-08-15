using System.Net;
using System.Net.Mime;
using Explory.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Explory.Pages;

public class IndexModel : PageModel
{
    public List<DirectoryInfo> Directories { get; set; } = new ();
    public List<FileInfo> Files { get; set; } = new ();
    public string Path { get; set; } = string.Empty;
    public string ParentLink { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    
    private readonly ILogger<IndexModel> _logger;
    private readonly ExploryOptions _options;

    public IndexModel
    (
        ILogger<IndexModel> logger,
        IOptions<ExploryOptions> options
    )
    {
        _logger = logger;
        _options = options.Value;
    }

    public IActionResult  OnGet()
    {
        try
        {
            var path = $"{_options.RootDir}{Request.Path}";
            var physicalPath = WebUtility.UrlDecode(path);
            if (System.IO.File.Exists(physicalPath))
            {
                var file = new FileInfo(physicalPath);
                return File(file.OpenRead(), MediaTypeNames.Application.Octet, file.Name);
            }

            var directoryInfo = new DirectoryInfo(WebUtility.UrlDecode(path));
            FullPath = path;
            Directories.AddRange(directoryInfo.GetDirectories());
            Files.AddRange(directoryInfo.GetFiles());
            Path = Request.Path;
            ParentLink = GetParentLink();
            return Page();
        }
        catch (Exception e)
        {
            if (e is DirectoryNotFoundException || e is FileNotFoundException)
                return NotFound();

            throw;
        }
    }

    private string GetParentLink()
    {
        var splitPath = Request.Path.Value.Split("/");

        if (splitPath[1] == string.Empty)
            return string.Empty;

        if (splitPath.Length == 2)
            return "/";
        
        return string.Join("/", splitPath[..^1]);
    }
}