using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Lab1Calculator {
    public class Cell {
        public string expression { get; set; }
        public string value { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        string name { get; set; }

        public List<Cell> DependenciesToThis = new List<Cell>();
        public List<Cell> referencesFromThis = new List<Cell>();
        public List<Cell> new_referencesFromThis = new List<Cell>();
        public Cell(int r, int c)
        {
            row = r;
            column = c;          
            name = NumberConverter.FromIntegerToChars(c) + Convert.ToString(r + 1);
            value = "0";
            expression = "";
        }

        public void setCell(string expr, string val, List<Cell> refs, List<Cell> deps)
        {
            this.value = "0";
            this.expression = "";
            this.referencesFromThis.Clear();
            this.referencesFromThis.AddRange(refs);
            this.DependenciesToThis.Clear();
            this.DependenciesToThis.AddRange(deps);
        }
        public string getName()
        {
            return name;
        }
        public bool CheckLoop(List<Cell> list)
        { 
                foreach (Cell cell in list)
                {
                    if (cell.name == name) return false;
                }
                foreach (Cell point in DependenciesToThis)
                {
                    foreach (Cell cell in list)
                    {
                        if (cell.name == point.name) return false;
                    }

                    if (!point.CheckLoop(list)) return false;
                }
                return true;
            }
        public void AddDependenciesAndReferences()
        {
        foreach (Cell point in new_referencesFromThis) point.DependenciesToThis.Add(this);
             referencesFromThis = new_referencesFromThis;
        }
        public void DeleteDependenciesAndReferences()
        {
            if (referencesFromThis != null)
            {
                foreach (Cell point in referencesFromThis) point.DependenciesToThis.Remove(this);
                referencesFromThis.Clear();
            }
        }
    }

}