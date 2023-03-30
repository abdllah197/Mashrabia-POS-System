using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using tik4net;
using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;
using System.Security.Cryptography;
using DevExpress.XtraReports.UI;
using System.Globalization;
using System.Net.Sockets;

namespace Mashrabia
{
    class Internet_Carts
    {
        private ITikConnection connection = ConnectionFactory.CreateConnection(TikConnectionType.Api);
        private String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/MicroTiklogin.txt";
        private String path1 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/MicroTikcounter.txt";
        private String path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Generate_Profiles.txt";
        private String Temp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Temp.txt";

        public Boolean write_connection(String host, String admin, String password)
        {
            try
            {
                connection.Open(host, admin, password);
                connection.Close();
                if (password == "")
                    password = "error";
                string createText = host + " " + admin + " " + password;
                File.WriteAllText(path, createText);
                return true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Wrong Ip Host");
                return false;
            }
            catch (TikCommandException)
            {
                MessageBox.Show("User Or Password Incorrect");
                return false;
            }
            catch (Exception)
            {
                MessageBox.Show("Error on save file");
                return false;
            }


        }

        public Boolean write_counter(String Count)
        {
            try
            {                
                string createText = Count;
                File.WriteAllText(path1, createText);
                return true;
            }
            
            catch (Exception)
            {
                MessageBox.Show("Error on save Counter");
                return false;
            }


        }

        public string[] read_connection()
        {
            String[] readText = { "error", "error", "error"};
            String temp = "";
            if (File.Exists(path))
            {
                temp = File.ReadAllText(path);
                readText = temp.Split(' ');                
            }

            
            
            return readText;
        }        
        
        public string read_counter()
        {
            String readText = "error";

            if (File.Exists(path1))
            {
                readText = File.ReadAllText(path1);
                return readText;
            }
            else
            {
                write_counter("0");
                return "0";
            }
        }

        public bool test_login()
        {
            
            String[] connection_text = read_connection();
            if (connection_text[0] != "error")
            {
                try
                {
                  
                    if (connection_text[2] == "error")
                    {                     
                        connection.Open(connection_text[0], connection_text[1], "");
                    }
                    else
                    {
                        connection.Open(connection_text[0], connection_text[1], connection_text[2]);
                    }
                    if (connection.IsOpened)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Connection timed out please try again");
                        return false;
                    }
                   
                }
                catch (System.Net.Sockets.SocketException)
                {
                    MessageBox.Show("Ip Host Not Founded");
                    return false;
                }
                catch (TikCommandException)
                {
                    MessageBox.Show("User Or Password Incorrect");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("check Microtik login in settings");
                return false;
            }

        }

        public List<String> profiles()
        {
            if (test_login())
            {

                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);

                var temp = connection.LoadList<HotspotUserProfile>();
                List<String> profiles = new List<string>();
                for (int i = 1; i < temp.Count(); i++)
                {
                    profiles.Add(temp.ElementAt(i).Name);
                }
                connection.Close();
                return profiles;


            }
            else
            {
                MessageBox.Show("faild to load profiles");
                return null;
            }
            
            
        }

