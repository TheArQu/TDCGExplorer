﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDCGExplorer;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace System.Windows.Forms
{
    class SaveFilePage : ZipFilePageControl
    {
        TDCGSaveFileInfo savefile;
        private DataGridView dataGridView;
        private static volatile bool busy = false;

        public static bool Busy
        {
            get { return busy; }
        }

        // zipファイルの中から
        public SaveFilePage(GenTahInfo tahInfo) : base(tahInfo)
        {
            busy = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                TDCGExplorer.TDCGExplorer.SetToolTips("データベース検索中...");
                ExtractFile();
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.ReadOnly = true;
                dataGridView.MultiSelect = false;
                dataGridView.AllowUserToAddRows = false;
                TDCGExplorer.TDCGExplorer.SetToolTips(Text);
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Cursor.Current = Cursors.Default;
            busy = false;
        }
        // ファイルから直接読み出す.
        public SaveFilePage(string path)
        {
            busy = true;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                InitializeComponent();
                TDCGExplorer.TDCGExplorer.SetToolTips("データベース検索中...");
                Text = Path.GetFileName(path);
                FileStream fs = File.OpenRead(path);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                using (MemoryStream ms = new MemoryStream(buffer))
                {
                    BindingStream(ms);
                }
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView.ReadOnly = true;
                dataGridView.MultiSelect = false;
                dataGridView.AllowUserToAddRows = false;
                TDCGExplorer.TDCGExplorer.SetToolTips(Text);
            }
            catch (System.InvalidCastException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            Cursor.Current = Cursors.Default;
            busy = false;
        }

        private void InitializeComponent()
        {
            this.dataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(0, 0);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.Resize += new System.EventHandler(this.dataGridView_Resize);
            this.dataGridView.MouseEnter += new System.EventHandler(this.dataGridView_MouseEnter);
            // 
            // SaveFilePage
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Controls.Add(this.dataGridView);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        private void dataGridView_MouseEnter(object sender, EventArgs e)
        {
            Control obj = (Control)sender;
            obj.Focus();
        }

        public override void BindingStream(MemoryStream ms)
        {
            savefile = new TDCGSaveFileInfo((Stream)ms);

            DataTable data = new DataTable();
            data.Columns.Add("パーツ", Type.GetType("System.String"));
            data.Columns.Add("属性", Type.GetType("System.String"));
            data.Columns.Add("TAHファイル", Type.GetType("System.String"));

            Application.DoEvents();

            // TSOビューワをリセットする
            TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
            TDCGExplorer.TDCGExplorer.MainFormWindow.clearTSOViewer();

            for (int i = 0; i < TDCGSaveFileInfo.PARTS_SIZE; ++i)
            {
                string[] partfile = {savefile.GetPartsName(i),savefile.GetPartsFileName(i),""};
                // TAHファイルを検索する
                string partsname = savefile.GetPartsFileName(i);
                if (partsname.StartsWith("items/")==true)
                {
                    {
                        // まずarcsからさがしてみる.
                        List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetTahFilesEntry(TAHUtil.CalcHash("data/model/" + partsname.Substring(6) + ".tso"));
                        if (files.Count > 0)
                        {
                            ArcsTahFilesEntry file = null;
                            ArcsTahEntry tah = null;
                            int pastVersion = -1;
                            foreach (ArcsTahFilesEntry subfile in files)
                            {
                                ArcsTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetTah(subfile.tahid);
                                if (subtah.version > pastVersion)
                                {
                                    file = subfile;
                                    tah = subtah;
                                    pastVersion = subtah.version;
                                }
                                Application.DoEvents();
                            }
                            if (tah != null)
                            {
                                partfile[2] = tah.path;
                                try
                                {
                                    // TSOを読み込む
                                    TahInfo info = new TahInfo(tah);
                                    using (TAHStream tahstream = new TAHStream(info, file))
                                    {
                                        TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                                        TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad();
                                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.FrameMove();
                                        TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.Render();
                                        Application.DoEvents();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("Error: " + ex);
                                }
                            }
                        }
                    }
                    // arcsに無かったら全zipから探す.
                    if (partfile[2] == "")
                    {
                        List<ArcsTahFilesEntry> files = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTahFilesEntries(TAHUtil.CalcHash("data/model/" + partsname.Substring(6) + ".tso"));
                        if (files.Count>0)
                        {
                            ArcsTahFilesEntry file = null;
                            ArcsZipTahEntry tah = null;
                            int pastVersion = -1;
                            foreach (ArcsTahFilesEntry subfile in files)
                            {
                                ArcsZipTahEntry subtah = TDCGExplorer.TDCGExplorer.ArcsDB.GetZipTah(subfile.tahid);
                                if (subtah.version > pastVersion)
                                {
                                    file = subfile;
                                    tah = subtah;
                                    pastVersion = subtah.version;
                                }
                                Application.DoEvents();
                            }
                            if (tah != null)
                            {
                                ArcsZipArcEntry zip = TDCGExplorer.TDCGExplorer.ArcsDB.GetZip(tah.zipid);
                                if (zip != null)
                                {
                                    partfile[2] = zip.path + "\\" + tah.path;

                                    try
                                    {
                                        // TSOを読み込む
                                        ZipTahInfo info = new ZipTahInfo(tah);
                                        using (TAHStream tahstream = new TAHStream(info, file))
                                        {
                                            TDCGExplorer.TDCGExplorer.MainFormWindow.makeTSOViwer();
                                            TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.LoadTSOFile(tahstream.stream);
                                            TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad();
                                            TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.FrameMove();
                                            TDCGExplorer.TDCGExplorer.MainFormWindow.Viewer.Render();
                                            Application.DoEvents();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine("Error: " + ex);
                                    }
                                }
                            }
                        }
                    }
                }
                TDCGExplorer.TDCGExplorer.MainFormWindow.doInitialTmoLoad(); // 初期tmoを読み込む.

                DataRow row = data.NewRow();
                row.ItemArray = partfile;
                data.Rows.Add(row);
            }
            for (int i = 0; i < TDCGSaveFileInfo.SLIDER_SIZE; ++i)
            {
                string[] partfile = {savefile.GetSliderName(i),savefile.GetSliderValue(i),""};
                DataRow row = data.NewRow();
                row.ItemArray = partfile;
                data.Rows.Add(row);
            }

            dataGridView.DataSource = data;
        }

        protected override void InitLayout()
        {
            base.InitLayout();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            dataGridView.Size = Size;
        }
    }
}