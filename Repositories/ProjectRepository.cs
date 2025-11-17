using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AGPS.Models;

namespace AGPS.Repositories
{
    public class ProjectRepository
    {
        private readonly string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=AGPSadmin;Integrated Security=True;";

        public List<Project> GetProjects()
        {
            var projects = new List<Project>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT id, projectname, partname, madeby, typeofwork, created_at, comments, isChecked FROM projects ORDER BY id DESC";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var project = new Project();

                            project.id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0;
                            project.projectname = reader["projectname"] != DBNull.Value ? Convert.ToString(reader["projectname"]) : string.Empty;
                            project.partname = reader["partname"] != DBNull.Value ? Convert.ToString(reader["partname"]) : string.Empty;
                            project.madeby = reader["madeby"] != DBNull.Value ? Convert.ToString(reader["madeby"]) : string.Empty;
                            project.typeofwork = reader["typeofwork"] != DBNull.Value ? Convert.ToString(reader["typeofwork"]) : string.Empty;
                            project.created_at = reader["created_at"] != DBNull.Value ? Convert.ToString(reader["created_at"]) : string.Empty;
                            project.comments = reader["comments"] != DBNull.Value ? Convert.ToString(reader["comments"]) : string.Empty;
                            project.isChecked = reader["isChecked"] != DBNull.Value ? Convert.ToString(reader["isChecked"]) : string.Empty;

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
                    string sql = "UPDATE projects SET madeby = @madeby, " +
                                 "typeofwork = @typeofwork, comments = @comments, isChecked = @isChecked WHERE id = @id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@madeby", project.madeby);
                        command.Parameters.AddWithValue("@typeofwork", project.typeofwork);
                        command.Parameters.AddWithValue("@comments", project.comments);
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
