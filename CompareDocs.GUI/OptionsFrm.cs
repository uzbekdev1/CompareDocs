using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CompareDocs.GUI
{
    public partial class OptionsFrm : Form
    {
        public OptionsFrm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var folder = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.DesktopDirectory,
                SelectedPath = Environment.CurrentDirectory,
                Description = @"Please choose data directory"
            };

            if (folder.ShowDialog(this) != DialogResult.OK)
                return;

            textBox1.Text = folder.SelectedPath;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DataDrectory = textBox1.Text;
            Properties.Settings.Default.Save(); 
        }

        private void OptionsFrm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.DataDrectory;
        }
    }
}
