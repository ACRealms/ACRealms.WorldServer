using System.Collections.Generic;

namespace ACE.Server.Physics.Common
{
    public class CellArray
    {
        public bool AddedOutside;
        public bool LoadCells;
        public Dictionary<uint, ObjCell> Cells;
        public int NumCells;

        public CellArray()
        {
            Cells = new Dictionary<uint, ObjCell>();
        }

        public void SetStatic()
        {
            AddedOutside = false;
            LoadCells = false;
            NumCells = 0;
        }

        public void SetDynamic()
        {
            AddedOutside = false;
            LoadCells = true;
            NumCells = 0;
        }

        public void add_cell(uint cellID, ObjCell cell)
        {
            if (Cells.TryAdd(cellID, cell))
                NumCells++;
        }

        public void remove_cell(uint cellId)
        {
            if (Cells.Remove(cellId))
                NumCells--;
        }

        public void remove_cell(ObjCell cell)
        {
            remove_cell(cell.ID);
        }
    }
}
