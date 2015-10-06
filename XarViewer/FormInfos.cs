using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XarViewer
{
    public partial class FormInfos : Form
    {
        private Xar.File file;

        public FormInfos(Xar.File file)
        {
            InitializeComponent();
            this.file = file;
        }

        private void FormInfos_Load(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            builder.AppendLine("----------------------------------------");
            builder.AppendLine(" Header");
            builder.AppendLine("----------------------------------------");
            builder.AppendLine(" Id: " + file.Id);
            builder.AppendLine(" File: " + file.FilePath);
            builder.AppendLine(" Version: " + file.Version);
            builder.AppendLine(" Index size: " + file.IndexSize);
            builder.AppendLine("----------------------------------------");
            txtInfos.Text = builder.ToString();
        }
    }
}
