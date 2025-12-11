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

        
    }
}
