using CountLinesOfCode.Models;
using CountLinesOfCode.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Forms;

namespace CountLinesOfCode
{
    public partial class Form1 : Form
    {
        private const string DefaultRepoPathsText = "Select the folder or fill in the path. You can enter multiple projects, each project path is on a separate line.";
        private ResultOutput _resultOutput; // Store results in memory
        public Form1()
        {
            InitializeComponent();
        }

        private TextBox txtAuthor;
        private TextBox txtRepoPaths;
        private Button btnSelectFolders;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private TextBox txtBranch;
        private Button btnProcess;
        private Button btnSaveResult;
        private TextBox txtOutput;
        private CheckBox chkShowCommits;
        private Label lblAuthor;
        private Label lblRepoPaths;
        private Label lblStartDate;
        private Label lblEndDate;
        private Label lblBranch;
        private Label lblShowCommits;

        private void InitializeComponent()
        {
            this.txtAuthor = new TextBox();
            this.txtRepoPaths = new TextBox();
            this.btnSelectFolders = new Button();
            this.dtpStartDate = new DateTimePicker();
            this.dtpEndDate = new DateTimePicker();
            this.txtBranch = new TextBox();
            this.btnProcess = new Button();
            this.btnSaveResult = new Button();
            this.txtOutput = new TextBox();
            this.chkShowCommits = new CheckBox();
            this.lblAuthor = new Label();
            this.lblRepoPaths = new Label();
            this.lblStartDate = new Label();
            this.lblEndDate = new Label();
            this.lblBranch = new Label();
            this.lblShowCommits = new Label();

            // lblAuthor
            this.lblAuthor.Text = "Author (name/email):";
            this.lblAuthor.Location = new System.Drawing.Point(20, 20);
            this.lblAuthor.Size = new System.Drawing.Size(100, 20);

            // txtAuthor
            this.txtAuthor.Location = new System.Drawing.Point(130, 20);
            this.txtAuthor.Size = new System.Drawing.Size(300, 20);

            // lblRepoPaths
            this.lblRepoPaths.Text = "Repository Folders:";
            this.lblRepoPaths.Location = new System.Drawing.Point(20, 50);
            this.lblRepoPaths.Size = new System.Drawing.Size(100, 20);

            // txtRepoPaths
            this.txtRepoPaths.Multiline = true;
            this.txtRepoPaths.Location = new System.Drawing.Point(130, 50);
            this.txtRepoPaths.Size = new System.Drawing.Size(300, 60);
            this.txtRepoPaths.ScrollBars = ScrollBars.Vertical;
            //this.txtRepoPaths.ReadOnly = true;
            this.txtRepoPaths.Text = DefaultRepoPathsText;
            this.txtRepoPaths.ForeColor = System.Drawing.Color.Gray;
            this.txtRepoPaths.Font = new System.Drawing.Font(this.txtRepoPaths.Font, System.Drawing.FontStyle.Italic);
            this.txtRepoPaths.Enter += new EventHandler(this.txtRepoPaths_Enter);
            this.txtRepoPaths.Leave += new EventHandler(this.txtRepoPaths_Leave);

            // btnSelectFolders
            this.btnSelectFolders.Text = "Select Folders";
            this.btnSelectFolders.Location = new System.Drawing.Point(330, 115);
            this.btnSelectFolders.Size = new System.Drawing.Size(100, 25);
            this.btnSelectFolders.Click += new EventHandler(this.btnSelectFolders_Click);

            // lblStartDate
            this.lblStartDate.Text = "Start Date:";
            this.lblStartDate.Location = new System.Drawing.Point(20, 150);
            this.lblStartDate.Size = new System.Drawing.Size(100, 20);

            // dtpStartDate
            this.dtpStartDate.Location = new System.Drawing.Point(130, 150);
            this.dtpStartDate.Size = new System.Drawing.Size(300, 20);
            this.dtpStartDate.Format = DateTimePickerFormat.Custom;
            this.dtpStartDate.CustomFormat = "MMM/dd/yyyy";
            this.dtpStartDate.Value = new DateTime(DateTime.Now.AddMonths(-6).Year, DateTime.Now.AddMonths(-6).Month, 1); // Default to six months ago

            // lblEndDate
            this.lblEndDate.Text = "End Date:";
            this.lblEndDate.Location = new System.Drawing.Point(20, 180);
            this.lblEndDate.Size = new System.Drawing.Size(100, 20);

            // dtpEndDate
            this.dtpEndDate.Location = new System.Drawing.Point(130, 180);
            this.dtpEndDate.Size = new System.Drawing.Size(300, 20);
            this.dtpEndDate.Format = DateTimePickerFormat.Custom;
            this.dtpEndDate.CustomFormat = "MMM/dd/yyyy";
            this.dtpEndDate.Value = DateTime.Now; // Default to today

            // lblBranch
            this.lblBranch.Text = "Branch:";
            this.lblBranch.Location = new System.Drawing.Point(20, 210);
            this.lblBranch.Size = new System.Drawing.Size(100, 20);

            // txtBranch
            this.txtBranch.Location = new System.Drawing.Point(130, 210);
            this.txtBranch.Size = new System.Drawing.Size(300, 20);
            this.txtBranch.Text = "main";

            // lblShowCommits
            this.lblShowCommits.Text = "Show Commits:";
            this.lblShowCommits.Location = new System.Drawing.Point(20, 240);
            this.lblShowCommits.Size = new System.Drawing.Size(100, 20);

            // chkShowCommits
            this.chkShowCommits.Text = "";
            this.chkShowCommits.Location = new System.Drawing.Point(130, 240);
            this.chkShowCommits.Size = new System.Drawing.Size(20, 20);
            this.chkShowCommits.Checked = false;

            // btnProcess
            this.btnProcess.Text = "Count Changed Lines";
            this.btnProcess.Location = new System.Drawing.Point(130, 270);
            this.btnProcess.Size = new System.Drawing.Size(130, 30);
            this.btnProcess.Click += new EventHandler(this.btnProcess_Click);

            // btnSaveResult
            this.btnSaveResult.Text = "Save Result";
            this.btnSaveResult.Location = new System.Drawing.Point(270, 270);
            this.btnSaveResult.Size = new System.Drawing.Size(100, 30);
            this.btnSaveResult.Click += new EventHandler(this.btnSaveResult_Click);
            this.btnSaveResult.Enabled = false;

            // txtOutput
            this.txtOutput.Multiline = true;
            this.txtOutput.ScrollBars = ScrollBars.Vertical;
            this.txtOutput.Location = new System.Drawing.Point(20, 310);
            this.txtOutput.Size = new System.Drawing.Size(410, 150);
            this.txtOutput.ReadOnly = true;

            // Form1
            this.Text = "GitLab AuthorDiff";
            this.Size = new System.Drawing.Size(450, 500);
            this.Controls.Add(this.txtAuthor);
            this.Controls.Add(this.txtRepoPaths);
            this.Controls.Add(this.btnSelectFolders);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.txtBranch);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnSaveResult);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblRepoPaths);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.lblBranch);
            this.Controls.Add(this.lblShowCommits);
            this.Controls.Add(this.chkShowCommits);
        }

        private void txtRepoPaths_Enter(object sender, EventArgs e)
        {
            if (txtRepoPaths.Text == DefaultRepoPathsText)
            {
                txtRepoPaths.Text = "";
                txtRepoPaths.ForeColor = System.Drawing.Color.Black;
                txtRepoPaths.Font = new System.Drawing.Font(txtRepoPaths.Font, System.Drawing.FontStyle.Regular);
            }
        }

        private void txtRepoPaths_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRepoPaths.Text))
            {
                txtRepoPaths.Text = DefaultRepoPathsText;
                txtRepoPaths.ForeColor = System.Drawing.Color.Gray;
                txtRepoPaths.Font = new System.Drawing.Font(txtRepoPaths.Font, System.Drawing.FontStyle.Italic);
            }
        }

        private void btnSelectFolders_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = DefaultRepoPathsText;
                folderDialog.ShowNewFolderButton = false;

                List<string> selectedPaths = txtRepoPaths.Text == DefaultRepoPathsText
                    ? new List<string>()
                    : txtRepoPaths.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                while (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    if (!selectedPaths.Contains(selectedPath))
                    {
                        selectedPaths.Add(selectedPath);
                    }

                    var result = MessageBox.Show("Add another folder?", "Select Folder", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                        break;
                }

                if (selectedPaths.Count > 0)
                {
                    txtRepoPaths.Text = string.Join(Environment.NewLine, selectedPaths);
                    txtRepoPaths.ForeColor = System.Drawing.Color.Black;
                    txtRepoPaths.Font = new System.Drawing.Font(txtRepoPaths.Font, System.Drawing.FontStyle.Regular);
                }
                else
                {
                    txtRepoPaths.Text = DefaultRepoPathsText;
                    txtRepoPaths.ForeColor = System.Drawing.Color.Gray;
                    txtRepoPaths.Font = new System.Drawing.Font(txtRepoPaths.Font, System.Drawing.FontStyle.Italic);
                }
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                // Show loading cursor
                Cursor.Current = Cursors.WaitCursor;
                btnProcess.Enabled = false;

                string author = txtAuthor.Text.Trim();
                string[] repoPaths = txtRepoPaths.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
                DateTime startDate = dtpStartDate.Value;
                DateTime endDate = dtpEndDate.Value;
                string branch = txtBranch.Text.Trim();
                bool showCommits = chkShowCommits.Checked;

                if (string.IsNullOrEmpty(author) || (repoPaths != null && repoPaths.Length == 0) || string.IsNullOrEmpty(branch) || txtRepoPaths.Text == DefaultRepoPathsText)
                {
                    txtOutput.Text = "Error: Please fill in all fields and select at least one repository.";
                    return;
                }

                _resultOutput = new ResultOutput { Results = new List<AuthorResult>() };
                var authorResult = new AuthorResult
                {
                    Author = author,
                    Repositories = new List<RepositoryResult>()
                };
                string outputText = $"Author: {author}\r\n";
                var isokForDnload = false;
                foreach (var repoPath in repoPaths)
                {
                    if (!Directory.Exists(repoPath))
                    {
                        outputText += $"\r\nRepository: {repoPath}\r\nError: Repository path does not exist.\r\n";
                        continue;
                    }

                    var (added, removed, total, commitCount, commits) = GitProcessor.CountChangedLines(
                        author, startDate, endDate, branch, repoPath);

                    var repoResult = new RepositoryResult
                    {
                        Path = repoPath,
                        AddedLines = added,
                        RemovedLines = removed,
                        TotalChangedLines = total,
                        CommitCount = commitCount,
                    };
                    outputText += $"\r\nRepository: {repoPath}\r\n";
                    outputText += $"Added lines: {added}\r\n";
                    outputText += $"Removed lines: {removed}\r\n";
                    outputText += $"Total changed lines: {total}\r\n";
                    outputText += $"Total commits: {commitCount}\r\n";
                    outputText += $"Added lines / Remove lines: {repoResult.AddPerRemovePercentage}\r\n";
                    outputText += $"Added lines / Total changed lines: {repoResult.AddPerTotalPercentage}\r\n";
                    outputText += $"Removed lines / Total changed lines: {repoResult.RemovePerTotalPercentage}\r\n";
                    if (showCommits)
                    {
                        outputText += "Commits:\r\n";
                        foreach (var commit in commits)
                        {
                            outputText += $"{commit.Message}\r\n";
                        }
                    }

                    if (showCommits)
                    {
                        repoResult.Commits = commits;
                    }
                    else
                    {
                        repoResult.Commits = new List<CommitDetail>();
                    }
                    authorResult.Repositories.Add(repoResult);
                    isokForDnload = true;
                }

                _resultOutput.Results.Add(authorResult);


                if(!isokForDnload)
                {
                    txtOutput.Text = "No valid repositories found or processed.";
                    return;
                }
                else
                {
                    txtOutput.Text = outputText + "\r\nProcessing complete. Click 'Download Result' to save the output.";

                    // Enable download button
                    btnSaveResult.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                txtOutput.Text = $"Error: {ex.Message}";
            }
            finally
            {
                // Reset cursor and re-enable process button
                Cursor.Current = Cursors.Default;
                btnProcess.Enabled = true;
            }
        }

        private void btnSaveResult_Click(object sender, EventArgs e)
        {
            if (_resultOutput == null)
            {
                MessageBox.Show("No results to download. Please process commits first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FileName = "Result.json";
                saveFileDialog.Title = "Save Result JSON File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        File.WriteAllText(saveFileDialog.FileName, JsonSerializer.Serialize(_resultOutput, options));
                        MessageBox.Show("Result.json saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void MakeControlRounded(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90); // Top left corner
            path.AddArc(rect.Width - radius, rect.Y, radius, radius, 270, 90); // Top-right corner
            path.AddArc(rect.Width - radius, rect.Height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(rect.X, rect.Height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();
            control.Region = new Region(path);
            // Xử lý sự kiện Paint để vẽ viền
            control.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(Color.Gray, 1)) // Viền màu xám, độ dày 1
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, path);
                }
            };
        }
    }
}