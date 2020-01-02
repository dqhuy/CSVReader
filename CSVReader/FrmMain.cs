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
    public partial class FrmMain : Form
    {
        private int childFormNumber = 0;

        public FrmMain()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            FrmInput childForm = new FrmInput();
            OpenForm(childForm);
        }

        private void OpenFile(object sender, EventArgs e)
        {
            FrmProcessing frm = new FrmProcessing();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                viewResultToolStripButton.PerformClick();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chưa cài đặt...!");
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }



        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        public void ShowStatus(string message)
        {
            this.toolStripStatusLabel.Text = message;
        }

        private void viewResultToolStripButton_Click(object sender, EventArgs e)
        {
            FrmResult childForm = new FrmResult();
            OpenForm(childForm);
        }
        public void showProgressBarStatus()
        {
            toolStripProgressBar1.Visible = true;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
        }
        public void hideProgressBarStatus()
        {
            toolStripProgressBar1.Visible = false;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

            hideProgressBarStatus();
            FrmWelcome frm = new FrmWelcome();
            OpenForm(frm);
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"help\help_eia.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Không tìm thấy file trợ giúp: Help\Help_EIA.pdf");
            }
        }

        private void OpenForm(Form child)
        {
            var parent = this;

            if (child == null) return;
            var activeChild = child;
            var isChildExist = false;
            if (parent.HasChildren)
            {
                foreach (var childItem in parent.MdiChildren)
                {
                    if (childItem.GetType() == child.GetType() && !child.IsDisposed)
                    {
                        activeChild = childItem;
                        isChildExist = true;
                        break;
                    }

                }
            }
            if (!isChildExist)
            {
                activeChild.MdiParent = parent;
                activeChild.WindowState = FormWindowState.Minimized;
                activeChild.Show(); //để fixbug form ko maxized
                activeChild.WindowState = FormWindowState.Maximized;
                activeChild.Show();
            }
            else
            {
                activeChild.Show();
                activeChild.Activate();
            }
        }
    }
}
