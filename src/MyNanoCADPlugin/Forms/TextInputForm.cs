namespace MyNanoCADPlugin.Forms
{
    public class TextInputForm : Form
    {
        // ─── Properties ────────────────────────────────────────────────
        public string InputText { get; private set; } = string.Empty;

        // ─── Controls ──────────────────────────────────────────────────
        private Label lblPrompt;
        private TextBox txtInput;
        private Button btnOk;
        private Button btnCancel;

        public TextInputForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Enter Text";
            Size = new Size(380, 160);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9f);

            // ─── Label ─────────────────────────────────────────────────
            lblPrompt = new Label
            {
                Text = "Enter text to add to drawing:",
                Location = new Point(12, 15),
                AutoSize = true
            };

            // ─── TextBox ───────────────────────────────────────────────
            txtInput = new TextBox
            {
                Location = new Point(12, 38),
                Size = new Size(340, 25),
                PlaceholderText = "Type your text here..."
            };

            // Handle Enter key
            txtInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnOk_Click(s, e);
                }
            };

            // ─── OK Button ─────────────────────────────────────────────
            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(196, 80),
                Size = new Size(75, 28),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnOk_Click;

            // ─── Cancel Button ─────────────────────────────────────────
            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(277, 80),
                Size = new Size(75, 28),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.Click += (s, e) => Close();

            // ─── Add Controls ──────────────────────────────────────────
            Controls.AddRange(new Control[]
            {
                lblPrompt,
                txtInput,
                btnOk,
                btnCancel
            });

            // ─── Default Buttons ───────────────────────────────────────
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                MessageBox.Show(
                    "Please enter some text.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            InputText = txtInput.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
