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
using System.Windows.Forms.DataVisualization.Charting;

namespace CSVReader
{
    public partial class FrmResult : Form
    {
        List<CSV2DataGridView> listcsvEditor = null;
        FrmMain parentForm = null;

        public FrmResult()
        {
            InitializeComponent();
        }

        private void ShowFilenameStatus(string filename)
        {
            var form = (FrmMain)this.ParentForm;
            form.ShowStatus(filename);
        }


        private void LoadDataEditor()
        {
            var metadata = @"DataOutput\metadata.csv";

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
                            var fileName = string.Format(@"DataOutput\{0}", row[0].ToString());
                            if (File.Exists(fileName))
                            {
                                //tạo gridview editor
                                var dg = new DataGridView();
                                CSV2DataGridView newEditor = new CSV2DataGridView(fileName, dg);
                                dg.Refresh();
                                newEditor.LoadData();
                                listcsvEditor.Add(newEditor);


                                //tạo tab dữ liệu và tạo GUI
                                var tabname = row[1].ToString();
                                TabPage newTab = new TabPage(tabname);
                                tabControl1.TabPages.Add(newTab);

                                newTab.Controls.Add(dg);


                                dg.Top = 10; ;
                                dg.Height = newTab.Height - dg.Top;
                                dg.Width = newTab.Width;
                                dg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                            //    if (fileName.Contains("DB_GDP.csv"))
                                {
                                    dg.Width = 600;

                                    //tạo chart
                                    ChartArea chartArea1 = new ChartArea();
                                    chartArea1.AxisX.MajorGrid.LineColor = Color.LightGray;
                                    chartArea1.AxisY.MajorGrid.LineColor = Color.LightGray;
                                    chartArea1.AxisX.LabelStyle.Font = new Font("Consolas", 8);
                                    chartArea1.AxisY.LabelStyle.Font = new Font("Consolas", 8);

                                    dg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;

                                    Chart chart1 = new Chart();
                                    chart1.ChartAreas.Add(chartArea1);
                                    chart1.Left = dg.Width + 10;
                                    chart1.Top = dg.Top;
                                    chart1.Width = newTab.Width - chart1.Left - 10;
                                    chart1.Height = dg.Height;
                                    chart1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

                                    var xserisname = newEditor.DataSoure.Columns[0].ColumnName;
                                    for (int i = 1; i < newEditor.DataSoure.Columns.Count; i++)
                                    {
                                        chart1.Series.Add(new Series()
                                        {
                                            ChartType = SeriesChartType.Line,
                                            XValueMember = xserisname,
                                            YValueMembers = newEditor.DataSoure.Columns[i].ColumnName,
                                            LegendText = newEditor.DataSoure.Columns[i].ColumnName,
                                            IsVisibleInLegend=true,
                                        });
                                    }
                                    chart1.Legends.Add(new Legend());
                                    chart1.Titles.Add(tabname);
                                    chart1.DataSource = newEditor.DataSoure;
                                    newTab.Controls.Add(chart1);
                                }

                            }

                        }

                    }


                }
            }
        }

        private void FrmResult_Load(object sender, EventArgs e)
        {
            LoadDataEditor();
        }
    }
}
