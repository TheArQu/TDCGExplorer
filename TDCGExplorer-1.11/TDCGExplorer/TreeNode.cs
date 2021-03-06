﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace TDCGExplorer
{
    public class GenericTahTreeNode : System.Windows.Forms.TreeNode
    {
        public GenericTahTreeNode(string text)
            : base(text)
        {
        }
        public virtual void DoTvTreeSelect()
        {
        }
        public virtual void DoEditAnnotation()
        {
        }
        public virtual void DoExploreNode()
        {
            TDCGExplorer.ExplorerPath(FullPath);
        }
    }

    // ファイル個別情報のファイルツリーノード.
    public class GenericFilesTreeNode : GenericTahTreeNode
    {
        private List<ArcsTahEntry> entries = new List<ArcsTahEntry>();

        public GenericFilesTreeNode(string text)
            : base(text)
        {
        }

        public List<ArcsTahEntry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        public override void DoTvTreeSelect()
        {
            TDCGExplorer.MainFormWindow.SetTahContextMenu(true); // TAH関連のメニューがあるcontext menu
            TDCGExplorer.MainFormWindow.ListBoxClear();
            foreach (ArcsTahEntry file in Entries)
            {
                TDCGExplorer.MainFormWindow.ListBoxMainView.Items.Add(new LbFileItem(file));
            }
        }
    }
    // ArcsZipFileEntryを保持するTreeNode
    public class GenericZipTreeNode : GenericTahTreeNode
    {
        int zipid;

        public GenericZipTreeNode(string text, int zipid)
            : base(text)
        {
            this.zipid = zipid;
        }

        public int Entry
        {
            get { return zipid; }
            set { zipid = value; }
        }

        public override void DoTvTreeSelect()
        {
            TDCGExplorer.MainFormWindow.SetTahContextMenu(true); // TAH関連のメニューがあるcontext menu
            TDCGExplorer.MainFormWindow.ListBoxClear();
            //セレクトされたときにSQLに問い合わせる.
            List<ArcsZipTahEntry> files = TDCGExplorer.ArcsDB.GetZipTahs(zipid);
            foreach (ArcsZipTahEntry file in files)
            {
                TDCGExplorer.MainFormWindow.ListBoxMainView.Items.Add(new LbZipFileItem(file));
            }
            if (TDCGExplorer.BusyTest() == true) return;

            // ZIPページを開いた時の動作を指定する (none:なにもしない server:サーバにアクセス image:画像表示 text:テキスト表示)
            // public string zippage_behavior
            switch (TDCGExplorer.SystemDB.zippage_behavior)
            {
                case "server":
                    {
                        TDCGExplorer.MainFormWindow.AssignTagPageControl(new MODRefPage(zipid));
                    }
                    return;
                case "image":
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".bmp" || ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".tif")
                        {
                            string savefilpath = entry.path.ToLower();
                            if (savefilpath.EndsWith(".tdcgsav.png") || savefilpath.EndsWith(".tdcgsav.bmp") || savefilpath.EndsWith(".tdcgpose.png")) continue;

                            TDCGExplorer.MainFormWindow.AssignTagPageControl(new ImagePageControl(new GenericZipsTahInfo(entry)));
                            return;
                        }
                    }
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".txt" || ext == ".doc" || ext == ".xml")
                        {
                            TDCGExplorer.MainFormWindow.AssignTagPageControl(new TextPageControl(new GenericZipsTahInfo(entry)));
                            return;
                        }
                    }
                    break;
                case "text":
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".txt" || ext == ".doc" || ext == ".xml")
                        {
                            TDCGExplorer.MainFormWindow.AssignTagPageControl(new TextPageControl(new GenericZipsTahInfo(entry)));
                            return;
                        }
                    }
                    foreach (ArcsZipTahEntry entry in files)
                    {
                        string ext = Path.GetExtension(entry.path).ToLower();
                        if (ext == ".bmp" || ext == ".png" || ext == ".jpg" || ext == ".gif" || ext == ".tif")
                        {
                            string savefilpath = entry.path.ToLower();
                            if (savefilpath.EndsWith(".tdcgsav.png") || savefilpath.EndsWith(".tdcgsav.bmp") || savefilpath.EndsWith(".tdcgpose.png")) continue;

                            TDCGExplorer.MainFormWindow.AssignTagPageControl(new ImagePageControl(new GenericZipsTahInfo(entry)));
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }

        }
        // zipファイルの場合はDBを調べる.
        public override void DoExploreNode()
        {
            ArcsZipArcEntry zip = TDCGExplorer.ArcsDB.GetZip(zipid);
            string fullpath = Path.Combine(TDCGExplorer.SystemDB.zips_path, zip.path);
            TDCGExplorer.ExplorerSelectPath(fullpath);
        }

        // アノテーションを入力する.
        public override void DoEditAnnotation()
        {
            AnnotationEdit edit = new AnnotationEdit();
            ArcsZipArcEntry zip = TDCGExplorer.ArcsDB.GetZip(zipid);
            Dictionary<string, string> annon = TDCGExplorer.AnnDB.annotation;
            edit.text = zip.GetDisplayPath();
            edit.code = zip.code;
            edit.Owner = TDCGExplorer.MainFormWindow;
            // エディットがOKなら書き換える.
            if (edit.ShowDialog() == DialogResult.OK)
            {
                TDCGExplorer.AnnDB.SetSqlValue(zip.code, edit.text);
                Text = edit.text;
            }
        }
    }

    public class GenericCollisionTahNode : GenericTahTreeNode
    {
        List<CollisionItem> entries = new List<CollisionItem>();

        public GenericCollisionTahNode(string text)
            : base(text)
        {
        }

        public List<CollisionItem> Entries
        {
            get { return entries; }
            set { entries = value; }
        }


        public override void DoTvTreeSelect()
        {
            TDCGExplorer.MainFormWindow.SetTahContextMenu(false); // TAH関連のメニューがないcontext menu
            TDCGExplorer.MainFormWindow.ListBoxClear();
            foreach (CollisionItem file in entries)
            {
                TDCGExplorer.MainFormWindow.ListBoxMainView.Items.Add(new LbCollisionItem(file));
            }
        }
    }

    public class GenericSavefileTreeNode : GenericTahTreeNode
    {
        List<string> Files = new List<string>();

        public GenericSavefileTreeNode(string text,string directory) : base( text )
        {
            string[] files = Directory.GetFiles(directory, "*.*");
            foreach (string file in files)
            {
                if (file.EndsWith(".tdcgsav.png") || file.EndsWith(".tdcgsav.bmp") || file.EndsWith(".tdcgpose.png") )
                {
                    Files.Add(file);
                }
            }
        }

        public override void DoTvTreeSelect()
        {
            TDCGExplorer.MainFormWindow.SetTahContextMenu(false); // TAH関連のメニューがないcontext menu
            TDCGExplorer.MainFormWindow.ListBoxClear();
            foreach (string file in Files)
            {
                TDCGExplorer.MainFormWindow.ListBoxMainView.Items.Add(new LbSaveFileItem(file));
            }
        }

        public int Count
        {
            get { return Files.Count; }
        }

        public void Add(string path)
        {
            foreach (string file in Files)
            {
                if (file.ToLower() == path.ToLower()) return; // 既にある.
            }
            Files.Add(path);
            DoTvTreeSelect();
        }

        public void Del(string path)
        {
            int index = 0;
            foreach (string file in Files)
            {
                if (file.ToLower() == path.ToLower())
                {
                    Files.RemoveAt(index);
                    DoTvTreeSelect();
                    break;
                }
                index++;
            }
        }
    }
}
