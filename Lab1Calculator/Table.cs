using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab1Calculator
{
    public class Table
    {
        public int colCount;
        public int rowCount;
        public static List<List<Cell>> grid = new List<List<Cell>>();
        public Dictionary<CellCoordinates, string> dictionary = new Dictionary<CellCoordinates, string>();
        public Table(int r, int c)
        {
            setTable(r, c);
        }
        public void ShowMessageError(string s)
        {
            MessageBox.Show("Помилка в клітинці: " + s, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void setTable(int row, int col)
        {
            Clear();
            colCount = col;
            rowCount = row;
            for (int i = 0; i < rowCount; ++i)
            {
                List<Cell> newRow = new List<Cell>();
                for (int j = 0; j < colCount; ++j)
                {

                    newRow.Add(new Cell(i, j));
                    dictionary.Add(NumberConverter.From26System(newRow.Last().getName()), "");
                }
                grid.Add(newRow);
            }
        }
        
        public void Clear()
        {
            foreach (List<Cell> list in grid)
            {
                list.Clear();
            }
            grid.Clear();
            dictionary.Clear();
            rowCount = 0;
            colCount = 0;
        }

        public void Open(int rows, int columns, StreamReader reader, DataGridView dgv)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    string cellName = reader.ReadLine();
                    string expression = reader.ReadLine();
                    string value = reader.ReadLine();
                    dictionary[NumberConverter.From26System(cellName)] = value;
                    int refCount = Convert.ToInt32(reader.ReadLine());
                    List<Cell> refs = new List<Cell>();
                    string reference;
                    for (int k = 0; k < refCount; ++k)
                    {
                        reference = reader.ReadLine();
                        CellCoordinates coord = NumberConverter.From26System(reference);
                        if (coord.row < rowCount && coord.column < colCount)
                            refs.Add(grid[coord.row][coord.column]);
                    }
                    int depCount = Convert.ToInt32(reader.ReadLine());
                    List<Cell> deps= new List<Cell>();
                    string dependency;
                    for (int k = 0; k < depCount; ++k)
                    {
                        dependency = reader.ReadLine();
                        CellCoordinates coord = NumberConverter.From26System(dependency);
                        deps.Add(grid[coord.row][coord.column]);
                    }
                    grid[i][j].setCell(expression, value, refs, deps);
                    grid[i][j].value = value;
                    grid[i][j].expression = expression;
                    int asize = dgv.Rows.Count;
                    int bsize = dgv.Columns.Count;
                    dgv[j, i].Value = expression;
                }
            }
        }

        public void Save(StreamWriter writer)
        {
            writer.WriteLine(rowCount);
            writer.WriteLine(colCount);
            foreach(var list in grid)
            {
                foreach (var cell in list)
                {
                    writer.WriteLine(cell.getName());
                    writer.WriteLine(cell.expression);
                    writer.WriteLine(cell.value);
                    if (cell.referencesFromThis == null) writer.WriteLine("0");
                    else
                    {
                        writer.WriteLine(cell.referencesFromThis.Count);
                        foreach (var pointCell in cell.referencesFromThis) writer.WriteLine(pointCell.getName());
                    }
                    if (cell.DependenciesToThis == null) writer.WriteLine("0");
                    else
                    {
                        writer.WriteLine(cell.DependenciesToThis.Count);
                        foreach (var pointCell in cell.DependenciesToThis) writer.WriteLine(pointCell.getName());
                    }
                }
            }
        }

        public void UpdateCellWithAllDependencies(int row, int col, string expression, DataGridView dgv)
        {
            List<Cell> new_referencesFromThis = new List<Cell>();
            grid[row][col].DeleteDependenciesAndReferences();
            grid[row][col].expression = expression;
            grid[row][col].new_referencesFromThis.Clear();
            if (expression != "" && expression[0] != '=')
            {
                    grid[row][col].value = expression;
                    dictionary[NumberConverter.From26System(FullName(row, col))] = expression;
                    foreach (Cell cell in grid[row][col].DependenciesToThis) UpdateCellAndDependencies(cell, dgv);
                    return;
            }
            string new_expression = ConvertReferences(row, col, expression);
            if (new_expression != "")
            {
                new_expression = new_expression.Remove(0, 1);
            }
            if (!grid[row][col].CheckLoop(grid[row][col].new_referencesFromThis))
            {
                MessageBox.Show("Було створено цикл! Змініть вирази.", "Цикл", MessageBoxButtons.OK, MessageBoxIcon.Error);
                grid[row][col].expression = "";
                grid[row][col].value = "0";
                dgv[col, row].Value = "0";
                return;
            }
            grid[row][col].AddDependenciesAndReferences();
            string val = Calculate(new_expression);
            if (val == "Помилка")
            {
                ShowMessageError(FullName(row, col));
                grid[row][col].expression = "";
                grid[row][col].value = "0";
                dgv[col, row].Value = "0";
                UpdateCellWithAllDependencies(row, col, "", dgv);
                return;
            }
            grid[row][col].value = val;
            dictionary[NumberConverter.From26System(FullName(row, col))] = val;
            foreach (Cell cell in grid[row][col].DependenciesToThis)
                UpdateCellAndDependencies(cell, dgv);
        }
        private string FullName(int row, int col)
        {
            Cell cell = new Cell(row, col);
            return cell.getName();
        }
        public bool UpdateCellAndDependencies(Cell cell, DataGridView dgv)
        {
            cell.new_referencesFromThis.Clear();
            string new_expression = ConvertReferences(cell.row, cell.column, cell.expression);
            new_expression = new_expression.Remove(0, 1);
            string Value = Calculate(new_expression);
            if (Value == "Помилка")
            {
                ShowMessageError(cell.getName());
                cell.expression = "";
                cell.value = "0";
                dgv[cell.column, cell.row].Value = "0";
                return false;
            }
            grid[cell.row][cell.column].value = Value;
            dictionary[NumberConverter.From26System(FullName(cell.row, cell.column))] = Value;
            if (!Program.formulaMode) dgv[cell.column, cell.row].Value = grid[cell.row][cell.column].value;
            else dgv[cell.column, cell.row].Value = grid[cell.row][cell.column].expression;
            foreach (Cell point in cell.DependenciesToThis)
            {
                if (!UpdateCellAndDependencies(point, dgv)) return false;
            }
            return true;
        }
        public string ConvertReferences(int row, int col, string expr)
        {
            string cellPattern = @"[A-Z]+[0-9]+";
            Regex regex = new Regex(cellPattern, RegexOptions.IgnoreCase);
            CellCoordinates nums;
            foreach (Match match in regex.Matches(expr))
            {
                if (dictionary.ContainsKey(NumberConverter.From26System(match.Value)))
                {
                    nums = NumberConverter.From26System(match.Value);
                    grid[row][col].new_referencesFromThis.Add(grid[nums.row][nums.column]);
                }
            }
            MatchEvaluator evaluator = new MatchEvaluator(ReferenceToValue);
            string new_expression = regex.Replace(expr, evaluator);
            return new_expression;
        }
        public string ReferenceToValue(Match m)
        {
            if (dictionary.ContainsKey(NumberConverter.From26System(m.Value)) && dictionary[NumberConverter.From26System(m.Value)] == "") return "0";
            else if (dictionary.ContainsKey(NumberConverter.From26System(m.Value))) return dictionary[NumberConverter.From26System(m.Value)];
            else return m.Value;
        }

        public string Calculate(string expression)
        {
            string res;
            try
            {
                res = (Calculator.Evaluate(expression)).ToString("G12", CultureInfo.CreateSpecificCulture("ua-UK"));
                if (res == "∞" || res == "-∞" || res == "NaN" || res == "-NaN")
                {
                    res = "Помилка";
                }
                return res;
            }
            catch
            {
                MessageBox.Show("Помилка в поточній клітинці", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Помилка";
            }
        }

        public void AddRow(DataGridView dgv)
        {
            List<Cell> newRow = new List<Cell>();
            for (int j = 0; j < colCount; ++j)
            {
                Cell newCell = new Cell(rowCount, j);
                newRow.Add(newCell);
                dictionary.Add(NumberConverter.From26System(newCell.getName()), "");
            }
            grid.Add(newRow);
            for (int j = 0; j < colCount; j++)
            {
                UpdateCellWithAllDependencies(rowCount, j, "", dgv);
            }
            ++rowCount;
        }
        public void AddColumn(DataGridView dgv)
        {
            for (int j = 0; j < rowCount; ++j)
            {
                Cell newCell = new Cell(j, colCount);
                grid[j].Add(newCell);
            }
            for (int j = 0; j < rowCount; ++j)
            {
                UpdateCellWithAllDependencies(j, colCount, "", dgv);
            }
            ++colCount;
        }


        public bool DeleteRow(DataGridView dgv)
        {
            List<Cell> pointsLastRow = new List<Cell>();
            List<Cell> cellsWithValue = new List<Cell>();
            if (rowCount == 1)
            {
                MessageBox.Show("У таблиці не може бути менше одного рядка", "Операція відмінена");
                return false;
            }
            int curCount = rowCount - 1;
            for (int i = 0; i < colCount; ++i)
            { 
                if (grid[curCount][i].expression != "") cellsWithValue.Add(grid[curCount][i]);
                if (grid[curCount][i].DependenciesToThis.Count != 0) pointsLastRow.AddRange(grid[curCount][i].DependenciesToThis);                
            }
            if (pointsLastRow.Count > 0)
            {
                string cellsNeedToDelete = "Для того, щоб видалити рядок потрібно видалити посилання з таких клітинок:\n";
                foreach (Cell cell in pointsLastRow)
                {
                    cellsNeedToDelete += cell.getName() + " ";
                }
                MessageBox.Show(cellsNeedToDelete, "Видалення не відбулося");
                return false;
            }
            foreach (var cell in cellsWithValue)
            {
                UpdateCellWithAllDependencies(cell.row, cell.column, "", dgv);
            }
            for (int i = 0; i < colCount; ++i)
            {
                CellCoordinates coord = NumberConverter.From26System(grid[curCount][i].getName());
                dictionary.Remove(coord);
            }
            grid.RemoveAt(curCount);
            --rowCount;
            return true;
        }
        public bool DeleteColumn(DataGridView dgv)
        {
            List<Cell> pointsLastCol = new List<Cell>();
            List<Cell> cellsWithValue = new List<Cell>();
            if (colCount == 1)
            {
                MessageBox.Show("У таблиці не може бути менше одного стовпця", "Операція відмінена");
                return false;
            }
            int curCount = colCount - 1;
            if (pointsLastCol.Count > 0)
            {
                string cellsNeedToDelete = "Для того, щоб видалити стовпець потрібно видалити посилання з таких клітинок:\n";
                foreach (Cell cell in pointsLastCol)
                {
                    cellsNeedToDelete += cell.getName() + " ";
                }
                MessageBox.Show(cellsNeedToDelete, "Видалення не відбулося");
                return false;
            }
            for (int i = 0; i < rowCount; ++i)
            {
                if (grid[i][curCount].expression != "") cellsWithValue.Add(grid[i][curCount]);
                if (grid[i][curCount].DependenciesToThis.Count != 0) pointsLastCol.AddRange(grid[i][curCount].DependenciesToThis);
            }
            foreach (var cell in cellsWithValue)
            {
                UpdateCellWithAllDependencies(cell.row, cell.column, "", dgv);
            }
            for (int i = 0; i < rowCount; ++i)
            {
                CellCoordinates coord = NumberConverter.From26System(grid[i][curCount].getName());
                dictionary.Remove(coord);
            }
            for (int i = 0; i < rowCount; ++i)
            {
                grid[i].RemoveAt(curCount);
            }
                --colCount;
            return true;
        }
    }
}
