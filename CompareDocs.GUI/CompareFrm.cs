using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CompareDocs.Comparer;
using CompareDocs.Exporter;
using CompareDocs.Extensions;
using CompareDocs.GUI.Properties;
using CompareDocs.Models;

namespace CompareDocs.GUI
{
    public partial class CompareFrm : Form
    {
        private string _filePath;
        private BackgroundWorker _worker;

        public CompareFrm(string filePath)
        {
            _filePath = filePath;

            InitializeComponent();
        }

        private void CompareFrm_Load(object sender, EventArgs e)
        {
            textBox1.Text = _filePath;
            listView1.Sorting = SortOrder.None;

            Run();
        }

        private void Run()
        {
            button1.Enabled = false;
            button3.Enabled = false;
            listView1.Items.Clear();

            _worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _worker.DoWork += WorkerOnDoWork;
            _worker.ProgressChanged += WorkerOnProgressChanged;
            _worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (runWorkerCompletedEventArgs.Cancelled)
                MessageBox.Show("Cancalled", "Cancel", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                if (runWorkerCompletedEventArgs.Error != null)
                    MessageBox.Show(runWorkerCompletedEventArgs.Error.Message, @"Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                else
                {
                    var results = (IList<CompareResult>)runWorkerCompletedEventArgs.Result;

                    foreach (var result in results.OrderByDescending(o => o.Result).Take(10))
                    {
                        listView1.Items.Add(new ListViewItem(new[]
                        {
                            result.FileName,
                            result.Percent,
                            result.Elapsed
                        })
                        {
                            Tag = result.ComparedFile
                        });
                    }

                    _filePath = string.Empty;
                    progressBar1.Value = 0;

                    MessageBox.Show("Finished", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            button1.Enabled = true;
            button3.Enabled = true;
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            progressBar1.Value = progressChangedEventArgs.ProgressPercentage;
        }

        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var files = Helpers.GetFiles(Settings.Default.DataDrectory);
            var index = 0;
            var results = new List<CompareResult>();

            foreach (var file in files)
            {
                index++;

                var comparer = new DocComparer(file.FullName, _filePath);
                var operationPercent = (int)(index * 100F / files.Count());
                var comparerResult = comparer.Compare();
                var comparerFile = file.DirectoryName.Replace(Settings.Default.DataDrectory, "") + "\\" + file.Name;

                _worker.ReportProgress(operationPercent);

                results.Add(new CompareResult
                {
                    FileName = comparerFile,
                    Result = comparerResult,
                    Elapsed = comparer.ElapsedTime,
                    ComparedFile = comparer.ComparedFile
                });
            }

            doWorkEventArgs.Result = results;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            try
            {
                var fileName = (string)listView1.SelectedItems[0].Tag;

                Process.Start(fileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog
            {
                ShowHelp = true,
                DefaultExt = "*.docx",
                AutoUpgradeEnabled = true,
                DereferenceLinks = true,
                Filter = @"Doc files | " + Helpers.Options.FILTER,
                InitialDirectory = Environment.CurrentDirectory,
                RestoreDirectory = true,
                SupportMultiDottedExtensions = true,
                Title = @"Please save compared doc"
            };

            if (save.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                File.Copy(_filePath, save.FileName, true);

                Process.Start(save.FileName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var items = listView1.Items.Cast<ListViewItem>()
                    .Select(item => new CompareResult
                    {
                        FileName = item.SubItems[0].Text,
                        Percent = item.SubItems[1].Text,
                        Elapsed = item.SubItems[2].Text
                    });
                var pdfFile = PdfExporter.Save(items, textBox1.Text);

                Process.Start(pdfFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
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

            _filePath = file.FileName;
            textBox1.Text = _filePath;

            Run();
        }
    }
}