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
        public Form1()
        {
            InitializeComponent();
            LoadParts();
            LoadProjects();

           

        }

        // Po projekto atnaujinimo, lange turi pasikeist duomenis
        private void UpdateWindow()
        { 
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
                comboBoxRemaining.DataSource = projects;
                comboBoxRemaining.DisplayMember = "remaining";



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

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.projectId == 0)
            {
                MessageBox.Show("No project selected to update.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var repo = new ProjectRepository();

            // get current project from the bound list or repository so we preserve current values
            Project current = null;
            if (comboBoxProject.SelectedItem is Project p) current = p;
            else
            {
                var all = repo.GetProjects();
                current = all.Find(x => x.id == this.projectId);
            }

            if (current == null)
            {
                MessageBox.Show("Selected project not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Leidzia tik irasyt skaicius
            if (!int.TryParse(this.textBoxDone.Text, out int addedDone))
            {
                MessageBox.Show("Enter a valid number in Done field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Jeigu visos dalis padarytos ir bandoma prideti nauju, programa neleidzia tai padaryti
            else if (current.remaining == 0 && addedDone > 0)
            {
                MessageBox.Show("No remaining work to complete.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Neleidzia pridet 0 daliu
            // Bet galima atimt dalis, jeigu pridetas klaidingas kiekis
            else if (addedDone == 0)
            {
                MessageBox.Show("Done can't be 0.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int newDone = current.done + addedDone;
            int newRemaining = current.remaining - addedDone;
            if (newRemaining < 0) newRemaining = 0;

            Project project = new Project
            {
                id = this.projectId,
                projectname = this.comboBoxProject.Text,
                partname = this.comboBoxPartList.Text,
                done = newDone,
                remaining = newRemaining,
                madeby = this.textBoxMadeBy.Text,
                typeofwork = this.comboBoxTypeOfWork.Text,
                created_at = this.dateTimePickerDate.Text,
                comments = this.textBoxComments.Text
            };

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to update this project?", "Confirm Update", MessageBoxButtons.YesNo);
            if(dialogResult == DialogResult.Yes)
            {
                repo.UpdateProject(project);
            }

            MessageBox.Show("Project updated successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}