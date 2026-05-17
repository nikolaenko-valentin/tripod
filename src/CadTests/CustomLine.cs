using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

namespace CadTests;

public class CustomLine
{
    [CommandMethod("DRAWLINE")]
    public void Draw()
    {
        var doc = Application.DocumentManager.MdiActiveDocument;
        var db = doc.Database;
        var ed = doc.Editor;

        ed.WriteMessage("\nDrawing a line from (0,0,0) to (100,100,0)...");

        using Transaction tr = db.TransactionManager.StartTransaction();
        BlockTable? bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead)
            as BlockTable;

        BlockTableRecord? modelSpace = tr.GetObject(
            bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite)
            as BlockTableRecord;

        Line line = new Line(
            new Point3d(0, 0, 0),
            new Point3d(100, 100, 0)
        );

        line.ColorIndex = 1; // Red

        _ = modelSpace.AppendEntity(line);
        tr.AddNewlyCreatedDBObject(line, true);

        tr.Commit();

        ed.WriteMessage("\nLine drawn successfully!");
    }
}
