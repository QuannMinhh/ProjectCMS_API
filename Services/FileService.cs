using Microsoft.Data.SqlClient;
using System.Data;
using System.IO.Compression;

namespace ProjectCMS.Services
{
    public class FileService
    {
        private IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public FileService(IWebHostEnvironment env, IConfiguration config) 
        {
            _env = env;
            _config = config;
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
            var sqlConnect = _config.GetConnectionString("DefaultConnection");
            using (SqlConnection cnt = new SqlConnection(sqlConnect))
            {
                cnt.Open();

                return CreateCSV(new SqlCommand("select * from _events", cnt).ExecuteReader()) ;
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
        public  int ExportTablesToCSV(string query,string tableName)
        {
            string[] tablesToSkip = { };
            using var connection = new SqlConnection("Server=DESKTOP-NPP0M4V\\HOANG;Database=Project;Trusted_Connection = True;MultipleActiveResultSets = True; TrustServerCertificate = True;Integrated Security=True");
            connection.Open();
            //string query = "select UserId,UserName,Email,Phone,DoB,Address,AddedDate,Role,Status,_departments.Name from _users Join _departments on _users.DepartmentID = _departments.DepId";
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            // Create a DataTable to hold the results of the query
            var dataTable = new DataTable();
            dataTable.Load(reader);

            // Write the results to a CSV file
            using var writer = new StreamWriter(tableName+".csv");
            var header = string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName));
            writer.WriteLine(header);

            foreach (DataRow row in dataTable.Rows)
            {
                var values = row.ItemArray.Select(value => value.ToString()).ToArray();
                var line = string.Join(",", values);
                writer.WriteLine(line);
            }

            return 0;
        }

    }
}
