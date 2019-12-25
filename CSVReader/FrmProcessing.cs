using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSVReader
{
    public partial class FrmProcessing : Form
    {
        string message = string.Empty;
        bool showResult = true;
        public FrmProcessing()
        {
            InitializeComponent();
        }

        private void FrmProcessing_Load(object sender, EventArgs e)
        {
            //   this.progressBar1.Visible = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                showResult = false;
                //    this.progressBar1.Visible = true;

                // Create the MATLAB instance 
                MLApp.MLApp matlab = new MLApp.MLApp();

                // Change to the directory where the function is located 
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("MatlabLib");
                matlab.Execute(@"cd " + di.FullName);

                // Define the output 
                object result = null;

                string path = Application.StartupPath;
                // Call the MATLAB function myfunc
                matlab.Feval("analysis", 1, out result,path);

                // Display result 
                object[] res = result as object[];

                message = res[0].ToString();

                showResult = true;
            }
            catch (Exception ex)
            {
                message = "Có lỗi xẩy ra khi thực thi tính toán. Lỗi: " + ex.Message;
                showResult = false;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //   this.progressBar1.Visible = false;
            if (message != null)
            {
                MessageBox.Show(message);
            }
            if (showResult)
            {
                this.DialogResult = DialogResult.OK;

            }

            this.Close();


        }
    }
}
