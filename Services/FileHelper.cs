using System.IO.Compression;

namespace quizz.Services;
public class FileHelper : IFileHelper
{
    public async ValueTask<bool> ValidateTestCaseAsync(IFormFile file)
    {
        if(file.Length < 1)
            return false;

        using var fileStream = new MemoryStream();
        
        await file.CopyToAsync(fileStream);

        using var zipArchive = new ZipArchive(fileStream);
        
        if(zipArchive.Entries.Count() < 2)
            return false;
        
        var inFiles = zipArchive.Entries.Where(i => i.FullName.EndsWith(".in", StringComparison.OrdinalIgnoreCase)).Select(f => f.Name);
        var outFiles = zipArchive.Entries.Where(i => i.FullName.EndsWith(".out", StringComparison.OrdinalIgnoreCase)).Select(f => f.Name);

        if(inFiles.Count() != outFiles.Count())
            return false;
        
        if(inFiles.Except(outFiles).Count() > 0
        || outFiles.Except(inFiles).Count() > 0)
            return false;

        return true;
    }
    public async  ValueTask<string?> WriteTestCaseAsync(IFormFile file, ulong questionId)
    {
        var filename = string.Format("'yyyy'-'MM'-'dd'-'hh'-'mm'-'ss'-'zzz'", DateTime.UtcNow);
        filename += $"-{questionId}.zip";

        var filePath = Path.Combine(TestCaseFolder, filename);

        using var fileStream = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.Write);
        await file.CopyToAsync(fileStream);

        return filename;
    }
    public async ValueTask<FileStream?> GetTestCaseAsync(string filename)
    {
        var filePath = Path.Combine(TestCaseFolder, filename);

        if(!File.Exists(filePath)) 
            return null;
        
        using var file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    
        return file;
    }
    public static  IEnumerable<string>? GetFilesInZip(FileStream stream)
    {
        using var zipArchive = new ZipArchive(stream);
        return zipArchive.Entries?.Select(e => e.FullName);
    }
    private static string TestCaseFolder => Path.Combine(Directory.GetCurrentDirectory(), "data/testcases");
}