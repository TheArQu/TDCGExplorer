using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TDCG;
using TDCG.TAHTool;

namespace TAHBackground
{
    public partial class Form1 : Form
    {
        string source_file = null;
        Decrypter decrypter = new Decrypter();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (diaOpen1.ShowDialog() == DialogResult.OK)
            {
                source_file = diaOpen1.FileName;
                decrypter.Open(source_file);
                btnCompress.Enabled = false;
                btnLoad.Enabled = false;
                lbStatus.Text = "Processing...";
                DumpEntries();
                lbStatus.Text = "ok. Loaded";
                btnLoad.Enabled = true;
                btnCompress.Enabled = true;
            }
        }

        private void DumpEntries()
        {
            gvEntries.Rows.Clear();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name;

                if (entry.flag % 2 == 1)
                {
                    file_name += TAHFileUtils.GetExtensionFromMagic(decrypter.ExtractResource(entry));
                }

                string ext = Path.GetExtension(file_name).ToLower();
                if (ext == ".tbn")
                {
                    if (file_name.StartsWith("script/backgrounds/"))
                    {
                        string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                        gvEntries.Rows.Add(row);
                    }
                }
                else
                if (ext == ".psd")
                {
                    if (file_name.StartsWith("data/icon/backgrounds/"))
                    {
                        string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                        gvEntries.Rows.Add(row);
                    }
                }
                else
                if (ext == ".tso")
                {
                    if (file_name.StartsWith("data/bgmodel/"))
                    {
                        string[] row = { entry.file_name, entry.offset.ToString(), entry.length.ToString() };
                        gvEntries.Rows.Add(row);
                    }
                }
            }
        }

        List<string> GetPSDPathList()
        {
            List<string> ret = new List<string>();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name.ToLower();
                string ext = Path.GetExtension(file_name);
                if (ext == ".psd")
                {
                    if (file_name.StartsWith("data/icon/backgrounds/"))
                        ret.Add(file_name);
                }
            }
            return ret;
        }

        private void DumpFiles()
        {
            bwCompress.RunWorkerAsync(source_file);
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            lbStatus.Text = "Processing...";
            btnCompress.Enabled = false;
            btnLoad.Enabled = false;
            DumpFiles();
        }

        private void bwCompress_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Dictionary<string, TAHEntry> entries = new Dictionary<string, TAHEntry>();
            foreach (TAHEntry entry in decrypter.Entries)
            {
                string file_name = entry.file_name.ToLower();

                if (entry.flag % 2 == 1)
                {
                    file_name += TAHFileUtils.GetExtensionFromMagic(decrypter.ExtractResource(entry));
                }

                string ext = Path.GetExtension(file_name).ToLower();
                if (ext == ".tbn")
                {
                    if (file_name.StartsWith("script/backgrounds/"))
                        entries[file_name] = entry;
                }
                else
                if (ext == ".psd")
                {
                    if (file_name.StartsWith("data/icon/backgrounds/"))
                        entries[file_name] = entry;
                }
                else
                if (ext == ".tso")
                {
                    if (file_name.StartsWith("data/bgmodel/"))
                        entries[file_name] = entry;
                }
            }

            List<string> PSDPathList = GetPSDPathList();
            int entries_count = PSDPathList.Count;
            int current_index = 0;
            foreach (string psd_path in PSDPathList)
            {
                Console.WriteLine("psd {0}", psd_path);

                TAHEntry psd_entry = entries[psd_path];
                MemoryStream psd_stream = new MemoryStream(decrypter.ExtractResource(psd_entry));

                string tbn_path = PngBack.GetTBNPathFromPSDPath(psd_path);
                Console.WriteLine("tbn {0}", tbn_path);
                TAHEntry tbn_entry = entries[tbn_path];
                MemoryStream tbn_stream = new MemoryStream(decrypter.ExtractResource(tbn_entry));

                PngBack back = new PngBack();
                back.Load(tbn_stream, psd_stream);
                foreach (string str in PngBack.GetTSOPathListFromTBNFile(back.Tbn))
                {
                    string tso_path = str.ToLower();
                    Console.WriteLine("tso {0}", tso_path);
                    TAHEntry tso_entry;
                    if (entries.TryGetValue(tso_path, out tso_entry))
                    {
                        back.AddTSOFile(decrypter.ExtractResource(tso_entry));
                    }
                }
                string png_path = Path.GetFileNameWithoutExtension(psd_path) + @".png";
                back.Save(png_path);
                current_index++;
                int percent = current_index * 100 / entries_count;
                worker.ReportProgress(percent);
            }
        }

        private void bwCompress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("completed");
            lbStatus.Text = "ok. Compressed";
            pbStatus.Value = 0;
            btnLoad.Enabled = true;
            btnCompress.Enabled = true;
        }

        private void bwCompress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            decrypter.Close();
        }
    }
}
