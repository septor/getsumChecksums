using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

using getsumChecksums.Properties;

namespace getsumChecksums
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                lblRootPath.Text = Settings.Default.lastPath;
                ListFiles(Settings.Default.lastPath);
            }
            catch { }
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            folderBrowser.SelectedPath = lblRootPath.Text;
            DialogResult result = folderBrowser.ShowDialog();
            if (result == DialogResult.OK)
            {
                lblRootPath.Text = folderBrowser.SelectedPath.ToString();
                ListFiles(folderBrowser.SelectedPath.ToString());
            }

            Settings.Default.lastPath = lblRootPath.Text;
            Settings.Default.Save();
        }

        private void lstFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstFiles.SelectedItems.Count != 0)
                Clipboard.SetText(lstFiles.SelectedItems[0].SubItems[2].Text);
        }
        #region Custom Functions
        /// <summary>
        /// Get the MD5 checksum for a given file.
        /// </summary>
        public static string md5Hash(string filename)
        {
            if (File.Exists(filename))
                using (var md5 = MD5.Create())
                    using (var stream = File.OpenRead(filename))
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
            else
                return null;
        }

        /// <summary>
        /// Recrusivly parse a directory for all files.
        /// Place the file name, md5 checksum, and sub directory (if applicible) in a ListView (lstFiles).
        /// </summary>
        public void ListFiles(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if (directoryInfo.Exists)
            {
                FileInfo[] fileInfo = directoryInfo.GetFiles();
                DirectoryInfo[] subdirectoryInfo = directoryInfo.GetDirectories();
                string directoryName;

                foreach (DirectoryInfo subDirectory in subdirectoryInfo)
                    ListFiles(subDirectory.FullName);

                foreach (FileInfo file in fileInfo)
                {
                    directoryName = file.DirectoryName.Replace(lblRootPath.Text + "\\", "");

                    if (directoryName == lblRootPath.Text)
                        directoryName = "";

                    lstFiles.Items.Add(new ListViewItem(new string[] { file.Name, directoryName, md5Hash(file.FullName) }));
                }
            }
        }
        #endregion
    }
}
