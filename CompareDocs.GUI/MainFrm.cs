using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CompareDocs.Extensions;

namespace CompareDocs.GUI
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutFrm().ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OptionsFrm().ShowDialog();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SearchFrm().Show();
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = Environment.CurrentDirectory + @"\" + "help.chm";

            try
            {
                Process.Start(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var file = new OpenFileDialog
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "*.docx",
                DereferenceLinks = true,
                InitialDirectory = Environment.CurrentDirectory,
                ShowHelp = true,
                RestoreDirectory = true,
                SupportMultiDottedExtensions = true,
                Title = @"Please select Doc file",
                Filter = @"Doc files|" + Helpers.Options.FILTER,
                ValidateNames = true
            };

            if (file.ShowDialog() != DialogResult.OK)
                return;

            new CompareFrm(file.FileName).Show();
        }
    }
}
