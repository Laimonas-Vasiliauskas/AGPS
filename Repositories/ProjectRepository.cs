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
            string raw = ConfigurationManager.ConnectionStrings["AGPSdb"].ConnectionString;

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
                            project.partname = Convert.ToString(reader["partname"]);
                            project.madeby = Convert.ToString(reader["madeby"]);
                            project.typeofwork = Convert.ToString(reader["typeofwork"]);
                            project.created_at = Convert.ToString(reader["created_at"]);
                            project.comments = Convert.ToString(reader["comments"]);
                            project.remaining = Convert.ToInt32(reader["remaining"]);
                            project.done = Convert.ToInt32(reader["done"]);

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

        public void UpdateProject(Project project)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE projects SET projectname = @projectname, partname = @partname, madeby = @madeby, " +
                                 "typeofwork = @typeofwork, created_at = @created_at, comments = @comments, remaining = @remaining, done = @done WHERE id = @id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@projectname", project.projectname);
                        command.Parameters.AddWithValue("@partname", project.partname);
                        command.Parameters.AddWithValue("@madeby", project.madeby);
                        command.Parameters.AddWithValue("@typeofwork", project.typeofwork);
                        command.Parameters.AddWithValue("@created_at", DateTime.Now);
                        command.Parameters.AddWithValue("@comments", project.comments);
                        command.Parameters.AddWithValue("@remaining", project.remaining);
                        command.Parameters.AddWithValue("@done", project.done);
                        command.Parameters.AddWithValue("@id", project.id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating project: " + ex.Message);
            }
        }

        public int GetOrCreateWorkerRowId(string projectName, string partName, string madeBy, string typeOfWork)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1) Ar darbuotojo eilutė jau egzistuoja?
                string findSql = @"SELECT TOP 1 id FROM projects WHERE projectname = @p AND partname = @part AND madeby = @madeby ORDER BY id DESC;";

                using (var cmd = new SqlCommand(findSql, conn))
                {
                    cmd.Parameters.AddWithValue("@p", projectName);
                    cmd.Parameters.AddWithValue("@part", partName);
                    cmd.Parameters.AddWithValue("@madeby", madeBy);

                    var res = cmd.ExecuteScalar();
                    if (res != null && res != DBNull.Value)
                        return Convert.ToInt32(res);
                }

                // 2) Nerasta -> paimam bendrą remaining (iš bazinio arba bet kurio įrašo tos grupės)
                int remaining = 0;
                string remSql = @"SELECT TOP 1 remaining FROM projects WHERE projectname = @p AND partname = @part ORDER BY id DESC;";

                using (var cmd = new SqlCommand(remSql, conn))
                {
                    cmd.Parameters.AddWithValue("@p", projectName);
                    cmd.Parameters.AddWithValue("@part", partName);

                    var res = cmd.ExecuteScalar();
                    remaining = (res == null || res == DBNull.Value) ? 0 : Convert.ToInt32(res);
                }

                // 3) Sukuriam naują darbuotojo eilutę (done=0, remaining=bendras)
                string insertSql = @"INSERT INTO projects (projectname, partname, madeby, typeofwork, created_at, comments, remaining, done) OUTPUT INSERTED.id VALUES (@p, @part, @madeby, @typeofwork, GETDATE(), '', @remaining, 0);";

                using (var cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@p", projectName);
                    cmd.Parameters.AddWithValue("@part", partName);
                    cmd.Parameters.AddWithValue("@madeby", madeBy);
                    cmd.Parameters.AddWithValue("@typeofwork", typeOfWork ?? "");
                    cmd.Parameters.AddWithValue("@remaining", remaining);

                    return (int)cmd.ExecuteScalar();
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
