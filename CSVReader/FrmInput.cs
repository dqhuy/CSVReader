using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSVReader
{
    public partial class FrmInput : Form
    {
        List<CSV2DataGridView> listcsvEditor = null;
        FrmMain parentForm = null;
        public FrmInput()
        {
            InitializeComponent();
        }

        private void LoadDataEditor()
        {
            var metadata = @"DataInput\metadata.csv";

            using (var reader = new StreamReader(metadata))
            {

                using (var csv = new CsvHelper.CsvReader(reader))
                {
                    using (var dr = new CsvHelper.CsvDataReader(csv))
                    {
                        var dt = new DataTable();
                        dt.Load(dr);

                        this.tabControl1.TabPages.Clear();

                        listcsvEditor = new List<CSV2DataGridView>();
                        foreach (DataRow row in dt.Rows)
                        {
                            var fileName = string.Format(@"DataInput\{0}", row[0].ToString());
                            if (File.Exists(fileName))
                            {
                                //tạo gridview editor
                                var dg = new DataGridView();
                                CSV2DataGridView newEditor = new CSV2DataGridView(fileName, dg);
                                newEditor.LoadData();
                                listcsvEditor.Add(newEditor);

                                //tạo nút ghi dữ liệu
                                Button saveButton = new Button();
                                saveButton.Text = "Ghi dữ liệu";

                                saveButton.Click += SaveButton_Click;

                                //tạo tab dữ liệu và tạo GUI
                                var tabname = row[1].ToString();
                                TabPage newTab = new TabPage(tabname);
                                tabControl1.TabPages.Add(newTab);

                                newTab.Controls.Add(saveButton);
                                newTab.Controls.Add(dg);

                                saveButton.Left = 10;
                                saveButton.Top = 10;

                                dg.Top = saveButton.Bottom + 10; ;
                                dg.Height = newTab.Height - dg.Top;
                                dg.Width = newTab.Width;
                                dg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                                saveButton.Font = new Font("Segoe UI", 9,FontStyle.Bold);
                                saveButton.ForeColor = Color.Blue;
                                saveButton.AutoSize = true;
                            }

                        }

                    }


                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            parentForm.showProgressBarStatus();
            var tabIndex = tabControl1.SelectedIndex;
            var csvEditor = listcsvEditor[tabIndex];
            csvEditor.SaveData();

            parentForm.hideProgressBarStatus();
        }

        private void FrmInput_Load(object sender, EventArgs e)
        {
            parentForm = (FrmMain)this.ParentForm;
            LoadDataEditor();
        }
    }
}
