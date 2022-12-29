using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 权限管理
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form2 f2 = new Form2();
            Form1 f1 = new Form1();
            f1.Strip_List = f2.StripList;
            if (f1.ShowDialog() == DialogResult.OK)
            {
                f2.NowName = f1.Now_Name;
                f2.ShowDialog();
            }
        }
    }
}