        public HotspotUserProfile get_Profile(String Name)
        {
            if (test_login())
            {

                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);

                var profiles = connection.LoadList<HotspotUserProfile>();  
                var profile =profiles.FirstOrDefault(c => c.Name.Equals(Name));
                connection.Close();
                if (profile == null)
                    MessageBox.Show("Profile Not Found");
                return profile;


            }
            else
            {
                MessageBox.Show("faild to load profiles");
                return null;
            }
        }

        public IEnumerable<HotspotUserProfile> get_Profiles()
        {
            if (test_login())
            {

                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);

                var profiles = connection.LoadList<HotspotUserProfile>();                
                return profiles;


            }
            else
            {
                MessageBox.Show("faild to load profiles");
                return null;
            }
        }

        public IEnumerable<HotspotActive> get_Active()
        {
            if (test_login())
            {

                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);

                var profiles = connection.LoadList<HotspotActive>();
                return profiles;


            }
            else
            {
                MessageBox.Show("faild to load profiles");
                return null;
            }
        } 

        public IEnumerable<HotspotUser> Users()
        {
            if (test_login())
            {

                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);
                var temp = connection.LoadList<HotspotUser>();                
                connection.Close();
                return temp;


            }
            else
            {
                MessageBox.Show("faild to load users");
                return null;
            }


        }

        public List<String> Users_Names()
        {
            if (test_login())
            {

                

                var temp = Users();
                List<String> profiles = new List<string>();
                for (int i = 1; i < temp.Count(); i++)
                {
                    profiles.Add(temp.ElementAt(i).Name);
                }
                connection.Close();
                return profiles;


            }
            else
            {
                MessageBox.Show("faild to load profiles");
                return null;
            }
        }

        public bool delete_user(HotspotUser user)
        {
            if (test_login())
            {
                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);
                connection.Delete<HotspotUser>(user);
                connection.Close();
                return true;


            }
            else
            {
                MessageBox.Show("faild to delete user");
                return false;
            }


        }

        public bool delete_user_name(String user)
        {
            IEnumerable<HotspotUser> u = Users();
            HotspotUser us = new HotspotUser();
            String[] data=null;
            bool f = false;
            for(int i=0;i<u.Count();i++)
            {
                if(u.ElementAt(i).Name==user)
                {
                    data = read_Generate_Profile(u.ElementAt(i).Profile);
                    us = u.ElementAt(i);
                    f = true;
                    break;
                }
            }
            
            if (f)
            {
                if (!us.Uptime.Contains("y") && !us.Uptime.Contains("w") && !us.Uptime.Contains("d") && !us.Uptime.Contains("h") && !us.Uptime.Contains("m") )
                {                                          
                        String[] s = read_connection();
                        if (s[2] == "error")
                            connection.Open(s[0], s[1], "");
                        else
                            connection.Open(s[0], s[1], s[2]);

                        connection.Delete<HotspotUser>(us);
                        connection.Close();
                        int i=Convert.ToInt32(read_counter())- Convert.ToInt32(data[7]);
                        write_counter(i.ToString());
                        return true;                    
                }
                else
                {
                    MessageBox.Show("لا يمكن مسحه لانه مستخدم ");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("لا يوجد كارت بهذا الرقم تأكد من رقم الكارت");
                return false;
            }
        }

        public bool delete_user_name_admin(String user)
        {
            IEnumerable<HotspotUser> u = Users();
            HotspotUser us = new HotspotUser();
            bool f = false;
            for (int i = 0; i < u.Count(); i++)
            {
                if (u.ElementAt(i).Name == user)
                {
                    us = u.ElementAt(i);
                    f = true;
                    break;
                }
            }
            if (f)
            {
                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);

                connection.Delete<HotspotUser>(us);
                connection.Close();                
                return true;
            }
            else
            {
                MessageBox.Show("User Not Found");
                return false;
            }
        }

        private int RandomInteger(int length)
        {
            if (length > 0)
            {
                int min = 0, max = 0;
                String temp = "", temp2 = ""; ;
                for (int i = 0; i < length; i++)
                {
                    if (i != 0)
                    {
                        temp += "0";
                        temp2 += "9";
                    }
                    else
                    {
                        temp += "1";
                        temp2 += "9";
                    }


                }
                min = Convert.ToInt32(temp);
                max = Convert.ToInt32(temp2);
                RNGCryptoServiceProvider Rand1 = new RNGCryptoServiceProvider();
                uint scale = uint.MaxValue;
                while (scale == uint.MaxValue)
                {
                    // Get four random bytes.
                    byte[] four_bytes = new byte[4];
                    Rand1.GetBytes(four_bytes);

                    // Convert that into an uint.
                    scale = BitConverter.ToUInt32(four_bytes, 0);
                }

                // Add min to the scaled difference between max and min.
                return (int)(min + (max - min) * (scale / (double)uint.MaxValue));
            }
            else
                MessageBox.Show("Length Must Be Large Than 0");
            return -1;
        }

        public bool Generate_Profiles(String Profile,String User,String Name,String letter,String Length,bool Name_Password,bool Name_Equal_Password,String Price, String LimitMB)
        {
            int counter = 0;
            if (File.Exists(Temp))
                File.Delete(Temp);
            string line = null;
            File.Copy(path2, Temp);
            using (StreamReader reader = new StreamReader(Temp))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    String[] temp = line.Split(' ');
                    if (temp[0] == Name)
                    {
                        counter++;
                        break;
                    }
                }
            }
            File.Delete(Temp);

            if (counter == 0)
            {
                String lut = get_Profile(Profile).KeepaliveTimeout, np, nep;
                if (Name_Password)
                    np = "y";
                else
                    np = "n";

                if (Name_Equal_Password)
                    nep = "y";
                else
                    nep = "n";
                try
                {
                    File.AppendAllText(path2, Name + " " + letter + " " + np + " " + nep + " " + lut + " " + Profile + " " + Length + " " + Price + " " + User + " " + LimitMB + Environment.NewLine);
                    return true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Can't Save");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Name Allready Exist");
                return false;
            }
        }

        public bool delete_Generate_Profiles(String Name)
        {
            if (File.Exists(path2))
            {
                if (File.Exists(Temp))
                    File.Delete(Temp);
                string line = null;
                File.Copy(path2,Temp);
                using (StreamReader reader = new StreamReader(Temp))
                {
                    using (StreamWriter writer = new StreamWriter(path2))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            String[] temp = line.Split(' ');
                            if (temp[0]==Name)
                                continue;

                            writer.WriteLine(line);
                        }
                    }
                }
                File.Delete(Temp);
                return true;
            }
            else
                return false;
        }

        public DataTable Generate_Profiles_Table()
        {
            List<String> Profiles = read_Generate_Profiles();
            if (Profiles != null)
            {
                var dt = new DataTable();
                List<String> headers = new List<String>();                
                headers.Add("Profile");
                headers.Add("Leter");
                headers.Add("Length");
                headers.Add("Price");
                headers.Add("Ur&Pass");
                headers.Add("Ur=Pass");

                for (int j = 0; j < headers.Count; j++)
                    dt.Columns.Add(headers[j]);

                for (int j = 0; j < Profiles.Count(); j++)
                {
                    DataRow Row = dt.NewRow();
                    String[] temp = Profiles.ElementAt(j).Split(' ');                    
                        
                    Row[headers.ElementAt(0)] = temp[5];
                    

                    if (temp[1] == "no_letter")
                        Row[headers.ElementAt(1)] = "";
                    else
                        Row[headers.ElementAt(1)] = temp[1];
                    Row[headers.ElementAt(3)] = temp[7];
                    Row[headers.ElementAt(2)] = temp[6];
                    Row[headers.ElementAt(4)] = temp[2];
                    Row[headers.ElementAt(5)] = temp[3];
                    dt.Rows.Add(Row);
                }
                return dt;
            }
            else
                return null;
        }

        public List<String> read_Generate_Profiles_Name() 
        {
            List<String> Profiles = read_Generate_Profiles();
            if (Profiles != null)
            {
                List<String> Names = new List<string>();
                for (int i = 0; i < Profiles.Count; i++)
                {
                    String[] temp;
                    temp = Profiles.ElementAt(i).Split(' ');
                    Names.Add(temp[0]);
                }
                return Names;
            }
            else
                return null;
        }

        public String[] read_Generate_Profile(String Generate_Name)
        {
            List<String> Profiles = read_Generate_Profiles();
            if (Profiles != null)
            {                
                for (int i = 0; i < Profiles.Count; i++)
                {
                    String[] temp;
                    temp = Profiles.ElementAt(i).Split(' ');
                    if(Generate_Name==temp[0])
                    {
                        return temp;
                    }
                    
                }
                return null;
            }
            else
                return null;
        }

        public List<String> read_Generate_Profiles()
        {
            if (File.Exists(path2))
            {
                string line;
                List<String> li = new List<string>();
                System.IO.StreamReader file = new System.IO.StreamReader(path2);
                while ((line = file.ReadLine()) != null)
                {
                    li.Add(line);
                }
                file.Close();
                return li;
            }
            else
            {
                File.WriteAllText(path2, "");
                return null;
            }
        }

        public void automatic_delete()
        {
            if(test_login())
            {
                IEnumerable<HotspotUser> us = Users();
                for (int i = 1; i < us.Count(); i++)
                {
                    //if(us.ElementAt(i).LimitUptime == us.ElementAt(i).Uptime )
                    if (us.ElementAt(i).Comment != "")
                    {
                        CultureInfo culture = new CultureInfo("en-US");
                        DateTime t = Convert.ToDateTime(us.ElementAt(i).Comment, culture);
                        TimeSpan ts = DateTime.Now.Subtract(t);
                        String lim = us.ElementAt(i).LimitUptime;
                        int year = (lim.IndexOf('y'));
                        int week = (lim.IndexOf('w'));
                        int day = (lim.IndexOf('d'));
                        int hour = (lim.IndexOf('h'));
                        int min = (lim.IndexOf('m'));

                        String temp = "";
                        for (int k = 0; k < lim.Length; k++)
                        {
                            if (k == year)
                            {
                                year = Convert.ToInt32(temp) * 525600;
                                temp = "";
                            }
                            else if (k == week)
                            {
                                week = Convert.ToInt32(temp) * 10080;
                                temp = "";
                            }
                            else if (k == day)
                            {
                                day = Convert.ToInt32(temp) * 1440;
                                temp = "";
                            }
                            else if (k == hour)
                            {
                                hour = Convert.ToInt32(temp) * 60;
                                temp = "";
                            }
                            else if (k == min)
                            {
                                min = Convert.ToInt32(temp);
                                break;
                            }
                            else
                            {
                                temp += lim.Substring(k, 1);
                            }

                        }
                        if (year == -1)
                            year = 0;
                        if (week == -1)
                            week = 0;
                        if (day == -1)
                            day = 0;
                        if (hour == -1)
                            hour = 0;
                        if (min == -1)
                            min = 0;
                        min += year + week + day + hour;
                        if (ts.TotalMinutes >= min)
                        {
                            String[] ss = read_connection();
                            if (ss[2] == "error")
                                connection.Open(ss[0], ss[1], "");
                            else
                                connection.Open(ss[0], ss[1], ss[2]);
                            connection.Delete<HotspotUser>(us.ElementAt(i));
                        }
                    }
                }
            }
        }

        public bool insert_user(String Generate_Name,String Number)
        {
            for (int i = 0; i < Convert.ToInt32(Number); i++)
            {                
                String[] data = read_Generate_Profile(Generate_Name);               
                String name, pass;
                List<String> un =Users_Names();
                while (true)
                {
                    if (data[2] == "y" && data[3] == "y")
                    {
                        if (data[1] == "no_letter")
                        {
                            pass = RandomInteger(Convert.ToInt32(data[6])).ToString();
                            name = RandomInteger(Convert.ToInt32(data[6])).ToString();
                        }
                        else
                        {
                            pass = data[1] + RandomInteger(Convert.ToInt32(data[6])).ToString();
                            name = data[1] + RandomInteger(Convert.ToInt32(data[6])).ToString();
                        }
                    }
                    else if (data[2] == "y" && data[3] == "n")
                    {
                        if (data[1] == "no_letter")
                            pass = RandomInteger(Convert.ToInt32(data[6])).ToString();
                        else
                            pass = data[1] + RandomInteger(Convert.ToInt32(data[6])).ToString();
                        name = data[8];                        
                    }
                    else
                    {
                        if (data[1] == "no_letter")
                            name = RandomInteger(Convert.ToInt32(data[6])).ToString();
                        else
                            name = data[1] + RandomInteger(Convert.ToInt32(data[6])).ToString();
                        pass = "";
                    }
                    if (!un.Contains(name))
                        break;

                }
                if (data[4] == "2m")
                {
                    var user = new HotspotUser()
                    {
                        Name = name,
                        Profile = data[5],
                        Password = pass                                               
                    };
                    String[] s = read_connection();
                    if (s[2] == "error")
                        connection.Open(s[0], s[1], "");
                    else
                        connection.Open(s[0], s[1], s[2]);
                    try
                    {
                        connection.Save(user);
                        Internet_Cart_Print p = new Internet_Cart_Print();
                        p.Parameters["User"].Value = name;
                        p.Parameters["Pass"].Value = pass;
                        p.Parameters["User"].Visible = false;
                        p.Parameters["Pass"].Visible = false;
                        ReportPrintTool pt = new ReportPrintTool(p);                        
                        pt.Print();
                        connection.Close();
                        int ii = Convert.ToInt32(read_counter()) + Convert.ToInt32(data[7]);
                        write_counter(ii.ToString());
                    }
                    catch (TikCommandException e)
                    {
                        MessageBox.Show(e.Message);
                        return false;
                    }
                }
                else
                {
                    var user = new HotspotUser()
                    {
                        Name = name,
                        Profile = data[5],
                        LimitUptime = data[4],
                        Password = pass,
                        LimitBytesTotal = long.Parse(data[9]+"000000")
                    };
                    String[] s = read_connection();
                    if (s[2] == "error")
                        connection.Open(s[0], s[1], "");
                    else
                        connection.Open(s[0], s[1], s[2]);
                    try
                    {
                        connection.Save(user);
                        Internet_Cart_Print p = new Internet_Cart_Print();
                        p.Parameters["User"].Value = name;
                        p.Parameters["Pass"].Value = pass;
                        p.Parameters["User"].Visible = false;
                        p.Parameters["Pass"].Visible = false;
                        ReportPrintTool pt = new ReportPrintTool(p);                        
                        pt.Print();
                        connection.Close();
                        int ii = Convert.ToInt32(read_counter()) + Convert.ToInt32(data[7]);
                        write_counter(ii.ToString());
                    }
                    catch (TikCommandException e)
                    {
                        MessageBox.Show(e.Message);
                        return false;
                    }
                }
                          
            }
            
            return true;
        }

        public bool Insert_profile(String Name, String Users, String up_limit, String down_limit, String keep_alive)
        {
            var profile=new HotspotUserProfile();
            if (test_login())
            {
                if (keep_alive.Contains("error"))
                {
                     profile = new HotspotUserProfile()
                    {
                        Name = Name,
                        MacCookieTimeout = "3d",
                        SharedUsers = Users,
                        RateLimit = up_limit + "k/" + down_limit + "k",
                        AddMacCookie = true,
                         KeepaliveTimeout = "00:02:00",
                         OpenStatusPage = "always",                        
                        StatusAutorefresh = "00:01:00",                        
                    };
                }
                else 
                {
                    String text = "{@  :local time [/system clock get time]@  :local date [/system clock get date]@:if ( [ /ip hotspot user get $user comment ] = \"\") do={[ /ip hotspot user set $user comment=\"$date $time\"];}@}";
                    text = text.Replace("@", "" + System.Environment.NewLine);
                    profile = new HotspotUserProfile()
                    {
                        Name = Name,
                        MacCookieTimeout = "3d",
                        SharedUsers = Users,
                        RateLimit = up_limit + "k/" + down_limit + "k",
                        AddMacCookie = true,
                        OpenStatusPage = "always",
                        KeepaliveTimeout = keep_alive,
                        StatusAutorefresh = "00:01:00",
                        OnLogin = text
                    };
                }

                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);
                try 
                {
                    connection.Save(profile);
                }
                catch (TikCommandException e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                connection.Close();
                return true;
            }
            else
            {
                return false;
            }            
            
        }

        public bool delete_profile(String Name)
        {
            if (test_login())
            {
                HotspotUserProfile profile = get_Profile(Name);
                String[] s = read_connection();
                if (s[2] == "error")
                    connection.Open(s[0], s[1], "");
                else
                    connection.Open(s[0], s[1], s[2]);
                connection.Delete<HotspotUserProfile>(profile);
                connection.Close();
                return true;


            }
            else
            {
                MessageBox.Show("faild to delete user");
                return false;
            }
        }

        public DataTable Profile_Table()
        {
            if (test_login())
            {
                var dt = new DataTable();
                List<String> headers = new List<String>();
                var data = get_Profiles();
                headers.Add("Profile Name");
                headers.Add("Users");
                //headers.Add("Up/Down Kb");
                headers.Add("Limit Time");
                for (int j = 0; j < headers.Count; j++)
                    dt.Columns.Add(headers[j]);
                
                for (int j = 0; j < data.Count(); j++)
                {
                    DataRow Row = dt.NewRow();
                    Row[headers.ElementAt(0)] = data.ElementAt(j).Name;
                    Row[headers.ElementAt(1)] = data.ElementAt(j).SharedUsers;
                    //Row[headers.ElementAt(2)] = data.ElementAt(j).RateLimit;
                    Row[headers.ElementAt(2)] = data.ElementAt(j).KeepaliveTimeout;
                    dt.Rows.Add(Row);
                }
                return dt;
            }
            else
            {                
                return null;
            }
        }

        public DataTable Users_Table()
        {
            if (test_login())
            {
                var dt = new DataTable();
                List<String> headers = new List<String>();
                var data = Users();
                headers.Add("Name");
                headers.Add("Profile");
                //headers.Add("Up/Down Kb");
                headers.Add("Up Time");
                for (int j = 0; j < headers.Count; j++)
                    dt.Columns.Add(headers[j]);
                
                for (int j = 0; j < data.Count(); j++)
                {
                    DataRow Row = dt.NewRow();
                    Row[headers.ElementAt(0)] = data.ElementAt(j).Name;
                    Row[headers.ElementAt(1)] = data.ElementAt(j).Profile;
                    //Row[headers.ElementAt(2)] = data.ElementAt(j).RateLimit;
                    Row[headers.ElementAt(2)] = data.ElementAt(j).Uptime;
                    dt.Rows.Add(Row);
                }
                return dt;
            }
            else
            {                
                return null;
            }
        }

        public DataTable Active_Users_Table()
        {
            if (test_login())
            {
                var dt = new DataTable();
                List<String> headers = new List<String>();
                var data = get_Active();
                headers.Add("Name");
                headers.Add("Up Time");
                //headers.Add("Up/Down Kb");
                headers.Add("Session Time");
                for (int j = 0; j < headers.Count; j++)
                    dt.Columns.Add(headers[j]);

                for (int j = 0; j < data.Count(); j++)
                {
                    DataRow Row = dt.NewRow();
                    Row[headers.ElementAt(0)] = data.ElementAt(j).UserName;
                    Row[headers.ElementAt(1)] = data.ElementAt(j).UpTime;
                    //Row[headers.ElementAt(2)] = data.ElementAt(j).RateLimit;
                    Row[headers.ElementAt(2)] = data.ElementAt(j).SessionTimeLeft;
                    dt.Rows.Add(Row);
                }
                return dt;
            }
            else
            {
                return null;
            }
        }        

    }
}
