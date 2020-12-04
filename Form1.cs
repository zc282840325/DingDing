using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo002
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string strPath = @"D:\zc\Studying\Demo001\ConsoleApp1\bin\Debug\ConsoleApp1.exe";

                Process p= Process.Start(strPath, "-ip:122.226.189.54 -port:2106 -cc:5 -account:xiaofeng -password:xiaofeng -nowebshop -multithread -charnamemenu -megaphone -ls -ncping -noauthgg -lang:chs");
              
                ////System.Windows.Forms.MessageBox.Show(strPath);
                //Process p = new Process();
                ////设置要启动的应用程序
                //p.StartInfo.FileName = "cmd.exe";
                ////是否使用操作系统shell启动
                //p.StartInfo.UseShellExecute = false;
                //// 接受来自调用程序的输入信息
                //p.StartInfo.RedirectStandardInput = true;
                ////输出信息
                //p.StartInfo.RedirectStandardOutput = true;
                //// 输出错误
                //p.StartInfo.RedirectStandardError = true;
                ////不显示程序窗口
                //p.StartInfo.CreateNoWindow = true;
                ////启动程序
                //p.Start();

                //string param = string.Format(" -ip:122.226.189.54 -port:2106 -cc:5 -account:xiaofeng -password:xiaofeng -nowebshop -multithread -charnamemenu -megaphone -ls -ncping -noauthgg -lang:chs");
                ////向cmd窗口发送输入信息
                //p.StandardInput.WriteLine(strPath + param);

                //p.StandardInput.AutoFlush = true;
                // p.EnableRaisingEvents = true;
                // p.Exited += Proc_Exited;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            MessageBox.Show("他退出了");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 获取 access_token
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string access_token = string.Empty;

            if (rad_SDK.Checked)
            {
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/gettoken");
                OapiGettokenRequest req = new OapiGettokenRequest();
                req.Appkey = txt_appkey.Text;
                req.Appsecret = txt_appsecret.Text;
                req.SetHttpMethod("GET");
                OapiGettokenResponse rsp = client.Execute(req, access_token);
                var obj = JsonConvert.DeserializeObject<JObject>(rsp.Body);
                access_token = obj["access_token"].ToString();
            }
            else
            {
                string url = string.Format("https://oapi.dingtalk.com/gettoken?appkey={0}&appsecret={1}",txt_appkey.Text,txt_appsecret.Text);
                var result= HttpHelper.Get(url,Encoding.UTF8);
                var obj = JsonConvert.DeserializeObject<JObject>(result);
                if (obj!=null)
                {
                    if (obj["errcode"].ToString().Trim()=="0")
                    {
                        access_token = obj["access_token"].ToString();
                    }
                    else
                    {
                        access_token = obj["errmsg"].ToString();
                    }
                }
            }
            txt_access_token.Text = access_token;
        }

        /// <summary>
        /// 创建群聊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            string chatid = string.Empty;
            JObject obj = null;

            if (rad_SDK.Checked)
            {
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/chat/create");
                OapiChatCreateRequest req = new OapiChatCreateRequest();
                req.Name = txt_Name.Text;
                req.Owner = txt_uid.Text;
                List<string> list = txt_child.Text.Split(',').ToList();
                req.Useridlist = list;
                OapiChatCreateResponse rsp = client.Execute(req, txt_access_token.Text);
                if (rsp != null)
                {
                    obj = JsonConvert.DeserializeObject<JObject>(rsp.Body);
                }
            }
            else
            {
                string path = string.Format("https://oapi.dingtalk.com/chat/create?access_token={0}",txt_access_token.Text);
                Dictionary<string, string> list = new Dictionary<string, string>();
                list.Add("name", txt_Name.Text);
                list.Add("owner", txt_uid.Text);
                string[] arr = txt_child.Text.Split(',');
                list.Add("useridlist", JsonConvert.SerializeObject(arr).ToString());
                var result= HttpHelper.Post(path,JsonConvert.SerializeObject(list).ToString());
                obj = JsonConvert.DeserializeObject<JObject>(result);
            }

            if (obj!=null)
            {
                if (obj["chatid"] != null)
                {
                    chatid = obj["chatid"].ToString();
                    string path = Environment.CurrentDirectory + "//data.txt";
                    if (!File.Exists(path))
                    {
                        File.Create(path).Dispose();
                    }
                    List<string> list_address = File.ReadAllLines(path, Encoding.UTF8).ToList();
                    list_address.Add(txt_Name.Text + "," + obj["chatid"].ToString());
                    File.WriteAllLines(path, list_address, Encoding.UTF8);
                }
                else
                {
                    chatid = obj["errmsg"].ToString();
                }
            }
            txt_cid.Text = chatid;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            JObject obj = null;
            string msg = string.Empty;
            if (rad_SDK.Checked)
            {
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/chat/send");
                OapiChatSendRequest req = new OapiChatSendRequest();
                req.Chatid = txt_cid.Text;
                OapiChatSendRequest.TextDomain obj1 = new OapiChatSendRequest.TextDomain();
                obj1.Content = txt_Msg.Text;
                req.Text_ = obj1;
                req.Msgtype = "text";
                OapiChatSendResponse rsp = client.Execute(req, txt_access_token.Text);
                if (rsp != null)
                {
                    obj = JsonConvert.DeserializeObject<JObject>(rsp.Body);
                    msg = obj["errmsg"].ToString();
                }
            }
            else
            {
                string url = string.Format("https://oapi.dingtalk.com/chat/send?access_token={0}",txt_access_token.Text);
                Dictionary<string, string> list = new Dictionary<string, string>();
                list.Add("chatid", txt_cid.Text);
                list.Add("msgtype", "text");
                Dictionary<string, string> list_message = new Dictionary<string, string>();
                list_message.Add("content", txt_Msg.Text);
                list.Add("text", JsonConvert.SerializeObject(list_message).ToString());

                var result = HttpHelper.Post(url, JsonConvert.SerializeObject(list).ToString());
                obj = JsonConvert.DeserializeObject<JObject>(result);
                msg = obj["errmsg"].ToString();
            }
            lbl_state.Text = msg;
            
        }
        /// <summary>
        /// 通过手机号获取UID（当前用户必须是内部用户）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            string uid = string.Empty;
            JObject obj=null;
            if (rad_SDK.Checked)
            {
                try
                {
                    IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/getbymobile");
                    OapiV2UserGetbymobileRequest req = new OapiV2UserGetbymobileRequest();
                    req.Mobile = txt_phone.Text;
                    OapiV2UserGetbymobileResponse rsp = client.Execute(req, txt_access_token.Text);
                    if (rsp != null)
                    {
                        obj = JsonConvert.DeserializeObject<JObject>(rsp.Body);
                     
                    }
                }
                catch (Exception)
                {
                    uid = "请求异常";
                }
            }
            else
            {
                string url = string.Format("https://oapi.dingtalk.com/topapi/v2/user/getbymobile?access_token={0}",txt_access_token.Text);
                Dictionary<string, string> list = new Dictionary<string, string>();
                list.Add("mobile", txt_phone.Text);
                string json = JsonConvert.SerializeObject(list).ToString();
                var result=HttpHelper.Post(url,json);
                obj= JsonConvert.DeserializeObject<JObject>(result);
            }

            if (obj!=null)
            {
                if (obj["result"]["userid"] == null)
                {
                    uid = obj["errmsg"].ToString();
                }
                else
                {
                    uid = obj["result"]["userid"].ToString();
                }
            }
            txt_Cuid.Text = uid;
        }
    }
}
