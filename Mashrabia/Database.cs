using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mashrabia
{
    public class Database
    {
        private OleDbConnection con = new OleDbConnection();
        private string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/ma5zan.mdb";
        private string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Category.mdb";
        private string path3 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Tables.mdb";
        private string path4 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/FinalData.mdb";
        private string path5 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Shift.mdb";
        private string path6 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/TakeAway&Edara.mdb";
        private string path7 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Password.mdb";
        private string path8 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/PrintOrders.mdb";


        public DataTable ViewTable(String table)
        {

            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select * from " + table;
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

        public List<String> GetProducts()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                List<String> list = new List<String>();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [الصنف] from Products";
                OleDbDataReader reader = c.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader.GetValue(0).ToString());
                }


                con.Close();
                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;
            }
        }

        public List<String> GetUnits()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                List<String> list = new List<String>();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [الوحدة] from Units";
                OleDbDataReader reader = c.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader.GetValue(0).ToString());
                }


                con.Close();
                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;
            }
        }

        public void INOUTStock(String Product, String Unit, String Quantity, String Total, String Date, int s)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (s == 0)
                {
                    c.CommandText = "INSERT INTO Log_File ([اسم الصنف],[الوحدة],[العدد],[الاجمالي],[التاريخ]) VALUES ('" + Product + "','" + Unit + "'," + Quantity + "," + Total + ",#" + Date + "#)";
                    c.ExecuteNonQuery();

                    c.CommandText = "select [العدد],[الاجمالي] from Stock where [اسم الصنف] ='" + Product + "'";
                    OleDbDataReader reader = c.ExecuteReader();
                    int f1 = 0;
                    String money = "0", q = "0";
                    Double sum = 0;
                    Double sumMoney = 0;
                    while (reader.Read())
                    {
                        q = reader.GetValue(0).ToString();
                        money = reader.GetValue(1).ToString();
                        f1++;
                    }

                    con.Close();
                    con.Open();
                    c.Connection = con;

                    if (f1 > 0)
                    {
                        sumMoney = Convert.ToDouble(money) + Convert.ToDouble(Total);
                        sum = Convert.ToDouble(Quantity) + Convert.ToDouble(q);
                        c.CommandText = "update Stock set [العدد]=" + sum + ",[الاجمالي]=" + sumMoney + " where [اسم الصنف]='" + Product + "'";
                        c.ExecuteNonQuery();
                    }
                    else if (f1 == 0)
                    {
                        c.CommandText = "INSERT INTO Stock ([اسم الصنف],[الوحدة],[العدد],[الاجمالي]) VALUES ('" + Product + "','" + Unit + "'," + Quantity + "," + Total + ")";
                        c.ExecuteNonQuery();
                    }


                }
                else if (s == 1)
                {
                    c.CommandText = "select [العدد],[الاجمالي] from Stock where [اسم الصنف] ='" + Product + "'";
                    OleDbDataReader reader12 = c.ExecuteReader();
                    Double q12 = -1;
                    Double p12 = 0;
                    while (reader12.Read())
                    {
                        q12 = Convert.ToDouble(reader12.GetValue(0).ToString());
                        p12 = Convert.ToDouble(reader12.GetValue(1).ToString());
                    }
                    con.Close();
                    con.Open();
                    c.Connection = con;

                    if (q12 == Convert.ToDouble(Quantity))
                    {
                        c.CommandText = "select [الاجمالي] from Stock where [اسم الصنف] ='" + Product + "'";
                        OleDbDataReader reader123 = c.ExecuteReader();
                        Double tt1 = 0;
                        while (reader123.Read())
                        {
                            tt1 = Convert.ToDouble(reader123.GetValue(0).ToString());
                        }
                        con.Close();
                        con.Open();
                        c.Connection = con;

                        Total = tt1.ToString();
                        c.CommandText = "delete from Stock where [اسم الصنف]='" + Product + "'";
                        c.ExecuteNonQuery();
                        c.CommandText = "INSERT INTO Log_File ([اسم الصنف],[الوحدة],[العدد],[الاجمالي],[التاريخ]) VALUES ('" + Product + "','" + Unit + "',-" + Quantity + "," + Total + ",#" + Date + "#)";
                        c.ExecuteNonQuery();
                    }

                    else if (q12 > Convert.ToDouble(Quantity))
                    {
                        c.CommandText = "select [العدد],[الاجمالي] from Stock where [اسم الصنف] ='" + Product + "'";
                        OleDbDataReader reader = c.ExecuteReader();
                        int f = 0;
                        String q = "0";
                        Double p = 0;
                        Double sum = 0;
                        while (reader.Read())
                        {
                            q = reader.GetValue(0).ToString();
                            p = Convert.ToDouble(reader.GetValue(1).ToString());
                            f++;
                        }
                        con.Close();
                        con.Open();
                        c.Connection = con;

                        if (f > 0)
                        {
                            sum = Convert.ToDouble(q) - Convert.ToDouble(Quantity);
                            Double pF = Convert.ToDouble(Quantity) * (p / Convert.ToDouble(q));
                            p = p - pF;
                            if (sum > 0)
                            {
                                c.CommandText = "INSERT INTO Log_File ([اسم الصنف],[الوحدة],[العدد],[الاجمالي],[التاريخ]) VALUES ('" + Product + "','" + Unit + "',-" + Quantity + "," + pF + ",'" + Date + "')";
                                c.ExecuteNonQuery();
                                c.CommandText = "update Stock set [العدد]=" + sum + ",[الاجمالي]=" + p + " where [اسم الصنف]='" + Product + "'";
                                c.ExecuteNonQuery();
                            }
                            else if (sum == 0)
                            {
                                c.CommandText = "INSERT INTO Log_File ([اسم الصنف],[الوحدة],[العدد],[الاجمالي],[التاريخ]) VALUES ('" + Product + "','" + Unit + "',-" + Quantity + "," + Total + ",'" + Date + "')";
                                c.ExecuteNonQuery();
                                c.CommandText = "delete from Stock where [اسم الصنف]='" + Product + "'";
                                c.ExecuteNonQuery();
                            }
                            else
                                MessageBox.Show("Number Of Out Is Bigger Than Stock Item : " + Product);
                        }
                        else if (f == 0)
                        {
                            MessageBox.Show("Outoff Item");
                        }

                    }

                    else
                        MessageBox.Show("Sorry .. Number Of Out Is Bigger Than Stock Item : " + Product);

                }


                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();

            }
        }

        public String CheckProductUnit(String Product)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [الوحدة] from Stock where [اسم الصنف]='" + Product + "'";
                OleDbDataReader reader = c.ExecuteReader();
                String j = "Error Unit";
                while (reader.Read())
                {
                    j = reader.GetValue(0).ToString();
                }

                con.Close();
                return j;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;
            }


        }

        public void deleteFromTabel(String TabelName, String ID, String NAME, String UNIT, String QUANTITY, String TOTAL, String DATE)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            if (TabelName.Equals("Log_File"))
            {
                try
                {
                    c.CommandText = "delete from Log_File where [اسم الصنف]='" + NAME + "'";
                    c.ExecuteNonQuery();
                    c.CommandText = "delete from Stock where [اسم الصنف]='" + NAME + "'";
                    c.ExecuteNonQuery();
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error from deleteFromTable " + ex);
                    con.Close();
                }
            }
            else if (TabelName.Equals("المخزن"))
            {
                c.CommandText = "delete from Log_File where [اسم الصنف]='" + NAME + "'";
                c.ExecuteNonQuery();

                c.CommandText = "select [العدد],[الاجمالي] from Stock where [اسم الصنف] ='" + NAME + "'";
                OleDbDataReader reader = c.ExecuteReader();
                int f1 = 0;
                Double sumq = 0;
                Double sumMoney = 0;
                while (reader.Read())
                {
                    sumq = Convert.ToDouble(reader.GetValue(0).ToString());
                    sumMoney = Convert.ToDouble(reader.GetValue(1).ToString());
                    f1++;
                }
                if (Convert.ToDouble(QUANTITY) < 0)
                    TOTAL = "-" + TOTAL;

                con.Close();
                con.Open();
                c.Connection = con;
                if (f1 == 1)
                {
                    sumq = sumq - Convert.ToDouble(QUANTITY);
                    sumMoney = sumMoney - Convert.ToDouble(TOTAL);
                    if (sumq == 0)
                    {
                        c.CommandText = "delete from Stock where [اسم الصنف]='" + NAME + "'";
                        c.ExecuteNonQuery();
                    }
                    else if (sumq > 0)
                    {
                        c.CommandText = "update Stock set [العدد]=" + sumq + ",[الاجمالي]=" + sumMoney + " where [اسم الصنف]='" + NAME + "'";
                        c.ExecuteNonQuery();
                    }
                }

                con.Close();
            }
            else
                MessageBox.Show("Error from Database Class Method deleteFromTabel");
            con.Close();
        }

        public void insert_Table(DataTable table, int ss)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "delete from Temp";
                c.ExecuteNonQuery();
                c.CommandText = "delete from tempStock";
                c.ExecuteNonQuery();
                var adapter = new OleDbDataAdapter();
                if (ss == 1)
                {
                    adapter.InsertCommand = new OleDbCommand("INSERT INTO temp ([كود],[اسم الصنف],[الوحدة],[العدد],[الاجمالي],[التاريخ]) VALUES (@a , @b , @c , @d , @e , @f)", con);
                    adapter.InsertCommand.Parameters.Add("a", OleDbType.VarChar, 40, "كود");
                    adapter.InsertCommand.Parameters.Add("b", OleDbType.VarChar, 40, "اسم الصنف");
                    adapter.InsertCommand.Parameters.Add("c", OleDbType.VarChar, 40, "الوحدة");
                    adapter.InsertCommand.Parameters.Add("d", OleDbType.VarChar, 40, "العدد");
                    adapter.InsertCommand.Parameters.Add("e", OleDbType.VarChar, 40, "الاجمالي");
                    adapter.InsertCommand.Parameters.Add("f", OleDbType.VarChar, 40, "التاريخ");
                }
                else
                {
                    adapter.InsertCommand = new OleDbCommand("INSERT INTO tempStock ([كود],[اسم الصنف],[الوحدة],[العدد],[الاجمالي]) VALUES (@a , @b , @c , @d , @e)", con);
                    adapter.InsertCommand.Parameters.Add("a", OleDbType.VarChar, 40, "كود");
                    adapter.InsertCommand.Parameters.Add("b", OleDbType.VarChar, 40, "اسم الصنف");
                    adapter.InsertCommand.Parameters.Add("c", OleDbType.VarChar, 40, "الوحدة");
                    adapter.InsertCommand.Parameters.Add("d", OleDbType.VarChar, 40, "العدد");
                    adapter.InsertCommand.Parameters.Add("e", OleDbType.VarChar, 40, "الاجمالي");
                }
                adapter.Update(table);


                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error from insert_table " + ex);
                con.Close();

            }

        }

        public void undo()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            try
            {
                c.CommandText = "select [كود],[اسم الصنف],[الوحدة],[العدد],[الاجمالي] from Log_File";
                OleDbDataReader reader = c.ExecuteReader();

                int q = -1, I = -1;
                String N = "", U = "";
                Double Q1 = 0, T1 = 0;
                while (reader.Read())
                {
                    q = Convert.ToInt16(reader.GetValue(0).ToString());
                    if (I < q)
                    {
                        I = q;
                        N = reader.GetValue(1).ToString();
                        U = reader.GetValue(2).ToString();
                        Q1 = Convert.ToDouble(reader.GetValue(3).ToString());
                        T1 = Convert.ToDouble(reader.GetValue(4).ToString());
                    }
                }
                con.Close();
                con.Open();
                c.Connection = con;
                if (Q1 < 0)
                {
                    Q1 = Q1 * -1;
                }
                else if (Q1 > 0)
                {
                    Q1 = Q1 * -1;
                    T1 = T1 * -1;
                }
                Boolean check = false;
                c.CommandText = "select [العدد],[الاجمالي] from Stock where [اسم الصنف]='" + N + "'";
                OleDbDataReader reader1 = c.ExecuteReader();

                while (reader1.Read())
                {
                    Q1 = Q1 + Convert.ToDouble(reader1.GetValue(0).ToString());
                    T1 = T1 + Convert.ToDouble(reader1.GetValue(1).ToString());
                    check = true;
                }



                con.Close();
                con.Open();
                c.Connection = con;
                DateTime D = DateTime.Today;

                if (q != -1 && Q1 > 0)
                {
                    c.CommandText = "delete from Log_File where [كود]=" + I;
                    c.ExecuteNonQuery();
                    if (check)
                    {
                        c.CommandText = "update Stock set [العدد]=" + Q1 + ",[الاجمالي]=" + T1 + " where [اسم الصنف]='" + N + "'";
                        c.ExecuteNonQuery();
                    }
                    else
                    {
                        c.CommandText = "INSERT INTO Stock ([اسم الصنف],[الوحدة],[العدد],[الاجمالي]) VALUES ('" + N + "','" + U + "'," + Q1 + "," + T1 + ")";
                        c.ExecuteNonQuery();
                    }
                }
                else if (Q1 == 0)
                {
                    c.CommandText = "delete from Log_File where [كود]=" + I;
                    c.ExecuteNonQuery();
                    c.CommandText = "delete from Stock where [اسم الصنف]='" + N + "'";
                    c.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error from deleteFromTable " + ex);
                con.Close();
            }
        }

        public DataTable Sort_LogFile(String From, String To, String Text)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                int n = 0;
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                Boolean isNumeric = int.TryParse(Text, out n);
                if (Text == "" && From != "" && To != "")
                    c.CommandText = "SELECT * from [Log_File] Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                else if (Text != "" && From != "" && To != "")
                {
                    if (isNumeric)
                        c.CommandText = "SELECT * from [Log_File] Where (([التاريخ] between #" + From + "# AND #" + To + "#) AND ([كود] =" + n + "))";
                    else
                        c.CommandText = "SELECT * from [Log_File] Where (([التاريخ] between #" + From + "# AND #" + To + "#) AND ([اسم الصنف] like '%" + Text + "%'))";
                }
                else if (Text != "" && (From == "" || To == ""))
                {
                    c.CommandText = "SELECT * from [Log_File] Where [اسم الصنف] like '%" + Text + "%'";
                }
                else
                    c.CommandText = "SELECT * from [Log_File]";

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

        public void insert_s(String table, String data)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (table == "Products")
                    c.CommandText = "INSERT INTO " + table + " ([الصنف]) VALUES ('" + data + "')";
                else
                    c.CommandText = "INSERT INTO " + table + " ([الوحدة]) VALUES ('" + data + "')";
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                con.Close();

            }
        }

        public void delete_s(String table, String id)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "delete from " + table + " where [كود]= " + id;

                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                con.Close();

            }
        }

        public void delete_category(String table)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "DROP TABLE [" + table + "];";
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                con.Close();

            }
        }

        public void insert_category(String table)
        {

            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "CREATE TABLE [" + table + "] ([السعر] double NOT NULL,[الاسم] VARCHAR(40) NOT NULL,[كود] AUTOINCREMENT NOT NULL PRIMARY KEY)";
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                con.Close();

            }

        }

        public DataTable ViewTable_category(String table)
        {

            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select * from [" + table + "]";
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

        public void insert_category_items(String table, String data, String price)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "INSERT INTO [" + table + "] ([الاسم],[السعر]) VALUES ('" + data + "'," + price + ")";
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                con.Close();

            }
        }

        public void delete_category_item(String table, String id)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path2 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "delete from [" + table + "] where [كود]= " + id;

                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                con.Close();

            }
        }

        public Double Total_Stock(String Text)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (Text != "" && !Text.Contains("'"))
                    c.CommandText = "select [الاجمالي] from Stock where [اسم الصنف] like'%" + Text + "%'";
                else if (!Text.Contains("'"))
                    c.CommandText = "select [الاجمالي] from Stock";
                OleDbDataReader reader = c.ExecuteReader();
                Double sum = 0;
                while (reader.Read())
                {
                    sum = sum + Convert.ToDouble(reader.GetValue(0).ToString());
                }

                con.Close();
                return sum;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return 0;
            }

        }

        public void Filtering_Final(String Shift, DateTime Date)
        {
            con.Close();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [الاسم] from Shift";
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
                List<String> NAMES = Names.Distinct().ToList();
                for (int i = 0; i < NAMES.Count(); i++)
                {
                    con.Close();
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    c.Connection = con;

                    String Price = "0";
                    Double Total = 0;
                    int Number = 0;
                    c.CommandText = "select [العدد],[سعر الوحدة],[الاجمالي] from Shift where [الاسم]='" + NAMES.ElementAt(i) + "'";
                    OleDbDataReader reader1 = c.ExecuteReader();
                    while (reader1.Read())
                    {
                        Total = Total + Convert.ToInt16(reader1.GetValue(2).ToString());
                        Number = Number + Convert.ToInt16(reader1.GetValue(0).ToString());
                        Price = reader1.GetValue(1).ToString();
                    }

                    con.Close();
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    c.Connection = con;
                    c.CommandText = "INSERT INTO Final_Items ([الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ]) VALUES ('" + NAMES.ElementAt(i) + "'," + Number + "," + Price + "," + Total + ",'" + Shift + "',#" + Date.ToString("yyyy/MM/dd") + "#)";
                    c.ExecuteNonQuery();
                    con.Close();
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    c.Connection = con;
                    c.CommandText = "delete from Shift where [الاسم]='" + NAMES.ElementAt(i) + "'";
                    c.ExecuteNonQuery();
                }
            }

            con.Close();
        }

        public void Filtering_Expenses(String Shift, DateTime Date)
        {
            con.Close();
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات] from Expenses";
            OleDbDataReader reader = c.ExecuteReader();
            int c1 = 0;
            List<String> Names = new List<string>();
            List<String> Value = new List<string>();
            List<String> Notes = new List<string>();
            while (reader.Read())
            {
                c1++;
                Names.Add(reader.GetValue(0).ToString());
                Value.Add(reader.GetValue(1).ToString());
                Notes.Add(reader.GetValue(2).ToString());
            }
            if (c1 != 0)
            {
                for (int i = 0; i < Names.Count(); i++)
                {
                    con.Close();
                    con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                    con.Open();
                    c.Connection = con;

                    c.CommandText = "INSERT INTO Final_Expenses ([المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]) VALUES ('" + Names.ElementAt(i) + "'," + Value.ElementAt(i) + ",'" + Notes.ElementAt(i) + "','" + Shift + "',#" + Date.ToString("yyyy/MM/dd") + "#)";
                    c.ExecuteNonQuery();
                }
            }

            con.Close();
        }

        public void Final_Data(String Total_In, String Total_Expense, String Total_Wages, String Total_Edara, String Total_Net, String Shift, DateTime Date)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                // DateTime Date = DateTime.Now;
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "INSERT INTO Final_Total ([ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ]) VALUES (" + Total_In + "," + Total_Expense + "," + Total_Wages + "," + Total_Edara + "," + Total_Net + ",'" + Shift + "',#" + Date.ToString("yyyy/MM/dd") + "#)";
                c.ExecuteNonQuery();
                con.Close();
                Filtering_Final(Shift, Date);
                Filtering_Expenses(Shift, Date);
                con.Close();
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path5 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.Connection = con;
                c.CommandText = "delete from Shift";
                c.ExecuteNonQuery();
                c.CommandText = "delete from Expenses";
                c.ExecuteNonQuery();
                c.CommandText = "delete from Wages";
                c.ExecuteNonQuery();

                con.Close();
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path6 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                c.Connection = con;
                c.CommandText = "delete from Edara";
                c.ExecuteNonQuery();
                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();

            }
        }

        public DataTable Final_Data_View(String table)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (table == "Final_Total")
                    c.CommandText = "select [ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ] from Final_Total";
                else if (table == "Final_Items")
                    c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items";
                else if (table == "Final_Expenses")
                    c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses";

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

        public Double Total_Data_View(String table)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (table == "Final_Total")
                    c.CommandText = "select [صافي] from Final_Total";
                else if (table == "Final_Items")
                    c.CommandText = "select [الاجمالي] from Final_Items";
                else if (table == "Final_Expenses")
                    c.CommandText = "select [القيمة]  from Final_Expenses";

                Double Total = 0;
                OleDbDataReader reader = c.ExecuteReader();
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

        public Double Total_Final_Data(String table, String text, String From, String To, String Shift)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;


                if (table == "تفاصيل شيفتات")
                {
                    if (From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [صافي] from Final_Total  Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                        else
                            c.CommandText = "select [صافي] from Final_Total  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [شيفت] = '" + Shift + "'";
                    }
                    else
                    {
                        c.CommandText = "select [صافي] from Final_Total";
                    }
                }
                else if (table == "تفاصيل ايرادات")
                {
                    if (text == "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [الاجمالي] from Final_Items  Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                        else
                            c.CommandText = "select [الاجمالي] from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [شيفت] = '" + Shift + "'";
                    }
                    else if (text != "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [الاجمالي] from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [الاسم] like '%" + text + "%'";
                        else
                            c.CommandText = "select [الاجمالي] from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "') and ([الاسم] like '%" + text + "%')";
                    }
                    else if (text != "" && (From == "" || To == ""))
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [الاجمالي] from Final_Items  Where [الاسم] like '%" + text + "%'";
                        else
                            c.CommandText = "select [الاجمالي] from Final_Items  Where ([شيفت] = '" + Shift + "') and ([الاسم] like '%" + text + "%')";
                    }
                    else
                    {
                        c.CommandText = "select [الاجمالي] from Final_Items";
                    }
                }
                else if (table == "تفاصيل مصروفات")
                {
                    if (text == "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [القيمة] from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#)";
                        else
                            c.CommandText = "select [القيمة]  from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "')";
                    }
                    else if (text != "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [القيمة] from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                        else
                            c.CommandText = "select [القيمة]  from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "') and ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                    }
                    else if (text != "" && (From == "" || To == ""))
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [القيمة] from Final_Expenses  Where ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                        else
                            c.CommandText = "select [القيمة]  from Final_Expenses  Where ([شيفت] = '" + Shift + "') and ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                    }
                    else
                    {
                        c.CommandText = "select [القيمة]  from Final_Expenses";
                    }
                }

                Double Total = 0;
                OleDbDataReader reader = c.ExecuteReader();
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

        public Double Total_Final_Shift(String table, String Shift)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;

                if (table == "تفاصيل شيفتات")
                {
                    if (Shift == "عرض الكل")
                    {
                        c.CommandText = "select [صافي] from Final_Total";
                    }
                    else
                    {
                        c.CommandText = "select [صافي] from Final_Total where [شيفت]='" + Shift + "'";
                    }
                }
                else if (table == "تفاصيل ايرادات")
                {
                    if (Shift == "عرض الكل")
                    {
                        c.CommandText = "select [الاجمالي] from Final_Items";
                    }
                    else
                    {
                        c.CommandText = "select [الاجمالي] from Final_Items where [شيفت]='" + Shift + "'";
                    }
                }
                else if (table == "تفاصيل مصروفات")
                {
                    if (Shift == "عرض الكل")
                    {
                        c.CommandText = "select [القيمة]  from Final_Expenses";
                    }
                    else
                    {
                        c.CommandText = "select [القيمة]  from Final_Expenses where [شيفت]='" + Shift + "'";
                    }
                }

                Double Total = 0;
                OleDbDataReader reader = c.ExecuteReader();
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

        public DataTable Search_Final_Data(String table, String text, String From, String To, String Shift)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;


                if (table == "تفاصيل شيفتات")
                {
                    if (From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ] from Final_Total  Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                        else
                            c.CommandText = "select [ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ] from Final_Total  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [شيفت] = '" + Shift + "'";
                    }
                    else
                    {
                        c.CommandText = "select [ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ] from Final_Total";
                    }
                }
                else if (table == "تفاصيل ايرادات")
                {
                    if (text == "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items  Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                        else
                            c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [شيفت] = '" + Shift + "'";
                    }
                    else if (text != "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [الاسم] like '%" + text + "%'";
                        else
                            c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "') and ([الاسم] like '%" + text + "%')";
                    }
                    else if (text != "" && (From == "" || To == ""))
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items  Where [الاسم] like '%" + text + "%'";
                        else
                            c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items  Where ([شيفت] = '" + Shift + "') and ([الاسم] like '%" + text + "%')";
                    }
                    else
                    {
                        c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items";
                    }
                }
                else if (table == "تفاصيل مصروفات")
                {
                    if (text == "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ] from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#)";
                        else
                            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "')";
                    }
                    else if (text != "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ] from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                        else
                            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "') and ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                    }
                    else if (text != "" && (From == "" || To == ""))
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ] from Final_Expenses  Where ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                        else
                            c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses  Where ([شيفت] = '" + Shift + "') and ([المصروفات] like '%" + text + "%' or [ملاحظات] like '%" + text + "%')";
                    }
                    else
                    {
                        c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses";
                    }
                }

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

        public void Delete_Final_Data(String table, String text, String From, String To, String Shift)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;


                if (table == "تفاصيل شيفتات")
                {
                    if (From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "delete from Final_Total  Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                        else
                            c.CommandText = "delete from Final_Total  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [شيفت] = '" + Shift + "'";
                    }
                }
                else if (table == "تفاصيل ايرادات")
                {
                    if (text == "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "delete from Final_Items Where [التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#";
                        else
                            c.CommandText = "delete from Final_Items  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and [شيفت] = '" + Shift + "'";
                    }
                }
                else if (table == "تفاصيل مصروفات")
                {
                    if (text == "" && From != "" && To != "")
                    {
                        if (Shift == "عرض الكل")
                            c.CommandText = "delete from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#)";
                        else
                            c.CommandText = "delete from Final_Expenses  Where ([التاريخ] >= #" + From + "# AND [التاريخ] <= #" + To + "#) and ([شيفت] = '" + Shift + "')";
                    }
                }
                c.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
            }
        }

        public DataTable Search_Final_Shift(String table, String Shift)
        {
            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;

                if (table == "تفاصيل شيفتات")
                {
                    if (Shift == "عرض الكل")
                    {
                        c.CommandText = "select [ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ] from Final_Total";
                    }
                    else
                    {
                        c.CommandText = "select [ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ] from Final_Total where [شيفت]='" + Shift + "'";
                    }
                }
                else if (table == "تفاصيل ايرادات")
                {
                    if (Shift == "عرض الكل")
                    {
                        c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items";
                    }
                    else
                    {
                        c.CommandText = "select [الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ] from Final_Items where [شيفت]='" + Shift + "'";
                    }
                }
                else if (table == "تفاصيل مصروفات")
                {
                    if (Shift == "عرض الكل")
                    {
                        c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses";
                    }
                    else
                    {
                        c.CommandText = "select [المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]  from Final_Expenses where [شيفت]='" + Shift + "'";
                    }
                }


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

        public void insert_Final_Print(DataTable table, int ss)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                var adapter = new OleDbDataAdapter();
                if (ss == 1)
                {
                    c.CommandText = "delete from Temp_Final_Total";
                    c.ExecuteNonQuery();
                    adapter.InsertCommand = new OleDbCommand("INSERT INTO Temp_Final_Total ([ايرادات],[مصروفات],[يوميات],[ادارة],[صافي],[شيفت],[التاريخ]) VALUES (@a , @b , @c , @d , @e , @f ,@g)", con);
                    adapter.InsertCommand.Parameters.Add("a", OleDbType.VarChar, 40, "ايرادات");
                    adapter.InsertCommand.Parameters.Add("b", OleDbType.VarChar, 40, "مصروفات");
                    adapter.InsertCommand.Parameters.Add("c", OleDbType.VarChar, 40, "يوميات");
                    adapter.InsertCommand.Parameters.Add("d", OleDbType.VarChar, 40, "ادارة");
                    adapter.InsertCommand.Parameters.Add("e", OleDbType.VarChar, 40, "صافي");
                    adapter.InsertCommand.Parameters.Add("f", OleDbType.VarChar, 40, "شيفت");
                    adapter.InsertCommand.Parameters.Add("g", OleDbType.VarChar, 40, "التاريخ");
                }
                else if (ss == 2)
                {
                    c.CommandText = "delete from Temp_Final_Items";
                    c.ExecuteNonQuery();
                    adapter.InsertCommand = new OleDbCommand("INSERT INTO Temp_Final_Items ([الاسم],[العدد],[سعر الوحدة],[الاجمالي],[شيفت],[التاريخ]) VALUES (@a , @b , @c , @d , @e ,@f)", con);
                    adapter.InsertCommand.Parameters.Add("a", OleDbType.VarChar, 40, "الاسم");
                    adapter.InsertCommand.Parameters.Add("b", OleDbType.VarChar, 40, "العدد");
                    adapter.InsertCommand.Parameters.Add("c", OleDbType.VarChar, 40, "سعر الوحدة");
                    adapter.InsertCommand.Parameters.Add("d", OleDbType.VarChar, 40, "الاجمالي");
                    adapter.InsertCommand.Parameters.Add("e", OleDbType.VarChar, 40, "شيفت");
                    adapter.InsertCommand.Parameters.Add("f", OleDbType.VarChar, 40, "التاريخ");
                }
                else
                {
                    c.CommandText = "delete from Temp_Final_Expenses";
                    c.ExecuteNonQuery();
                    adapter.InsertCommand = new OleDbCommand("INSERT INTO Temp_Final_Expenses ([المصروفات],[القيمة],[ملاحظات],[شيفت],[التاريخ]) VALUES (@a , @b , @c , @d , @e)", con);
                    adapter.InsertCommand.Parameters.Add("a", OleDbType.VarChar, 40, "المصروفات");
                    adapter.InsertCommand.Parameters.Add("b", OleDbType.VarChar, 40, "القيمة");
                    adapter.InsertCommand.Parameters.Add("c", OleDbType.VarChar, 40, "ملاحظات");
                    adapter.InsertCommand.Parameters.Add("d", OleDbType.VarChar, 40, "شيفت");
                    adapter.InsertCommand.Parameters.Add("e", OleDbType.VarChar, 40, "التاريخ");
                }
                adapter.Update(table);


                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error from insert_table " + ex);
                con.Close();

            }

        }

        public String get_admin()
        {
            try
            {

                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select * from [Password]";
                OleDbDataReader reader = c.ExecuteReader();
                String admin = "0";
                while (reader.Read())
                {
                    admin = reader.GetValue(0).ToString();
                }
                con.Close();
                return admin;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error from insert_table " + ex);
                con.Close();
                return null;
            }
        }

        public void set_admin(String password)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "delete from [Password]";
            c.ExecuteNonQuery();
            c.CommandText = "insert into [Password] ([pass]) VALUES ('" + password + "')";
            c.ExecuteNonQuery();

            con.Close();

        }

        public Boolean Serial()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [Serial] from [Serial]";
            OleDbDataReader reader = c.ExecuteReader();
            String temp = "error";
            while (reader.Read())
            {
                temp = reader.GetValue(0).ToString();
                break;
            }
            con.Close();
            if (temp != "error")
            {
                if (temp == cpuInfo)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public Boolean insert_serial(String serial)
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }
            if (serial == cpuInfo)
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "delete from Serial";
                c.ExecuteNonQuery();
                c.CommandText = "INSERT INTO Serial ([Serial]) VALUES ('" + serial + "')";
                c.ExecuteNonQuery();
                con.Close();
                return true;
            }
            else
                return false;
        }

        public int state_read()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select * from [State]";
            OleDbDataReader reader = c.ExecuteReader();
            String temp = "0";
            while (reader.Read())
            {
                temp = reader.GetValue(0).ToString();
                break;
            }
            con.Close();
            return Convert.ToInt16(temp);
        }
        public void state_write(int s)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path7 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "delete from [State]";
            c.ExecuteNonQuery();
            c.CommandText = "insert into [State] ([State]) VALUES (" + s + ")";
            c.ExecuteNonQuery();
            con.Close();
        }

        public void Mowarden_insert(String name, String number, String notes)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "INSERT INTO Mowarden ([الاسم],[الرقم],[الملاحظات],[القيمة]) VALUES ('" + name + "', " + number + ", '" + notes + "',0)";
            c.ExecuteNonQuery();
            con.Close();
        }

        public void Mowarden_value(Double value, String name)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "select [القيمة] from Mowarden where [الاسم]='" + name + "'";
            OleDbDataReader reader = c.ExecuteReader();
            double v = 0;
            while (reader.Read())
            {
                v = value + Convert.ToDouble(reader.GetValue(0).ToString());
            }
            DateTime d = DateTime.Now;
            c.CommandText = "update Mowarden set [القيمة]=" + value;
            c.ExecuteNonQuery();
            c.CommandText = "INSERT INTO Mowarden_Log ([الاسم],[القيمة],[التاريخ]) VALUES ('" + name + "', " + value + ", #" + d.ToString("yyyy/MM/dd") + "#)";
            c.ExecuteNonQuery();

            con.Close();
        }

        public void Mowarden_delete(String name)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            con.Open();
            OleDbCommand c = new OleDbCommand();
            c.Connection = con;
            c.CommandText = "delete from Mowarden where [الاسم]='" + name + "'";
            c.ExecuteNonQuery();
            con.Close();
        }

        public DataTable Mowarden_View(int op)
        {

            try
            {
                con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                if (op == 0)
                    c.CommandText = "select * from Mowarden";
                else if (op == 1)
                    c.CommandText = "select * from Mowarden_Log";
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

        public List<String> GetMowarden()
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                List<String> list = new List<String>();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;
                c.CommandText = "select [الاسم] from Mowarden";
                OleDbDataReader reader = c.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader.GetValue(0).ToString());
                }


                con.Close();
                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("error " + ex);
                con.Close();
                return null;
            }
        }

        public void files()
        {
            String location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/files/";
            String location1 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/";

            if (!File.Exists(path))
            {
                System.IO.File.Copy(location + "ma5zan.mdb", path, true);
            }
            if (!File.Exists(path2))
            {
                System.IO.File.Copy(location + "Category.mdb", path2, true);
            }
            if (!File.Exists(path3))
            {
                System.IO.File.Copy(location + "Tables.mdb", path3, true);
            }
            if (!File.Exists(path4))
            {
                System.IO.File.Copy(location + "FinalData.mdb", path4, true);
            }
            if (!File.Exists(path5))
            {
                System.IO.File.Copy(location + "Shift.mdb", path5, true);
            }
            if (!File.Exists(path6))
            {
                System.IO.File.Copy(location + "TakeAway&Edara.mdb", path6, true);
            }
            if (!File.Exists(path7))
            {
                System.IO.File.Copy(location + "Password.mdb", path7, true);
            }
            if (!File.Exists(path8))
            {
                System.IO.File.Copy(location + "PrintOrders.mdb", path8, true);
            }
            if (!File.Exists(location1 + "icon.ico"))
            {
                System.IO.File.Copy(location + "icon.ico", location1 + "icon.ico", true);
            }
            if (!File.Exists(location1 + "images.png"))
            {
                System.IO.File.Copy(location + "images.png", location1 + "images.png", true);
            }
            if (!File.Exists(location1 + "img.png"))
            {
                System.IO.File.Copy(location + "img.png", location1 + "img.png", true);
            }
            if (!File.Exists(location1 + "graphic.jpg"))
            {
                System.IO.File.Copy(location + "graphic.jpg", location1 + "graphic.jpg", true);
            }

        }

        public void INOUTMowardeen(String Name, String Money, String Date)
        {
            con.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path4 + ";Jet OLEDB:Database Password=3bood197";
            try
            {
                con.Open();
                OleDbCommand c = new OleDbCommand();
                c.Connection = con;

                c.CommandText = "INSERT INTO Mowarden_Log ([الاسم],[القيمة],[التاريخ]) VALUES ('" + Name + "'," + Money + ",#" + Date + "#)";
                c.ExecuteNonQuery();

                c.CommandText = "select [القيمة] from Mowarden where [الاسم] ='" + Name + "'";
                OleDbDataReader reader = c.ExecuteReader();
                int f1 = 0;
                String m = "0";
                Double sumMoney = 0;
                while (reader.Read())
                {
                    m = reader.GetValue(0).ToString();
                    f1++;
                }

                con.Close();
                con.Open();
                c.Connection = con;

                if (f1 > 0)
                {
                    sumMoney = Convert.ToDouble(m) + Convert.ToDouble(Money);
                    c.CommandText = "update Mowarden set [القيمة]=" + sumMoney + " where [الاسم]='" + Name + "'";
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

    }
}
