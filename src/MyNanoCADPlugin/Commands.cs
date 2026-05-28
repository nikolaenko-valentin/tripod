using HostMgd.EditorInput;
using MyNanoCADPlugin.Forms;
using Teigha.Runtime;
using Application = HostMgd.ApplicationServices.Application;

namespace MyNanoCADPlugin
{
    public class Commands
    {
        [CommandMethod("MYPLUGIN", CommandFlags.Modal)]
        public void ShowMainForm()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
            {
                MessageBox.Show("No active document!");
                return;
            }

            try
            {
                var form = new Forms.MainForm(doc);
                form.ShowDialog();
            }
            catch (System.Exception ex)
            {
                doc.Editor.WriteMessage(
                    $"\nError: {ex.Message}\n"
                );
            }
        }

        [CommandMethod("MYPLUGIN_LINE", CommandFlags.Modal)]
        public void DrawLine()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var ed = doc.Editor;

            try
            {
                // Get start point from user
                var startOpts = new PromptPointOptions(
                    "\nSpecify start point: "
                );
                var startResult = ed.GetPoint(startOpts);
                if (startResult.Status != PromptStatus.OK) return;

                // Get end point from user
                var endOpts = new PromptPointOptions(
                    "\nSpecify end point: "
                );
                endOpts.BasePoint = startResult.Value;
                endOpts.UseBasePoint = true;

                var endResult = ed.GetPoint(endOpts);
                if (endResult.Status != PromptStatus.OK) return;

                // Draw line
                var service = new Services.DrawingService(doc);
                service.DrawLine(
                    startResult.Value,
                    endResult.Value
                );

                ed.WriteMessage("\n✅ Line drawn!\n");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\n❌ Error: {ex.Message}\n");
            }
        }

        [CommandMethod("MYPLUGIN_CIRCLE", CommandFlags.Modal)]
        public void DrawCircle()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var ed = doc.Editor;

            try
            {
                // Get center point
                var centerOpts = new PromptPointOptions(
                    "\nSpecify center point: "
                );
                var centerResult = ed.GetPoint(centerOpts);
                if (centerResult.Status != PromptStatus.OK) return;

                // Get radius
                var radiusOpts = new PromptDistanceOptions(
                    "\nSpecify radius: "
                );
                radiusOpts.BasePoint = centerResult.Value;
                radiusOpts.UseBasePoint = true;

                var radiusResult = ed.GetDistance(radiusOpts);
                if (radiusResult.Status != PromptStatus.OK) return;

                // Draw circle
                var service = new Services.DrawingService(doc);
                service.DrawCircle(
                    centerResult.Value,
                    radiusResult.Value
                );

                ed.WriteMessage("\n✅ Circle drawn!\n");
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage($"\n❌ Error: {ex.Message}\n");
            }
        }

        [CommandMethod("MYPLUGIN_INFO", CommandFlags.Modal)]
        public void ShowInfo()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var ed = doc.Editor;
            var db = doc.Database;

            ed.WriteMessage("\n=== Document Info ===\n");
            ed.WriteMessage($"File: {doc.Name}\n");
            ed.WriteMessage($"Entities: {CountEntities(db)}\n");
            ed.WriteMessage($"Layers: {CountLayers(db)}\n");
            ed.WriteMessage("====================\n");
        }

        private int CountEntities(Teigha.DatabaseServices.Database db)
        {
            try
            {
                using var tr = db.TransactionManager.StartTransaction();
                var bt = (Teigha.DatabaseServices.BlockTable)tr.GetObject(
                    db.BlockTableId,
                    Teigha.DatabaseServices.OpenMode.ForRead
                );
                var ms = (Teigha.DatabaseServices.BlockTableRecord)tr.GetObject(
                    bt[Teigha.DatabaseServices.BlockTableRecord.ModelSpace],
                    Teigha.DatabaseServices.OpenMode.ForRead
                );
                int count = 0;
                foreach (var id in ms) count++;
                tr.Commit();
                return count;
            }
            catch { return -1; }
        }

        private int CountLayers(Teigha.DatabaseServices.Database db)
        {
            try
            {
                using var tr = db.TransactionManager.StartTransaction();
                var lt = (Teigha.DatabaseServices.LayerTable)tr.GetObject(
                    db.LayerTableId,
                    Teigha.DatabaseServices.OpenMode.ForRead
                );
                int count = 0;
                foreach (var id in lt) count++;
                tr.Commit();
                return count;
            }
            catch { return -1; }
        }
    }
}
