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
            var repo = new ProjectRepository();

            string projectName = comboBoxProject.Text.Trim();
            string partName = comboBoxPartList.Text.Trim();
            string madeBy = textBoxMadeBy.Text.Trim();
            string typeOfWork = comboBoxTypeOfWork.Text;

            if (!int.TryParse(textBoxDone.Text, out int doneDelta) || doneDelta <= 0)
            {
                MessageBox.Show("Done must be more than 0");
                return;
            }

            // 1) Randam arba susikuriam darbuotojo row
            int rowId = repo.GetOrCreateWorkerRowId(projectName, partName, madeBy, typeOfWork);

            // 2) Vykdom SP: done +delta (tik šitam row), remaining -delta (visiems)
            repo.AddWork(rowId, doneDelta); // AddWork kviečia dbo.sp_AddWork

            // 3) Atnaujini langą (persikrauni Remaining ir Done)
            LoadProjects();
            LoadParts();
            MessageBox.Show("Upadated!");

        }

        private void comboBoxTypeOfWork_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void RefreshCurrentSelection()
        {
            var repo = new ProjectRepository();
            var projects = repo.GetProjects();

            string projectName = comboBoxProject.Text;
            string partName = comboBoxPartList.Text;
            string madeBy = textBoxMadeBy.Text;

            // done - tik šito darbuotojo
            var myRow = projects.FirstOrDefault(x =>
                x.projectname == projectName &&
                x.partname == partName &&
                x.madeby == madeBy);

            // remaining - bendras (visi vienodą turi), imam bet kurią eilutę iš grupės
            var anyRow = projects.FirstOrDefault(x =>
                x.projectname == projectName &&
                x.partname == partName);

            if (myRow != null)
                textBoxDone.Text = myRow.done.ToString(); // jei nori rodyti "mano total done" kažkur kitur, ne delta laukelyje

            if (anyRow != null)
                comboBoxRemaining.Text = anyRow.remaining.ToString();
        }

    }
}