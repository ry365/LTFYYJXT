using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;

namespace LTFYYJXT
{
    public struct stuTag
    {
        public int ID;
        public string scxm;
        public string scnr;
        public int jb;
    }

    public partial class Form1 : Form
    {
        public List<CheckBox> CheckBoxList;
        public int currentLevel;
        private int currentPatientID = 0;
        public DataSet ds;
        public DataTable dt;
        public DataValue dv;
        private readonly DataValueList dvl;
        private bool historyData;


        public OracleCommand oraComm;
        public OracleConnection oraconn;
        public OracleDataAdapter oraDA;

        

        public List<CheckBox> selectLst;

        public Form1()
        {
            InitializeComponent();
            dv = new DataValue();
            dvl = new DataValueList();
        }


        private bool CheckIDCard(string Id)
        {
            if (Id.Length == 18)
            {
                var check = CheckIDCard18(Id);
                return check;
            }
            if (Id.Length == 15)
            {
                var check = CheckIDCard15(Id);
                return check;
            }
            return false;
        }

        private bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) ||
                long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
                return false; //数字验证
            var address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
                return false; //省份验证
            var birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            var time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
                return false; //生日验证
            var arrVarifyCode = "1,0,x,9,8,7,6,5,4,3,2".Split(',');
            var Wi = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',');
            var Ai = Id.Remove(17).ToCharArray();
            var sum = 0;
            for (var i = 0; i < 17; i++)
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            var y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
                return false; //校验码验证
            return true; //符合GB11643-1999标准
        }

        private bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
                return false; //数字验证
            var address =
                "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
                return false; //省份验证
            var birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            var time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
                return false; //生日验证
            return true; //符合15位身份证标准
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            selectLst = new List<CheckBox>();
            CheckBoxList = new List<CheckBox>();

            var con = string.Format("Data Source={0};User ID={1};Password={2}", "HIS", "zlhis", "HIS");
  //          var con = string.Format("Data Source={0};User ID={1};Password={2}", "ORA155", "us", "US");
            oraconn = new OracleConnection(con);
            oraconn.Open();


            oraComm = new OracleCommand("select * from 产妇妊娠风险筛查", oraconn);
            oraDA = new OracleDataAdapter(oraComm);
            dt = new DataTable("产妇妊娠风险筛查");
            oraDA.Fill(dt);
            InitSelectText();
            
