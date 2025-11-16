using AGPS.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGPS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadProjects();
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

                comboBox1.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load projects: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
