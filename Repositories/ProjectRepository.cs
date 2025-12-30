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

                            if (!projects.TryGetValue(projectId, out Project project))
                            {
                                project = new Project
                                {
                                    id = projectId,
                                    projectname = reader.IsDBNull(reader.GetOrdinal("projectname")) ? string.Empty : reader.GetString(reader.GetOrdinal("projectname"))
                                };
                                projects.Add(projectId, project);
                            }

                            // Pridėda dalį, jeigu dalis egzistuoja
                            if (!reader.IsDBNull(reader.GetOrdinal("PartId")))
                            {
                                var part = new Part
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("PartId")),
                                    project_id = reader["project_id"] != DBNull.Value ? Convert.ToInt32(reader["project_id"]) : 0,
                                    partname = reader["partname"] != DBNull.Value ? Convert.ToString(reader["partname"]) : string.Empty,
                                    madeby = reader["madeby"] != DBNull.Value ? Convert.ToString(reader["madeby"]) : string.Empty,
                                    typeofwork = reader["typeofwork"] != DBNull.Value ? Convert.ToString(reader["typeofwork"]) : string.Empty,
                                    created_at = reader["created_at"] != DBNull.Value ? Convert.ToDateTime(reader["created_at"]) : DateTime.Now,
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

        public void InsertUserPartAndRemoveAdmin(
        int projectId,
        string partName,
        string madeBy,
        string typeOfWork,
        string comments,
        int doneDelta)
        {
            using (var con = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("dbo.sp_RegisterWork", con)
)
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@project_id", projectId);
                cmd.Parameters.AddWithValue("@partname", partName);
                cmd.Parameters.AddWithValue("@madeby", madeBy);
                cmd.Parameters.AddWithValue("@typeofwork", typeOfWork);
                cmd.Parameters.AddWithValue("@comments", comments ?? "");
                cmd.Parameters.AddWithValue("@doneDelta", doneDelta);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }
}
