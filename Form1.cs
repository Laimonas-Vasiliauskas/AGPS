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
            

            updateTimer = new Timer(); // Laikinas real time update sprendimas
            updateTimer.Interval = 20000; // Kas 20 s
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(Object sender, EventArgs e) // Laikinas real time update sprendimas
        {
            LoadProjects();
            LoadParts();
            
        }

        private void LoadProjects()
        {
            try
            {
                var repo = new ProjectRepository();
                var projects = repo.GetProjects();

                comboBox1.DataSource = projects;
                comboBox1.DisplayMember = "projectname";
                comboBox1.ValueMember = "id";

                

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

                comboBox2.DataSource = parts;
                comboBox2.DisplayMember = "partname";

                
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load parts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //comboBox1.SelectedIndex = 0;
            //comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = comboBox1.SelectedIndex;
        }

        private int projectId = 0; 

        private void button1_Click(object sender, EventArgs e) // Kol kas neveikia
        {
            /*Project project = new Project(); 
            project.id = this.projectId;
            project.projectname = this.comboBox1.Text;
            project.partname = this.comboBox2.Text;
            project.madeby = this.textBox1.Text;
            project.typeofwork = this.comboBox3.Text;    
            project.created_at = this.dateTimePicker1.Text;
            project.comments = this.textBox2.Text;
            

            var repo = new ProjectRepository();

            if (this.projectId != 0)
            {
                repo.UpdateProject(project);
            }
            else
            {
                MessageBox.Show("No project selected to update.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.DialogResult = DialogResult.OK; */ 
        }

    }
}
