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

            // Initial load of projects and parts
            LoadProjectsAndParts();

            // Set initial projectId from selection (if any)
            if (comboBoxProject.SelectedValue != null && int.TryParse(comboBoxProject.SelectedValue.ToString(), out int id))
                project_id = id;
            else if (comboBoxProject.SelectedItem is Project p)
                project_id = p.id;

            // Ensure parts combobox is populated for the selected project
            if (project_id != 0)
            {
                LoadPartsForProject(project_id);
            }
            else
            {
                // If no project selected, ensure parts list still populated
                LoadPartsForProject(null);
            }

            // Atnaujinimas po update
            RefreshCurrentSelection();
        }

        // Po projekto atnaujinimo, lange turi pasikeist duomenis
        private void UpdateWindow()
        {
            // Reload projects list and preserve selection
            LoadProjectsAndParts(project_id != 0 ? (int?)project_id : null);
            // Ensure parts refreshed for current selection
            LoadPartsForProject(project_id != 0 ? (int?)project_id : null);
            RefreshCurrentSelection();
        }

        private void LoadProjectsAndParts(int? selectedProjectId = null)
        {
            try
            {
                var repo = new ProjectRepository();
                var projects = repo.GetProjectsWithParts();

                // ---------- PROJECTS ----------
                comboBoxProject.DataSource = projects;
                comboBoxProject.DisplayMember = "projectname";
                comboBoxProject.ValueMember = "id";

                // If a project id was supplied, try to select it
                if (selectedProjectId.HasValue)
                {
                    comboBoxProject.SelectedValue = selectedProjectId.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load projects: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Load only parts (used on project selection change)
        private void LoadPartsForProject(int? selectedProjectId = null)
        {
            try
            {
                var repo = new ProjectRepository();
                List<Part> parts;

                if (selectedProjectId.HasValue && selectedProjectId.Value != 0)
                {
                    parts = repo.GetPartsByProjectId(selectedProjectId.Value);
                }
                else
                {
                    // No specific project - aggregate parts from all projects
                    var projects = repo.GetProjectsWithParts();
                    parts = projects.SelectMany(p => p.Parts).ToList();
                }

                // Unikalūs Part pagal vardą
                var uniqueParts = parts
                    .GroupBy(p => p.partname)
                    .Select(g => g.First())
                    .ToList();

                comboBoxPartList.DataSource = uniqueParts;
                comboBoxPartList.DisplayMember = "partname";
                comboBoxPartList.ValueMember = "id";

                // Ensure a part is selected so remaining can be shown
                if (comboBoxPartList.Items.Count > 0)
                    comboBoxPartList.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to load parts: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
            // Update selected project id
            if (comboBoxProject.SelectedValue != null && int.TryParse(comboBoxProject.SelectedValue.ToString(), out int id))
            {
                project_id = id;
            }
            else if (comboBoxProject.SelectedItem is Project p)
            {
                project_id = p.id;
            }

            // Load only parts for the selected project to avoid resetting project combo
            LoadPartsForProject(project_id != 0 ? (int?)project_id : null);
            RefreshCurrentSelection();
        }

        private int project_id = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            var repo = new ProjectRepository();

            string projectName = comboBoxProject.Text.Trim();
            string partName = comboBoxPartList.Text.Trim();
            string madeBy = textBoxMadeBy.Text.Trim();
            string typeOfWork = comboBoxTypeOfWork.Text;
            string comments = textBoxComments.Text.Trim();

            if (!int.TryParse(textBoxDone.Text, out int doneDelta) || doneDelta <= 0)
            {
                MessageBox.Show("Done must be number or more than 0.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Užtikrina ar projektas pasirinktas
            if (project_id == 0)
            {
                MessageBox.Show("Please select a project.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            var partsForProject = repo.GetPartsByProjectId(project_id);
            var anyPartRow = partsForProject.FirstOrDefault(x => x.partname == partName);

            if (anyPartRow != null && doneDelta > anyPartRow.remaining)
            {
                MessageBox.Show("Done can't be more than remaining.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Made by negali but tuscia
            if (string.IsNullOrEmpty(madeBy))
            {
                MessageBox.Show("The 'Made By' field can't be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1) Randam arba susikuriam darbuotojo row
            int rowId = repo.GetOrCreateWorkerRowId(projectName, partName, madeBy, typeOfWork, comments);

            // 2) Vykdom SP: done +delta (tik šitam row), remaining -delta (visiems)
            repo.AddWork(rowId, doneDelta); // AddWork kviecia dbo.sp_AddWork

            // 3) Atnaujina langą
            UpdateWindow();

            MessageBox.Show("Updated!");
        }

        private void comboBoxTypeOfWork_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Ensure remaining/done fields update when user selects a different part
        private void comboBoxPartList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCurrentSelection();
        }

        private void RefreshCurrentSelection()
        {
            var repo = new ProjectRepository();

            // Gauna dalis pasirenktam projektui
            var parts = repo.GetPartsByProjectId(project_id);

            string projectName = comboBoxProject.Text;
            string partName = comboBoxPartList.Text;
            string madeBy = textBoxMadeBy.Text;

            // done - tik šito darbuotojo
            var myRow = parts.FirstOrDefault(x =>
                x.partname == partName &&
                x.madeby == madeBy);

            // remaining - bendras (visi vienodą turi),imam bet kurią eilutę iš grupės
            var anyRow = parts.FirstOrDefault(x => x.partname == partName);

            if (myRow != null)
                textBoxDone.Text = myRow.done.ToString(); // jei nori rodyti "mano total done" kažkur kitur, ne delta laukelyje

            if (anyRow != null)
                comboBoxRemaining.Text = anyRow.remaining.ToString();
            else
                comboBoxRemaining.Text = string.Empty;
        }

    }
}