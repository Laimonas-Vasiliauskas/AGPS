using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AGPS.Models;
using System.Configuration;

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
        public List<Project> GetProjects()
        {
            var projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * From projects ORDER BY id DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Project project = new Project();

                            project.id = Convert.ToInt32(reader["id"]);
                            project.projectname = Convert.ToString(reader["projectname"]);

                            projects.Add(project);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving projects: " + ex.Message);
            }

            return projects;
        }

        public List<Part> GetParts()
        {
            var parts = new List<Part>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * From parts ORDER BY id DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
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
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving parts: " + ex.Message);
            }

            return parts;
        }

        // New: return parts filtered by project id
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

                // 1) Check if worker row already exists in parts table for this project/part/madeBy
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

                // 2) Not found -> get common remaining from parts (or 0)
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

                // 3) Create a new worker row in parts table (done=0, remaining=bendras)
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

                using (SqlCommand cmd = new SqlCommand("dbo.sp_AddWork", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@doneDelta", doneDelta);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
