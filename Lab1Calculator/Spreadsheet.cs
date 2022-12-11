using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab1Calculator
{
    public partial class Spreadsheet : Form
    {
        private const int defaultCol = 10;
        private const int defaultRow = 10;
        Table table = new Table(defaultCol, defaultRow);
        public Spreadsheet()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            InitializeDataGridView(defaultCol, defaultRow);
        }
        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            AddColumn();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddRow();
        }

        private void btnDeleteColumn_Click(object sender, EventArgs e)
        {
            DeleteColumn();
        }
        
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            DeleteRow();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            Calculate();
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            EditCell();
        }

        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            EditCell();
        }

        private void btnChangeMode_Click(object sender, EventArgs e)
        {
            ChangeMode();
        }

        private void FillHeaders(int rows, int columns) {
            for (int i = 0; i < columns; ++i)
            {
                string columnName = NumberConverter.FromIntegerToChars(i);
                dgv.Columns[i].Name = columnName;
                dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < rows; ++i)
            {
                dgv.Rows.Add("");
                dgv.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }
        private void InitializeDataGridView(int rows, int columns)
        {
            dgv.ColumnCount = columns;
            FillHeaders(rows, columns);
            dgv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            table.setTable(rows, columns);
        }

        public void OpenFile()
        {
            openFileDialog.Filter = "TableFile|*.txt";
            openFileDialog.Title = "Відкрити";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            StreamReader reader = new StreamReader(openFileDialog.FileName);
            table.Clear();
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            int row, col;
            Int32.TryParse(reader.ReadLine(), out row);
            Int32.TryParse(reader.ReadLine(), out col);
            InitializeDataGridView(row, col);
            table.Open(row, col, reader, dgv);
            reader.Close();
        }

        public void SaveFile()
        {
            saveFileDialog.Filter = "TableFile|*.txt";
            saveFileDialog.Title = "Зберегти файл як...";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                FileStream fileStream = (FileStream)saveFileDialog.OpenFile();
                StreamWriter writer = new StreamWriter(fileStream);
                table.Save(writer);
                writer.Close();
                fileStream.Close();
            }
        }

        private void EditCell()
        {
            int col = dgv.SelectedCells[0].ColumnIndex;
            int row = dgv.SelectedCells[0].RowIndex;
            string expression = "";
            try
            {
                expression = Table.grid[row][col].expression;
            }
            catch
            {
                return;
            }
            string value = Table.grid[row][col].value;
            formulaBar.Text = expression;
            formulaBar.Focus();
        }

        private void Calculate()
        {
            int col = dgv.SelectedCells[0].ColumnIndex;
            int row = dgv.SelectedCells[0].RowIndex;
            string expression = formulaBar.Text;
            if (expression == "") return;
            table.UpdateCellWithAllDependencies(row, col, expression, dgv);
            if (!Program.formulaMode) dgv[col, row].Value = Table.grid[row][col].value;
            else dgv[col, row].Value = Table.grid[row][col].expression;
        }

        public void ChangeMode()
        {
            Program.formulaMode = !Program.formulaMode;
            for (int row = 0; row < dgv.RowCount; ++row)
            {
                for (int col = 0; col < dgv.ColumnCount; ++col)
                {
                    if (!Program.formulaMode) dgv[col, row].Value = Table.grid[row][col].value;
                    else dgv[col, row].Value = Table.grid[row][col].expression;
                }
            }
        }

        private void AddRow()
        {
            ChangeMode();
            DataGridViewRow row = new DataGridViewRow();
            dgv.Rows.Add(row);
            dgv.Rows[table.rowCount].HeaderCell.Value = (table.rowCount + 1).ToString();
            table.AddRow(dgv);
            ChangeMode();
        }

        private void AddColumn()
        {
            ChangeMode();
            string name = NumberConverter.FromIntegerToChars(table.colCount);
            dgv.Columns.Add(name, name);
            table.AddColumn(dgv);
            ChangeMode();
        }

        private void DeleteColumn()
        {
            if (!table.DeleteColumn(dgv)) return;
            dgv.Columns.RemoveAt(table.colCount);
        }

        private void DeleteRow()
        {
            if (!table.DeleteRow(dgv)) return;
            dgv.Rows.RemoveAt(table.rowCount);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Spreadsheet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (
            MessageBox.Show(
                "Бажаєте вийти з програми?", "Вихід",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.None,
                MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}