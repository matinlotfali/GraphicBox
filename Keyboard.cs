using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphDLL
{
    public static partial class Graph
    {
        public static bool keydown(Keys key)
        {
            return form.myKeys.IsKeyPressed(key);
        }
        public static bool kbhit()
        {
            return form.myKeys.IsKeyPressed();
        }

        public static char getch()
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = true;
            delay(0);
            Graph.imediateDrawing = temp;
            GForm.ch = '\0';
            Graph.form.Invoke((MethodInvoker)delegate
            {
                Graph.form.Activate();
            });            
            while (GForm.ch == '\0') ;
            lastRefresh = DateTime.Now;
            return GForm.ch;
        }
        public static char getche()
        {
            char a = getch();
            Console.Write(a);
            return a;
        }

        public static Keys getkey()
        {
            bool temp = Graph.imediateDrawing;
            Graph.imediateDrawing = true;
            delay(0);
            Graph.imediateDrawing = temp;
            GForm.key = Keys.None;
            Graph.form.Invoke((MethodInvoker)delegate
            {
                Graph.form.Activate();
            });
            while (GForm.key == Keys.None) ;
            lastRefresh = DateTime.Now;
            return GForm.key;
        }
    }    
}
