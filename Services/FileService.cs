using Microsoft.Data.SqlClient;
using System.Data;
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
            var fileName = directoryName+ ".Zip";
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
        public string GetCSV()
        {
            using (SqlConnection cnt = new SqlConnection("Server=DESKTOP-NPP0M4V\\HOANG;Database=ProjectCMSAPI;Trusted_Connection = True;MultipleActiveResultSets = True; TrustServerCertificate = True;Integrated Security=True"))
            {
                cnt.Open();
                return CreateCSV(new SqlCommand("Select * from _events",cnt).ExecuteReader());
            }    
        }
        public string CreateCSV(IDataReader reader)
        {
            string file = "C:\\Users\\hoang\\Desktop\\Data.csv";
            List<string> lines = new();
            string headerLine = "";
            if(reader.Read())
            {
                string[] columns = new string[reader.FieldCount];
                for(int i=0;i<reader.FieldCount;i++)
                {
                    columns[i] = reader.GetName(i);
                }
                headerLine = string.Join(",", columns);
                lines.Add(headerLine);
            }    
            while(reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);
                lines.Add(string.Join(",",values));
            }    
            System.IO.File.WriteAllLinesAsync(file, lines);
            return file;
        }
        public string connectionString()
        {
            return @"Server=DESKTOP-NPP0M4V\\HOANG;Database=ProjectCMSAPI;Trusted_Connection = True;MultipleActiveResultSets = True; TrustServerCertificate = True;Integrated Security=True";
        }
    }
}
