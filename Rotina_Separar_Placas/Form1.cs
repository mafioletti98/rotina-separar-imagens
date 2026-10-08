using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using System.Data;




namespace Rotina_Separar_Placas
{
    public partial class Form1 : Form
    {

        public string selectedExcelFilePath1;
        public string selectedImageDirectory1;
        public string selectedDestinationDirectory1;


        public Form1()
        {
            InitializeComponent();
        }

        private void btnDirMaster_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedImageDirectory1 = folderBrowserDialog.SelectedPath;
                    btnDirMaster.Text = selectedImageDirectory1;

                    MessageBox.Show("Diretório selecionado: " + selectedImageDirectory1);
                }
            }

        }

        private void btnDirDestino_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    selectedDestinationDirectory1 = fbd.SelectedPath;
                    btnDirDestino.Text = selectedDestinationDirectory1;
                }
            }
        }

        private void btnExecutar_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string cell = dataGridView1.Rows[i].Cells[1].Value.ToString();
                string caminhoCompleto = Path.Combine(selectedImageDirectory1, cell);
                if (File.Exists(caminhoCompleto))
                {
                    string caminhoCompletoDestino = Path.Combine(selectedDestinationDirectory1, cell);
                    if (!File.Exists(caminhoCompletoDestino))
                    {
                        string criarDiretorio = Path.GetDirectoryName(cell);
                        string caminhoCompleto2 = Path.Combine(selectedDestinationDirectory1, criarDiretorio);
                        Directory.CreateDirectory(caminhoCompleto2);
                    }
                    string caminhoCompleto3 = Path.Combine(selectedDestinationDirectory1, cell);
                    File.Copy(caminhoCompleto, caminhoCompleto3);
                }
            }
            MessageBox.Show("Feito!");
        }

        string arquivoXls;

        private void btnSelecionarArquivo_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                arquivoXls = ofd.FileName;
                MessageBox.Show("Arquivo Xls Selecionado");
                PreencherDataGridComExcel(arquivoXls, dataGridView1);
                dataGridView1.ClearSelection();
            }
        }

        public void PreencherDataGridComExcel(string filePath, DataGridView dataGridView)
        {
            try
            {
                IWorkbook workbook;
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (Path.GetExtension(filePath).ToLower() == ".xls")
                    {
                        workbook = new HSSFWorkbook(fileStream); // Para arquivos .xls
                    }
                    else if (Path.GetExtension(filePath).ToLower() == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(fileStream); // Para arquivos .xlsx
                    }
                    else
                    {
                        throw new Exception("Formato de arquivo não suportado");
                    }
                }

                ISheet sheet = workbook.GetSheetAt(0); // Lê a primeira aba da planilha
                DataTable dt = new DataTable();

                // Ler as colunas (cabeçalho)
                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                for (int i = 0; i < cellCount; i++)
                {
                    dt.Columns.Add(headerRow.GetCell(i).ToString());
                }

                // Ler as linhas (dados)
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = dt.NewRow();

                    for (int j = 0; j < cellCount; j++)
                    {
                        dataRow[j] = row.GetCell(j)?.ToString() ?? string.Empty;
                    }

                    dt.Rows.Add(dataRow);
                }

                // Vincular o DataTable ao DataGridView
                dataGridView.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao ler o arquivo Excel: " + ex.Message);
            }
        }
    }
}






