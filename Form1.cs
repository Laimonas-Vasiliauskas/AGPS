using AGPS.Models;
using AGPS.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AGPS
{
    public partial class Form1 : Form
    {
        private Timer updateTimer;
        public Form1()
        {
            InitializeComponent();
            LoadParts();
            LoadProjects();

        }

        private void LoadProjects()
        {
            try
            {
                var repo = new ProjectRepository();
                var projects = repo.GetProjects();

                comboBoxProject.DataSource = projects;
                comboBoxProject.DisplayMember = "projectname";
                comboBoxProject.ValueMember = "id";



            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load projects: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadParts()
        {
            try
            {
                var repo = new ProjectRepository();
                var parts = repo.GetProjects();

                comboBoxPartList.DataSource = parts;
                comboBoxPartList.DisplayMember = "partname";




            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load parts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxTypeOfWork.SelectedIndex = 0;


        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxProject.SelectedValue != null && int.TryParse(comboBoxProject.SelectedValue.ToString(), out int id))
            {
                projectId = id;
            }
            else if (comboBoxProject.SelectedItem is Project p)
            {
                projectId = p.id;
            }
            comboBoxPartList.SelectedIndex = comboBoxProject.SelectedIndex;
        }

        private int projectId = 0;

        // replace/adjust your button handler so it only shows success after a real update
        private void button1_Click(object sender, EventArgs e)
        {
            if (this.projectId == 0)
            {
                MessageBox.Show("No project selected to update.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var repo = new ProjectRepository();

            Project project = new Project
            {
                id = this.projectId,
                projectname = this.comboBoxProject.Text,
                partname = this.comboBoxPartList.Text,
                madeby = this.textBoxMadeBy.Text,
                typeofwork = this.comboBoxTypeOfWork.Text,
                created_at = this.dateTimePickerDate.Text,
                comments = this.textBoxComments.Text
            };

            // Option A: preserve DB-only fields inside repository (recommended)
            repo.UpdateProject(project);

            MessageBox.Show("Project updated successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}