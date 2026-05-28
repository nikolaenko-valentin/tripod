using HostMgd.ApplicationServices;
using Teigha.Geometry;

namespace MyNanoCADPlugin.Forms
{
    public class MainForm : Form
    {
        private readonly Document _doc;
        private readonly Services.DrawingService _drawService;
        private readonly Services.LayerService _layerService;

        // Controls
        private TabControl tabControl;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        // Draw tab
        private ComboBox cmbLayer;
        private Button btnLine;
        private Button btnCircle;
        private Button btnRectangle;
        private Button btnText;
        private NumericUpDown numX1, numY1;
        private NumericUpDown numX2, numY2;
        private NumericUpDown numRadius;

        // Layer tab
        private ListBox lstLayers;
        private TextBox txtNewLayer;
        private Button btnAddLayer;
        private Button btnSetCurrentLayer;

        public MainForm(Document doc)
        {
            _doc = doc;
            _drawService = new Services.DrawingService(doc);
            _layerService = new Services.LayerService(doc);
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "NanoCAD 25 Plugin";
            Size = new Size(480, 520);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = new Font("Segoe UI", 9f);

            // ─── Header ────────────────────────────────────────────────
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var lblTitle = new Label
            {
                Text = "⚙ NanoCAD 25 Plugin",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            header.Controls.Add(lblTitle);

            // ─── Tab Control ───────────────────────────────────────────
            tabControl = new TabControl
            {
                Location = new Point(10, 55),
                Size = new Size(450, 370)
            };

            tabControl.TabPages.Add(CreateDrawTab());
            tabControl.TabPages.Add(CreateLayerTab());
            tabControl.TabPages.Add(CreateInfoTab());

            // ─── Status Strip ──────────────────────────────────────────
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel("Ready")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            statusStrip.Items.Add(lblStatus);

            // ─── Close Button ──────────────────────────────────────────
            var btnClose = new Button
            {
                Text = "Close",
                Size = new Size(80, 28),
                Location = new Point(375, 435)
            };
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                header,
                tabControl,
                btnClose,
                statusStrip
            });
        }

