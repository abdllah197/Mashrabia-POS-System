using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Mashrabia
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Database database = new Database();
        private databaseOrderSys database_OrderSys = new databaseOrderSys();
        private DataTable sort_search = new DataTable();
        private int undoB = 0;
        private int tableNumBer = 0;
        private List<int> tables_ = new List<int>();
        private Internet_Carts internet_carts = new Internet_Carts();
        private int Microtik_deletion = 0;
        
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);        

        public MainWindow()
        {
            InitializeComponent();
            database.files();
            if (!database.Serial())
            {
                while(!database.Serial())
                {
                    InputDialog input_dialog = new InputDialog("Enter Serial Number ", "");
                    if (input_dialog.ShowDialog() == true)
                    {
                        if (database.insert_serial(input_dialog.Answer) == true)
                        {
                            MessageBox.Show("Success");
                            break;
                        }
                        else
                            MessageBox.Show("Wrong Serial Number");
                    }
                    else
                        Environment.Exit(0);
                }                
                               

            }
            string procName = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by the "procName"       
            Process[] processes = Process.GetProcessesByName(procName);            
            if (processes.Length > 1)
            {
                MessageBox.Show("😡البرنامج مفتوح اعم بتفتحو تانى ليه");                
                Environment.Exit(0);
            }

                if (database.state_read()==0)
            {

                _1.Visibility = Visibility.Hidden;
                //_3.Visibility = Visibility.Hidden;
                _4.Visibility = Visibility.Hidden;
                //_5.Visibility = Visibility.Hidden;
                _6.Visibility = Visibility.Hidden;
                setting.IsEnabled = false;
                tabs_.SelectedIndex = 2;
                Log_in.Content = "تسجيل الدخول";
            }
            else
            {
                _1.Visibility = Visibility.Visible;
                //_3.Visibility = Visibility.Visible;
                _4.Visibility = Visibility.Visible;
                //_5.Visibility = Visibility.Visible;
                _6.Visibility = Visibility.Visible;
                setting.IsEnabled = true;
                Log_in.Content = "تسجيل الخروج";
            }
            TablesType.Items.Add("حركة المخزن");
            TablesType.Items.Add("المخزن");
            TablesType.SelectedIndex = 0;
            
            dg.ItemsSource = database.ViewTable("Log_File").DefaultView;
            
            AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Table0", "Tables").DefaultView;
            
            ExpensesView.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
            
            End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
            
            Final_View.ItemsSource = database.Final_Data_View("Final_Total").DefaultView;
            
            Products.ItemsSource = database.GetProducts();
            ProductUnit.ItemsSource = database.GetUnits();
            TypeIn.Items.Add("المصروفات");            
            TypeIn.Items.Add("اليوميات");
            TypeIn.SelectedIndex = 0;

            OrderType.Items.Add("طاولات");
            OrderType.Items.Add("تيك اواي");
            OrderType.Items.Add("ادارة");
            OrderType.SelectedIndex = 0;

            ViewEnd.Items.Add("ايرادات");
            ViewEnd.Items.Add("مصروفات");
            ViewEnd.Items.Add("يوميات");
            ViewEnd.Items.Add("ادارة");
            ViewEnd.SelectedIndex = 0;

            mowarden_table.Items.Add("حركة موردين");
            mowarden_table.Items.Add("موردين");
            mowarden_table.SelectedIndex = 0;

            Shift_Choose.Items.Add("شيفت صباحي");
            Shift_Choose.Items.Add("شيفت مسائي");

            Final_Data_View.Items.Add("تفاصيل شيفتات");
            Final_Data_View.Items.Add("تفاصيل ايرادات");
            Final_Data_View.Items.Add("تفاصيل مصروفات");
            Final_Data_View.SelectedIndex = 0;

            Final_Data_Shift.Items.Add("عرض الكل");
            Final_Data_Shift.Items.Add("شيفت صباحي");
            Final_Data_Shift.Items.Add("شيفت مسائي");
            Final_Data_Shift.SelectedIndex = 0;

            
            mowarden.ItemsSource = database.GetMowarden();


            int empty = database_OrderSys.Last_ID("Shift");
            if (empty != 0)
                TotalIncome.Content = database_OrderSys.Total("Shift");
            else
                TotalIncome.Content = "0";
            empty = database_OrderSys.Last_ID("Wages");
            if (empty != 0)
                TotalWages.Content = database_OrderSys.Total("Wages");
            else
                TotalWages.Content = "0";
            empty = database_OrderSys.Last_ID("Expenses");
            if (empty != 0)
                TotalExpenses.Content = database_OrderSys.Total("Expenses");
            else
                TotalExpenses.Content = "0";
            empty = database_OrderSys.Last_ID("Edara");
            if (empty != 0)
                TotalEdara.Content = database_OrderSys.Total("Edara");
            else
                TotalEdara.Content = "0";
            TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
            if (Convert.ToDouble(TotalNet.Content) > 0)
            {
                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
            }
            else if (Convert.ToDouble(TotalNet.Content) < 0)
            {
                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
            }
            else
            {
                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
            }
            Undo.IsEnabled = false;
            ItemsTabs();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += timer_Tick;
            timer.Start();            
            int x = database_OrderSys.PrintToEnableChangeTableNumber();
            if (x != 0)
            {
                String op = database_OrderSys.temp_new_orders_filter("tempNewOrder", "NewOrdersPrint");
                if (op != "null")
                {
                    op = op.Replace("Table", "");
                    op = op.Replace("TakeAway", "تيك اواي");
                    op = op.Replace("Edara", "ادارة");
                    NewOrders neworders = new NewOrders();

                    if (op != "ادارة" && op != "تيك اواي")
                        neworders.Parameters["TableNumber"].Value = ("طاولة" + database_OrderSys.toPersianNumber(op));
                    else
                        neworders.Parameters["TableNumber"].Value = (op);
                    neworders.Parameters["TableNumber"].Visible = false;
                    ReportPrintTool pt = new ReportPrintTool(neworders);
                    pt.ShowPreviewDialog();
                }
            }
            Stock_Total.Content = database.Total_Stock("").ToString();
            if (Convert.ToDouble(Stock_Total.Content) > 0)
            {
                Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
            }
            else if (Convert.ToDouble(Stock_Total.Content) < 0)
            {
                Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
            }
            else
            {
                Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
            }


            /*List<String> tables = new List<String>();           
            tables = database_OrderSys.Name_Of_Tables("Tables");            
            for (int i = 0; i < tables.Count(); i++)
            {
                if (tables.ElementAt(i) != "Table0" && tables.ElementAt(i).Contains("Table"))
                    tables_.Add(Convert.ToInt32(tables.ElementAt(i).Replace("Table", "")));
            }
            tables_.Sort();*/
            

        }
        

        public class ColorConverter
        {
            public static SolidColorBrush GetColorFromHexa(string hexaColor)
            {
                return new SolidColorBrush(
                    Color.FromArgb(
                        Convert.ToByte(hexaColor.Substring(1, 2), 16),
                        Convert.ToByte(hexaColor.Substring(3, 2), 16),
                        Convert.ToByte(hexaColor.Substring(5, 2), 16),
                        Convert.ToByte(hexaColor.Substring(7, 2), 16)
                    )
                );
            }
        }

        private void ItemInsert(object sender, RoutedEventArgs e)
        {
            int numbers = 1;
            String Name = "", Category = "", Table_Number = "Table" + CurrentTableNumber.Content.ToString();
            var button = sender as Button;
            if (button != null && CurrentTableNumber.Content.ToString() != "#")
            {
                if (Total_Items.Text != "")
                    numbers = Convert.ToInt16(Total_Items.Text);

                Name = button.Content.ToString();
                TabItem ti = tabcontrol.SelectedItem as TabItem;
                Category = ti.Header.ToString();
                Total_Items.Text = "";
                if (OrderType.SelectedIndex == 0)
                {
                    database_OrderSys.Insert_Item(Category, Name, numbers, Table_Number);
                    AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable(Table_Number, "Tables").DefaultView;
                    Total_Price.Content = database_OrderSys.Total("Table" + CurrentTableNumber.Content.ToString());
                }
                else if (OrderType.SelectedIndex == 1)
                {
                    database_OrderSys.Insert_Item(Category, Name, numbers, "TakeAway");
                    AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("TakeAway", "TakeAway&Edara").DefaultView;
                    Total_Price.Content = database_OrderSys.Total("TakeAway");
                }
                else if (OrderType.SelectedIndex == 2)
                {
                    InputDialog input_dialog = new InputDialog("Enter Notes For This Record ", "");
                    if (input_dialog.ShowDialog() == true)
                    {
                        if (input_dialog.Answer != "")
                        {
                            database_OrderSys.Insert_Edara(Category, Name, numbers, "Edara", input_dialog.Answer);
                            AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                            Total_Price.Content = database_OrderSys.Total("Edara");
                            if (ViewEnd.SelectedIndex == 2)
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                            TotalEdara.Content = database_OrderSys.Total("Edara");
                            TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                            if (Convert.ToDouble(TotalNet.Content) > 0)
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                            }
                            else if (Convert.ToDouble(TotalNet.Content) < 0)
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                            }
                            else
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            }

                        }
                        else
                            MessageBox.Show("Can Not Be Empty");
                    }
                }
                Total_Items.Focus();
            }
            else
                MessageBox.Show("Select Table First");            
        }

        private void button_action()
        {
            if(Date.Text=="")
            {
                DateTime d = DateTime.Now;
                Date.Text = d.ToString("yyyy/MM/dd");
            }
            if
                (
                IN.IsChecked == true
                && Products.SelectedItem != null
                && ProductUnit.SelectedItem != null
                && !Total.Text.Equals("")
                && !Quantity.Text.Equals("")
                && !Total.Text.Equals(".")
                && !Quantity.Text.Equals(".")
                )
            {
                if (ProductUnit.SelectedIndex == 1 && Quantity.Text.Contains("."))
                {
                    MessageBox.Show("Quantity Must Be Non Fractional Number Because Of Unit : " + ProductUnit.SelectedValue.ToString());
                    Quantity.Focus();
                    Quantity.SelectAll();
                }
                else
                {
                    if (Date.Text == "")
                    {
                        DateTime D = DateTime.Now;
                        database.INOUTStock(Products.SelectedItem.ToString(), ProductUnit.SelectedItem.ToString(), Quantity.Text, Total.Text, D.ToString("yyyy/MM/dd"), 0);
                    }
                    else
                        database.INOUTStock(Products.SelectedItem.ToString(), ProductUnit.SelectedItem.ToString(), Quantity.Text, Total.Text, Convert.ToDateTime(Date.Text).ToString("yyyy/MM/dd"), 0);
                    if (TablesType.SelectedIndex == 0)
                        dg.ItemsSource = database.ViewTable("Log_File").DefaultView;
                    else if (TablesType.SelectedIndex == 1)
                        dg.ItemsSource = database.ViewTable("Stock").DefaultView;

                    Products.SelectedIndex = -1;
                    ProductUnit.IsEnabled = true;
                    ProductUnit.SelectedIndex = -1;
                    Quantity.Text = "";
                    Total.Text = "";
                    DateTime today = DateTime.Today;
                    Date.Text = today.Date.ToString();
                    IN.IsChecked = true;
                    OUT.IsChecked = false;
                    Search.Text = "";
                    Date_Sort_From.Text = "";
                    Date_Sort_To.Text = "";
                    undoB++;
                    Undo.IsEnabled = true;
                    Stock_Total.Content = database.Total_Stock("").ToString();
                    if (Convert.ToDouble(Stock_Total.Content) > 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(Stock_Total.Content) < 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                }
            }

            else if
                (
                OUT.IsChecked == true
                && Products.SelectedItem != null
                && ProductUnit.SelectedItem != null
                && !Quantity.Text.Equals("")
                && !Total.Text.Equals(".")
                && !Quantity.Text.Equals(".")
                )
            {
                if (ProductUnit.SelectedIndex == 1 && Quantity.Text.Contains("."))
                {
                    MessageBox.Show("Quantity Must Be Non Fractional Number Because Of Unit : " + ProductUnit.SelectedValue.ToString());
                    Quantity.Focus();
                    Quantity.SelectAll();
                }
                else
                {
                    database.INOUTStock(Products.SelectedItem.ToString(), ProductUnit.SelectedItem.ToString(), Quantity.Text, "0", Convert.ToDateTime(Date.Text).ToString("yyyy/MM/dd"), 1);

                    if (TablesType.SelectedIndex == 0)
                        dg.ItemsSource = database.ViewTable("Log_File").DefaultView;
                    else if (TablesType.SelectedIndex == 1)
                        dg.ItemsSource = database.ViewTable("Stock").DefaultView;
                    undoB++;
                    Undo.IsEnabled = true;
                    Products.SelectedIndex = -1;
                    ProductUnit.IsEnabled = true;
                    ProductUnit.SelectedIndex = -1;
                    Quantity.Text = "";
                    Total.Text = "";
                    DateTime today = DateTime.Today;
                    Date.Text = today.Date.ToString();
                    IN.IsChecked = true;
                    OUT.IsChecked = false;
                    Search.Text = "";
                    Date_Sort_From.Text = "";
                    Date_Sort_To.Text = "";
                    Stock_Total.Content = database.Total_Stock("").ToString();
                    if (Convert.ToDouble(Stock_Total.Content) > 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(Stock_Total.Content) < 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Check All Fields");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            button_action();
        }

        private void TablesType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TablesType.SelectedItem.ToString().Equals("حركة المخزن"))
            {
                dg.ItemsSource = database.ViewTable("Log_File").DefaultView;
                Date_Sort_From.IsEnabled = true;
                Date_Sort_To.IsEnabled = true;
                Stock_Total.Visibility = Visibility.Hidden;
                Total_Stock_Label.Visibility = Visibility.Hidden;

            }
            else if (TablesType.SelectedItem.ToString().Equals("المخزن"))
            {
                dg.ItemsSource = database.ViewTable("Stock").DefaultView;
                Date_Sort_From.IsEnabled = false;
                Date_Sort_To.IsEnabled = false;
                Stock_Total.Visibility = Visibility.Visible;
                Total_Stock_Label.Visibility = Visibility.Visible;


            }

            Search.Text = "";

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(Total!=null)
                Total.IsEnabled = true;

            if (OUT != null)
                OUT.IsChecked = false;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            Total.IsEnabled = false;

            if (IN != null)
                IN.IsChecked = false;
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Total_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Products_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Products.SelectedItem != null)
            {
                if (database.CheckProductUnit(Products.SelectedItem.ToString()) != "Error Unit")
                {
                    ProductUnit.SelectedItem = database.CheckProductUnit(Products.SelectedItem.ToString());
                    ProductUnit.IsEnabled = false;
                }
                else if (database.CheckProductUnit(Products.SelectedItem.ToString()) != "Error Unit")
                {
                    ProductUnit.SelectedItem = database.CheckProductUnit(Products.SelectedItem.ToString());
                    ProductUnit.IsEnabled = false;
                }
                else
                    ProductUnit.IsEnabled = true;
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TablesType.SelectedItem.ToString().Equals("حركة المخزن"))
            {
                if (!Search.Text.Contains("'"))
                {
                    if (Date_Sort_From.Text != "" && Date_Sort_To.Text != "")
                        dg.ItemsSource = database.Sort_LogFile(Convert.ToDateTime(Date_Sort_From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(Date_Sort_To.Text).ToString("yyyy/MM/dd"), Search.Text).DefaultView;
                    else if (Date_Sort_From.Text == "" && Date_Sort_To.Text != "")
                        dg.ItemsSource = database.Sort_LogFile("", Convert.ToDateTime(Date_Sort_To.Text).ToString("yyyy/MM/dd"), Search.Text).DefaultView;
                    else if (Date_Sort_To.Text == "" && Date_Sort_From.Text != "")
                        dg.ItemsSource = database.Sort_LogFile(Convert.ToDateTime(Date_Sort_From.Text).ToString("yyyy/MM/dd"), "", Search.Text).DefaultView;
                    else
                        dg.ItemsSource = database.Sort_LogFile("", "", Search.Text).DefaultView;
                }

            }

            else if (TablesType.SelectedItem.ToString().Equals("المخزن"))
            {
                if (!Search.Text.Equals("") && !Search.Text.Contains("'"))
                {
                    DataTable t1 = new DataTable();
                    t1 = database.ViewTable("Stock");
                    DataView dv1 = t1.DefaultView;
                    int n = 0;
                    Boolean isNumeric = int.TryParse(Search.Text, out n);
                    if (isNumeric)
                        dv1.RowFilter = "[كود]=" + Search.Text;
                    else
                        dv1.RowFilter = "[اسم الصنف] LIKE '%" + Search.Text + "%'";
                    DataTable dtNew = dv1.ToTable();
                    dg.ItemsSource = dtNew.DefaultView;
                    Stock_Total.Content = database.Total_Stock(Search.Text).ToString();
                    if (Convert.ToDouble(Stock_Total.Content) > 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(Stock_Total.Content) < 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                }
                else
                {
                    dg.ItemsSource = database.ViewTable("Stock").DefaultView;
                    Stock_Total.Content = database.Total_Stock("").ToString();
                    if (Convert.ToDouble(Stock_Total.Content) > 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(Stock_Total.Content) < 0)
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                }

            }


        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

            DataTable table = new DataTable();
            table = ((DataView)dg.ItemsSource).ToTable();

            if (TablesType.SelectedIndex == 0)
            {
                database.insert_Table(table, 1);
                XtraReport1 report = new XtraReport1();
                report.Parameters["parameter1"].Value = "حركة المخزن";
                report.Parameters["parameter1"].Visible = false;

                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreviewDialog();
            }
            else
            {
                database.insert_Table(table, 2);
                XtraReport2 report = new XtraReport2();
                report.Parameters["parameter1"].Value = "المخزن";
                report.Parameters["parameter1"].Visible = false;

                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreviewDialog();
            }


        }

        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                object i = dg.SelectedItem;
                string id = "error", name = "error", unit = "error", quantity = "error", total = "error";
                if (dg.SelectedCells.Count != 0)
                {
                    id = (dg.SelectedCells[4].Column.GetCellContent(i) as TextBlock).Text;
                    name = (dg.SelectedCells[3].Column.GetCellContent(i) as TextBlock).Text;
                    unit = (dg.SelectedCells[2].Column.GetCellContent(i) as TextBlock).Text;
                    quantity = (dg.SelectedCells[1].Column.GetCellContent(i) as TextBlock).Text;
                    total = (dg.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                }
                if (!id.Equals("error"))
                {
                    if (TablesType.SelectedItem.ToString().Equals("المخزن") && MessageBox.Show("Are You Sure To Delete This Record", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        DateTime t = DateTime.Today;
                        database.deleteFromTabel(TablesType.SelectedItem.ToString(), id, name, unit, quantity, total, t.ToString("d/M/yyyy"));
                        undoB = 0;
                        Undo.IsEnabled = false;
                        Search.Text = "";
                        dg.ItemsSource = database.ViewTable("Stock").DefaultView;
                        Stock_Total.Content = database.Total_Stock("").ToString();
                        if (Convert.ToDouble(Stock_Total.Content) > 0)
                        {
                            Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                        }
                        else if (Convert.ToDouble(Stock_Total.Content) < 0)
                        {
                            Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                        }
                        else
                        {
                            Stock_Total.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        }
                    }

                    Search.Text = "";

                }
                else
                    MessageBox.Show("Please Select Record First");


            }
        }

        private void Total_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void Quantity_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void Date_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void Button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void ProductUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void Products_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void IN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void OUT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                button_action();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure To Undo Last Record ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                database.undo();
                dg.ItemsSource = database.ViewTable("Log_File").DefaultView;

                undoB--;
                if (undoB == 0)
                    Undo.IsEnabled = false;
                if (TablesType.SelectedIndex == 0)
                {
                    dg.ItemsSource = database.ViewTable("Log_File").DefaultView;
                    Search.Text = "";
                    Date_Sort_From.Text = "";
                    Date_Sort_To.Text = "";
                }
                else if (TablesType.SelectedIndex == 1)
                {
                    dg.ItemsSource = database.ViewTable("Stock").DefaultView;
                    Search.Text = "";
                    Date_Sort_From.Text = "";
                    Date_Sort_To.Text = "";
                }
            }

        }

        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
            
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));
        }

        private void TableNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void ItemsTabs()
        {
            List<String> category = new List<String>();
            List<String> names = new List<String>();
            category = database_OrderSys.Name_Of_Tables("Category");

            for (int i = 0; i < category.Count; i++)
            {
                TabItem ti = new TabItem();
                ti.Header = category.ElementAt(i);
                ti.FontSize = 14;
                ti.FontWeight = FontWeights.Bold;
                names = database_OrderSys.Category_Items(category.ElementAt(i), "الاسم");
                ScrollViewer sc = new ScrollViewer();
                WrapPanel wp = new WrapPanel();
                wp.FlowDirection = FlowDirection.RightToLeft;
                for (int y = 0; y < names.Count; y++)
                {
                    Button s = new Button();
                    s.Content = names.ElementAt(y);
                    s.Name = "button" + y;
                    s.Height = 80;
                    s.Width = 135;
                    s.FontSize = 20;
                    s.FontWeight = FontWeights.Bold;

                    s.Style = (Style)TryFindResource("MetroButton");
                    
                    MaterialDesignThemes.Wpf.ShadowAssist.SetShadowDepth(s,MaterialDesignThemes.Wpf.ShadowDepth.Depth4);
                    s.Margin = new Thickness(2, 6, 2, 6);
                    s.Click += ItemInsert;
                    wp.Children.Add(s);
                }
                sc.Content = wp;
                sc.FlowDirection = FlowDirection.LeftToRight;
                ti.Content = sc;
                tabcontrol.Items.Add(ti);
            }
        }

        private void table_num()
        {
            DataTable Orders = new DataTable();
            if (TableNum.Text == "")
            {
                
                Orders = database_OrderSys.ViewTable("Table0", "Tables");

                CurrentTableNumber.Content = "#";
                Total_Price.Content = "0";
                AllOrdersOnTable.ItemsSource = Orders.DefaultView;
            }
            if (TableNum.Text.Contains(" "))
            {

                string s = TableNum.Text;
                if (s.Length > 1)
                {
                    s = s.Substring(0, s.Length - 1);
                }
                else
                {
                    s = "";
                }
                TableNum.Text = s;


                Orders = database_OrderSys.ViewTable("Table0", "Tables");

                CurrentTableNumber.Content = "#";
                Total_Price.Content = "0";
                AllOrdersOnTable.ItemsSource = Orders.DefaultView;
            }
            else if (TableNum.Text != "")
            {
                if (Convert.ToInt16(TableNum.Text) > 100 || Convert.ToInt16(TableNum.Text) < 1)
                {

                    Orders = database_OrderSys.ViewTable("Table0", "Tables");

                    CurrentTableNumber.Content = "#";
                    Total_Price.Content = "0";
                    AllOrdersOnTable.ItemsSource = Orders.DefaultView;

                }
            }
            int id = database_OrderSys.Last_ID("TempNewOrder");
            if (id == 0)
            {
                if (TableNum.Text != "" && !TableNum.Text.Contains(" "))
                {

                    int i = Convert.ToInt32(TableNum.Text);
                        if ( i>0 && i<=100)
                        {
                            Orders = database_OrderSys.ViewTable("Table" + (i), "Tables");
                            CurrentTableNumber.Content = (i);
                            tableNumBer = (i);
                            Total_Price.Content = database_OrderSys.Total("Table" + (i));
                            AllOrdersOnTable.ItemsSource = Orders.DefaultView;
                        }
                    
                }
            }
            else
            {
                String op = database_OrderSys.temp_new_orders_filter("tempNewOrder", "NewOrdersPrint");
                if (op != "null")
                {
                    op = op.Replace("Table", "");
                    op = op.Replace("TakeAway", "تيك اواي");
                    op = op.Replace("Edara", "ادارة");
                    NewOrders neworders = new NewOrders();

                    if (op != "ادارة" && op != "تيك اواي")
                        neworders.Parameters["TableNumber"].Value = ("طاولة" + database_OrderSys.toPersianNumber(op));
                    else
                        neworders.Parameters["TableNumber"].Value = (op);
                    neworders.Parameters["TableNumber"].Visible = false;
                    ReportPrintTool pt = new ReportPrintTool(neworders);
                    pt.ShowPreviewDialog();
                    if (TableNum.Text != "")
                    {
                        int i = Convert.ToInt32(TableNum.Text);
                        if (i > 0 && i <= 100)
                        {
                            Orders = database_OrderSys.ViewTable("Table" + (i), "Tables");
                            CurrentTableNumber.Content = (i);
                            tableNumBer = (i);
                            Total_Price.Content = database_OrderSys.Total("Table" + (i));
                            AllOrdersOnTable.ItemsSource = Orders.DefaultView;
                        }
                    }
                }
            }



        }

        
        private void timer_Tick(object sender, EventArgs e)
        {
            Time.Content = DateTime.Now.ToString("hh:mm tt");
            DateLive.Content = DateTime.Today.ToString("dd/MM/yyyy");
            Microtik_deletion++;
            if (Microtik_deletion ==(6000) * 15 )
            {                
                Microtik_deletion = 0;
                internet_carts.automatic_delete();                
            }
        }

        private void DeleteFromTable_Click(object sender, RoutedEventArgs e)
        {
            object i = AllOrdersOnTable.SelectedItem;
            string item = "error", quantity = "error";
            if (AllOrdersOnTable.SelectedCells.Count != 0)
            {
                item = (AllOrdersOnTable.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                quantity = (AllOrdersOnTable.SelectedCells[1].Column.GetCellContent(i) as TextBlock).Text;
            }

            if (!item.Equals("error"))
            {
                if (OrderType.SelectedIndex == 0 && CurrentTableNumber.Content.ToString() != "ادارة" && CurrentTableNumber.Content.ToString() != "تيك اواي" && CurrentTableNumber.Content.ToString() != "#" && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    database_OrderSys.delete_order("Table" + CurrentTableNumber.Content, item, quantity);
                    AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Table" + CurrentTableNumber.Content, "Tables").DefaultView;
                    Total_Price.Content = database_OrderSys.Total("Table" + CurrentTableNumber.Content);
                }
                else if (CurrentTableNumber.Content.ToString() == "تيك اواي" && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    database_OrderSys.delete_order("TakeAway", item, quantity);
                    AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("TakeAway", "TakeAway&Edara").DefaultView;
                    Total_Price.Content = database_OrderSys.Total("TakeAway");
                }
                else if (CurrentTableNumber.Content.ToString() == "ادارة" && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    database_OrderSys.delete_order("Edara", item, quantity);
                    AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                    Total_Price.Content = database_OrderSys.Total("Edara");
                    TotalEdara.Content = database_OrderSys.Total("Edara");
                    TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                    if (Convert.ToDouble(TotalNet.Content) > 0)
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(TotalNet.Content) < 0)
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                    if (ViewEnd.SelectedIndex == 2)
                        End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                    End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                }

            }
            else
                MessageBox.Show("Please Select Order First Or Select Valid Table");

        }

        private void AllOrdersOnTable_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                object i = AllOrdersOnTable.SelectedItem;
                string item = "error", quantity = "error";
                if (AllOrdersOnTable.SelectedCells.Count != 0)
                {
                    item = (AllOrdersOnTable.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                    quantity = (AllOrdersOnTable.SelectedCells[1].Column.GetCellContent(i) as TextBlock).Text;
                }

                if (!item.Equals("error"))
                {                    
                        if (OrderType.SelectedIndex == 0 && CurrentTableNumber.Content.ToString() != "ادارة" && CurrentTableNumber.Content.ToString() != "تيك اواي" && CurrentTableNumber.Content.ToString() != "#" && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            database_OrderSys.delete_order("Table" + CurrentTableNumber.Content, item, quantity);
                            AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Table" + CurrentTableNumber.Content, "Tables").DefaultView;
                            Total_Price.Content = database_OrderSys.Total("Table" + CurrentTableNumber.Content);
                        }
                        else if (CurrentTableNumber.Content.ToString() == "تيك اواي" && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            database_OrderSys.delete_order("TakeAway", item, quantity);
                            AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("TakeAway", "TakeAway&Edara").DefaultView;
                            Total_Price.Content = database_OrderSys.Total("TakeAway");
                        }
                        else if (CurrentTableNumber.Content.ToString() == "ادارة" && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            database_OrderSys.delete_order("Edara", item, quantity);
                            AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                            Total_Price.Content = database_OrderSys.Total("Edara");
                            TotalEdara.Content = database_OrderSys.Total("Edara");
                            TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                            if (Convert.ToDouble(TotalNet.Content) > 0)
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                            }
                            else if (Convert.ToDouble(TotalNet.Content) < 0)
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                            }
                            else
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            }
                            if (ViewEnd.SelectedIndex == 2)
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                            End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                        }
                    
                }
                else
                    MessageBox.Show("Please Select Order First Or Select Valid Table");
            }
        }

        private void ChangeTable_Click(object sender, RoutedEventArgs e)
        {
            int n = 0, f = 0;
            InputDialog input_dialog = new InputDialog("Enter Number Of New Table", "");
            if (CurrentTableNumber.Content.ToString() != "#")
            {

                if (input_dialog.ShowDialog() == true)
                {
                    Boolean isNumeric = int.TryParse(input_dialog.Answer, out n);

                    if (isNumeric)
                        if (n > 0 && n < 501)
                            if (n != Convert.ToInt16(CurrentTableNumber.Content.ToString()))
                            {
                                int id = database_OrderSys.Last_ID("TempNewOrder");
                                if (id == 0)
                                    f = database_OrderSys.change_table_num("Table" + CurrentTableNumber.Content, "Table" + input_dialog.Answer);
                                else
                                    MessageBox.Show("Please Print New Order First Before Change Table");

                            }
                            else
                                MessageBox.Show("You Can Not Move Table" + n + " To The Same Table" + n + "");
                        else
                            MessageBox.Show("Enter Valid Table Number");
                    else
                        MessageBox.Show("Enter Valid Table Number");
                }
            }
            else
                MessageBox.Show("Select Valid Table First");
            
            if (f == 1)
            {
                TableNum.Text = input_dialog.Answer;
                CurrentTableNumber.Content = input_dialog.Answer;
            }
        }

        private void PrintNewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTableNumber.Content.ToString() != "#")
            {
                int x = database_OrderSys.PrintToEnableChangeTableNumber();
                if (x != 0)
                {
                    String op = database_OrderSys.temp_new_orders_filter("tempNewOrder", "NewOrdersPrint");
                    if (op != "null")
                    {
                        NewOrders neworders = new NewOrders();
                        if (CurrentTableNumber.Content.ToString() == "تيك اواي")
                            neworders.Parameters["TableNumber"].Value = ("تيك اواي");
                        else if (CurrentTableNumber.Content.ToString() == "ادارة")
                            neworders.Parameters["TableNumber"].Value = ("ادارة");
                        else
                        {
                            neworders.Parameters["TableNumber"].Value = ("طاولة" + database_OrderSys.toPersianNumber(CurrentTableNumber.Content.ToString()));
                            TableNum.Text = CurrentTableNumber.Content.ToString();
                        }
                        neworders.Parameters["TableNumber"].Visible = false;
                        ReportPrintTool pt = new ReportPrintTool(neworders);
                        pt.ShowPreviewDialog();
                        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/PrintOrders.mdb";
                        if (File.Exists(path))                        
                            System.IO.File.Delete(path);                        
                        database.files();
                    }
                }
                else
                    MessageBox.Show("No Orders to Print");
            }
            else
                MessageBox.Show("Select Table First");
        }

        private void Pay_Print_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTableNumber.Content.ToString() != "#")
            {
                int id = database_OrderSys.Last_ID("TempNewOrder");
                if (id != 0)
                {
                    if (CurrentTableNumber.Content.ToString() != "#")
                    {
                        int x = database_OrderSys.PrintToEnableChangeTableNumber();
                        if (x != 0)
                        {
                            if (database_OrderSys.Last_ID_S("Bills") > 900000)
                                database_OrderSys.deleteBillTable();
                            String op = database_OrderSys.temp_new_orders_filter("tempNewOrder", "NewOrdersPrint");
                            if (op != "null")
                            {
                                NewOrders neworders = new NewOrders();
                                if (CurrentTableNumber.Content.ToString() == "تيك اواي")
                                    neworders.Parameters["TableNumber"].Value = ("تيك اواي");
                                else if (CurrentTableNumber.Content.ToString() == "ادارة")
                                    neworders.Parameters["TableNumber"].Value = ("ادارة");
                                else
                                {
                                    neworders.Parameters["TableNumber"].Value = ("طاولة" + database_OrderSys.toPersianNumber(CurrentTableNumber.Content.ToString()));
                                    TableNum.Text = CurrentTableNumber.Content.ToString();
                                }
                                neworders.Parameters["TableNumber"].Visible = false;
                                ReportPrintTool pt = new ReportPrintTool(neworders);
                                pt.ShowPreviewDialog();
                                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/PrintOrders.mdb";
                                if (File.Exists(path))
                                    System.IO.File.Delete(path);
                                database.files();
                            }
                        }
                        else
                            MessageBox.Show("No Orders to Print");
                    }
                    else
                        MessageBox.Show("Select Table First");
                }
                if (CurrentTableNumber.Content.ToString() != "تيك اواي" && CurrentTableNumber.Content.ToString() != "ادارة")
                    {
                        int done = database_OrderSys.Payment_Print("Table" + CurrentTableNumber.Content.ToString());
                    if (done != 0)
                    {                        
                        PaymentBill pay = new PaymentBill();
                        pay.Parameters["TableNum"].Value = ("طاولة" + database_OrderSys.toPersianNumber(CurrentTableNumber.Content.ToString()));
                        pay.Parameters["Total"].Value = (database_OrderSys.toPersianNumber(Total_Price.Content.ToString()));
                        pay.Parameters["ID"].Value = database_OrderSys.Last_ID_S("Bills");
                        pay.Parameters["TableNum"].Visible = false;
                        pay.Parameters["Total"].Visible = false;
                        pay.Parameters["ID"].Visible = false;
                        TableNum.Text = CurrentTableNumber.Content.ToString();
                        ReportPrintTool pt = new ReportPrintTool(pay);
                        pt.ShowPreviewDialog();
                        TableNum.Text = "";
                        CurrentTableNumber.Content = "#";
                        Total_Price.Content = "0";
                        AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Table0", "Tables").DefaultView;
                        database_OrderSys.deletePaymentTable();
                        TotalIncome.Content = database_OrderSys.Total("Shift");
                        TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                        if (Convert.ToDouble(TotalNet.Content) > 0)
                        {
                            TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                            safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                        }
                        else if (Convert.ToDouble(TotalNet.Content) < 0)
                        {
                            TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                            safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                        }
                        else
                        {
                            TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        }
                        if (ViewEnd.SelectedIndex == 0)
                            End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
                        

                    }
                   
                    }
                    else if (CurrentTableNumber.Content.ToString() == "تيك اواي")
                    {
                        int done = database_OrderSys.Payment_Print("TakeAway");
                        if (done != 0)
                        {
                            PaymentBill pay = new PaymentBill();
                            pay.Parameters["TableNum"].Value = ("تيك اواي");
                            pay.Parameters["Total"].Value = (database_OrderSys.toPersianNumber(Total_Price.Content.ToString()));
                            pay.Parameters["ID"].Value = database_OrderSys.Last_ID_S("Bills");
                            pay.Parameters["TableNum"].Visible = false;
                            pay.Parameters["ID"].Visible = false;
                            pay.Parameters["Total"].Visible = false;
                            ReportPrintTool pt = new ReportPrintTool(pay);
                            pt.ShowPreviewDialog();
                            database_OrderSys.deletePaymentTable();
                            OrderType.SelectedIndex = 0;
                            OrderType.SelectedIndex = 1;
                            TotalIncome.Content = database_OrderSys.Total("Shift");
                            TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                            if (Convert.ToDouble(TotalNet.Content) > 0)
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                            }
                            else if (Convert.ToDouble(TotalNet.Content) < 0)
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                            }
                            else
                            {
                                TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                                safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            }
                            if (ViewEnd.SelectedIndex == 0)
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
                            
                        }
                        
                    }
                
            }
            else
                MessageBox.Show("Select Table First");
        }

        private void TypeIn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeIn.SelectedIndex == 0)
            {
                typeNames.Content = "بيان";                
                ExpensesView.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
            }
            else if (TypeIn.SelectedIndex == 1)
            {
                typeNames.Content = "الاسم";
                ExpensesView.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
            }
            typeNames.Focus();
        }

        private void Expenses_Clk()
        {
            if (TypeIn.SelectedIndex == 0)
            {
                if (Value.Text != "." && Value.Text != "" && Expenses.Text != "")
                {
                    String Type = "Expenses";
                    database_OrderSys.insertEx(Type, Expenses.Text, Value.Text, Notes.Text);
                    ExpensesView.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                    if (ViewEnd.SelectedIndex == 1)
                        End_Shift.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                    TotalExpenses.Content = database_OrderSys.Total("Expenses");
                    TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                    if (Convert.ToDouble(TotalNet.Content) > 0)
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(TotalNet.Content) < 0)
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                    Expenses.Text = "";
                    Value.Text = "";
                    Notes.Text = "";
                }
                else
                    MessageBox.Show("Please Fill All Fields");
            }
            else if (TypeIn.SelectedIndex == 1)
            {
                if (Value.Text != "." && Value.Text != "" && Expenses.Text != "")
                {
                    String Type = "Wages";
                    database_OrderSys.insertEx(Type, Expenses.Text, Value.Text, Notes.Text);
                    ExpensesView.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                    if (ViewEnd.SelectedIndex == 1)
                        End_Shift.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                    TotalWages.Content = database_OrderSys.Total("Wages");
                    TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                    if (Convert.ToDouble(TotalNet.Content) > 0)
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                    }
                    else if (Convert.ToDouble(TotalNet.Content) < 0)
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                    }
                    else
                    {
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                    }
                    Expenses.Text = "";
                    Value.Text = "";
                    Notes.Text = "";
                }
                else
                    MessageBox.Show("Please Fill All Fields");
            }
        }
        private void ExpensesIN_Click(object sender, RoutedEventArgs e)
        {
            Expenses_Clk();
            Expenses.Focus();
        }

        private void ExpensesView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && MessageBox.Show("Are You Sure To Delete This Order", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                object i = ExpensesView.SelectedItem;
                string Name = "error", Value = "error", Note = "error";
                if (ExpensesView.SelectedCells.Count != 0)
                {
                    Name = (ExpensesView.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                    Value = (ExpensesView.SelectedCells[1].Column.GetCellContent(i) as TextBlock).Text;
                    Note = (ExpensesView.SelectedCells[2].Column.GetCellContent(i) as TextBlock).Text;
                }

                if (!Name.Equals("error"))
                {
                    if (TypeIn.SelectedIndex == 0)
                    {
                        database_OrderSys.deleteEx("Expenses", Name, Value, Note);
                        ExpensesView.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                        TotalExpenses.Content = database_OrderSys.Total("Expenses");
                        TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                    }
                    else if (TypeIn.SelectedIndex == 1)
                    {
                        database_OrderSys.deleteEx("Wages", Name, Value, Note);
                        ExpensesView.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                        TotalWages.Content = database_OrderSys.Total("Wages");
                        TotalNet.Content = (Convert.ToDouble(TotalIncome.Content) - (Convert.ToDouble(TotalExpenses.Content) + Convert.ToDouble(TotalWages.Content) + Convert.ToDouble(TotalEdara.Content))).ToString();
                    }

                }
                else
                    MessageBox.Show("Please Select Record");
            }
        }

        private void ExpensesView_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd-hh:mmtt";
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle= (Style)(TryFindResource("DataGridColumnHeader"));
        }

        private void EndView_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd-hh:mmtt";
                (e.Column as DataGridTextColumn).Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                
            }
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));
        }

        private void OrderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = database_OrderSys.Last_ID("TempNewOrder");
            if (id != 0)
            {
                String op = database_OrderSys.temp_new_orders_filter("tempNewOrder", "NewOrdersPrint");
                if (op != "null")
                {
                    op = op.Replace("Table", "");
                    op = op.Replace("TakeAway", "تيك اواي");
                    op = op.Replace("Edara", "ادارة");
                    NewOrders neworders = new NewOrders();

                    if (op != "ادارة" && op != "تيك اواي")
                        neworders.Parameters["TableNumber"].Value = ("طاولة" + database_OrderSys.toPersianNumber(op));
                    else
                        neworders.Parameters["TableNumber"].Value = (op);
                    neworders.Parameters["TableNumber"].Visible = false;
                    ReportPrintTool pt = new ReportPrintTool(neworders);
                    pt.ShowPreviewDialog();
                }
            }

            if (OrderType.SelectedIndex == 0)
            {
                TableNum.IsEnabled = true;
                Pay_Print.IsEnabled = true;
                PrintNewOrder.IsEnabled = true;
                ChangeTable.IsEnabled = true;
                CurrentTableNumber.Content = "#";
                NameOrderLabel.Content = "طلبات طاولة رقم";
                NameOrderLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                TableNum.Text = "";
                AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Table0", "Tables").DefaultView;
                Total_Price.Content = "0";
            }
            else if (OrderType.SelectedIndex == 1)
            {
                TableNum.Text = "";
                NameOrderLabel.Content = "طلبات";
                NameOrderLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
                TableNum.IsEnabled = false;
                Pay_Print.IsEnabled = true;
                PrintNewOrder.IsEnabled = true;
                ChangeTable.IsEnabled = false;
                AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("TakeAway", "TakeAway&Edara").DefaultView;
                CurrentTableNumber.Content = "تيك اواي";
                Total_Price.Content = database_OrderSys.Total("TakeAway");
            }
            else if (OrderType.SelectedIndex == 2)
            {
                NameOrderLabel.Content = "طلبات";
                NameOrderLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
                TableNum.Text = "";
                TableNum.IsEnabled = false;
                Pay_Print.IsEnabled = false;
                PrintNewOrder.IsEnabled = true;
                ChangeTable.IsEnabled = false;
                AllOrdersOnTable.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                CurrentTableNumber.Content = "ادارة";
                Total_Price.Content = database_OrderSys.Total("Edara");
            }



        }

        private void ViewEnd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewEnd.SelectedIndex == 0)
            {
                End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
                int lastColumn = End_Shift.Columns.Count - 1;
                if (lastColumn != -1)
                    End_Shift.Columns[lastColumn].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            }
            else if (ViewEnd.SelectedIndex == 1)
            {
                End_Shift.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
            }
            else if (ViewEnd.SelectedIndex == 2)
            {
                End_Shift.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
            }
            else if (ViewEnd.SelectedIndex == 3)
            {
                End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
            }
            EndSearch.Text = "";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ViewEnd.SelectedIndex == 0 && !EndSearch.Text.Contains("'"))
            {
                if (EndSearch.Text == "")
                    End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
                else
                    End_Shift.ItemsSource = database_OrderSys.EndSearch("Shift", EndSearch.Text).DefaultView;

            }
            else if (ViewEnd.SelectedIndex == 1 && !EndSearch.Text.Contains("'"))
            {
                if (EndSearch.Text == "")
                    End_Shift.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                else
                    End_Shift.ItemsSource = database_OrderSys.EndSearch("Expenses", EndSearch.Text).DefaultView;

            }
            else if (ViewEnd.SelectedIndex == 2 && !EndSearch.Text.Contains("'"))
            {
                if (EndSearch.Text == "")
                    End_Shift.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                else
                    End_Shift.ItemsSource = database_OrderSys.EndSearch("Wages", EndSearch.Text).DefaultView;
            }
            else if (ViewEnd.SelectedIndex == 3 && !EndSearch.Text.Contains("'"))
            {
                if (EndSearch.Text == "")
                    End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                else
                    End_Shift.ItemsSource = database_OrderSys.EndSearch("Edara", EndSearch.Text).DefaultView;
            }
            else
                EndSearch.Text = "";

        }

        private void Date_Sort_From_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (TablesType.SelectedItem.ToString().Equals("حركة المخزن"))
            {
                if (Date_Sort_From.Text != "" && Date_Sort_To.Text != "")
                    dg.ItemsSource = database.Sort_LogFile(Convert.ToDateTime(Date_Sort_From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(Date_Sort_To.Text).ToString("yyyy/MM/dd"), Search.Text).DefaultView;
                else if (Date_Sort_From.Text == "" && Date_Sort_To.Text != "")
                    dg.ItemsSource = database.Sort_LogFile("", Convert.ToDateTime(Date_Sort_To.Text).ToString("yyyy/MM/dd"), Search.Text).DefaultView;
                else if (Date_Sort_To.Text == "" && Date_Sort_From.Text != "")
                    dg.ItemsSource = database.Sort_LogFile(Convert.ToDateTime(Date_Sort_From.Text).ToString("yyyy/MM/dd"), "", Search.Text).DefaultView;
                else
                    dg.ItemsSource = database.Sort_LogFile("", "", Search.Text).DefaultView;

            }
        }

        private void Date_Sort_To_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (TablesType.SelectedItem.ToString().Equals("حركة المخزن"))
            {
                if (Date_Sort_From.Text != "" && Date_Sort_To.Text != "")
                    dg.ItemsSource = database.Sort_LogFile(Convert.ToDateTime(Date_Sort_From.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(Date_Sort_To.Text).ToString("yyyy/MM/dd"), Search.Text).DefaultView;
                else if (Date_Sort_From.Text == "" && Date_Sort_To.Text != "")
                    dg.ItemsSource = database.Sort_LogFile("", Convert.ToDateTime(Date_Sort_To.Text).ToString("yyyy/MM/dd"), Search.Text).DefaultView;
                else if (Date_Sort_To.Text == "" && Date_Sort_From.Text != "")
                    dg.ItemsSource = database.Sort_LogFile(Convert.ToDateTime(Date_Sort_From.Text).ToString("yyyy/MM/dd"), "", Search.Text).DefaultView;
                else
                    dg.ItemsSource = database.Sort_LogFile("", "", Search.Text).DefaultView;

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Visibility=Visibility.Hidden;
            if(settings.ShowDialog()==false)
                this.Visibility = Visibility.Visible;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
           String names = database_OrderSys.tables_check();
            String message = "Opened Tables :\n"+names;
            
            if (Shift_Choose.SelectedIndex != -1 && Close_Shift_Date.Text != "")
            {
                if (names!="")
                {
                    if (MessageBox.Show(message + "Are You Sure To Continue ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (MessageBox.Show("Are You Sure To Close This Shift ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {

                            database.Final_Data(TotalIncome.Content.ToString(), TotalExpenses.Content.ToString(), TotalWages.Content.ToString(), TotalEdara.Content.ToString(), TotalNet.Content.ToString(), Shift_Choose.SelectedItem.ToString(), Convert.ToDateTime(Close_Shift_Date.Text));
                            TotalIncome.Content = "0";
                            TotalExpenses.Content = "0";
                            TotalWages.Content = "0";
                            TotalEdara.Content = "0";
                            TotalNet.Content = "0";
                            Close_Shift_Date.Text = "";
                            TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            if (OrderType.SelectedIndex == 2)
                                Total_Price.Content = "0";
                            if (ViewEnd.SelectedIndex == 0)
                            {
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
                                int lastColumn = End_Shift.Columns.Count - 1;
                                if (lastColumn != -1)
                                    End_Shift.Columns[lastColumn].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                            }
                            else if (ViewEnd.SelectedIndex == 1)
                            {
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                            }
                            else if (ViewEnd.SelectedIndex == 2)
                            {
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                            }
                            else if (ViewEnd.SelectedIndex == 3)
                            {
                                End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                            }
                            EndSearch.Text = "";
                            Shift_Choose.SelectedIndex = -1;


                            if (Final_Data_View.SelectedIndex == 0)
                            {
                                Final_View.ItemsSource = database.Final_Data_View("Final_Total").DefaultView;
                            }
                            else if (Final_Data_View.SelectedIndex == 1)
                            {
                                Final_View.ItemsSource = database.Final_Data_View("Final_Items").DefaultView;
                            }
                            else if (Final_Data_View.SelectedIndex == 2)
                            {
                                Final_View.ItemsSource = database.Final_Data_View("Final_Expenses").DefaultView;
                            }

                            if (TypeIn.SelectedIndex == 0)
                            {
                                typeNames.Content = "بيان";
                                ExpensesView.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                            }
                            else if (TypeIn.SelectedIndex == 1)
                            {
                                typeNames.Content = "الاسم";
                                ExpensesView.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                            }

                            Final_Data_View.SelectedIndex = 0;
                            Final_Data_Shift.SelectedIndex = 0;
                            Final_Data_Search.Text = "";
                            From_Final_Data.Text = "";
                            To_Final_Data.Text = "";
                            Final_View.ItemsSource = database.Final_Data_View("Final_Total").DefaultView;
                            Total_Final_Data.Content = database.Total_Data_View("Final_Total").ToString();
                            if (Convert.ToDouble(Total_Final_Data.Content) > 0)
                            {
                                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                            }
                            else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
                            {
                                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                            }
                            else
                            {
                                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                            }

                            

                            database_OrderSys.items_clear();



                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("Are You Sure To Close This Shift ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {

                        database.Final_Data(TotalIncome.Content.ToString(), TotalExpenses.Content.ToString(), TotalWages.Content.ToString(), TotalEdara.Content.ToString(), TotalNet.Content.ToString(), Shift_Choose.SelectedItem.ToString(), Convert.ToDateTime(Close_Shift_Date.Text));
                        TotalIncome.Content = "0";
                        TotalExpenses.Content = "0";
                        TotalWages.Content = "0";
                        TotalEdara.Content = "0";
                        TotalNet.Content = "0";
                        Close_Shift_Date.Text = "";
                        TotalNet.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        safy.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        if (OrderType.SelectedIndex == 2)
                            Total_Price.Content = "0";
                        if (ViewEnd.SelectedIndex == 0)
                        {
                            End_Shift.ItemsSource = database_OrderSys.ViewTable("Shift", "Shift").DefaultView;
                            int lastColumn = End_Shift.Columns.Count - 1;
                            if (lastColumn != -1)
                                End_Shift.Columns[lastColumn].Width = new DataGridLength(2, DataGridLengthUnitType.Star);
                        }
                        else if (ViewEnd.SelectedIndex == 1)
                        {
                            End_Shift.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                        }
                        else if (ViewEnd.SelectedIndex == 2)
                        {
                            End_Shift.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                        }
                        else if (ViewEnd.SelectedIndex == 3)
                        {
                            End_Shift.ItemsSource = database_OrderSys.ViewTable("Edara", "TakeAway&Edara").DefaultView;
                        }
                        EndSearch.Text = "";
                        Shift_Choose.SelectedIndex = -1;


                        if (Final_Data_View.SelectedIndex == 0)
                        {
                            Final_View.ItemsSource = database.Final_Data_View("Final_Total").DefaultView;
                        }
                        else if (Final_Data_View.SelectedIndex == 1)
                        {
                            Final_View.ItemsSource = database.Final_Data_View("Final_Items").DefaultView;
                        }
                        else if (Final_Data_View.SelectedIndex == 2)
                        {
                            Final_View.ItemsSource = database.Final_Data_View("Final_Expenses").DefaultView;
                        }

                        if (TypeIn.SelectedIndex == 0)
                        {
                            typeNames.Content = "بيان";
                            ExpensesView.ItemsSource = database_OrderSys.ViewTable("Expenses", "Shift").DefaultView;
                        }
                        else if (TypeIn.SelectedIndex == 1)
                        {
                            typeNames.Content = "الاسم";
                            ExpensesView.ItemsSource = database_OrderSys.ViewTable("Wages", "Shift").DefaultView;
                        }

                        Final_Data_View.SelectedIndex = 0;
                        Final_Data_Shift.SelectedIndex = 0;
                        Final_Data_Search.Text = "";
                        From_Final_Data.Text = "";
                        To_Final_Data.Text = "";
                        Final_View.ItemsSource = database.Final_Data_View("Final_Total").DefaultView;
                        Total_Final_Data.Content = database.Total_Data_View("Final_Total").ToString();
                        if (Convert.ToDouble(Total_Final_Data.Content) > 0)
                        {
                            Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                        }
                        else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
                        {
                            Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                        }
                        else
                        {
                            Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                        }

                        

                        database_OrderSys.items_clear();



                    }
                }
            }
            else
                MessageBox.Show("Please Check Shift And Date");
        }

        private void Final_Data_View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Final_Data_View.SelectedIndex == 0)
            {
                Final_View.ItemsSource = database.Final_Data_View("Final_Total").DefaultView;
                Total_Final_Data.Content = database.Total_Data_View("Final_Total").ToString();

                Final_Data_Search.Text = "";
                Final_Data_Search.IsEnabled = false;
                Final_Data_Search.Text = "";
                To_Final_Data.Text = "";
                Final_Data_Shift.SelectedIndex = 0;
            }
            else if (Final_Data_View.SelectedIndex == 1)
            {
                Final_View.ItemsSource = database.Final_Data_View("Final_Items").DefaultView;
                Total_Final_Data.Content = database.Total_Data_View("Final_Items").ToString();

                Final_Data_Search.Text = "";
                Final_Data_Search.IsEnabled = true;
                Final_Data_Search.Text = "";
                To_Final_Data.Text = "";
                Final_Data_Shift.SelectedIndex = 0;
            }
            else if (Final_Data_View.SelectedIndex == 2)
            {
                Final_View.ItemsSource = database.Final_Data_View("Final_Expenses").DefaultView;
                Total_Final_Data.Content = database.Total_Data_View("Final_Expenses").ToString();

                Final_Data_Search.Text = "";
                Final_Data_Search.IsEnabled = true;
                Final_Data_Search.Text = "";
                To_Final_Data.Text = "";
                Final_Data_Shift.SelectedIndex = 0;
            }
            if (Convert.ToDouble(Total_Final_Data.Content) > 0)
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
            }
            else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
            }
            else
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
            }
        }

        private void Final_View_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "yyyy/MM/dd";
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));
        }

        private void Final_Data_Shift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Final_Data_Search.Text == "" && (From_Final_Data.Text == "" || To_Final_Data.Text == "") || Final_Data_Search.Text.Contains("'"))
            {
                Final_View.ItemsSource = database.Search_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                Total_Final_Data.Content = database.Total_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).ToString();
            }
            else if (From_Final_Data.Text != "" && To_Final_Data.Text != "")
            {
                Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).ToString();
            }
            else
            {
                Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).ToString();
            }
            if (Convert.ToDouble(Total_Final_Data.Content) > 0)
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
            }
            else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
            }
            else
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
            }
        }

        private void Final_Data_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Final_Data_Search.Text == "" && (From_Final_Data.Text == "" || To_Final_Data.Text == "") || Final_Data_Search.Text.Contains("'"))
            {
                Final_View.ItemsSource = database.Search_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                Total_Final_Data.Content = database.Total_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).ToString();
            }
            else if (From_Final_Data.Text != "" && To_Final_Data.Text != "")
            {
                Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).ToString();
            }
            else
            {
                Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).ToString();
            }
            if (Convert.ToDouble(Total_Final_Data.Content) > 0)
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
            }
            else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
            }
            else
            {
                Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
            }
        }

        private void From_Final_Data_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (From_Final_Data.Text != "" && To_Final_Data.Text != "")
            {
                if (Final_Data_Search.Text == "" && (From_Final_Data.Text == "" || To_Final_Data.Text == "") || Final_Data_Search.Text.Contains("'"))
                {
                    Final_View.ItemsSource = database.Search_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                    Total_Final_Data.Content = database.Total_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).ToString();
                }
                else if (From_Final_Data.Text != "" && To_Final_Data.Text != "")
                {
                    Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                    Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).ToString();
                }
                else
                {
                    Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                    Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).ToString();
                }
                if (Convert.ToDouble(Total_Final_Data.Content) > 0)
                {
                    Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                }
                else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
                {
                    Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                }
                else
                {
                    Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                }
            }
        }

        private void To_Final_Data_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (From_Final_Data.Text != "" && To_Final_Data.Text != "")
            {
                if (Final_Data_Search.Text == "" && (From_Final_Data.Text == "" || To_Final_Data.Text == "") || Final_Data_Search.Text.Contains("'"))
                {
                    Final_View.ItemsSource = database.Search_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                    Total_Final_Data.Content = database.Total_Final_Shift(Final_Data_View.SelectedItem.ToString(), Final_Data_Shift.SelectedItem.ToString()).ToString();
                }
                else if (From_Final_Data.Text != "" && To_Final_Data.Text != "")
                {
                    Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                    Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, Convert.ToDateTime(From_Final_Data.Text).ToString("yyyy/MM/dd"), Convert.ToDateTime(To_Final_Data.Text).ToString("yyyy/MM/dd"), Final_Data_Shift.SelectedItem.ToString()).ToString();
                }
                else
                {
                    Final_View.ItemsSource = database.Search_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).DefaultView;
                    Total_Final_Data.Content = database.Total_Final_Data(Final_Data_View.SelectedItem.ToString(), Final_Data_Search.Text, "", "", Final_Data_Shift.SelectedItem.ToString()).ToString();
                }
                if (Convert.ToDouble(Total_Final_Data.Content) > 0)
                {
                    Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF0AA041"));
                }
                else if (Convert.ToDouble(Total_Final_Data.Content) < 0)
                {
                    Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF950000"));
                }
                else
                {
                    Total_Final_Data.Foreground = ColorConverter.GetColorFromHexa(("#FF000000"));
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DataTable table = new DataTable();
            table = ((DataView)Final_View.ItemsSource).ToTable();

            if (Final_Data_View.SelectedIndex == 0)
            {
                database.insert_Final_Print(table, 1);
                Final_Total report = new Final_Total();
                report.Parameters["parameter1"].Value = "تفاصيل شيفتات";
                report.Parameters["parameter1"].Visible = false;

                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreviewDialog();
            }
            else if (Final_Data_View.SelectedIndex == 1)
            {
                database.insert_Final_Print(table, 2);
                Total_Items report = new Total_Items();
                report.Parameters["parameter1"].Value = "تفاصيل ايرادات";
                report.Parameters["parameter1"].Visible = false;

                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreviewDialog();
            }
            else
            {
                database.insert_Final_Print(table, 3);
                Total_Expenses report = new Total_Expenses();
                report.Parameters["parameter1"].Value = "تفاصيل مصروفات";
                report.Parameters["parameter1"].Visible = false;

                ReportPrintTool pt = new ReportPrintTool(report);
                pt.ShowPreviewDialog();
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            String login = database.get_admin();
            while (true) {
                Password password = new Password("ادخل رمز الحماية ", "");
                if (password.ShowDialog() == true)
                {
                    if (password.Answer == login && Log_in.Content.ToString() == "تسجيل الدخول")
                    {
                        _1.Visibility = Visibility.Visible;                        
                        _3.Visibility = Visibility.Visible;
                        _4.Visibility = Visibility.Visible;                        
                        _6.Visibility = Visibility.Visible;
                        setting.IsEnabled = true;
                        Log_in.Content = "تسجيل الخروج";
                        database.state_write(1);
                        break;
                    }
                    else if (password.Answer == login && Log_in.Content.ToString() == "تسجيل الخروج")
                    {
                        _1.Visibility = Visibility.Hidden;                        
                        //_3.Visibility = Visibility.Hidden;
                        _4.Visibility = Visibility.Hidden;
                        //_5.Visibility = Visibility.Hidden;
                        _6.Visibility = Visibility.Hidden;
                        setting.IsEnabled = false;
                        tabs_.SelectedIndex = 2;
                        Log_in.Content = "تسجيل الدخول";
                        database.state_write(0);
                        break;
                    }
                    else
                    {
                        MessageBox.Show("Password Not Correct");
                    }
                }
                else
                    break;
            }

        }
                
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        

        

        

        private void TableNum_PreviewKeyUp(object sender, KeyEventArgs e)
        {   
            table_num();
        }

        

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            database.state_write(0);
            Environment.Exit(0);
        }

        private void Mowarden_table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(mowarden_table.SelectedIndex == 0)
            {
                mowarden_view.ItemsSource = database.Mowarden_View(1).DefaultView;
            }
            else
            {
                mowarden_view.ItemsSource = database.Mowarden_View(0).DefaultView;
            }
        }

        private void Value_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Expenses_Clk();
                Expenses.Focus();

            }
            
        }

        private void Notes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Expenses_Clk();
                Expenses.Focus();

            }
        }

        private void Expenses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Expenses_Clk();
                Expenses.Focus();

            }
        }

        private void Mowarden_view_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";

            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));
        }

        private void AllOrdersOnTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));
        }

        

        private void _get3_Checked(object sender, RoutedEventArgs e)
        {
            if (_get4 != null)
                _get4.IsChecked = false;
        }

        private void _get4_Checked(object sender, RoutedEventArgs e)
        {
            if (_get3 != null)
                _get3.IsChecked = false;
        }
        private void mowarden_input()
        {
            if (mowarden.SelectedItem != null)
                if (mowarden.SelectedIndex != -1 && mowarden_value.Text != "" && mowarden_value.Text != ".")
                {
                    if (_get3.IsChecked == true)
                    {
                        DateTime d = DateTime.Today;
                        database.INOUTMowardeen(mowarden.SelectedItem.ToString(), mowarden_value.Text, d.ToString("yyyy/MM/dd"));
                    }

                    if (_get4.IsChecked == true)
                    {
                        DateTime d = DateTime.Today;
                        double m = Convert.ToDouble(mowarden_value.Text) * -1;
                        database.INOUTMowardeen(mowarden.SelectedItem.ToString(), m.ToString(), d.ToString("yyyy/MM/dd"));
                    }
                    if (mowarden_table.SelectedIndex == 0)
                    {
                        mowarden_view.ItemsSource = database.Mowarden_View(1).DefaultView;
                    }
                    else
                    {
                        mowarden_view.ItemsSource = database.Mowarden_View(0).DefaultView;
                    }
                    mowarden_value.Clear();

                }
                else
                    MessageBox.Show("Please Fill all Fields");
            else
                MessageBox.Show("Select Name First");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            mowarden_input();
        }

        private void Total_Items_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void TableNum_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        

        private void Tabcontrol_GotFocus(object sender, RoutedEventArgs e)
        {
            Total_Items.Focus();
        }

        private void mowarden_value_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                mowarden_input();

            }
        }

        private void Microtic_login(object sender, RoutedEventArgs e)
        {
            if(internet_carts.test_login())
            {

            }
            

        }

        private void Internet_Carts(object sender, RoutedEventArgs e)
        {
            if (internet_carts.test_login()==true)
            {
                internet_cart_menu mm = new internet_cart_menu();
                mm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mm.ShowDialog();
            }
        }

        private void Get_Bill_Click(object sender, RoutedEventArgs e)
        {
            List<String> bill = new List<string>();
            InputDialog ask = new InputDialog("ادخل رقم الشيك","");

            if (ask.ShowDialog() == true)
            {
                try
                {
                    bill = database_OrderSys.get_bill(Convert.ToInt32(ask.Answer));
                    if (bill != null)
                    {
                        Get_Bills bills = new Get_Bills();
                        bills.Parameters["TableNum"].Value = (bill.ElementAt(0));
                        bills.Parameters["Total"].Value = (bill.ElementAt(1));
                        bills.Parameters["Date"].Value = (Convert.ToDateTime(bill.ElementAt(2)));
                        bills.Parameters["ID"].Value = ask.Answer;
                        bills.Parameters["TableNum"].Visible = false;
                        bills.Parameters["Total"].Visible = false;
                        bills.Parameters["ID"].Visible = false;
                        bills.Parameters["Date"].Visible = false;
                        ReportPrintTool pt = new ReportPrintTool(bills);
                        pt.ShowPreviewDialog();
                        database_OrderSys.deletePaymentTable();                        
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("رقم شيك غير صالح يرجى اعادة المحاولة");
                }
            }
        }

        private void close_month_Click(object sender, RoutedEventArgs e)
        {
            close_month close_Month = new close_month();
            close_Month.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           // close_Month.ShowDialog();
            if (close_Month.ShowDialog() == false)
            {
                Final_Data_View.SelectedIndex = 0;                                       
                Final_Data_Shift.SelectedIndex = 0;
                Final_Data_Search.Text = "";
                From_Final_Data.Text = "";
                To_Final_Data.Text = "";
            }
        }
    }
}
