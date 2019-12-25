using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data;

namespace CSVReader
{
    public class CSV2DataGridView
    {
        string csvFileName = string.Empty;
        DataGridView datagridView = null;

        public DataTable DataSoure { get; set; }

        public CSV2DataGridView(string filename, DataGridView dg)
        {
            this.csvFileName = filename;
            this.datagridView = dg;
            this.datagridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.datagridView.CellEnter += DatagridView_CellEnter;
            this.datagridView.CellLeave += DatagridView_CellLeave;
            this.datagridView.Leave += DatagridView_Leave;
            
        }

        private void DatagridView_Leave(object sender, EventArgs e)
        {
        //    this.datagridView.Refresh();
        }

        private void DatagridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            datagridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;            
            
        }

        private void DatagridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            datagridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = false;

        }

        /// <summary>
        /// Nạp dữ liệu từ file csv -> DataGridView
        /// </summary>
        public void LoadData()
        {
            try
            {
                using (var reader = new StreamReader(this.csvFileName))
                {

                    using (var csv = new CsvHelper.CsvReader(reader))
                    {
                        using (var dr = new CsvHelper.CsvDataReader(csv))
                        {
                            var dt = new DataTable();
                            dt.Load(dr);


                            this.datagridView.AutoGenerateColumns = true;
                            for (int i= 0; i < dt.Columns.Count; i++)
                            {
                                dt.Columns[i].ReadOnly = false;
                            }
                            this.DataSoure = dt;
                            this.datagridView.DataSource = dt;
                           
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Có lỗi xảy ra khi đọc file:{0}. Chi tiết lỗi: {1}", csvFileName, ex.Message));
            }
        }


        /// <summary>
        /// Lưu dữ liệu từ DataGridView -> file csv
        /// </summary>
        public void SaveData()
        {
            try
            {
                DataTable data = (DataTable)this.datagridView.DataSource;
                using (var writer = new StreamWriter(this.csvFileName))
                {
                    using (var csv = new CsvHelper.CsvWriter(writer))
                    {
                        //write header
                        foreach (DataColumn c in data.Columns)
                        {
                            csv.WriteField(c.ColumnName);
                        }
                        csv.NextRecord();

                        //write records
                        foreach (DataRow r in data.Rows)
                        {
                            for (var i = 0; i < data.Columns.Count; i++)
                            {
                                csv.WriteField(r[i]);
                            }
                            csv.NextRecord();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Có lỗi xảy ra khi lưu file: {0}. Chi tiết lỗi: {1}", csvFileName, ex.Message));

            }
        }
    }
}