        // ─── Draw Tab ──────────────────────────────────────────────────
        private TabPage CreateDrawTab()
        {
            var tab = new TabPage("✏️ Draw");

            // Layer selector
            var lblLayer = new Label
            {
                Text = "Layer:",
                Location = new Point(10, 15),
                AutoSize = true
            };

            cmbLayer = new ComboBox
            {
                Location = new Point(55, 12),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Coordinates group
            var grpCoords = new GroupBox
            {
                Text = "Coordinates",
                Location = new Point(10, 45),
                Size = new Size(420, 110)
            };

            // Point 1
            AddLabel(grpCoords, "X1:", 10, 25);
            numX1 = AddNumeric(grpCoords, 45, 22, -100000, 100000);

            AddLabel(grpCoords, "Y1:", 130, 25);
            numY1 = AddNumeric(grpCoords, 165, 22, -100000, 100000);

            // Point 2 / Radius
            AddLabel(grpCoords, "X2:", 10, 60);
            numX2 = AddNumeric(grpCoords, 45, 57, -100000, 100000, 100);

            AddLabel(grpCoords, "Y2:", 130, 60);
            numY2 = AddNumeric(grpCoords, 165, 57, -100000, 100000, 100);

            AddLabel(grpCoords, "R:", 260, 60);
            numRadius = AddNumeric(grpCoords, 280, 57, 0, 100000, 50);

            grpCoords.Controls.AddRange(new Control[]
            {
                numX1, numY1, numX2, numY2, numRadius
            });

            // Draw buttons
            var grpDraw = new GroupBox
            {
                Text = "Draw",
                Location = new Point(10, 165),
                Size = new Size(420, 150)
            };

            btnLine = CreateActionButton(
                "📏 Line", 10, 25,
                "Draw line from (X1,Y1) to (X2,Y2)",
                BtnLine_Click);

            btnCircle = CreateActionButton(
                "⭕ Circle", 120, 25,
                "Draw circle at (X1,Y1) with radius R",
                BtnCircle_Click);

            btnRectangle = CreateActionButton(
                "▬ Rectangle", 230, 25,
                "Draw rectangle from (X1,Y1) to (X2,Y2)",
                BtnRectangle_Click);

            btnText = CreateActionButton(
                "🔤 Text", 10, 85,
                "Add text at (X1,Y1)",
                BtnText_Click);

            grpDraw.Controls.AddRange(new Control[]
            {
                btnLine,
                btnCircle,
                btnRectangle,
                btnText
            });

            tab.Controls.AddRange(new Control[]
            {
                lblLayer,
                cmbLayer,
                grpCoords,
                grpDraw
            });

            return tab;
        }

        // ─── Layer Tab ─────────────────────────────────────────────────
        private TabPage CreateLayerTab()
        {
            var tab = new TabPage("📋 Layers");

            // Add layer
            var grpAdd = new GroupBox
            {
                Text = "New Layer",
                Location = new Point(10, 10),
                Size = new Size(420, 65)
            };

            txtNewLayer = new TextBox
            {
                PlaceholderText = "Layer name...",
                Location = new Point(10, 28),
                Size = new Size(220, 25)
            };

            btnAddLayer = new Button
            {
                Text = "Add Layer",
                Location = new Point(240, 26),
                Size = new Size(90, 28),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddLayer.FlatAppearance.BorderSize = 0;
            btnAddLayer.Click += BtnAddLayer_Click;

            grpAdd.Controls.AddRange(new Control[]
            {
                txtNewLayer,
                btnAddLayer
            });

            // Layer list
            var grpList = new GroupBox
            {
                Text = "Available Layers",
                Location = new Point(10, 85),
                Size = new Size(420, 230)
            };

            lstLayers = new ListBox
            {
                Location = new Point(10, 25),
                Size = new Size(395, 165),
                Font = new Font("Consolas", 9f)
            };

            btnSetCurrentLayer = new Button
            {
                Text = "Set as Current Layer",
                Location = new Point(10, 198),
                Size = new Size(150, 25)
            };
            btnSetCurrentLayer.Click += BtnSetCurrentLayer_Click;

            grpList.Controls.AddRange(new Control[]
            {
                lstLayers,
                btnSetCurrentLayer
            });

            tab.Controls.AddRange(new Control[]
            {
                grpAdd,
                grpList
            });

            return tab;
        }

        // ─── Info Tab ──────────────────────────────────────────────────
        private TabPage CreateInfoTab()
        {
            var tab = new TabPage("ℹ️ Info");

            var rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9f),
                BorderStyle = BorderStyle.None
            };

            rtb.AppendText("=== NanoCAD 25 Plugin ===\n\n");
            rtb.AppendText("Available Commands:\n\n");
            rtb.AppendText("  MYPLUGIN        → Open this dialog\n");
            rtb.AppendText("  MYPLUGIN_LINE   → Draw line interactively\n");
            rtb.AppendText("  MYPLUGIN_CIRCLE → Draw circle interactively\n");
            rtb.AppendText("  MYPLUGIN_INFO   → Show document info\n\n");
            rtb.AppendText($"Document: {_doc.Name}\n");
            rtb.AppendText($"Framework: .NET 6\n");
            rtb.AppendText($"Platform: x64\n");

            tab.Controls.Add(rtb);
            return tab;
        }

        // ─── Data Loading ──────────────────────────────────────────────
        private void LoadData()
        {
            // Load layers into combo and list
            var layers = _layerService.GetAllLayers();

            cmbLayer.Items.Clear();
            lstLayers.Items.Clear();

            foreach (var layer in layers)
            {
                cmbLayer.Items.Add(layer);
                lstLayers.Items.Add(layer);
            }

            if (cmbLayer.Items.Count > 0)
                cmbLayer.SelectedIndex = 0;
        }

