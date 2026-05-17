using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.IO;
using CSMath;


namespace CadManagerUi
{
    public partial class Form1 : Form
    {
        private CadDocument _cadDoc;
        // private Panel _canvas;

        public Form1()
        {
            InitializeComponent();
            this.Text = "ACadSharp WinForms Viewer";
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Setup a Canvas (Panel) to draw on
            //_canvas = new Panel
            //{
            //    Dock = DockStyle.Fill,
            //    BackColor = System.Drawing.Color.WhiteSmoke
            //};

            // Redraw when the window is resized
            panel1.Resize += (s, e) => panel1.Invalidate();
            panel1.Paint += Canvas_Paint;

            // this.Controls.Add(_canvas);

            // LoadCadFile();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            // Open a file dialog so the user can pick the DXF file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "DXF Files (*.dxf)|*.dxf|All Files (*.*)|*.*";
                openFileDialog.Title = "Select a CAD File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Read the selected file
                        _cadDoc = DxfReader.Read(openFileDialog.FileName);

                        // IMPORTANT: Tell the canvas to redraw itself!
                        // This triggers the Canvas_Paint event below.
                        panel1.Invalidate();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error reading file: {ex.Message}", "Error");
                    }
                }
            }
        }

        // 5. The Drawing Logic (Same as before)
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            // If the button hasn't been clicked or file failed to load, do nothing
            if (_cadDoc == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            float scale = 4.0f;
            float marginX = 50f;
            float marginY = panel1.Height - 50f;

            Pen drawingPen = new Pen(System.Drawing.Color.Navy, 2f);

            foreach (Entity entity in _cadDoc.Entities)
            {
                if (entity is Line line)
                {
                    PointF p1 = new PointF(
                        marginX + (float)line.StartPoint.X * scale,
                        marginY - (float)line.StartPoint.Y * scale
                    );

                    PointF p2 = new PointF(
                        marginX + (float)line.EndPoint.X * scale,
                        marginY - (float)line.EndPoint.Y * scale
                    );

                    g.DrawLine(drawingPen, p1, p2);
                }
                else if (entity is Circle circle)
                {
                    float cx = marginX + (float)circle.Center.X * scale;
                    float cy = marginY - (float)circle.Center.Y * scale;
                    float r = (float)circle.Radius * scale;

                    g.DrawEllipse(drawingPen, cx - r, cy - r, r * 2, r * 2);
                }
            }
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Generating CAD file...");

            // 1. Create a new CAD Document
            CadDocument doc = new CadDocument();

            // 2. Create a Line entity
            Line line = new Line();
            line.StartPoint = new XYZ(0, 0, 0);       // Start at X:0, Y:0, Z:0
            line.EndPoint = new XYZ(100, 100, 0);     // End at X:100, Y:100, Z:0

            // 3. Create a Circle entity
            Circle circle = new Circle();
            circle.Center = new XYZ(50, 50, 0);       // Center at X:50, Y:50
            circle.Radius = 25.0;                     // Radius of 25

            // 4. Add the entities to the document's Model Space
            doc.Entities.Add(line);
            doc.Entities.Add(circle);

            // 5. Save the document as a DXF file
            string filePath = "MySimpleDrawing.dxf";

            // The 'false' parameter indicates we want a text-based DXF, not binary.
            using (DxfWriter writer = new(filePath, doc, false))
            {
                writer.Write();
            }

            Console.WriteLine($"Successfully created: {filePath}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = "MySimpleDrawing.dxf";

            // 1. Check if the file exists
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Cannot find file: {filePath}. Please generate it first!");
                return;
            }

            Console.WriteLine($"Reading CAD file: {filePath}\n");

            try
            {
                // 2. Read the document using DxfReader
                // Note: If you had a DWG file, you would use DwgReader.Read(filePath) instead
                CadDocument doc = DxfReader.Read(filePath);

                Console.WriteLine("=== ENTITIES IN MODEL SPACE ===");

                // 3. Loop through all graphical items (entities) in the document
                foreach (Entity entity in doc.Entities)
                {
                    Console.WriteLine($"Entity Type: {entity.GetType().Name}");

                    // 4. Use C# pattern matching to read specific properties based on the type
                    switch (entity)
                    {
                        case Line line:
                            Console.WriteLine($"  Start Point: X:{line.StartPoint.X}, Y:{line.StartPoint.Y}");
                            Console.WriteLine($"  End Point:   X:{line.EndPoint.X}, Y:{line.EndPoint.Y}");
                            break;

                        case Circle circle:
                            Console.WriteLine($"  Center Point: X:{circle.Center.X}, Y:{circle.Center.Y}");
                            Console.WriteLine($"  Radius:       {circle.Radius}");
                            break;

                        case LwPolyline polyline:
                            Console.WriteLine($"  Polyline with {polyline.Vertices.Count} vertices.");
                            break;

                        case TextEntity text:
                            Console.WriteLine($"  Text Value: {text.Value}");
                            break;

                        default:
                            // Catch-all for any other entity types (Arcs, Hatches, Splines, etc.)
                            Console.WriteLine("  (No specific properties extracted for this type)");
                            break;
                    }
                    Console.WriteLine(); // Add a blank line for readability
                }

                // 5. BONUS: You can also read non-graphical items, like Layers!
                Console.WriteLine("=== LAYERS IN DOCUMENT ===");
                foreach (var layer in doc.Layers)
                {
                    Console.WriteLine($"Layer Name: {layer.Name}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
            }
        }
    }
}
