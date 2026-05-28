using Teigha.DatabaseServices;
using Teigha.Colors;
using HostMgd.ApplicationServices;

namespace MyNanoCADPlugin.Services
{
    public class LayerService
    {
        private readonly Document _doc;
        private readonly Database _db;

        public LayerService(Document doc)
        {
            _doc = doc;
            _db = doc.Database;
        }

        public bool CreateLayer(
            string name,
            short colorIndex = 7)
        {
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var lt = (LayerTable)tr.GetObject(
                    _db.LayerTableId,
                    OpenMode.ForWrite
                );

                if (!lt.Has(name))
                {
                    var ltr = new LayerTableRecord
                    {
                        Name = name,
                        Color = Teigha.Colors.Color.FromColorIndex(
                            ColorMethod.ByAci, colorIndex)
                    };

                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }

                tr.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _doc.Editor.WriteMessage(
                    $"\n❌ CreateLayer Error: {ex.Message}\n"
                );
                return false;
            }
        }

        public List<string> GetAllLayers()
        {
            var layers = new List<string>();
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var lt = (LayerTable)tr.GetObject(
                    _db.LayerTableId,
                    OpenMode.ForRead
                );

                foreach (var id in lt)
                {
                    var ltr = (LayerTableRecord)tr.GetObject(
                        id, OpenMode.ForRead
                    );
                    layers.Add(ltr.Name);
                }

                tr.Commit();
            }
            catch (Exception ex)
            {
                _doc.Editor.WriteMessage(
                    $"\n❌ GetAllLayers Error: {ex.Message}\n"
                );
            }

            return layers;
        }

        public bool SetCurrentLayer(string name)
        {
            try
            {
                using var tr = _db.TransactionManager.StartTransaction();

                var lt = (LayerTable)tr.GetObject(
                    _db.LayerTableId,
                    OpenMode.ForRead
                );

                if (lt.Has(name))
                {
                    _db.Clayer = lt[name];
                    tr.Commit();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _doc.Editor.WriteMessage(
                    $"\n❌ SetCurrentLayer Error: {ex.Message}\n"
                );
                return false;
            }
        }
    }
}