//            p1.BringToFront();
            radioButton1.Checked = true;

            edtpgsj.DateTime = DateTime.Now;
            edtbgrq.DateTime = DateTime.Now;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                var cb = sender as CheckBox;
                label8.Text = "";
                int v, j = 0;
                if (cb.Checked)
                    selectLst.Add(cb);

                else
                    selectLst.Remove(cb);

                foreach (var x in selectLst)
                {
                    stuTag t = (stuTag)x.Tag;
                    if (string.IsNullOrEmpty(label8.Text))
                        label8.Text = "   " +t.scxm + ":" + x.Text;
                    else
                        label8.Text = label8.Text + "\n   " + t.scxm + ":" + x.Text;

                    stuTag vv = (stuTag)x.Tag;
                    v = vv.jb; ;
                    if (v > j) j = v;
                }

                foreach (var var in CheckBoxList)
                {
                    stuTag v1 = (stuTag)var.Tag;
                    if (j == 0) var.Visible = true;
                    if (Convert.ToInt32(v1.jb) <= j && selectLst.IndexOf(var) < 0)
                        var.Visible = false;
                }

                switch (j)
                {
                    case 1:
                        label8.ForeColor = Color.Yellow;
                        break;
                    case 2:
                        label8.ForeColor = Color.Coral;
                        break;
                    case 3:
                        label8.ForeColor = Color.Red;
                        break;
                    case 4:
                        label8.ForeColor = Color.Magenta;
                        break;
                }
                currentLevel = j;
            }
            else
            {


            }


        }

        private stuTag SetData(DataRow dr)
        {
            stuTag tmp;
            tmp.ID = Convert.ToInt32(dr["ID"].ToString());
            tmp.scnr = dr["筛查内容"].ToString();
            tmp.scxm = dr["筛查项目"].ToString();
            tmp.jb = Convert.ToInt32(dr["级别"].ToString());
            return tmp;
        }

        private void initDataControl(Panel fl, string strFilter)
        {
            DataRow[] drs;
            drs = dt.Select(strFilter);
            foreach (var dr in drs)
            {
                var cb = new CheckBox();

                cb.Text = dr["筛查内容"].ToString();
                if (cb.Text.Length > 50)
                    cb.Size = new Size(450, 60);
                else if (cb.Text.Length > 25)
                    cb.Size = new Size(450, 40);
                else
                    cb.AutoSize = true;


                cb.Tag = SetData(dr);
                //cb.Tag = Convert.ToInt32(dr["级别"].ToString());


                switch (dr["级别"].ToString())
                {
                    case "1":
                        cb.BackColor = Color.Yellow;
                        break;
                    case "2":
                        cb.BackColor = Color.Coral;
                        break;
                    case "3":
                        cb.BackColor = Color.Red;
                        break;
                    case "4":
                        cb.BackColor = Color.Magenta;
                        break;
                }


                cb.CheckedChanged += checkBox1_CheckedChanged;
                fl.Controls.Add(cb);

                cb.Dock = DockStyle.Top;
                cb.FlatStyle = FlatStyle.Flat;

                CheckBoxList.Add(cb);
            }
        }

        private void InitSelectText()
        {
            initDataControl(p1, "筛查项目='基本情况'");
            initDataControl(p2, "筛查项目='异常妊娠及分娩史'");
            initDataControl(p3, "筛查项目='妇产科疾病及手术史'");
            initDataControl(p4, "筛查项目='既往疾病及手术史'");
            initDataControl(p5, "筛查项目='呼吸系统疾病'");
            initDataControl(p6, "筛查项目='心血管系统疾病'");
            initDataControl(p7, "筛查项目='消化系统疾病'");
            initDataControl(p8, "筛查项目='泌尿系统疾病'");
            initDataControl(p9, "筛查项目='内分泌'");
            initDataControl(p10, "筛查项目='血液'");
            initDataControl(p11, "筛查项目='性传播传染病'");
            initDataControl(p12, "筛查项目='精神、神经'");
            initDataControl(p13, "筛查项目='免疫'");
            initDataControl(p14, "筛查项目='其他'");
        }




        private void ShowContext_Click(object sender, EventArgs e)
        {
            var ctl = sender as RadioButton;
            switch (ctl.Text)
            {
                case "1、基本信息":
                    p1.BringToFront();
                    break;
                case "2、异常妊娠及分娩史":
                    p2.BringToFront();
                    break;
                case "3、妇产科疾病及手术史":
                    p3.BringToFront();
                    break;
                case "4、既往病史及手术史":
                    p4.BringToFront();
                    break;
                case "5、呼吸系统疾病":
                    p5.BringToFront();
                    break;
                case "6、心血管系统疾病":
                    p6.BringToFront();
                    break;
                case "7、消化系统疾病":
                    p7.BringToFront();
                    break;
                case "8、泌尿系统疾病":
                    p8.BringToFront();
                    break;
                case "9、内分泌系统":
                    p9.BringToFront();
                    break;
                case "10、血液":
                    p10.BringToFront();
                    break;
                case "11、性传播传染病":
                    p11.BringToFront();
                    break;
                case "12、神经、精神":
                    p12.BringToFront();
                    break;
                case "13、免疫":
                    p13.BringToFront();
                    break;
                case "14、其他":
                    p14.BringToFront();
                    break;
            }
        }

        private void UpdateData(DataRow dr)
        {
            edtage.Text = dr["年龄"].ToString();
            edtbgr.Text = dr["报告人"].ToString().IsEmpty() ? edtbgr.Text : dr["报告人"].ToString();
            ;
            edtbgrq.Text = dr["报告日期"].ToString().IsEmpty() ? DateTime.Now.ToShortDateString() : dr["报告日期"].ToString();

            edtcsrq.Text = dr["出生日期"].ToString(); //.DateTime.ToString("yyyy年MM月dd日");
            edtcbzd.Text = dr["初步诊断"].ToString();
            ;
            edtsfzh.Text = dr["身份证号"].ToString();
            edtsfzh_Leave(this, null);
            edtyz.Text = dr["孕周"].ToString();
            //    edtbgjg.Text = dr["报告机构"].ToString();
            edtName.Text = dr["姓名"].ToString();
            edtmzh.Text = dr["门诊号"].ToString();
            edtlxdh.Text = dr["联系电话"].ToString();
            edtpgsj.Text = dr["评估时间"].ToString().IsEmpty() ? DateTime.Now.ToShortDateString() : dr["评估时间"].ToString();

            string[] v = dr["筛查ID"].ToString().Split(';');
            foreach (string vv in v)
            {
                foreach (CheckBox cb in CheckBoxList)
                {
                    stuTag t = (stuTag) cb.Tag;
                    if (t.ID.ToString() == vv)
                    {
                        cb.Checked = true;
                        cb.Visible = true;
                        break;
                    }
                }
            }

        }


        private void UpdateData()
        {
            dvl.Clear();
            dv.Age = edtage.Text;
            dv.Bgr = edtbgr.Text;
            dv.Bgrq = edtbgrq.Text;
            dv.Birthday = edtcsrq.Text; //.DateTime.ToString("yyyy年MM月dd日");
            dv.Cbzd = edtcbzd.Text;
            dv.Sfzh = edtsfzh.Text;
            dv.Yz = edtyz.Text;
            dv.Bgjg = edtbgjg.Text;
            dv.Name = edtName.Text;
            dv.Mzh = edtmzh.Text;
            dv.Lxdh = edtlxdh.Text;
            dv.Pgsj = edtpgsj.Text;
            switch (currentLevel)
            {
                case 1:
                    dv.Pgfj = "黄色";
                    break;
                case 2:
                    dv.Pgfj = "橙色";
                    break;
                case 3:
                    dv.Pgfj = "红色";
                    break;
                case 4:
                    dv.Pgfj = "紫色";
                    break;
            }


            dvl.Add(dv);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            UpdateData();
            SaveDataToDB(dv);

            clearControlText();
            textEdit1.Focus();
        }


        private void clearControlText()
        {
            edtcbzd.Text = "";

            edtName.Text = "";
            edtage.Text = "";
            edtsfzh.Text = "";
            edtlxdh.Text = "";
            label8.Text = "";
            edtyz.Text = "";
            edtmzh.Text = "";
            foreach (var cb in CheckBoxList)
            {
                cb.Visible = true;
                cb.Checked = false;
            }
            ShowContext_Click(radioButton1, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdateData();
            var xx = new XtraReport1();
            xx.DataSource = dvl;

            xx.ShowPreviewDialog();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            edtmzh.Text = textEdit1.Text;
            historyData = false;
            dt.Clear();
            oraComm.CommandText = "select * from view_筛查信息_1 where 门诊号=" + textEdit1.Text;
            oraDA.Fill(dt);
            if (dt.Rows.Count == 0)
            {
                oraComm.CommandText = "select * from view_筛查信息_2 where 门诊号=" + textEdit1.Text;
                oraDA.Fill(dt);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("未找到门诊号对应的信息，请重新输入后再试！");
                    textEdit1.SelectAll();
                    textEdit1.Focus();
                }
                else
                {
                    UpdateData(dt.Rows[0]);
                }
            }
            else
            {
                historyData = true;
                UpdateData(dt.Rows[0]);
            }
        }

        public string GetBirthdayAndSex(string identityCard, out string sex)
        {
            var birthday = "";

            sex = "";

            if (identityCard.Length == 18) //处理18位的身份证号码从号码中得到生日和性别代码
            {
                birthday = identityCard.Substring(6, 4) + "-" + identityCard.Substring(10, 2) + "-" +
                           identityCard.Substring(12, 2);

                sex = identityCard.Substring(14, 3);
            }

            if (identityCard.Length == 15)

            {
                birthday = "19" + identityCard.Substring(6, 2) + "-" + identityCard.Substring(8, 2) + "-" +
                           identityCard.Substring(10, 2);

                sex = identityCard.Substring(12, 3);
            }


            if (int.Parse(sex) % 2 == 0) //性别代码为偶数是女性奇数为男性

                sex = "女";

            else

                sex = "男";

            return birthday;
        }

        private void SaveDataToDB(DataValue dv)
        {
            var sql = "insert into 筛查信息(ID,姓名,年龄,出生日期,身份证号,孕周,联系电话,初步诊断,评估时间," +
                      "报告人,报告日期,评估分级,门诊号,筛查ID) values ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')";

            string strv = "";

            foreach (var v in selectLst)
            {
                stuTag t = (stuTag) v.Tag;
                strv = t.ID.ToString() + ";" + strv;

            }

            if (historyData)
            {
                oraComm.CommandText = "delete 筛查信息 where 门诊号= '" + dv.Mzh + "'";
                oraComm.ExecuteNonQuery();
            }
            oraComm.CommandText = string.Format(sql, dv.Mzh, dv.Name, dv.Age, dv.Birthday, dv.Sfzh, dv.Yz, dv.Lxdh,
                dv.Cbzd, dv.Pgsj,
                dv.Bgr, dv.Bgrq, dv.Pgfj, dv.Mzh,strv);
            oraComm.ExecuteNonQuery();
        }

        private void edtsfzh_Leave(object sender, EventArgs e)
        {
            if (edtsfzh.Text.IsEmpty()) return;
            if (!CheckIDCard(edtsfzh.Text))
            {
                MessageBox.Show("身份证号不正确，请重新输入");
                edtsfzh.SelectAll();
                edtsfzh.Focus();
            }
            string sex;
            edtcsrq.Text = GetBirthdayAndSex(edtsfzh.Text, out sex);
            edtage.Text = (DateTime.Now.Year - edtcsrq.DateTime.Year).ToString() ;
        }

        private void textEdit1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) button3_Click_1(sender, e);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            this.ShowContext_Click(sender, e);
        }

        private void label8_DoubleClick(object sender, EventArgs e)
        {
            selectLst.Clear();
            currentLevel = 0;
            foreach (var var in CheckBoxList)
            {
                var.Visible = true;
                var.Checked = false;
            }

        }
}
}