using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text;

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
        public int  CreateCSV()
        {
            
            StringBuilder sb = new StringBuilder();

            using (SqlConnection cnt = new SqlConnection("Server=DESKTOP-NPP0M4V\\HOANG;Database=Project;Trusted_Connection = True;MultipleActiveResultSets = True; TrustServerCertificate = True;Integrated Security=True"))
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from _events; select * from _departments;select * from _users;select * from _categories;select * from _comments;select * from _idea;select * from _interactions", cnt);
                DataSet ds = new DataSet();
                da.Fill(ds);

                ds.Tables[0].TableName = "Events";
                ds.Tables[1].TableName = "Deparments";
                ds.Tables[2].TableName = "Users";
                ds.Tables[3].TableName = "Categories";
                ds.Tables[4].TableName = "Comments";
                ds.Tables[5].TableName = "Ideas";
                ds.Tables[6].TableName = "Interactions";
                sb.Append("Events\r\n");
                sb.Append("ID" + ',');
                sb.Append("Name" + ',');
                sb.Append("First_Closure" + ',');
                sb.Append("Last_Closure" + ',');
                sb.Append("\r\n");
                foreach (DataRow evt in ds.Tables["Events"].Rows)
                {
                    int evtID = Convert.ToInt32(evt["Id"]);
                    sb.Append(evtID.ToString() + ',');
                    sb.Append(evt["name"].ToString() + ',');
                    sb.Append(evt["First_closure"].ToString() + ',');
                    sb.Append(evt["Last_closure"].ToString() + ',');
                    sb.Append("\r\n");
                }
                sb.Append("\r\n");
                sb.Append("Department\r\n");
                sb.Append("ID" + ',');
                sb.Append("Name" + ',');
                sb.Append("\r\n");
                foreach (DataRow dep in ds.Tables["Deparments"].Rows)
                {
                    int DepId = Convert.ToInt32(dep["DepId"]);
                    sb.Append(DepId.ToString() + ',');
                    sb.Append(dep["name"].ToString() + ',');
                    sb.Append("\r\n");
                }
                sb.Append("\r\n");
                sb.Append("Users\r\n");
                sb.Append("ID" + ',');
                sb.Append("Username" + ',');
                sb.Append("Email" + ',');
                sb.Append("Phone" + ',');
                sb.Append("DoB" + ',');
                sb.Append("Address" + ',');
                sb.Append("Avatar" + ',');
                sb.Append("AddedDate" + ',');
                sb.Append("Role" + ',');
                sb.Append("Department" + ',');
                sb.Append("\r\n");
                foreach (DataRow usr in ds.Tables["Users"].Rows)
                {
                    int uID = Convert.ToInt32(usr["UserId"]);
                    sb.Append(uID.ToString() + ',');
                    sb.Append(usr["UserName"].ToString() + ',');
                    sb.Append(usr["Email"].ToString() + ',');
                    sb.Append(usr["Phone"].ToString() + ',');
                    sb.Append(usr["DoB"].ToString() + ',');
                    sb.Append(usr["Address"].ToString() + ',');
                    sb.Append(usr["Avatar"].ToString() + ',');
                    sb.Append(usr["AddedDate"].ToString() + ',');
                    sb.Append(usr["Role"].ToString() + ',');
                    sb.Append(usr["DepartmentID"].ToString() + ',');
                    sb.Append("\r\n");
                }
                sb.Append("\r\n");
                sb.Append("Categories\r\n");
                sb.Append("ID" + ',');
                sb.Append("Name" + ',');
                sb.Append("Content" + ',');
                sb.Append("AddedDate" + ',');
                sb.Append("\r\n");
                foreach (DataRow cate in ds.Tables["Categories"].Rows)
                {
                    int cateId = Convert.ToInt32(cate["Id"]);
                    sb.Append(cateId.ToString() + ',');
                    sb.Append(cate["name"].ToString() + ',');
                    sb.Append(cate["Content"].ToString() + ',');
                    sb.Append(cate["AddedDate"].ToString() + ',');
                    sb.Append("\r\n");
                }
                sb.Append("\r\n");
                sb.Append("Comments\r\n");
                sb.Append("ID" + ',');
                sb.Append("UserId" + ',');
                sb.Append("IdeaId" + ',');
                sb.Append("AddedDate" + ',');
                sb.Append("Content" + ',');
                sb.Append("\r\n");
                foreach (DataRow cmt in ds.Tables["Comments"].Rows)
                {
                    int cmtId = Convert.ToInt32(cmt["CommentId"]);
                    sb.Append(cmtId.ToString() + ',');
                    sb.Append(cmt["UserId"].ToString() + ',');
                    sb.Append(cmt["IdeaId"].ToString() + ',');
                    sb.Append(cmt["AddedDate"].ToString() + ',');
                    sb.Append(cmt["Content"].ToString() + ',');
                    sb.Append("\r\n");
                }
                sb.Append("\r\n");
                sb.Append("Ideas\r\n");
                sb.Append("ID" + ',');
                sb.Append("Name" + ',');
                sb.Append("Content" + ',');
                sb.Append("AddedDate" + ',');
                sb.Append("Vote" + ',');
                sb.Append("Viewed" + ',');
                sb.Append("IdeaFile" + ',');
                sb.Append("Event ID" + ',');
                sb.Append("Category ID" + ',');
                sb.Append("User ID" + ',');
                sb.Append("\r\n");
                foreach (DataRow idea in ds.Tables["Ideas"].Rows)
                {
                    int iId = Convert.ToInt32(idea["Id"]);
                    sb.Append(iId.ToString() + ',');
                    sb.Append(idea["Name"].ToString() + ',');
                    sb.Append(idea["Content"].ToString() + ',');
                    sb.Append(idea["AddedDate"].ToString() + ',');
                    sb.Append(idea["Vote"].ToString() + ',');
                    sb.Append(idea["Viewed"].ToString() + ',');
                    sb.Append(idea["IdeaFile"].ToString() + ',');
                    sb.Append(idea["EvId"].ToString() + ',');
                    sb.Append(idea["CateId"].ToString() + ',');
                    sb.Append(idea["UserId"].ToString() + ',');
                    sb.Append("\r\n");
                }
                sb.Append("\r\n");
                sb.Append("Interactions\r\n");
                sb.Append("ID" + ',');
                sb.Append("UserID" + ',');
                sb.Append("IdeaID" + ',');
                sb.Append("Voted" + ',');
                sb.Append("Viewed" + ',');
                sb.Append("Vote" + ',');
                sb.Append("\r\n");
                foreach (DataRow inter in ds.Tables["Interactions"].Rows)
                {
                    int interactionId = Convert.ToInt32(inter["InteracId"]);
                    sb.Append(interactionId.ToString() + ',');
                    sb.Append(inter["UserId"].ToString() + ',');
                    sb.Append(inter["IdeaId"].ToString() + ',');
                    sb.Append(inter["Voted"].ToString() + ',');
                    sb.Append(inter["Viewed"].ToString() + ',');
                    sb.Append(inter["Vote"].ToString() + ',');
                    sb.Append("\r\n");
                }
            }
            StreamWriter file = new StreamWriter("C:\\Users\\hoang\\Desktop\\DataTest.csv");
            file.WriteLine(sb.ToString());
            file.Close();
            return 0;
        }

        public int GPT()
        {
            string connectionString = "Server=DESKTOP-NPP0M4V\\HOANG;Database=Project;Trusted_Connection = True;MultipleActiveResultSets = True; TrustServerCertificate = True;Integrated Security=True";

            Dictionary<string, List<object>> tableData = new Dictionary<string, List<object>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve all table names from the database
                DataTable tables = connection.GetSchema("Tables");

                // Loop through each table and retrieve its data
                foreach (DataRow table in tables.Rows)
                {
                    string tableName = (string)table[2];

                    if (tableName == "__EFMigrationsHistory")
                    {
                        continue; // skip this table                        
                    }
                    // Retrieve the data from the table
                    string query = $"SELECT * FROM {tableName}";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {                    
                            List<object> rowData = new List<object>();

                            // Read each row of data and add it to the list
                            while (reader.Read())
                            {
                                object[] values = new object[reader.FieldCount];
                                reader.GetValues(values);
                                rowData.AddRange(values);
                            }

                            // Add the row data to the table data dictionary
                            tableData.Add(tableName, rowData);
                        
                    }
                }

                // Write the table data to the CSV file
                using (StreamWriter writer = new StreamWriter("output.csv"))
                using (CsvWriter csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    // Write the header row with the table names
                    csv.WriteField("");
                    foreach (string tableName in tableData.Keys)
                    {
                        csv.WriteField(tableName);
                    }
                    csv.NextRecord();

                    // Write the data rows with the table data
                    int maxRows = tableData.Values.Max(l => l.Count);
                    for (int i = 0; i < maxRows; i++)
                    {
                        csv.WriteField(i + 1);

                        foreach (string tableName in tableData.Keys)
                        {
                            if (i < tableData[tableName].Count)
                            {
                                csv.WriteField(tableData[tableName][i]);
                            }
                            else
                            {
                                csv.WriteField("");
                            }
                        }

                        csv.NextRecord();
                    }
                }
            }
            return 0;
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
