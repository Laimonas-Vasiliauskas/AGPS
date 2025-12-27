using AGPS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AGPS.Repositories
{
    public class ProjectRepository
    {
        private readonly string connectionString;
        public ProjectRepository()
        {
            string raw = ConfigurationManager.ConnectionStrings["AGPStestDB"].ConnectionString;

            string pwd = Environment.GetEnvironmentVariable("AGPSDB_PASSWORD");

            connectionString = raw.Replace("{PWD}", pwd);
        }
        public List<Project> GetProjectsWithParts()
        {
            var projects = new Dictionary<int, Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    p.id AS ProjectId,
                    p.projectname,
                    pa.id AS PartId,
                    pa.project_id,
                    pa.partname,
                    pa.madeby,
                    pa.typeofwork,
                    pa.created_at,
                    pa.comments,
                    pa.remaining,
                    pa.done
                FROM projects p
                LEFT JOIN parts pa ON pa.project_id = p.id
                ORDER BY p.id DESC";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int projectId = reader.GetInt32(reader.GetOrdinal("ProjectId"));

                            // create project once
                            if (!projects.TryGetValue(projectId, out Project project))
                            {
                                project = new Project
                                {
                                    id = projectId,
                                    projectname = reader.IsDBNull(reader.GetOrdinal("projectname")) ? string.Empty : reader.GetString(reader.GetOrdinal("projectname"))
                                };
                                projects.Add(projectId, project);
                            }

                            // add part if exists
                            if (!reader.IsDBNull(reader.GetOrdinal("PartId")))
                            {
                                var part = new Part
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("PartId")),
                                    project_id = reader["project_id"] != DBNull.Value ? Convert.ToInt32(reader["project_id"]) : 0,
                                    partname = reader["partname"] != DBNull.Value ? Convert.ToString(reader["partname"]) : string.Empty,
                                    madeby = reader["madeby"] != DBNull.Value ? Convert.ToString(reader["madeby"]) : string.Empty,
                                    typeofwork = reader["typeofwork"] != DBNull.Value ? Convert.ToString(reader["typeofwork"]) : string.Empty,
                                    created_at = reader["created_at"] != DBNull.Value ? Convert.ToDateTime(reader["created_at"]) : default(DateTime),
                                    comments = reader["comments"] != DBNull.Value ? Convert.ToString(reader["comments"]) : string.Empty,
                                    remaining = reader["remaining"] != DBNull.Value ? Convert.ToInt32(reader["remaining"]) : 0,
                                    done = reader["done"] != DBNull.Value ? Convert.ToInt32(reader["done"]) : 0
                                };

                                project.Parts.Add(part);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving projects with parts: " + ex.Message);
            }

            return projects.Values.ToList();
        }

        public List<Part> GetPartsByProjectId(int project_id)
        {
            var parts = new List<Part>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM parts WHERE project_id = @project_id ORDER BY id DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@project_id", project_id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Part part = new Part();

                                part.id = Convert.ToInt32(reader["id"]);
                                part.project_id = reader["project_id"] != DBNull.Value ? Convert.ToInt32(reader["project_id"]) : 0;
                                part.partname = Convert.ToString(reader["partname"]);
                                part.madeby = Convert.ToString(reader["madeby"]);
                                part.typeofwork = Convert.ToString(reader["typeofwork"]);
                                part.created_at = Convert.ToDateTime(reader["created_at"]);
                                part.comments = Convert.ToString(reader["comments"]);
                                part.remaining = reader["remaining"] != DBNull.Value ? Convert.ToInt32(reader["remaining"]) : 0;
                                part.done = reader["done"] != DBNull.Value ? Convert.ToInt32(reader["done"]) : 0;

                                parts.Add(part);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving parts by project id: " + ex.Message);
            }

            return parts;
        }

        public int GetOrCreateWorkerRowId(string projectName, string partName, string madeBy, string typeOfWork, string comments = "")
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 0) Find project id by name
                int project_id = 0;
                string projSql = "SELECT TOP 1 id FROM projects WHERE projectname = @p ORDER BY id DESC";
                using (var cmd = new SqlCommand(projSql, conn))
                {
                    cmd.Parameters.AddWithValue("@p", projectName);
                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                        project_id = Convert.ToInt32(res);
                }

                if (project_id == 0)
                {
                    // project not found - caller should ensure projects exist
                    throw new InvalidOperationException($"Project '{projectName}' not found.");
                }

                // 1) Ar darbuotojo eilutė jau egzistuoja?
                string findSql = @"SELECT TOP 1 id
                           FROM parts
                           WHERE project_id = @project_id AND partname = @part AND madeby = @madeby
                           ORDER BY id DESC;";

                using (var cmd = new SqlCommand(findSql, conn))
                {
                    cmd.Parameters.AddWithValue("@project_id", project_id);
                    cmd.Parameters.AddWithValue("@part", partName);
                    cmd.Parameters.AddWithValue("@madeby", madeBy);

                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                        return Convert.ToInt32(res);
                }

                // 2) Nerasta -> paimam bendrą remaining
                int remaining = 0;
                string remSql = @"SELECT TOP 1 remaining
                          FROM parts
                          WHERE project_id = @project_id AND partname = @part
                          ORDER BY id DESC;";

                using (var cmd = new SqlCommand(remSql, conn))
                {
                    cmd.Parameters.AddWithValue("@project_id", project_id);
                    cmd.Parameters.AddWithValue("@part", partName);

                    var res = cmd.ExecuteScalar();
                    remaining = (res == null || res == DBNull.Value) ? 0 : Convert.ToInt32(res);
                }

                // 3) Sukuriam naują darbuotojo eilutę (done=0, remaining=bendras)
                //    Svarbu: SCOPE_IDENTITY() veikia su triggeriais
                string insertSql = @"INSERT INTO parts (project_id, partname, madeby, typeofwork, created_at, comments, remaining, done)
                                    VALUES (@project_id, @part, @madeby, @typeofwork, GETDATE(), @comments, @remaining, 0);
                                    SELECT CAST(SCOPE_IDENTITY() AS int);";
                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@project_id", project_id);
                    cmd.Parameters.AddWithValue("@part", partName ?? string.Empty);
                    cmd.Parameters.AddWithValue("@madeby", madeBy ?? string.Empty);
                    cmd.Parameters.AddWithValue("@typeofwork", typeOfWork ?? string.Empty);
                    cmd.Parameters.AddWithValue("@comments", comments ?? string.Empty);
                    cmd.Parameters.AddWithValue("@remaining", remaining);

                    var newId = cmd.ExecuteScalar();
                    return (newId != null && newId != DBNull.Value) ? Convert.ToInt32(newId) : 0;
                }
            }
        }


        public void AddWork(int id, int doneDelta)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(@"
    UPDATE parts SET done = done + @doneDelta WHERE id = @id;
    UPDATE parts SET remaining = remaining - @doneDelta WHERE project_id = 
        (SELECT project_id FROM parts WHERE id = @id) AND partname = 
        (SELECT partname FROM parts WHERE id = @id);
", conn))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    cmd.Parameters.Add("@doneDelta", SqlDbType.Int).Value = doneDelta;
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
