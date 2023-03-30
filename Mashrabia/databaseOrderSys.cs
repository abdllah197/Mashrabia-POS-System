using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mashrabia
{

    public class databaseOrderSys
    {
        private OleDbConnection con = new OleDbConnection();
        private string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Category.mdb";
        private string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Tables.mdb";
        private string path3 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/PrintOrders.mdb";
        private string path4 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Shift.mdb";
        private string path5 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/TakeAway&Edara.mdb";
        public List<String> Name_Of_Tables(String require)
        {
            try
            {
                if (require == "Category")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                else if (require == "Tables")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                List<String> N = new List<String>();
                DataTable dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                foreach (DataRow row in dt.Rows)
                {
                    N.Add(row[2].ToString());
                }

                con.Close();
                return N;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;

            }

        }

        public List<String> Category_Items(String table, String select)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;

                c.CommandText = "select [" + select + "] from [" + table + "]";
                OleDbDataReader reader = c.ExecuteReader();
                List<String> Result = new List<String>();
                while (reader.Read())
                {
                    Result.Add(reader.GetValue(0).ToString());
                }

                con.Close();
                return Result;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;

            }
        }

        public DataTable ViewTable(String table, String file)
        {

            try
            {
                OleDbCommand c = new OleDbCommand();
                if (file == "Category")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                else if (file == "Tables")
                {
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
                    c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي] from " + table;
                }
                else if (file == "Shift")
                {
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                    if (table == "Expenses")
                        c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[التاريخ] from " + table;
                    else if (table == "Wages")
                        c.CommandText = "select [يوميات],[القيمة],[ملاحظات],[التاريخ] from " + table;
                    else if (table == "Shift")
                        c.CommandText = "select [كود],[طاولات],[الاسم],[العدد],[سعر الوحدة],[الاجمالي],[التاريخ] from " + table;
                    else if (table == "Items")
                        c.CommandText = "select [السعر],[العدد],[الصنف] from " + table;
                }
                else if (file == "TakeAway&Edara")
                {
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                    if (table == "TakeAway")
                        c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي] from " + table;
                    else if (table == "Edara")
                        c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[ملاحظات] from " + table;

                }
                con.Open();

                c.Connection = con;

                OleDbDataAdapter ad = new OleDbDataAdapter(c);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;
            }
        }

        public Double Total(String table)
        {
            if (table == "TakeAway" || table == "Edara")
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
            else if (table == "Shift" || table == "Wages" || table == "Expenses")
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            else
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";

            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (table == "Shift")
                    c.CommandText = "select [الاجمالي] from " + table;
                else if (table == "Wages" || table == "Expenses")
                    c.CommandText = "select [القيمة] from " + table;
                else
                    c.CommandText = "select [الاجمالي] from " + table;
                OleDbDataReader reader = c.ExecuteReader();
                Double Total = 0;
                while (reader.Read())
                {
                    Total = Total + Convert.ToDouble(reader.GetValue(0).ToString());
                }

                con.Close();
                return Total;

            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return 0;
            }

        }

        public void delete_order(String table, String item, String quntity)
        {

            try
            {
                if (table == "TakeAway" || table == "Edara")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                else
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";

                con.Open();
                String id = "";
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [ID],[الاسم],[العدد] from " + table;
                OleDbDataReader reader = c.ExecuteReader();
                while (reader.Read())
                {
                    if (item == reader.GetValue(1).ToString() && quntity == reader.GetValue(2).ToString())
                    {
                        id = reader.GetValue(0).ToString();

                    }
                }
                con.Close();
                con.Open();
                c.Connection = con;
                if (id != "")
                {
                    c.CommandText = "delete from " + table + " where [ID]=" + id;
                    c.ExecuteNonQuery();
                }
                con.Close();
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path3 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.Connection = con;
                id = "";
                c.CommandText = "select [ID],[Item],[Quantity] from TempNewOrder";
                OleDbDataReader reader2 = c.ExecuteReader();
                while (reader2.Read())
                {
                    if (item == reader2.GetValue(1).ToString() && quntity == reader2.GetValue(2).ToString())
                    {
                        id = reader2.GetValue(0).ToString();
                        break;
                    }
                }
                con.Close();
                con.Open();
                c.Connection = con;
                if (id != "")
                {
                    c.CommandText = "delete from TempNewOrder where [ID]=" + id;
                    c.ExecuteNonQuery();
                }
                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
            }
        }

        public int change_table_num(String OldTable, String NewTable)
        {
            try
            {
                int done = 0;
                List<String> Valid = new List<string>();
                Valid = Name_Of_Tables("Tables");
                if (Valid.Contains(NewTable))
                {
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    OleDbCommand c = new OleDbCommand();
                    c.Connection = con;
                    c.CommandText = "select * from " + NewTable;
                    OleDbDataReader reader = c.ExecuteReader();
                    int Counter = 0;
                    while (reader.Read())
                    {
                        Counter++;
                    }

                    con.Close();
                    con.Open();
                    c.Connection = con;
                    char[] a = OldTable.ToCharArray();
                    char[] b = NewTable.ToCharArray();
                    if (Counter == 0 && MessageBox.Show("هل انت منتأكد من نقل الطاولة " + a[5] + " الى الطاولة الفارغة " + b[5] + " ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        c.CommandText = "insert into " + NewTable + " select * from " + OldTable;
                        c.ExecuteNonQuery();
                        c.CommandText = "delete from " + OldTable;
                        c.ExecuteNonQuery();
                        MessageBox.Show("Done");
                        done = 1;
                    }
                    else if (Counter > 0 && MessageBox.Show("هل تريد دمج الطاولة " + a[5] + " و " + b[5] + " ؟", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        c.CommandText = "insert into " + NewTable + " select [الاسم] , [العدد] , [سعر الوحدة] , [الاجمالي] from " + OldTable;
                        c.ExecuteNonQuery();
                        c.CommandText = "delete from " + OldTable;
                        c.ExecuteNonQuery();
                        MessageBox.Show("Done");
                        done = 1;
                    }

                    con.Close();
                }
                else
                    MessageBox.Show("New Table Number Not Found");
                return done;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return 0;
            }
        }

        public void Insert_Item(String Category, String Item_Name, int total_number, String Table_Name)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [السعر] from [" + Category + "] where [الاسم] ='" + Item_Name + "'";
                OleDbDataReader reader = c.ExecuteReader();
                Double Price = 0, Total = 0;
                while (reader.Read())
                {
                    Price = Convert.ToDouble(reader.GetValue(0).ToString());
                }
                Total = Price * total_number;
                con.Close();
                if (Table_Name == "TakeAway")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                else
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.Connection = con;

                c.CommandText = "INSERT INTO " + Table_Name + " ([الاسم],[العدد],[سعر الوحدة],[الاجمالي]) VALUES ('" + Item_Name + "'," + total_number + "," + Price + "," + Total + ")";
                c.ExecuteNonQuery();
                con.Close();
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path3 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.CommandText = "INSERT INTO TempNewOrder ([Item],[Quantity],[Table Number]) VALUES ('" + Item_Name + "'," + total_number + ",'" + Table_Name + "')";
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
            }

        }

        public void Insert_Edara(String Category, String Item_Name, int total_number, String Table_Name, String Note)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [السعر] from [" + Category + "] where [الاسم] ='" + Item_Name + "'";
                OleDbDataReader reader = c.ExecuteReader();
                Double Price = 0, Total = 0;
                while (reader.Read())
                {
                    Price = Convert.ToDouble(reader.GetValue(0).ToString());
                }
                Total = Price * total_number;
                con.Close();

                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.Connection = con;

                c.CommandText = "INSERT INTO " + Table_Name + " ([الاسم],[العدد],[سعر الوحدة],[الاجمالي],[ملاحظات]) VALUES ('" + Item_Name + "'," + total_number + "," + Price + "," + Total + ",'" + Note + "')";
                c.ExecuteNonQuery();
                con.Close();
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path3 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.CommandText = "INSERT INTO TempNewOrder ([Item],[Quantity],[Table Number]) VALUES ('" + Item_Name + "'," + total_number + ",'" + Table_Name + "')";
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
            }

        }

        public String temp_new_orders_filter(String table, String Filtered_table)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path3 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "delete from [" + Filtered_table + "]";
            c.ExecuteNonQuery();
            c.CommandText = "select [Item] from " + table;
            OleDbDataReader reader = c.ExecuteReader();
            int c1 = 0;
            String TableNumber = "null";
            List<String> Names = new List<string>();
            while (reader.Read())
            {
                c1++;
                Names.Add(reader.GetValue(0).ToString());
            }
            if (c1 != 0)
            {
                con.Close();
                con.Open();
                c.Connection = con;
                List<String> NAMES = Names.Distinct().ToList();
                for (int i = 0; i < NAMES.Count(); i++)
                {
                    Double Quantity = 0;
                    c.CommandText = "select [Quantity],[Table Number] from [" + table + "] where [Item]='" + NAMES.ElementAt(i) + "'";
                    OleDbDataReader reader1 = c.ExecuteReader();
                    while (reader1.Read())
                    {
                        Quantity = Quantity + Convert.ToInt16(reader1.GetValue(0).ToString());
                        TableNumber = reader1.GetValue(1).ToString();
                    }

                    con.Close();
                    con.Open();
                    c.Connection = con;
                    c.CommandText = "INSERT INTO " + Filtered_table + " ([Item],[Quantity]) VALUES ('" + NAMES.ElementAt(i) + "','" + toPersianNumber(Quantity.ToString()) + "')";
                    c.ExecuteNonQuery();
                    c.CommandText = "delete from " + table + " where [Item]='" + NAMES.ElementAt(i) + "'";
                    c.ExecuteNonQuery();

                }
            }
            else
                MessageBox.Show("No Orders To Print");

            con.Close();
            return TableNumber;
        }

        public int Pay_filter(int ID, String table, String Filtered_table)
        {
            String order_id = Properties.Settings.Default.order_id ;
            if (table == "TakeAway")
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
            else
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
            String Temp = table;
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [الاسم] from " + Temp;
            OleDbDataReader reader = c.ExecuteReader();
            int c1 = 0;
            List<String> Names = new List<string>();
            while (reader.Read())
            {
                c1++;
                Names.Add(reader.GetValue(0).ToString());
            }
            if (c1 != 0)
            {
                con.Close();
                con.Open();
                c.Connection = con;
                List<String> NAMES = Names.Distinct().ToList();
                int i;
                for (i = 0; i < NAMES.Count(); i++)
                {
                    Double Quantity = 0, T = 0;
                    String P = "0";
                    con.Close();
                    if (Temp == "TakeAway")
                        con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                    else
                        con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    c.Connection = con;
                    c.CommandText = "select [العدد],[سعر الوحدة],[الاجمالي] from " + Temp + " where [الاسم]='" + NAMES.ElementAt(i) + "'";

                    OleDbDataReader reader1 = c.ExecuteReader();
                    while (reader1.Read())
                    {
                        Quantity = Quantity + Convert.ToInt16(reader1.GetValue(0).ToString());
                        P = reader1.GetValue(1).ToString();
                        T = T + Convert.ToDouble(reader1.GetValue(2).ToString());
                    }

                    con.Close();
                    con.Open();
                    c.Connection = con;
                    c.CommandText = "delete from " + Temp + " where [الاسم]='" + NAMES.ElementAt(i) + "'";
                    c.ExecuteNonQuery();
                    con.Close();

                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    c.Connection = con;
                    DateTime Date = DateTime.Now;
                    table = table.Replace("Table", "طاولة");
                    table = table.Replace("TakeAway", "تيك اواي");
                    table = toPersianNumber(table);
                    c.CommandText = "INSERT INTO " + Filtered_table + " ([الاسم],[العدد],[سعر الوحدة],[الاجمالي]) VALUES ('" + NAMES.ElementAt(i) + "','" + toPersianNumber(Quantity.ToString()) + "','" + toPersianNumber(P) + "','" + toPersianNumber(T.ToString()) + "')";
                    c.ExecuteNonQuery();
                    c.CommandText = "INSERT INTO Shift ([كود],[طاولات],[الاسم],[العدد],[سعر الوحدة],[الاجمالي],[التاريخ]) VALUES ('" + order_id + "','" + table + "','" + NAMES.ElementAt(i) + "'," + Quantity + "," + P + "," + T + ",#" + Date.ToString("yyyy/MM/dd hh:mmtt") + "#)";
                    c.ExecuteNonQuery();
                    c.CommandText = "INSERT INTO Bills ([كود],[طاولات],[الاسم],[العدد],[سعر الوحدة],[الاجمالي],[التاريخ]) VALUES (" + ID + ",'" + table + "','" + NAMES.ElementAt(i) + "'," + Quantity + "," + P + "," + T + ",#" + Date.ToString("yyyy/MM/dd hh:mmtt") + "#)";
                    c.ExecuteNonQuery();

                }
                con.Close();
                String temp = Properties.Settings.Default.order_id;
                Double t1 = Double.Parse(temp);
                Double t2 = temp.Length;
                Double t3 = temp.Length;
                t1++;
                t2=Math.Floor(Math.Log10(t1) + 1);
                t2 = t3 - t2;
                temp = t1.ToString();
                for(int g=0;g<t2;g++)
                {
                    temp = "0" + temp;
                }
                Properties.Settings.Default.order_id = temp;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                return i;
            }
            else
            {
                MessageBox.Show("No Orders To Print");
                con.Close();
                return 0;
            }
        }
        public List<String> get_bill(int ID)
        {

            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [الاسم],[الاجمالي],[طاولات],[التاريخ] from Bills where [كود] = " + ID;
            OleDbDataReader reader = c.ExecuteReader();
            int c1 = 0;
            Double total = 0;
            String table = "", date = "";
            List<String> Names = new List<String>();
            List<String> Done = new List<String>();
            while (reader.Read())
            {
                c1++;
                Names.Add(reader.GetValue(0).ToString());
                total += reader.GetDouble(1);
                table = reader.GetString(2);
                date = reader.GetValue(3).ToString();
            }
            Done.Add(table);
            Done.Add(toPersianNumber(total.ToString()));
            Done.Add(date);

            if (c1 != 0)
            {
                con.Close();
                con.Open();
                c.Connection = con;
                List<String> NAMES = Names.Distinct().ToList();
                int i;
                for (i = 0; i < NAMES.Count(); i++)
                {
                    Double Quantity = 0, T = 0;
                    String P = "0";
                    con.Close();

                    con.Open();
                    c.Connection = con;
                    c.CommandText = "select [العدد],[سعر الوحدة],[الاجمالي] from Bills where ([الاسم]='" + NAMES.ElementAt(i) + "' AND [كود] =" + ID + " )";

                    OleDbDataReader reader1 = c.ExecuteReader();
                    while (reader1.Read())
                    {
                        Quantity = Convert.ToInt16(reader1.GetValue(0).ToString());
                        P = reader1.GetValue(1).ToString();
                        T = Convert.ToDouble(reader1.GetValue(2).ToString());
                    }
                    con.Close();


                    con.Open();
                    c.Connection = con;

                    c.CommandText = "INSERT INTO Payment ([الاسم],[العدد],[سعر الوحدة],[الاجمالي]) VALUES ('" + NAMES.ElementAt(i) + "','" + toPersianNumber(Quantity.ToString()) + "','" + toPersianNumber(P) + "','" + toPersianNumber(T.ToString()) + "')";
                    c.ExecuteNonQuery();

                }
                con.Close();
                return Done;
            }
            else
            {
                MessageBox.Show("لا يوجد شيك او رقم الشيك خاطئ");
                con.Close();
                return null;
            }
        }
        public int PrintToEnableChangeTableNumber()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path3 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select * from TempNewOrder";
            OleDbDataReader reader = c.ExecuteReader();
            int Counter = 0;
            while (reader.Read())
            {
                Counter++;
            }
            con.Close();
            return Counter;
        }

        public string toPersianNumber(string input)
        {
            string[] persian = new string[10] { "۰", "۱", "۲", "۳", "٤", "۵", "٦", "۷", "۸", "۹" };
            for (int j = 0; j < persian.Length; j++)
            {
                input = input.Replace(j.ToString(), persian[j]);
            }
            return input;
        }

        public int Last_ID(String table)
        {
            if (table == "TempNewOrder")
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path3 + ";Jet OLEDB:Database Password=3bood197";
            else if (table == "Shift" || table == "Wages" || table == "Expenses")
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            else if (table == "Edara")
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                String id = "0", temp = "0";
                c.CommandText = "select [ID] from " + table;
                if (table == "Shift")
                    c.CommandText = "select [كود] from " + table;
                OleDbDataReader reader = c.ExecuteReader();
                while (reader.Read())
                {

                    temp = reader.GetValue(0).ToString();
                    if (Convert.ToInt32(id) < Convert.ToInt32(temp))
                        id = temp;
                }
                con.Close();
                return Convert.ToInt32(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return 0;
            }

        }

        public int Last_ID_S(String table)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                String id = "100000", temp = "100000";
                c.CommandText = "select [كود] from " + table;
                OleDbDataReader reader = c.ExecuteReader();
                while (reader.Read())
                {

                    temp = reader.GetValue(0).ToString();
                    if (Convert.ToInt32(id) < Convert.ToInt32(temp))
                        id = temp;
                }
                con.Close();
                return Convert.ToInt32(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return 0;
            }

        }

        public int Payment_Print(String Table_Number)
        {
            int id = Last_ID_S("Bills") + 1;
            int done = Pay_filter(id, Table_Number, "Payment");
            return done;
        }

        public void deletePaymentTable()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "delete from Payment";
            c.ExecuteNonQuery();
            con.Close();
        }

        public void deleteBillTable()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "delete from Bills";
            c.ExecuteNonQuery();
            con.Close();
        }

        public void insertEx(String Type, String Name, String value, String Note)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            DateTime Date = DateTime.Now;
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            if (Type == "Expenses")
                c.CommandText = "INSERT INTO " + Type + " ([المصروفات],[القيمة],[ملاحظات],[التاريخ]) VALUES ('" + Name + "'," + value + ",'" + Note + "',#" + Date.ToString("yyyy/MM/dd hh:mmtt") + "#)";
            else if (Type == "Wages")
                c.CommandText = "INSERT INTO " + Type + " ([يوميات],[القيمة],[ملاحظات],[التاريخ]) VALUES ('" + Name + "'," + value + ",'" + Note + "',#" + Date.ToString("yyyy/MM/dd hh:mmtt") + "#)";

            c.ExecuteNonQuery();
            con.Close();
        }

        public void deleteEx(String Type, String Name, String value, String Note)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (Type == "Expenses")
                    c.CommandText = "delete from " + Type + " where [المصروفات]='" + Name + "' and [القيمة]=" + value + " and [ملاحظات]='" + Note + "'";
                else if (Type == "Wages")
                    c.CommandText = "delete from " + Type + " where [يوميات]='" + Name + "' and [القيمة]=" + value + " and [ملاحظات]='" + Note + "'";

                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
            }
        }

        public DataTable EndSearch(String table, String text)
        {
            try
            {
                if (table == "Shift" || table == "Wages" || table == "Expenses")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                else if (table == "Edara")
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (table == "Shift")
                {
                    int n = 0;
                    Boolean isNumeric = int.TryParse(text, out n);
                    String n1 = text;
                    if (isNumeric)
                        c.CommandText = "select [كود],[طاولات],[الاسم],[العدد],[سعر الوحدة],[الاجمالي],[التاريخ] from " + table + " where [كود] ='" + n1+"'";
                    else
                        c.CommandText = "select [كود],[طاولات],[الاسم],[العدد],[سعر الوحدة],[الاجمالي],[التاريخ] from " + table + " where [طاولات] like '%" + toPersianNumber(text) + "%' or [الاسم] like '%" + text + "%'";
                }
                else if (table == "Wages")
                    c.CommandText = "select [يوميات],[القيمة],[ملاحظات],[التاريخ] from " + table + " where [يوميات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%'";
                else if (table == "Expenses")
                    c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[التاريخ] from " + table + " where [المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%'";
                else if (table == "Edara")
                    c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[ملاحظات] from " + table + " where [الاسم] like '%" + text + "%' or [ملاحظات] like '%" + text + "%'";
                OleDbDataAdapter ad = new OleDbDataAdapter(c);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;
            }

        }

        public void Total_Items(String table, String name)
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            c.CommandText = "select [السعر] from " + table + " where [الاسم] ='" + name + "'";
            OleDbDataReader reader = c.ExecuteReader();
            Double Price = 0;
            while (reader.Read())
            {
                Price = Convert.ToDouble(reader.GetValue(0).ToString());
            }
            con.Close();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            c.CommandText = "INSERT INTO Items ([الصنف],[العدد],[السعر]) VALUES ('" + name + "',0," + Price + ")";
            c.ExecuteNonQuery();
            c.CommandText = "INSERT INTO Items2 ([الصنف],[العدد],[السعر]) VALUES ('" + name + "',0," + Price + ")";
            c.ExecuteNonQuery();
            con.Close();
        }

        public void delete_item_Total(String name, int op)
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            if (op == 1)
                c.CommandText = "delete from Items where [الصنف]='" + name + "'";
            else
                c.CommandText = "delete from Items2 where [الصنف]='" + name + "'";
            c.ExecuteNonQuery();
            con.Close();
        }

        public List<String> items_Names(int op)
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            if (op == 1)
                c.CommandText = "select [الصنف] from Items";
            else
                c.CommandText = "select [الصنف] from Items2";
            OleDbDataReader reader = c.ExecuteReader();
            List<String> names = new List<string>();
            while (reader.Read())
            {
                names.Add(reader.GetValue(0).ToString());
            }
            con.Close();
            return names;
        }

        public String items_number(String name, int op)
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            if (op == 1)
                c.CommandText = "select [العدد] from Items where [الصنف] ='" + name + "'";
            else
                c.CommandText = "select [العدد] from Items2 where [الصنف] ='" + name + "'";
            OleDbDataReader reader = c.ExecuteReader();
            String number = "0";
            while (reader.Read())
            {
                number = reader.GetValue(0).ToString();
            }
            con.Close();
            return number;
        }

        public Double items_Total_Price(String name, int op)
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            if (op == 1)
                c.CommandText = "select [العدد],[السعر] from Items where [الصنف] ='" + name + "'";
            else
                c.CommandText = "select [العدد],[السعر] from Items2 where [الصنف] ='" + name + "'";
            OleDbDataReader reader = c.ExecuteReader();
            int number = 0;
            Double Total = 0, Price = 0;
            while (reader.Read())
            {
                number = Convert.ToInt16(reader.GetValue(0).ToString());
                Price = Convert.ToDouble(reader.GetValue(1).ToString());
            }
            Total = number * Price;
            con.Close();
            return Total;
        }

        public void items_number_update(int Number, String name, int op)
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            if (op == 1)
                c.CommandText = "select [العدد] from Items where [الصنف] ='" + name + "'";
            else
                c.CommandText = "select [العدد] from Items2 where [الصنف] ='" + name + "'";
            OleDbDataReader reader = c.ExecuteReader();
            int number = 0;
            while (reader.Read())
            {
                number = Convert.ToInt16(reader.GetValue(0).ToString());
            }
            con.Close();
            number = number + Number;
            con.Open();
            c.Connection = con;
            if (op == 1)
                c.CommandText = "update Items set [العدد]=" + number + " where [الصنف]='" + name + "'";
            else
                c.CommandText = "update Items2 set [العدد]=" + number + " where [الصنف]='" + name + "'";
            c.ExecuteNonQuery();
            con.Close();

        }

        public void items_clear()
        {
            OleDbCommand c = new OleDbCommand();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            c.Connection = con;
            List<String> names = new List<String>();

            c.CommandText = "select [الصنف] from Items";
            OleDbDataReader reader1 = c.ExecuteReader();

            while (reader1.Read())
            {
                names.Add(reader1.GetValue(0).ToString());
            }
            con.Close();




            for (int i = 0; i < names.Count; i++)
            {
                con.Open();
                c.Connection = con;

                c.CommandText = "select [العدد] from Items2 where [الصنف]='" + names.ElementAt(i) + "'";
                OleDbDataReader reader = c.ExecuteReader();
                int number = 0;
                while (reader.Read())
                {
                    number = Convert.ToInt16(reader.GetValue(0).ToString());
                }
                con.Close();

                con.Open();
                c.Connection = con;

                c.CommandText = "update Items set [العدد]=" + number + " where [الصنف]='" + names.ElementAt(i) + "'";
                c.ExecuteNonQuery();
                number = 0;
                c.CommandText = "update Items2 set [العدد]=" + number + " where [الصنف]='" + names.ElementAt(i) + "'";
                c.ExecuteNonQuery();
                con.Close();
            }
        }

        public String tables_check()
        {
            OleDbCommand c = new OleDbCommand();

            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
            OleDbDataReader reader1;
            int j = 0;
            String message = "";
            for (int i = 1; i <= 100; i++)
            {
                con.Open();
                c.Connection = con;

                c.CommandText = "select * from [Table" + i + "]";
                reader1 = c.ExecuteReader();

                if (reader1.Read())
                {
                    j++;
                }
                con.Close();
                if (j > 0)
                {
                    message += "Table " + i + "\n";
                }
                j = 0;
            }

            return message;
        }
    }

}
