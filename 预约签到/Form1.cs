using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using Ry.Defined;

namespace 预约签到
{
    public partial class Form1 : Form
    {

        public OracleConnection ORAconn;
        public OracleCommand ORAcmd;
        public OracleDataAdapter ORAadp;
        public string DBIP;
        public string DBName;
        public string DBPasswd;
        public string DBPort;
        public string ServerName;
        public string methodName;
        public string methodCodeType;
        public string methodCodeOther;
        public string methodProgram;
        private string _beep;
        public string configFile = MyConst.ConfigFilePath + "\\Config.xml";
        public string ConnectOracle()
        {
            try
            {
                if (ORAconn == null)
                    if (ORAconn == null)
                    {
                        string connString =string.Format(
                            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})" +
                            "(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                            "Persist Security Info=True;User ID={3};Password={4};",DBIP,DBPort, ServerName, DBName, DBPasswd);
                        ORAconn = new OracleConnection(connString);
                        ORAconn.Open();
                        ORAcmd = new OracleCommand("", ORAconn);
                        ORAadp = new OracleDataAdapter(ORAcmd);
                    }

                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return ex.ToString();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string,object> dict = new Dictionary<string, object>();
            dict.Add("method", methodName);
            dict.Add("CodeType", methodCodeType);
            dict.Add("code", textBox1.Text);
            dict.Add("other", methodCodeOther);

            ExecuteProgram(methodProgram,dict);

            if (_beep != "0")
                Ry.Function.FunctionCommon.MessageBeep(5555);
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            DBIP = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "ConnectDB/DBHostIP", "127.0.0.1");
            DBName = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "ConnectDB/DBUserName", "system"); ;
            DBPasswd = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "ConnectDB/DBPasswd", "Oracle");
            DBPort = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "ConnectDB/DBPort", "1521"); 
            ServerName = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "ConnectDB/DBName", "Oracle"); 
            ConnectOracle();

            methodName = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "设置/处理方式", "执行预约");

            methodCodeOther = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "设置/其他条件", "");

            methodProgram = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "设置/存储过程名", "pro_根据相关的ID执行");

            _beep = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "设置/蜂鸣提示", "1");

            DataTable dt = Ry.Function.FunctionCommon.XmlFile2Datatable(MyConst.ConfigFilePath + "\\CodeType.xml");

            comboBox1.DataSource = dt;
            comboBox1.ValueMember = "ID";
            comboBox1.DisplayMember = "NAME";


            methodCodeType = XMLFileOperate.CXmlFile.GetStringFromXmlFile(configFile,
                "设置/默认操作", "0");


            comboBox1.SelectedValue = methodCodeType;

        }


        public int ExecuteProgram(string programName,Dictionary<string, object> dictParam)
        {
            ORAcmd.CommandText = programName;
            ORAcmd.CommandType = CommandType.StoredProcedure;
            ORAcmd.Parameters.Clear();
            foreach (KeyValuePair<string, object> KeyValue in dictParam)
            {
                OracleParameter p1 = new OracleParameter(KeyValue.Key,OracleDbType.Varchar2);
                p1.Value = KeyValue.Value;
                p1.Direction = ParameterDirection.Input;
                ORAcmd.Parameters.Add(p1);
            }
            ORAcmd.ExecuteNonQuery();
            return MyConst.Success;
        }

        public  int GetDataSet(string sqlString, CommandType commandType, Dictionary<string, object> dictParam, ref DataTable ds)
        {

            DataSet dt = new DataSet();
            try
            {

                ORAcmd.CommandText = sqlString;
                ORAcmd.CommandType = commandType;

                ORAcmd.Parameters.Clear();
                foreach (KeyValuePair<string, object> KeyValue in dictParam)
                {
                    //OracleParameter p1 = new OracleParameter(KeyValue.Key, OracleType.VarChar);
                    //p1.Value = KeyValue.Value;
                    //p1.Direction = ParameterDirection.Input;
                    //this._command.Parameters.Add(p1);
                    ORAcmd.Parameters.Add(KeyValue.Key, KeyValue.Value);
                }

               // OracleParameter O = new OracleParameter("result", OracleDbType.RefCursor,ParameterDirection.Output);

               // O.Direction = ParameterDirection.Output;
               // ORAcmd.Parameters.Add(O);

                ORAadp.Fill(ds);
            }
            catch (Exception e)
            {
                MyConst.Err.SetError(20002, "GetDataSet - " + sqlString, e.Message, sqlString);
                return MyConst.Failed;
            }

            return MyConst.Success;
        }


        private void Form1_Shown(object sender, EventArgs e)
        {
          
            

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            methodCodeType = comboBox1.SelectedValue.ToString();
            XMLFileOperate.CXmlFile.SaveStringToXmlFile(comboBox1.SelectedValue.ToString(), configFile, "设置/默认操作");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(13))
            {
                button1_Click(sender,e);
            }

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }
    }
}
