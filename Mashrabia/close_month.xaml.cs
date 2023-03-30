using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mashrabia
{
    /// <summary>
    /// Interaction logic for colse_month.xaml
    /// </summary>
    public partial class close_month : Window
    {
        public close_month()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(From.Text == "" || To.Text == "" || (EXCEL.IsChecked==false && PDF.IsChecked==false)))
                {
                    Database database = new Database();
                    var ds = new System.Windows.Forms.FolderBrowserDialog();
                    if (ds.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        String temp = ds.SelectedPath + "\\" + Convert.ToDateTime(From.Text).ToString("yyyy-MM-dd") + " " + Convert.ToDateTime(To.Text).ToString("yyyy-MM-dd");
                        Directory.CreateDirectory(temp);
                        DataView tt = new DataView();
                        DataTable table = new DataTable();

                        tt = database.Search_Final_Data("تفاصيل شيفتات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل").DefaultView;
                        table = tt.ToTable();
                        database.insert_Final_Print(table, 1);
                        Final_Total_close report = new Final_Total_close();
                        report.Parameters["parameter1"].Value = "تفاصيل شيفتات";
                        report.Parameters["From"].Value = From.Text;
                        report.Parameters["To"].Value = To.Text;
                        report.Parameters["Total"].Value = database.Total_Final_Data("تفاصيل شيفتات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل").ToString();
                        report.Parameters["Total"].Visible = false;
                        report.Parameters["From"].Visible = false;
                        report.Parameters["To"].Visible = false;
                        report.Parameters["parameter1"].Visible = false;
                        if(PDF.IsChecked==true)
                            report.ExportToPdf(temp + "\\تفاصيل شيفتات.pdf");
                        if(EXCEL.IsChecked==true)
                            report.ExportToXlsx(temp + "\\تفاصيل شيفتات.xlsx");

                        tt = database.Search_Final_Data("تفاصيل ايرادات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل").DefaultView;
                        table = tt.ToTable();
                        database.insert_Final_Print(table, 2);
                        Total_Items_close report1 = new Total_Items_close();
                        report1.Parameters["parameter1"].Value = "تفاصيل ايرادات";
                        report1.Parameters["From"].Value = From.Text;
                        report1.Parameters["To"].Value = To.Text;
                        report1.Parameters["Total"].Value = database.Total_Final_Data("تفاصيل ايرادات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل");
                        report1.Parameters["Total"].Visible = false;
                        report1.Parameters["From"].Visible = false;
                        report1.Parameters["To"].Visible = false;
                        report1.Parameters["parameter1"].Visible = false;
                        if (PDF.IsChecked == true)
                            report1.ExportToPdf(temp + "\\تفاصيل ايرادات.pdf");
                        if (EXCEL.IsChecked == true)
                            report1.ExportToXlsx(temp + "\\تفاصيل ايرادات.xlsx");

                        tt = database.Search_Final_Data("تفاصيل مصروفات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل").DefaultView;
                        table = tt.ToTable();
                        database.insert_Final_Print(table, 3);
                        Total_Expenses_close report2 = new Total_Expenses_close();
                        report2.Parameters["parameter1"].Value = "تفاصيل مصروفات";
                        report2.Parameters["From"].Value = From.Text;
                        report2.Parameters["To"].Value = To.Text;
                        report2.Parameters["Total"].Value = database.Total_Final_Data("تفاصيل مصروفات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل").ToString();
                        report2.Parameters["Total"].Visible = false;
                        report2.Parameters["parameter1"].Visible = false;
                        report2.Parameters["From"].Visible = false;
                        report2.Parameters["To"].Visible = false;
                        if (PDF.IsChecked == true)
                            report2.ExportToPdf(temp + "\\تفاصيل مصروفات.pdf");
                        if (EXCEL.IsChecked == true)
                            report2.ExportToXlsx(temp + "\\تفاصيل مصروفات.xlsx");
                        database.Delete_Final_Data("تفاصيل شيفتات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل");
                        database.Delete_Final_Data("تفاصيل ايرادات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل");
                        database.Delete_Final_Data("تفاصيل مصروفات", "", Convert.ToDateTime(From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To.Text).ToString("yyyy/MM/dd"), "عرض الكل");
                        MessageBox.Show("تم بنجاح");
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("تأكد من ادخال تاريخ صحيح واخيار نوع الملف");                    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("تأكد من ادخال تاريخ صحيح واخيار نوع الملف");
                From.Text = "";
                To.Text = "";               

            }
        }
    }
}
