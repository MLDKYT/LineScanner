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

namespace LineScanner {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private string pathToScan;
        private int scannedDirectories, scannedFiles, scannedLinesOfCode;

        private string[] fileFormats = new[] {
            "cs", "js", "css", "html", "htm", "php", "sql", "xml", "json", "txt", "md",
            "bat", "ini", "config", "csproj", "sln", "resx", "xaml", "vb", "vbproj", "aspx", "ascx", "cshtml",
            "config", "csproj", "sln", "vb", "vbproj", "resx", "xaml"
        };

        private void button1_Click(object sender, EventArgs e) {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK) {
                pathToScan = fbd.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(pathToScan)) {
                MessageBox.Show("Please select a folder to scan", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _ = StartScanDirectory(pathToScan);
        }

        async Task StartScanDirectory(string path) {
            await ScanDirectory(path);
            UpdateLabel(path);
        }
        
        string SplitNumber(int num) {
            // split number using ' every 3 digits
            var str = num.ToString();
            var result = "";
            var count = 0;
            for (var i = str.Length - 1; i >= 0; i--) {
                result = str[i] + result;
                count++;
                if (count == 3 && i != 0) {
                    result = "'" + result;
                    count = 0;
                }
            }
            
            return result;
        }

        void UpdateLabel(string path) {
            label3.Text = $@"Scanned folders: {SplitNumber(scannedDirectories)}
Scanned files: {SplitNumber(scannedFiles)}
Lines of code found: {SplitNumber(scannedLinesOfCode)}
Currently scanning: {path.ToLower()}";
        }

        private int temp1 = 0;
        
        async Task ScanDirectory(string path) {
            var files = Directory.GetFiles(path);
            foreach (var file in files) {
                try {
                    if (fileFormats.Contains(Path.GetExtension(file).Substring(1))) {
                        scannedFiles += 1;
                        var lines = File.ReadAllLines(file);
                        scannedLinesOfCode += lines.Length;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories) {
                scannedDirectories += 1;
                await ScanDirectory(directory);
            }

            temp1++;
            if (temp1 > 10) {
                temp1 = 0;
                UpdateLabel(path);
                await Task.Delay(1);
            }
            UpdateLabel(path);
        }
    }
}