        // ─── Event Handlers ────────────────────────────────────────────
        private void BtnLine_Click(object sender, EventArgs e)
        {
            try
            {
                var layer = cmbLayer.SelectedItem?.ToString() ?? "0";
                var result = _drawService.DrawLine(
                    new Point3d((double)numX1.Value, (double)numY1.Value, 0),
                    new Point3d((double)numX2.Value, (double)numY2.Value, 0),
                    layer
                );
                SetStatus(result, "Line drawn!", "Failed to draw line");
            }
            catch (Exception ex)
            {
                SetStatus(false, "", ex.Message);
            }
        }

        private void BtnCircle_Click(object sender, EventArgs e)
        {
            try
            {
                var layer = cmbLayer.SelectedItem?.ToString() ?? "0";
                var result = _drawService.DrawCircle(
                    new Point3d((double)numX1.Value, (double)numY1.Value, 0),
                    (double)numRadius.Value,
                    layer
                );
                SetStatus(result, "Circle drawn!", "Failed to draw circle");
            }
            catch (Exception ex)
            {
                SetStatus(false, "", ex.Message);
            }
        }

        private void BtnRectangle_Click(object sender, EventArgs e)
        {
            try
            {
                var layer = cmbLayer.SelectedItem?.ToString() ?? "0";
                var result = _drawService.DrawRectangle(
                    new Point3d((double)numX1.Value, (double)numY1.Value, 0),
                    (double)(numX2.Value - numX1.Value),
                    (double)(numY2.Value - numY1.Value),
                    layer
                );
                SetStatus(result, "Rectangle drawn!", "Failed");
            }
            catch (Exception ex)
            {
                SetStatus(false, "", ex.Message);
            }
        }

        private void BtnText_Click(object sender, EventArgs e)
        {
            using var inputForm = new TextInputForm();
            if (inputForm.ShowDialog() != DialogResult.OK) return;

            try
            {
                var layer = cmbLayer.SelectedItem?.ToString() ?? "0";
                var result = _drawService.DrawText(
                    inputForm.InputText,
                    new Point3d((double)numX1.Value, (double)numY1.Value, 0),
                    2.5,
                    layer
                );
                SetStatus(result, "Text added!", "Failed to add text");
            }
            catch (Exception ex)
            {
                SetStatus(false, "", ex.Message);
            }
        }

        private void BtnAddLayer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewLayer.Text)) return;

            var name = txtNewLayer.Text.Trim();
            var result = _layerService.CreateLayer(name);

            if (result)
            {
                cmbLayer.Items.Add(name);
                lstLayers.Items.Add(name);
                txtNewLayer.Clear();
                SetStatus(true, $"Layer '{name}' created!", "");
            }
            else
            {
                SetStatus(false, "", $"Failed to create layer '{name}'");
            }
        }

        private void BtnSetCurrentLayer_Click(object sender, EventArgs e)
        {
            if (lstLayers.SelectedItem == null) return;

            var name = lstLayers.SelectedItem.ToString()!;
            var result = _layerService.SetCurrentLayer(name);
            SetStatus(result,
                $"Current layer: {name}",
                $"Failed to set layer: {name}");
        }

        // ─── Helpers ───────────────────────────────────────────────────
        private void SetStatus(bool success, string okMsg, string errMsg)
        {
            lblStatus.Text = success ? $"✅ {okMsg}" : $"❌ {errMsg}";
            lblStatus.ForeColor = success
                ? Color.DarkGreen
                : Color.DarkRed;
        }

        private static Label AddLabel(
            Control parent, string text,
            int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private static NumericUpDown AddNumeric(
            Control parent, int x, int y,
            decimal min, decimal max,
            decimal value = 0)
        {
            var num = new NumericUpDown
            {
                Location = new Point(x, y),
                Size = new Size(80, 25),
                Minimum = min,
                Maximum = max,
                Value = value,
                DecimalPlaces = 2
            };
            parent.Controls.Add(num);
            return num;
        }

        private static Button CreateActionButton(
            string text, int x, int y,
            string tooltip,
            EventHandler handler)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 45),
                FlatStyle = FlatStyle.Flat
            };
            btn.Click += handler;
            new ToolTip().SetToolTip(btn, tooltip);
            return btn;
        }
    }
}
