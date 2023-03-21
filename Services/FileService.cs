using System.IO.Compression;

namespace ProjectCMS.Services
{
    public class FileService
    {
        private IWebHostEnvironment _env;   
        public FileService(IWebHostEnvironment env) 
        {
            _env = env;
        }

        public (string filtType, byte[] archivedate,string achiveName ) DownloadZip(string directoryName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", directoryName);
            var fileName = "Idea.Zip";
            var files = Directory.GetFiles(path);
            using (var memoryStream = new MemoryStream())
            {
                using(var achive = new ZipArchive(memoryStream,ZipArchiveMode.Create)) 
                {
                    foreach(var file in files) 
                    {
                        achive.CreateEntryFromFile(file,Path.GetFileName(file));
                    }
                }
                return ("aplication/zip", memoryStream.ToArray(),fileName);
            }
        }
    }
}
