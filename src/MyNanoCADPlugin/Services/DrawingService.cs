using HostMgd.ApplicationServices;
using Teigha.DatabaseServices;
using Teigha.Geometry;

namespace MyNanoCADPlugin.Services
{
    public class DrawingService
    {
        private readonly Document _doc;
        private readonly Database _db;

        public DrawingService(Document doc)
        {
            _doc = doc;
            _db = doc.Database;
        }

        // ─── Line ──────────────────────────────────────────────────────
        public bool DrawLine(
            Point3d start,
            Point3d end,
            string layer = "0")
        {
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var modelSpace = GetModelSpace(tr);

                var line = new Line(start, end);
                line.Layer = layer;

                modelSpace.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);
                tr.Commit();

                return true;
            }
            catch (Exception ex)
            {
                WriteError("DrawLine", ex);
                return false;
            }
        }

        // ─── Circle ────────────────────────────────────────────────────
        public bool DrawCircle(
            Point3d center,
            double radius,
            string layer = "0")
        {
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var modelSpace = GetModelSpace(tr);

                var circle = new Circle(
                    center,
                    Vector3d.ZAxis,
                    radius
                );
                circle.Layer = layer;

                modelSpace.AppendEntity(circle);
                tr.AddNewlyCreatedDBObject(circle, true);
                tr.Commit();

                return true;
            }
            catch (Exception ex)
            {
                WriteError("DrawCircle", ex);
                return false;
            }
        }

        // ─── Rectangle ─────────────────────────────────────────────────
        public bool DrawRectangle(
            Point3d origin,
            double width,
            double height,
            string layer = "0")
        {
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var modelSpace = GetModelSpace(tr);

                var pline = new Polyline();
                pline.AddVertexAt(0,
                    new Point2d(origin.X, origin.Y), 0, 0, 0);
                pline.AddVertexAt(1,
                    new Point2d(origin.X + width, origin.Y), 0, 0, 0);
                pline.AddVertexAt(2,
                    new Point2d(origin.X + width, origin.Y + height), 0, 0, 0);
                pline.AddVertexAt(3,
                    new Point2d(origin.X, origin.Y + height), 0, 0, 0);
                pline.Closed = true;
                pline.Layer = layer;

                modelSpace.AppendEntity(pline);
                tr.AddNewlyCreatedDBObject(pline, true);
                tr.Commit();

                return true;
            }
            catch (Exception ex)
            {
                WriteError("DrawRectangle", ex);
                return false;
            }
        }

        // ─── Text ──────────────────────────────────────────────────────
        public bool DrawText(
            string text,
            Point3d position,
            double height = 2.5,
            string layer = "0")
        {
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var modelSpace = GetModelSpace(tr);

                var dbText = new DBText
                {
                    TextString = text,
                    Position = position,
                    Height = height,
                    Layer = layer
                };

                modelSpace.AppendEntity(dbText);
                tr.AddNewlyCreatedDBObject(dbText, true);
                tr.Commit();

                return true;
            }
            catch (Exception ex)
            {
                WriteError("DrawText", ex);
                return false;
            }
        }

        // ─── Helpers ───────────────────────────────────────────────────
        private BlockTableRecord GetModelSpace(Transaction tr)
        {
            var bt = (BlockTable)tr.GetObject(
                _db.BlockTableId,
                OpenMode.ForRead
            );
            return (BlockTableRecord)tr.GetObject(
                bt[BlockTableRecord.ModelSpace],
                OpenMode.ForWrite
            );
        }

        private void WriteError(string method, Exception ex)
        {
            _doc.Editor.WriteMessage(
                $"\n❌ {method} Error: {ex.Message}\n"
            );
        }
    }
}
