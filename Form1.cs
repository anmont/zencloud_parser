using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace zencloud_parser
{
    public partial class Form1 : Form
    {
        public string[] rawTransactions;
        public List<string> processedTransactions = new List<string>();
        public int id_count = 1;
        public OpenFileDialog openFile = new OpenFileDialog();
        public Form1()
        {
            InitializeComponent();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
                string html = File.ReadAllText(openFile.FileName);
                prep_file(html);
                process_transactions();
                writeTobox();
                fileSave();
                Application.Exit();

        }

        private void prep_file(string file)
        {
            string replace = "\"user\":{\"_id\"";
            string replaceWith = "\n\n\"user\":{\"_id\"";

            string newSt = file.Replace(replace, replaceWith);

            rawTransactions = newSt.Split(new string[] { "\n\n" }, StringSplitOptions.None);
        }

        private void process_transactions()
        {
            foreach (string tString in rawTransactions)
            {
                int loc = tString.IndexOf("action\\\">") + 9;
                if (loc > 8)
                {
                    int loca = tString.IndexOf("<", loc);
                    string compare = tString.Substring(loc, loca - loc);
                    if (compare == "service charge")
                    {
                        process_charge(tString);
                    }
                    else if (compare == "payout")
                    {
                        process_payout(tString);
                    }
                    else if (compare == "transfer completed")
                    {
                        process_transfer(tString);
                    }
                }
            }
        }

        private void process_transfer(string transfer)
        {
            string type0 = "transfer in";
            string date0 = "";
            string time0 = "";
            string amount0 = "";
            string device0 = " ";

            //pull date
            int locD = transfer.IndexOf("atedAt\":\"") + 9;
            int locaD = transfer.IndexOf("T", locD);
            date0 = transfer.Substring(locD, locaD - locD);
            //pull time
            int locbT = transfer.IndexOf("Z", locaD);
            time0 = transfer.Substring(locaD + 1, locbT - locaD - 1);
            //pull amount
            int loc = transfer.IndexOf("funding for ") + 12;
            int loca = transfer.IndexOf(" ", loc);
            amount0 = transfer.Substring(loc, loca - loc);
            //pull device
            if (transfer.IndexOf("\"devices\":[]") == -1)
            {
                int locDe = transfer.IndexOf("\"name\":\"") + 8;
                int locaDe = transfer.IndexOf("\"", locDe);
                device0 = transfer.Substring(locDe, locaDe - locDe);
            }



                string full = build_string(type0, date0, time0, amount0, device0);

                processedTransactions.Add(full);

        }

        private void process_payout(string payout)
        {
            string type0 = "payout";
            string date0 = "";
            string time0 = "";
            string amount0 = "";
            string device0 = " ";

            
            //pull date
            int locD = payout.IndexOf("atedAt\":\"") + 9;
            int locaD = payout.IndexOf("T", locD);
            date0 = payout.Substring(locD, locaD - locD);
            //pull time
            int locbT = payout.IndexOf("Z", locaD);
            time0 = payout.Substring(locaD + 1, locbT - locaD - 1);
            //pull amount
            int loc = payout.IndexOf("yout of ") + 8;
            int loca = payout.IndexOf(" ", loc);
            amount0 = payout.Substring(loc, loca - loc);
            //pull device
            if (payout.IndexOf("\"devices\":[]") == -1)
            {
                int locDe = payout.IndexOf("\"name\":\"") + 8;
                int locaDe = payout.IndexOf("\"", locDe);
                device0 = payout.Substring(locDe, locaDe - locDe);
            }

            if (payout.IndexOf(" HashPoints<") == -1)
            {
                string full = build_string(type0, date0, time0, amount0, device0);

                processedTransactions.Add(full);
            }
        }

        private void process_charge(string charge)
        {
            string type0 = "service charge";
            string date0 = "";
            string time0 = "";
            string amount0 = "";
            string device0 = " ";

            //pull date
            int locD = charge.IndexOf("atedAt\":\"") + 9;
            int locaD = charge.IndexOf("T", locD);
            date0 = charge.Substring(locD, locaD - locD);
            //pull time
            int locbT = charge.IndexOf("Z", locaD);
            time0 = charge.Substring(locaD+1, locbT - locaD -1);
            //pull amount
            int loc = charge.IndexOf("ce fee -") + 8;
            int loca = charge.IndexOf(" ", loc);
            amount0 = "-" + charge.Substring(loc, loca - loc);
            //pull device
            if (charge.IndexOf("\"devices\":[]") == -1)
            {
                int locDe = charge.IndexOf("\"name\":\"") + 8;
                int locaDe = charge.IndexOf("\"", locDe);
                device0 = charge.Substring(locDe, locaDe - locDe);
            }


            string full = build_string(type0, date0, time0, amount0, device0);
            processedTransactions.Add(full);
        }

        private string build_string(string type, string date, string time, string amount, string device)
        {
            string stringRet = "";

            stringRet = id_count.ToString() + "," + type + "," + date + "," + time + "," + amount + "," + device;
            id_count++;

            return stringRet;
        }

        private void writeTobox()
        {
            foreach (string tString in processedTransactions)
            {
                rtbInfo.Text = rtbInfo.Text + tString + "\n";
            }
        }

        private void fileSave()
        {
            
            SaveFileDialog saveFile1 = new SaveFileDialog();
            saveFile1.DefaultExt = "*.csv";
            saveFile1.Filter = "CSV Files|*.csv";

            if (saveFile1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
               saveFile1.FileName.Length > 0)
            {
                File.WriteAllLines(saveFile1.FileName, processedTransactions);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
               openFile.FileName.Length > 0)
            {
                txtOpen.Text = openFile.FileName;
            }
            if (txtOpen.Text != "")
            {
                btnParse.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnParse.Enabled = false;
        }
    }
}
