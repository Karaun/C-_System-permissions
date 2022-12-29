using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 权限管理
{
    public partial class Form1 : Form
    {
        public ToolStripItemCollection Strip_List;//窗体2的菜单控件信息
        OleDbConnection DC = new OleDbConnection();  //定义数据库对象
        OleDbCommand MyCommand = new OleDbCommand(); //定义数据库操作对象
        public List<string> NameList = new List<string>();//存储所有的用户名
        public List<string> PassWordList = new List<string>();//存储用户密码
        public string Now_Name;//当前登录用户的名称;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Name = textBox1.Text;
            string PassWord = textBox2.Text;

            for (int i = 0; i < NameList.Count; i++)
            {
                if (Name == NameList[i])
                {
                    MessageBox.Show("该用户存在,请重新输入");
                    textBox1.Text = "";
                    return;
                }
            }
            GetName_PassWord(Name, PassWord);
            GetStripName(Name, Strip_List);

            MessageBox.Show("注册成功");
            NameList.Add(Name);
            PassWordList.Add(PassWord);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OleDbDataReader MyReader; //进行数据库只读对象
            MyCommand.Connection = DC;//进行数据库连接
            DC.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Users\\12554\\Desktop\\权限控制\\数据库.mdb";//选取数据库
            DC.Open();//开启数据库

            MyCommand.CommandText = "Select * from 角色表";
            MyReader = MyCommand.ExecuteReader();
            while (MyReader.Read())
            {
                NameList.Add(MyReader.GetValue(MyReader.GetOrdinal("用户名")).ToString());
                PassWordList.Add(MyReader.GetValue(MyReader.GetOrdinal("用户密码")).ToString());
            }
            MyReader.Close();
        }

        //将用户名和控件名称写入数据库
        public void GetStripName(string Name, ToolStripItemCollection StripName)
        {
            for (int i = 0; i < StripName.Count; i++)
            {
                if (StripName[i] is ToolStripMenuItem)
                {
                    ToolStripMenuItem temp = (ToolStripMenuItem)StripName[i];
                    MyCommand.Parameters.Clear();
                    MyCommand.CommandText = "Insert into 权限表 (用户名,控件名称) Values(@Str1,@Str2)";
                    MyCommand.Parameters.AddWithValue("Str1", Name);
                    MyCommand.Parameters.AddWithValue("Str2", temp.Name);
                    MyCommand.ExecuteScalar();

                    if (temp.HasDropDown)
                    {
                        GetStripName(Name, temp.DropDownItems);
                    }
                }
            }
        }


        //将用户名和用户密码写入数据库
        public void GetName_PassWord(string Name, string PassWord)
        {
            MyCommand.Parameters.Clear();
            MyCommand.CommandText = "Insert into 角色表 (用户名,用户密码) Values(@Str1,@Str2)";
            MyCommand.Parameters.AddWithValue("Str1", Name);
            MyCommand.Parameters.AddWithValue("Str2", PassWord);
            MyCommand.ExecuteScalar();    
        }


        public bool Singin(string Name, string PassWord)
        { 
            for (int i = 0; i < NameList.Count; i++)
            {
                if (Name == NameList[i] && PassWord == PassWordList[i])
                {
                    return true;
                }
            }
            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Name = textBox1.Text;
            string PassWord = textBox2.Text;

            if (Singin(Name, PassWord))
            {
                MessageBox.Show("登陆成功");
                Now_Name = Name;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("登陆失败");
            }
            
        }
    }
}
