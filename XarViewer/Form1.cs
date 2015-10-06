using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Humanizer;

namespace XarViewer
{
    public partial class Form1 : Form
    {
        private Xar.File file;
        private string filePath;

        public Form1(string filePath)
        {
            InitializeComponent();
            this.filePath = filePath;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mnuToolbarExtract.Enabled = false;
            mnuToolbarInfo.Enabled = false;
            if (filePath != null)
            {
                OpenFile(filePath);
            }
        }

        private void OpenFile(string filePath)
        {
            mnuToolbarExtract.Enabled = false;
            mnuToolbarInfo.Enabled = false;
            lstFiles.Items.Clear();
            try
            {
                string fileName = Path.GetFileName(filePath);
                file = new Xar.File(filePath);

                foreach (Xar.File.Entry entry in file.GetEntries())
                {
                    var item = new ListViewItem(new String[] {
                        entry.Id,
                        entry.Size.Bytes().Humanize("0.00")
                    });
                    lstFiles.Items.Add(item);
                }
                
                Text = "XarViewer - " + fileName;
                mnuToolbarInfo.Enabled = true;
            } catch (Xar.FileException ex)
            {
                file = null;
                MessageBox.Show(ex.Message, "Could not open XAR file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExtractFile(string id)
        {
            string extension = Path.GetExtension(id);
            string fileName = Path.GetFileNameWithoutExtension(id);
            saveFileDialog1.FileName = fileName;
            saveFileDialog1.Filter = extension + "|*" + extension + "|All files|*.*";
            DialogResult res = saveFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                file.ExtractFile(id, saveFileDialog1.FileName);
                MessageBox.Show("Extracted " + fileName + extension + " to " + saveFileDialog1.FileName, "Extracted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            OpenFile(((OpenFileDialog)sender).FileName);
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                mnuToolbarExtract.Enabled = true;
            }
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuToolbarExtract_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                ExtractFile(lstFiles.SelectedItems[0].Text);
            }
        }

        private void mnuToolbarOpen_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void mnuToolbarInfo_Click(object sender, EventArgs e)
        {
            if (file != null)
            {
                (new FormInfos(file)).ShowDialog();
            }
        }
    }
}
