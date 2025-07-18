﻿using CountLinesCodeChanged_V3;
using CountLinesCodeChanged_V3.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;

namespace CountLinesCodeChanged_V3
{
    public partial class MainForm : Form
    {
        private Button btnSelectRepos;
        private Button btnAddPath;
        private Button btnSaveJson;
        private Button btnSaveCsv;
        private TextBox txtRepoPaths;
        private TextBox txtSearch;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private TextBox txtBranch;
        private Button btnCountLines;
        private DataGridView dgvSummary;
        private DataGridView dgvStats;
        private DataGridView dgvSummaryForAuthor;
        private TextBox txtSearchSummaryForAuthor;
        private Label lblMessage;
        private List<string> repoPaths = new List<string>();
        private List<RepositoryStats> allStats = new List<RepositoryStats>();
        private List<RepositoryStats> statByAuthor = new List<RepositoryStats>();

        public MainForm()
        {
            InitializeComponents();
        }

        // Trong phương thức InitializeComponents, thay toàn bộ code bằng:
        private void InitializeComponents()
        {
            this.Size = new Size(1300, 1000);
            this.Text = "Code Line Counter";
            this.Font = new Font("Segoe UI", 9);

            // Repository selection
            btnSelectRepos = new Button { Text = "Select\nRepositories", Size = new Size(100, 100), Location = new Point(430, 20) };
            btnSelectRepos.Click += BtnSelectRepos_Click;

            // Message label
            lblMessage = new Label
            {
                Text = "Please fill or select least one repository path.",
                AutoSize = true,
                Location = new Point(230, 140),
                ForeColor = Color.Red,
                Visible = true
            };

            // Repository paths text box
            txtRepoPaths = new TextBox
            {
                Multiline = true,
                Size = new Size(400, 100),
                Location = new Point(20, 20),
                ScrollBars = ScrollBars.Vertical,
                PlaceholderText = "Enter repository paths (one per line)..."
            };
            txtRepoPaths.TextChanged += TxtRepoPaths_TextChanged;

            // Start Date label and control
            var lblStartDate = new Label { Text = "Start Date:", AutoSize = true, Location = new Point(540, 20) };
            dtpStartDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Size = new Size(150, 40), Location = new Point(640, 20), Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 6, 1) };
            dtpStartDate.ValueChanged += (s, e) => { dtpStartDate.Value = dtpStartDate.Value.Date; UpdateCountButtonState(); };

