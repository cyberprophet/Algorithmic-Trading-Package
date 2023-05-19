using System.IO;
using System.Threading.Tasks;

namespace ShareInvest.Services;

class File
{
    public async Task<FileInfo> WriteAllBytesAsync(byte[] bytes)
    {
        await System.IO.File.WriteAllBytesAsync(path, bytes);

        return new FileInfo(path);
    }
    public File(string path)
    {
        if (Path.GetDirectoryName(path) is string directory)
        {
            DirectoryInfo di = new(directory);

            if (di.Exists is false)
                di.Create();
        }
        this.path = path;
    }
    readonly string path;
}