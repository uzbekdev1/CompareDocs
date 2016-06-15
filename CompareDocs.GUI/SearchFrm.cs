using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CompareDocs.Extensions;
using CompareDocs.GUI.Properties;

namespace CompareDocs.GUI
{
    public partial class SearchFrm : Form
    {
        private readonly IEnumerable<FileInfo> _files;

        public SearchFrm()
        {
            _files = Helpers.GetFiles(Settings.Default.DataDrectory);

            InitializeComponent();
        }

        private void SearchFrm_Load(object sender, EventArgs e)
        {
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // clear list items before adding 
            listView1.Items.Clear();

            // filter the items match with search key and add result to list view 
            if (textBox1.TextLength == 0)
            {
                foreach (var file in _files)
                {
                    listView1.Items.Add(
                        new ListViewItem(new[] { file.DirectoryName.Replace(Settings.Default.DataDrectory, "") + "\\" + file.Name }));
                }
            }
            else
            {
                foreach ( var file in _files.Where(i =>   i.Name.Contains(textBox1.Text)))
                {
                    listView1.Items.Add(
                        new ListViewItem(new[] { file.DirectoryName.Replace(Settings.Default.DataDrectory, "") + "\\" + file.Name }));
                }
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var root = Settings.Default.DataDrectory;
            var filePath = root + listView1.SelectedItems[0].SubItems[0].Text;

            try
            {
                Process.Start(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}