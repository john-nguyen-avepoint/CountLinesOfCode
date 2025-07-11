namespace CountLinesCodeChanged_V3
{
    partial class CommitDetailForm : System.Windows.Forms.Form
    {
        public CommitDetailForm(List<CommitInfo> commits)
        {
            this.Size = new Size(600, 400);
            this.Text = "Commit Details";
            this.Font = new Font("Segoe UI", 12);

            var dgvCommits = new DataGridView
            {

                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                AllowUserToAddRows = false
            };
            dgvCommits.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvCommits.ColumnHeadersHeight = 50; // Tăng chiều cao header lên 50 pixels
            dgvCommits.Columns.Add("Message", "Commit Message");
            dgvCommits.Columns.Add("Date", "Date");
            foreach (var commit in commits)
            {
                dgvCommits.Rows.Add(commit.Message, commit.Date.ToString("g"));
            }

            this.Controls.Add(dgvCommits);
        }
    }
}