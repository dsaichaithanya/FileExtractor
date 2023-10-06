using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace FileExtractor
{
    public partial class Form1 : Form
    {
        string filepath = null;
        DataTable dt = new DataTable();
        string fileDate = null;     

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblversion.Text = "V.1.0.0.0";

            dt.Columns.Add("Folder No");
            dt.Columns.Add("Response Date");
            dt.Columns.Add("Response Time");
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    lblFileName.Text = openFileDialog1.SafeFileName;
                    filepath = openFileDialog1.FileName;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            List<string> finalData = new List<string>();
            try
            {
                if (filepath != null)
                {
                    string[] lines = File.ReadAllLines(filepath);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains("Order successfully created"))
                        {
                            string[] splitFolder = lines[i].Split(',');
                            string folderName = splitFolder[2].ToString().Substring(0, splitFolder[2].Length - 1);

                            if (lines[i - 1] != null)
                            {
                                string[] splitDt = lines[i - 1].Split('/');
                                string date = splitDt[1].ToString().Substring(1, splitDt[1].Length - 2);
                                fileDate = date;
                                date = date.Insert(4, "/");
                                date = date.Insert(7, "/");
                                string time = splitDt[2].ToString().Substring(1, splitDt[2].Length - 2);

                                ProcessData(folderName, date, time);
                            }
                        }
                    }

                    bool flag = ExportExcel();
                    
                    if(flag == true)
                    {
                        MessageBox.Show("File Exported Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error occurred while exporting", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select the file to process", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool ExportExcel()
        {
            bool flg = false;

            try
            {
                //Exporting to Excel
                string folderPath = "C:\\Excel\\";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, "LogFile" + fileDate);
                    wb.SaveAs(folderPath + "LogFile_" + fileDate + ".xlsx");
                }
                flg = true;
            }
          
            catch (Exception ex)
            {
                flg = false;
                //MessageBox.Show(ex.Message);
            }
            return flg;
        }

        private void ProcessData(string folderName, string date, string time)
        {
            if (folderName != null && date != null && time != null)
            {
                dt.Rows.Add(folderName, date, time);
            }
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
