using System;
using System.Collections.Generic;

namespace GraphDLL
{
    internal static class Queues
    {
        static List<node> files = new List<node>(100);
        static List<node> pictures = new List<node>(100);
        static List<node> texts = new List<node>(100);

        static public node SearchFile(string s)
        {
            s = s.ToLower();
            foreach (node p in files)
            {
                if (p.s == s)
                {
                    if (files[0] != p)
                    {
                        files.Remove(p);
                        files.Insert(0, p);
                    }
                    return p;
                }
            }

            if (files.Count == files.Capacity)
                files.RemoveAt(files.Count - 1);

            node q = new node();
            q.s = s;
            files.Insert(0, q);
            return q;
        }

        static public Bitmap SearchPicture(Bitmap relation, int w, int h)
        {
            foreach (node p in pictures)
            {
                Bitmap pic = (Bitmap)p.data;
                if (p.hash == relation.GetHashCode() && pic.Width == w && pic.Height == h)
                {
                    if (pictures[0] != p)
                    {
                        pictures.Remove(p);
                        pictures.Insert(0, p);
                    }
                    return pic;
                }
            }

            if (pictures.Count == pictures.Capacity)
                pictures.RemoveAt(pictures.Count - 1);

            node q = new node();
            q.hash = relation.GetHashCode();
            q.data = new Bitmap(relation, w, h);
            pictures.Insert(0, q);
            return (Bitmap)q.data;
        }

        static public node SearchTexts(string s, System.Drawing.Font f)
        {
            int h = f.GetHashCode();
            foreach (node p in texts)
            {
                Bitmap pic = (Bitmap)p.data;
                if (p.hash == h && p.s == s)
                {
                    if (texts[0] != p)
                    {
                        texts.Remove(p);
                        texts.Insert(0, p);
                    }
                    return p;
                }
            }

            if (texts.Count == texts.Capacity)
                texts.RemoveAt(texts.Count - 1);

            node q = new node();
            q.hash = h;
            q.s = s;
            texts.Insert(0, q);
            return q;
        }
    }

    internal class node
    {
        public string s;
        public int hash;
        public object data;
    }
}