            // End Date label and control
            var lblEndDate = new Label { Text = "End Date:", AutoSize = true, Location = new Point(540, 55) };
            dtpEndDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Size = new Size(150, 40), Location = new Point(640, 50), Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59) };
            dtpEndDate.ValueChanged += (s, e) => { dtpEndDate.Value = dtpEndDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59); UpdateCountButtonState(); };

            // Branch label and text box
            var lblBranch = new Label { Text = "Branch:", AutoSize = true, Location = new Point(540, 90) };
            txtBranch = new TextBox { PlaceholderText = "Enter branch name...", Size = new Size(150, 40), Location = new Point(640, 85), Text = "main" };
            txtBranch.TextChanged += (s, e) => UpdateCountButtonState();

            // Count lines button
            btnCountLines = new Button { Text = "Retrieve", Size = new Size(200, 40), Location = new Point(20, 130), BackColor = Color.LightGreen };
            btnCountLines.Enabled = true;
            btnCountLines.Click += BtnCountLines_Click;

            // Search label and bar
            var lblSearch = new Label { Text = "Search:", AutoSize = true, Location = new Point(20, 345) };
            txtSearch = new TextBox { PlaceholderText = "Search by author, repository...", Size = new Size(300, 40), Location = new Point(100, 340) };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Save JSON button
            btnSaveJson = new Button { Text = "Save JSON", Size = new Size(100, 40), Location = new Point(920, 20), BackColor = Color.LightGreen };
            btnSaveJson.Enabled = true;
            btnSaveJson.Click += BtnSaveJson_Click;

            // Save CSV button
            btnSaveCsv = new Button { Text = "Save CSV", Size = new Size(100, 40), Location = new Point(1030, 20), BackColor = Color.LightGreen };
            btnSaveCsv.Enabled = true;
            btnSaveCsv.Click += BtnSaveCsv_Click;

            // Summary panel
            var lblSummary = new Label { Text = "Summary:", AutoSize = true, Location = new Point(20, 200) };
            dgvSummary = new DataGridView
            {
                Size = new Size(500, 140),
                Location = new Point(20, 180),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ScrollBars = ScrollBars.Both,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = true,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            SetupSummaryDataGridView();

            var lblSummaryForAuthor = new Label { Text = "Summary For Author: ", AutoSize = true, Location = new Point(540, 150) };
            txtSearchSummaryForAuthor = new TextBox
            {
                PlaceholderText = "Search by author...",
                Size = new Size(300, 40),
                Location = new Point(670, 145)
            };

            txtSearchSummaryForAuthor.TextChanged += (s, e) =>
            {
                UpdateSummaryForAuthorDataGridView(statByAuthor);
            };

            dgvSummaryForAuthor = new DataGridView
            {
                Size = new Size(600, 140),
                Location = new Point(540, 180),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ScrollBars = ScrollBars.Both,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = true,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            SetupSummaryForAuthorDataGridView();

            // DataGridView
            var lblStats = new Label { Text = "Summary:", AutoSize = true, Location = new Point(20, 380) };
            dgvStats = new DataGridView
            {
                Size = new Size(1130, 400),
                Location = new Point(20, 370),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ScrollBars = ScrollBars.Both,
                AllowUserToResizeColumns = true,
                AllowUserToResizeRows = true,
                AllowUserToOrderColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            SetupDataGridView();



            this.Controls.AddRange(new Control[] { btnSelectRepos, lblMessage, txtRepoPaths, lblStartDate, dtpStartDate, lblEndDate, dtpEndDate, lblBranch, txtBranch, btnCountLines, lblSearch, txtSearch, btnSaveJson, btnSaveCsv, dgvSummary, dgvStats, txtSearchSummaryForAuthor, lblSummaryForAuthor, dgvSummaryForAuthor });
        }

        private void SetupSummaryDataGridView()
        {
            dgvSummary.Columns.Add("RepoName", "Repository");
            dgvSummary.Columns.Add("TotalCommits", "Total Commits");
            dgvSummary.Columns.Add("TotalAuthors", "Total Authors");
            dgvSummary.Columns.Add("TotalAdded", "Total Added");
            dgvSummary.Columns.Add("TotalRemoved", "Total Removed");
            dgvSummary.Columns.Add("AddPerRemove", "Add / Remove");
            dgvSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvSummary.ColumnHeadersHeight = 50; // Giữ chiều cao header giống dgvStats
            dgvSummary.Columns["RepoName"].DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            foreach (DataGridViewColumn column in dgvSummary.Columns)
            {
                column.MinimumWidth = 70;
            }
        }

        private void SetupDataGridView()
        {
            dgvStats.Columns.Add("Author", "Author");
            dgvStats.Columns.Add("RepoName", "Repository");
            dgvStats.Columns.Add("Branch", "Branch");
            dgvStats.Columns.Add("CommitCount", "Commits");
            dgvStats.Columns.Add("AddedLines", "Added Lines");
            dgvStats.Columns.Add("RemovedLines", "Removed Lines");
            dgvStats.Columns.Add("TotalChangedLines", "Total Changed");
            dgvStats.Columns.Add("AddPerRemovePercentage", "Add / Remove");
            var buttonColumn = new DataGridViewButtonColumn
            {
                Name = "ShowCommits",
                HeaderText = "Button",
                Text = "Show Commits",
                UseColumnTextForButtonValue = true,
                Width = 120, // Độ rộng cố định cho nút
                MinimumWidth = 100
            };
            dgvStats.Columns.Add(buttonColumn);
            dgvStats.Columns["ShowCommits"].DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvStats.Columns["Author"].DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvStats.Columns["RepoName"].DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dgvStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvStats.ColumnHeadersHeight = 50; // Tăng chiều cao header lên 50 pixels
            //dgvStats.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False; // Ngăn wrap tiêu đề
            dgvStats.CellContentClick += DgvStats_CellContentClick;
            foreach (DataGridViewColumn column in dgvStats.Columns)
            {
                column.MinimumWidth = 70;
            }
        }

        private void SetupSummaryForAuthorDataGridView()
        {
            dgvSummaryForAuthor.Columns.Add("Author", "Author");
            dgvSummaryForAuthor.Columns.Add("RepoName", "All Repository");
            dgvSummaryForAuthor.Columns.Add("Branch", "All Branch");
            dgvSummaryForAuthor.Columns.Add("CommitCount", "All Commits");
            dgvSummaryForAuthor.Columns.Add("AddedLines", "All Added Lines");
            dgvSummaryForAuthor.Columns.Add("RemovedLines", "All Removed Lines");
            dgvSummaryForAuthor.Columns.Add("TotalChangedLines", "All Total Changed");
            dgvSummaryForAuthor.Columns.Add("AddPerRemovePercentage", "Add / Remove");
            dgvSummaryForAuthor.ColumnHeadersHeight = 50;
            foreach (DataGridViewColumn column in dgvSummaryForAuthor.Columns)
            {
                column.MinimumWidth = 70;
            }
        }

        private void BtnCountLines_Click(object sender, EventArgs e)
        {
            var repoPaths = txtRepoPaths.Lines.ToList();
            bool isValid = repoPaths.Any() && !string.IsNullOrWhiteSpace(txtBranch.Text) && dtpEndDate.Value >= dtpStartDate.Value;
            lblMessage.Visible = !isValid;
            if (lblMessage.Visible)
            {
                if (!repoPaths.Any())
                {
                    lblMessage.Text = "Please select at least one repository.";
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtBranch.Text))
                {
                    lblMessage.Text = "Please enter a branch name.";
                    return;
                }
                if (dtpEndDate.Value < dtpStartDate.Value)
                {
                    lblMessage.Text = "Please select a valid date range.";
                    return;
                }
            }
            if (!repoPaths.Any())
            {
                MessageBox.Show("Please fill in all required fields: at least one repository path, a branch, and valid date range.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtBranch.Text))
            {
                MessageBox.Show("Please fill in all required fields: at least one repository path, a branch, and valid date range.");
                return;
            }
            else if (dtpEndDate.Value < dtpStartDate.Value)
            {

            }

            try
            {
                Cursor = Cursors.WaitCursor; // Đặt con trỏ quay
                allStats = GitProcessor.ProcessRepositories(repoPaths, dtpStartDate.Value, dtpEndDate.Value, txtBranch.Text);
                statByAuthor = allStats.GroupBy(x => x.Author)
                .Select(x => new RepositoryStats
                {
                    Author = x.Key,
                    RepoName = string.Join("; ", x.Select(p => p.RepoName).Distinct()),
                    Branch = x.First().Branch,
                    CommitCount = x.Sum(y => y.CommitCount),
                    AddedLines = x.Sum(y => y.AddedLines),
                    RemovedLines = x.Sum(y => y.RemovedLines)
                }).ToList();
                UpdateSummary();
                UpdateDataGridView(allStats);
                UpdateSummaryForAuthorDataGridView(statByAuthor);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing repositories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default; // Khôi phục con trỏ
            }
        }


        private void UpdateSummary()
        {
            dgvSummary.Rows.Clear();
            var summaries = GitProcessor.GetSummary(allStats);
            foreach (var summary in summaries)
            {
                dgvSummary.Rows.Add(
                    summary.RepoName,
                    summary.TotalCommits,
                    summary.TotalAuthors,
                    summary.TotalAdded,
                    summary.TotalRemoved,
                    summary.AddPerRemove,
                    summary.AddPerTotal + "%",
                    summary.RemovePerTotal + "%"
                );
            }
        }

        private void UpdateDataGridView(List<RepositoryStats> stats)
        {
            dgvStats.Rows.Clear();
            foreach (var stat in stats)
            {
                dgvStats.Rows.Add(
                    stat.Author,
                    stat.RepoName,
                    stat.Branch,
                    stat.CommitCount,
                    stat.AddedLines,
                    stat.RemovedLines,
                    stat.TotalChangedLines,
                    stat.AddPerRemovePercentage,
                    stat.AddPerTotalPercentage,
                    stat.RemovePerTotalPercentage
                );
            }
        }
        private void UpdateSummaryForAuthorDataGridView(List<RepositoryStats> stats)
        {
            dgvSummaryForAuthor.Rows.Clear();
            var filteredStats = string.IsNullOrEmpty(txtSearchSummaryForAuthor.Text)
                ? stats
                : stats.Where(s => s.Author.ToLower().Contains(txtSearchSummaryForAuthor.Text.ToLower()))
                .ToList();
            foreach (var stat in filteredStats)
            {
                var removedLines = stat.RemovedLines == 0 ? 1 : stat.RemovedLines;
                dgvSummaryForAuthor.Rows.Add(
                    stat.Author,
                    stat.RepoName,
                    stat.Branch,
                    stat.CommitCount,
                    stat.AddedLines,
                    stat.RemovedLines,
                    stat.TotalChangedLines,
                    Math.Round((double)(stat.AddedLines / removedLines), 2),
                    Math.Round((double)(stat.AddedLines / stat.TotalChangedLines), 2),
                    Math.Round((double)stat.RemovedLines / stat.TotalChangedLines, 2)
                );
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            var filteredStats = string.IsNullOrEmpty(txtSearch.Text)
                ? allStats
                : allStats.Where(s => s.Author.ToLower().Contains(txtSearch.Text.ToLower())
                || s.RepoName.ToLower().Contains(txtSearch.Text.ToLower())
                || s.Branch.ToLower().Contains(txtSearch.Text.ToLower()))
                .ToList();
            UpdateDataGridView(filteredStats);
        }

        private void BtnSelectRepos_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (!repoPaths.Contains(fbd.SelectedPath))
                    {
                        repoPaths.Add(fbd.SelectedPath);
                        UpdateRepoPathsTextBox();
                        MessageBox.Show($"Added repository: {fbd.SelectedPath}");
                    }
                    else
                    {
                        MessageBox.Show("Repository already added.");
                    }
                }
            }
        }

        private void TxtRepoPaths_TextChanged(object sender, EventArgs e)
        {
            repoPaths.Clear();
            var paths = txtRepoPaths.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths)
            {
                var trimmedPath = path.Trim();
                if (!string.IsNullOrWhiteSpace(trimmedPath) && !repoPaths.Contains(trimmedPath) && Directory.Exists(Path.Combine(trimmedPath, ".git")))
                {
                    repoPaths.Add(trimmedPath);
                }
            }
            UpdateRepoPathsTextBox();
            UpdateCountButtonState();
        }

        private void UpdateRepoPathsTextBox()
        {
            txtRepoPaths.Text = string.Join(Environment.NewLine, repoPaths);
            if (repoPaths.Any())
            {
                btnSelectRepos.Text = "Select More Repositories";
            }
        }

        private void DgvStats_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvStats.Columns["ShowCommits"].Index && e.RowIndex >= 0)
            {
                var stat = allStats[e.RowIndex];
                var commitForm = new CommitDetailForm(stat.Commits);
                commitForm.ShowDialog();
            }
        }
        private void UpdateCountButtonState()
        {
            bool isValid = repoPaths.Any() && !string.IsNullOrWhiteSpace(txtBranch.Text) && dtpEndDate.Value >= dtpStartDate.Value;
            lblMessage.Text = "";
            //btnCountLines.Enabled = isValid;

        }

        private void BtnSaveJson_Click(object sender, EventArgs e)
        {
            try
            {
                if (!allStats.Any())
                {
                    MessageBox.Show("No data to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "JSON files (*.json)|*.json";
                    sfd.FileName = "output.json";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var jsonData = new
                        {
                            summary = GitProcessor.GetSummary(allStats),
                            stats = allStats
                        };
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        File.WriteAllText(sfd.FileName, JsonSerializer.Serialize(jsonData, options));
                        MessageBox.Show("JSON file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving JSON file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveCsv_Click(object sender, EventArgs e)
        {
            try
            {
                if (!allStats.Any())
                {
                    MessageBox.Show("No data to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "CSV files (*.csv)|*.csv";
                    sfd.FileName = "output.csv";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (var writer = new StreamWriter(sfd.FileName))
                        {
                            // Write Summary sheet
                            writer.WriteLine("Summary");
                            writer.WriteLine("Repository,Total Commits,Total Authors,Total Added,Total Removed,Add/Remove %,Add/Total %,Remove/Total %,Start Date,End Date");
                            writer.WriteLine($",,,,,,,,{dtpStartDate.Value:yyyy-MM-dd},{dtpEndDate.Value:yyyy-MM-dd}");
                            foreach (var summary in GitProcessor.GetSummary(allStats))
                            {
                                writer.WriteLine($"{EscapeCsv(summary.RepoName)},{summary.TotalCommits},{summary.TotalAuthors},{summary.TotalAdded},{summary.TotalRemoved},{summary.AddPerRemove},{summary.AddPerTotal},{summary.RemovePerTotal},,");
                            }
                            writer.WriteLine();

                            // Write Stats sheet
                            writer.WriteLine("Stats Count Lines of Code Changed by Author");
                            writer.WriteLine("Author,Repository,Branch,Added Lines,Removed Lines,Total Changed,Add/Remove %,Add/Total %,Remove/Total %,Commits,Start Date,End Date");
                            writer.WriteLine($",,,,,,,,,,{dtpStartDate.Value:yyyy-MM-dd},{dtpEndDate.Value:yyyy-MM-dd}");
                            foreach (var stat in allStats)
                            {
                                writer.WriteLine($"{EscapeCsv(stat.Author)},{EscapeCsv(stat.RepoName)},{EscapeCsv(stat.Branch)},{stat.AddedLines},{stat.RemovedLines},{stat.TotalChangedLines},{stat.AddPerRemovePercentage},{stat.AddPerTotalPercentage},{stat.RemovePerTotalPercentage},{stat.CommitCount},,");
                            }
                        }
                        MessageBox.Show("CSV file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving CSV file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Thêm phương thức hỗ trợ để thoát ký tự CSV:
        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
        private Button button1;
    }
}