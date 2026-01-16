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
        private readonly Timer _doneWatchTimer = new Timer();
        private string _projectsSignature = string.Empty;
        // Flag to avoid multiple message boxes showing at the same time
        private bool _notificationShowing = false;
        public Form1()
        {
            InitializeComponent();
            StartNewProjectWatcher();

            // Užkrauna projektus ir dalis
            LoadProjectsAndParts();

            // Inicializuoti parašą pagal esamą DB būseną, kad nebūtų iškart pranešama
            try
            {
                var repo = new ProjectRepository();
                var projects = repo.GetProjectsWithParts();
                _projectsSignature = BuildProjectsSignature(projects);
            }
            catch
            {
                _projectsSignature = string.Empty;
            }
        }

        private int project_id = 0;

        private string BuildProjectsSignature(List<Project> projects)
        {
            if (projects == null) return string.Empty;

            var parts = projects
                .OrderBy(p => p.id)
                .Select(p =>
                {
                    // Suveikia pagal naujus, unikalius dalių vardus 
                    var uniquePartNames = p.Parts
                        .Select(pp => (pp.partname ?? string.Empty).Trim())
                        .Where(n => !string.IsNullOrEmpty(n))
                        .Select(n => n.ToLowerInvariant())
                        .Distinct()
                        .OrderBy(n => n);

                    var partSig = string.Join(",", uniquePartNames);
                    return p.id + ":" + p.projectname + ":" + partSig;
                });

            return string.Join("|", parts);
        }


        private void LoadProjectsAndParts(int? selectedProjectId = null)
        {
            try
            {
                var repo = new ProjectRepository();
                var projects = repo.GetProjectsWithParts();


                comboBoxProject.DataSource = projects;
                comboBoxProject.DisplayMember = "projectname";
                comboBoxProject.ValueMember = "id";

 
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
        // Užkrauna tik dalis, kai keičiasi projekto pasirinkimas 
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

                // Remaining priristas prie part
                comboBoxRemaining.DataSource = uniqueParts;
                comboBoxRemaining.DisplayMember = "remaining";
                    
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxProject.SelectedValue != null && int.TryParse(comboBoxProject.SelectedValue.ToString(), out int id))
            {
                project_id = id;
            }
            else if (comboBoxProject.SelectedItem is Project p)
            {
                project_id = p.id;
            }

            // Užkrauna tik dalis pasirenktam projektui, kad išvengti comboBox reset
            LoadPartsForProject(project_id != 0 ? (int?)project_id : null);
            
        }

        // Mygtukas UPDATE
        private void button1_Click(object sender, EventArgs e)
        {
            var repo = new ProjectRepository();

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

            // Patikrinam remaining (kaip pas tave buvo)
            var partsForProject = repo.GetPartsByProjectId(project_id);
            var anyPartRow = partsForProject.FirstOrDefault(x => x.partname == partName);

            if (anyPartRow != null && doneDelta > anyPartRow.remaining)
            {
                MessageBox.Show("Done can't be more than remaining.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Made by negali but tuscia
            if (string.IsNullOrWhiteSpace(madeBy))
            {
                MessageBox.Show("The 'Made By' field can't be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // VIENAS KVietimas: INSERT user row + DELETE admin template row
            try
            {
                repo.InsertUserPartAndRemoveAdmin(project_id, partName, madeBy, typeOfWork, comments, doneDelta);

                MessageBox.Show("Updated!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPartsForProject(project_id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("DB update failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartNewProjectWatcher()
        {
            
            _doneWatchTimer.Interval = 5000;
            _doneWatchTimer.Tick += NewProjectWatchTimer_Tick;
            _doneWatchTimer.Start();
        }

        // Jeigu DB sukuriamas naujas projektas ar dalis, gaunamas pranėšimas
        private void NewProjectWatchTimer_Tick(object sender, EventArgs e)
        {
            
            // DB tikrinimas fone
            Task.Run(() =>
            {
                try
                {
                    var repo = new ProjectRepository();
                    var projects = repo.GetProjectsWithParts();
                    var sig = BuildProjectsSignature(projects);
                    if (sig != _projectsSignature)
                    {
                        _projectsSignature = sig;
                        
                        this.BeginInvoke(new Action(() =>
                        {
                            // Užtikrina, kad rodomas tik vienas pranešimas
                            if (_notificationShowing) return;

                            _notificationShowing = true;
                            try
                            {
                                MessageBox.Show("Database changed: project added or deleted.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                int sel = project_id;
                                LoadProjectsAndParts(sel != 0 ? (int?)sel : null);
                                LoadPartsForProject(sel != 0 ? (int?)sel : null);
                                RefreshCurrentSelection();
                            }
                            finally
                            {
                                _notificationShowing = false;
                            }
                        }));
                    }
                }
                catch
                {
                    // ignore
                }
            });
        }

        // Po pranesimo atnaujina laukelius
        private void RefreshCurrentSelection()
        {
            try
            {
                var repo = new ProjectRepository();
                var parts = repo.GetPartsByProjectId(project_id);

                string partName = comboBoxPartList.Text;
                string madeBy = textBoxMadeBy.Text;

                var myRow = parts.FirstOrDefault(x => x.partname == partName && x.madeby == madeBy);
                var anyRow = parts.FirstOrDefault(x => x.partname == partName);

                if (myRow != null)
                    textBoxDone.Text = myRow.done.ToString();
                else
                    textBoxDone.Text = "";

                if (anyRow != null)
                    comboBoxRemaining.Text = anyRow.remaining.ToString();
                else
                    comboBoxRemaining.Text = string.Empty;
            }
            catch
            {
                // ignore
            }
        }
     }
 }