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
    public partial class Form2 : Form
    {
        public ToolStripItemCollection StripList;//存储窗体2的菜单控件信息
        OleDbConnection DC = new OleDbConnection();  //定义数据库对象
        OleDbCommand MyCommand = new OleDbCommand(); //定义数据库操作对象
        public string NowName;//当前登录用户的名称
        public List<string> NameList = new List<string>();//存储所有的用户名

        public Form2()
        {
            InitializeComponent();
            StripList = menuStrip1.Items;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            OleDbDataReader MyReader; //进行数据库只读对象
            MyCommand.Connection = DC;//进行数据库连接
            DC.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Users\\12554\\Desktop\\权限控制\\数据库.mdb";//选取数据库
            DC.Open();//开启数据库
            GetEnable(NowName, menuStrip1.Items);
            SetTreeView(menuStrip1.Items, treeView1.Nodes);

            MyCommand.CommandText = "Select * from 角色表";
            MyReader = MyCommand.ExecuteReader();
            while (MyReader.Read())
            {
                NameList.Add(MyReader.GetValue(MyReader.GetOrdinal("用户名")).ToString());
                listBox1.Items.Add(MyReader.GetValue(MyReader.GetOrdinal("用户名")).ToString());
            }
            MyReader.Close();

        }


        //将用户名和控件名称作为查询条件,查询控件在数据库的可用性
        public bool GetStrip_Enable(string Name, string StripName)
        {
            MyCommand.Parameters.Clear();
            MyCommand.CommandText = "Select 可用性 from 权限表 where 用户名 = @Str1 and 控件名称 = @Str2";
            MyCommand.Parameters.AddWithValue("Str1", Name);
            MyCommand.Parameters.AddWithValue("Str2", StripName);
            return Convert.ToBoolean(MyCommand.ExecuteScalar());
        }

        //传入对应的用户名,和控件名称,设置当前控件可用性
        public void GetEnable(string Name, ToolStripItemCollection StripName)
        {
            for (int i = 0; i < StripName.Count; i++)
            {
                if (StripName[i] is ToolStripMenuItem)
                {
                    ToolStripMenuItem temp = (ToolStripMenuItem)StripName[i];
                    if (GetStrip_Enable(Name, temp.Name))
                    {
                        temp.Enabled = true;
                    }
                    else
                    {
                        temp.Enabled = false;
                    }

                    if (temp.HasDropDown)
                    {
                        GetEnable(Name, temp.DropDownItems);
                    }
                }
            }
        }


        //删除权限表中的用户数据
        public void Delete1(string Name)
        {
            MyCommand.Parameters.Clear();
            MyCommand.CommandText = "Delete from 权限表 where 用户名 = @Str1";
            MyCommand.Parameters.AddWithValue("Str1", Name);
            MyCommand.ExecuteScalar();
        }

        //删除角色表中的用户数据
        public void Delete2(string Name)
        {
            MyCommand.Parameters.Clear();
            MyCommand.CommandText = "Delete from 角色表 where 用户名 = @Str1";
            MyCommand.Parameters.AddWithValue("Str1", Name);
            MyCommand.ExecuteScalar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Name = listBox1.Items[listBox1.SelectedIndex].ToString();
            Delete1(Name);
            Delete2(Name);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            MessageBox.Show("删除角色成功");
        }

        //将菜单显示在树形列表中
        public void SetTreeView(ToolStripItemCollection StripName, TreeNodeCollection TreeList)
        {
            for (int i = 0; i < StripName.Count; i++)
            {
                if (StripName[i] is ToolStripMenuItem)
                {
                    ToolStripMenuItem temp = (ToolStripMenuItem)StripName[i];
                    TreeNode newnode = TreeList.Add(temp.Name, temp.Text);
                    if (temp.HasDropDown)
                    {
                        SetTreeView(temp.DropDownItems, newnode.Nodes);
                    }
                }
            }
        }


        //设置权限表中的用户控件的可用性
        public void SetEnable_Strip(string Name, string StripName, bool Enable)
        {
            MyCommand.Parameters.Clear();
            MyCommand.CommandText = "update 权限表 set 可用性 = @Str3 where 用户名 = @Str1 and 控件名称 = @Str2";
            MyCommand.Parameters.AddWithValue("Str3", Enable);
            MyCommand.Parameters.AddWithValue("Str1", Name);
            MyCommand.Parameters.AddWithValue("Str2", StripName);
            MyCommand.ExecuteScalar();
        }

        public void Update(string Name, TreeNodeCollection NodeList)
        {
            for (int i = 0; i < NodeList.Count; i++)
            {
                TreeNode NewNode = NodeList[i];
                if(NewNode.Checked == true)
                   SetEnable_Strip(Name, NewNode.Name, true);

                if (NewNode.Nodes.Count > 0)
                {
                    Update(Name, NewNode.Nodes);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string Name = textBox1.Text;
            Update(Name, treeView1.Nodes);

            MessageBox.Show("权限更改成功");
        }
    }
}
