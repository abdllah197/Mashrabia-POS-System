using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mashrabia
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private Database database = new Database();
        private databaseOrderSys database_OrderSys = new databaseOrderSys();
        private Internet_Carts internet_carts = new Internet_Carts();
        public Settings()
        {
            InitializeComponent();
            dg.CellStyle = (Style)TryFindResource("DataGridCellCentered");
            dg.ColumnHeaderStyle = (Style)TryFindResource("DataGridColumnHeader");

            coded.Items.Add("تكويد اصناف المخزن");
            coded.Items.Add("تكويد وحدات اصناف المخزن");
            coded.Items.Add("تكويد مجموعات");
            coded.Items.Add("تكويد اصناف المجموعات");
            coded.Items.Add("تكويد استلام اصناف");
            coded.Items.Add("تكويد موردين");
            coded.SelectedIndex = 0;

            List<String> combo = database_OrderSys.Name_Of_Tables("Category");
            for (int i = 0; i < combo.Count; i++)
            {
                category.Items.Add(combo.ElementAt(i));
            }

        }

        private void Coded_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (coded.SelectedIndex == 0)
            {
                dg.ItemsSource = database.ViewTable("Products").DefaultView;
                mowaerden.Visibility = Visibility.Hidden;
                category_s.Visibility = Visibility.Hidden;
                griditems.Visibility = Visibility.Hidden;
                threeonone.Visibility = Visibility.Visible;
                Stock_title.Content = "اسم الصنف";
                category.Items.Clear();
                category_item.Text = "";
                category_price.Text = "";
            }
            else if (coded.SelectedIndex == 1)
            {
                dg.ItemsSource = database.ViewTable("Units").DefaultView;
                mowaerden.Visibility = Visibility.Hidden;
                category_s.Visibility = Visibility.Hidden;
                griditems.Visibility = Visibility.Hidden;
                threeonone.Visibility = Visibility.Visible;
                Stock_title.Content = "الوحدة";
                category.Items.Clear();
                category_item.Text = "";
                category_price.Text = "";
            }
            else if (coded.SelectedIndex == 2)
            {
                view();
                mowaerden.Visibility = Visibility.Hidden;
                category_s.Visibility = Visibility.Hidden;
                griditems.Visibility = Visibility.Hidden;
                threeonone.Visibility = Visibility.Visible;
                Stock_title.Content = "اسم المجموعة";
                category.Items.Clear();
                category_item.Text = "";
                category_price.Text = "";
            }
            else if (coded.SelectedIndex == 3)
            {
                mowaerden.Visibility = Visibility.Hidden;
                threeonone.Visibility = Visibility.Hidden;
                griditems.Visibility = Visibility.Hidden;
                category_s.Visibility = Visibility.Visible;
                dg.ItemsSource = null;
                category.Items.Clear();
                List<String> combo = database_OrderSys.Name_Of_Tables("Category");
                for (int i = 0; i < combo.Count; i++)
                {
                    category.Items.Add(combo.ElementAt(i));
                }
                Stock_in.Text = "";
            }
            else if (coded.SelectedIndex == 4)
            {
                mowaerden.Visibility = Visibility.Hidden;
                threeonone.Visibility = Visibility.Hidden;
                category_s.Visibility = Visibility.Hidden;
                griditems.Visibility = Visibility.Visible;
                dg.ItemsSource = database_OrderSys.ViewTable("Items", "Shift").DefaultView;
                category_1.Items.Clear();
                item_1.Items.Clear();
                List<String> combo = database_OrderSys.Name_Of_Tables("Category");
                for (int i = 0; i < combo.Count; i++)
                {
                    category_1.Items.Add(combo.ElementAt(i));
                }

            }
            else if (coded.SelectedIndex == 5)
            {
                threeonone.Visibility = Visibility.Hidden;
                category_s.Visibility = Visibility.Hidden;
                griditems.Visibility = Visibility.Hidden;
                mowaerden.Visibility = Visibility.Visible;
                dg.ItemsSource = database.Mowarden_View(0).DefaultView;
                mowared_name.Text = "";
                mowared_notes.Text = "";
                mowared_number.Text = "";
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void In_Click()
        {
            if (coded.SelectedIndex != -1 && (coded.SelectedIndex == 1 || coded.SelectedIndex == 2 || coded.SelectedIndex == 0))
                if (Stock_in.Text != "")
                {
                    if (coded.SelectedIndex == 0)
                    {
                        database.insert_s("Products", Stock_in.Text);
                        dg.ItemsSource = database.ViewTable("Products").DefaultView;
                        Stock_in.Text = "";
                    }
                    else if (coded.SelectedIndex == 1)
                    {
                        database.insert_s("Units", Stock_in.Text);
                        dg.ItemsSource = database.ViewTable("Units").DefaultView;
                        Stock_in.Text = "";
                    }
                    else if (coded.SelectedIndex == 2)
                    {
                        database.insert_category(Stock_in.Text);
                        category.Items.Clear();
                        List<String> combo = database_OrderSys.Name_Of_Tables("Category");
                        for (int j = 0; j < combo.Count; j++)
                        {
                            category.Items.Add(combo.ElementAt(j));
                        }
                        view();
                        Stock_in.Text = "";
                    }
                }
                else
                    MessageBox.Show("Please Fill All Fields");
            else if (coded.SelectedIndex == 4)
            {
                if (category_1.SelectedIndex != -1 && item_1.SelectedIndex != -1)
                {
                    database_OrderSys.Total_Items(category_1.SelectedItem.ToString(), item_1.SelectedItem.ToString());
                    dg.ItemsSource = database_OrderSys.ViewTable("Items", "Shift").DefaultView;
                    category_1.SelectedIndex = -1;
                    item_1.Items.Clear();
                }
                else
                    MessageBox.Show("Please Fill All Fields");
            }
            else if (coded.SelectedIndex == 5)
            {
                if (mowared_name.Text != "" && mowared_number.Text != "")
                {
                    database.Mowarden_insert(mowared_name.Text, mowared_number.Text, mowared_notes.Text);
                    mowared_name.Text = "";
                    mowared_notes.Text = "";
                    mowared_number.Text = "";
                    dg.ItemsSource = database.Mowarden_View(0).DefaultView;
                }
            }
            else
            {
                if (category.SelectedIndex != -1)
                {
                    if (category_item.Text != "" && category_price.Text != "." && category_price.Text != "")
                    {
                        database.insert_category_items(category.SelectedItem.ToString(), category_item.Text, category_price.Text);
                        category_item.Text = "";
                        category_price.Text = "";
                        dg.ItemsSource = database.ViewTable_category(category.SelectedItem.ToString()).DefaultView;
                    }
                    else
                        MessageBox.Show("Please Fill All Fields");
                }
                else
                    MessageBox.Show("Select Category First");
            }
        }
        private void Setings_in_Click(object sender, RoutedEventArgs e)
        {
            In_Click();
        }

        private void Dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                object i = dg.SelectedItem;
                string id = "error", Name = "error";
                if (coded.SelectedIndex != 3 && coded.SelectedIndex != 4 && coded.SelectedIndex != 5)
                {
                    if (dg.SelectedCells.Count != 0)
                    {
                        id = (dg.SelectedCells[1].Column.GetCellContent(i) as TextBlock).Text;
                        Name = (dg.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                    }
                }
                else if (coded.SelectedIndex == 3)
                {
                    if (dg.SelectedCells.Count != 0)
                    {
                        id = (dg.SelectedCells[2].Column.GetCellContent(i) as TextBlock).Text;
                    }
                }
                else if (coded.SelectedIndex == 4)
                {
                    if (dg.SelectedCells.Count != 0)
                    {
                        id = (dg.SelectedCells[2].Column.GetCellContent(i) as TextBlock).Text;
                    }
                }

                else if (coded.SelectedIndex == 5)
                {
                    if (dg.SelectedCells.Count != 0)
                    {
                        id = (dg.SelectedCells[3].Column.GetCellContent(i) as TextBlock).Text;
                    }
                }

                if (!id.Equals("error"))
                {
                    if (MessageBox.Show("Are You Sure To Delete This Record", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (coded.SelectedIndex == 0)
                        {
                            database.delete_s("Products", id);
                            dg.ItemsSource = database.ViewTable("Products").DefaultView;
                        }
                        else if (coded.SelectedIndex == 1)
                        {
                            database.delete_s("Units", id);
                            dg.ItemsSource = database.ViewTable("Units").DefaultView;
                        }
                        else if (coded.SelectedIndex == 2)
                        {
                            database.delete_category(Name);
                            category.Items.Clear();
                            List<String> combo = database_OrderSys.Name_Of_Tables("Category");
                            for (int j = 0; j < combo.Count; j++)
                            {
                                category.Items.Add(combo.ElementAt(j));
                            }
                            view();
                        }
                        else if (coded.SelectedIndex == 3)
                        {
                            database.delete_category_item(category.SelectedItem.ToString(), id);
                            dg.ItemsSource = database.ViewTable_category(category.SelectedItem.ToString()).DefaultView;
                        }
                        else if (coded.SelectedIndex == 4)
                        {
                            database_OrderSys.delete_item_Total(id, 1);
                            database_OrderSys.delete_item_Total(id, 0);
                            dg.ItemsSource = database_OrderSys.ViewTable("Items", "Shift").DefaultView;
                        }
                        else if (coded.SelectedIndex == 5)
                        {
                            database.Mowarden_delete(id);
                            dg.ItemsSource = database.Mowarden_View(0).DefaultView;
                        }
                    }
                }
            }
        }

        private void view()
        {

            var dt = new DataTable();
            List<String> headers = new List<String>();
            List<String> data = new List<String>();
            data = database_OrderSys.Name_Of_Tables("Category");
            headers.Add("اسم المجموعة");
            headers.Add("كود");
            for (int j = 0; j < headers.Count; j++)
                dt.Columns.Add(headers[j]);

            for (int j = 0; j < data.Count; j++)
            {
                DataRow Row = dt.NewRow();
                Row[headers.ElementAt(1)] = j + 1;
                Row[headers.ElementAt(0)] = data.ElementAt(j);
                dt.Rows.Add(Row);
            }

            dg.ItemsSource = dt.DefaultView;
        }

        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (category.Items != null)
                if (category.Items.Count > 0)
                    dg.ItemsSource = database.ViewTable_category(category.SelectedItem.ToString()).DefaultView;
        }

        private void Category_price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }

        private void Stock_in_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                In_Click();
        }

        private void Category_item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                In_Click();
        }

        private void Category_price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                In_Click();
        }

        private void click1()
        {
            String old = database.get_admin();
            if (Password.Password == Confirm_Password.Password && old_password.Password == old)
            {
                database.set_admin(Password.Password.ToString());
                old_password.Password = "";
                Password.Password = "";
                Confirm_Password.Password = "";
                MessageBox.Show("Done");
            }
            else
            {
                old_password.Password = "";
                Password.Password = "";
                Confirm_Password.Password = "";
                MessageBox.Show("Please cheack Password Fields");

            }
        }


        private void Category_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (category_1.SelectedItem != null)
            {
                item_1.Items.Clear();
                List<String> combo = database_OrderSys.Category_Items(category_1.SelectedItem.ToString(), "الاسم");
                for (int i = 0; i < combo.Count; i++)
                {
                    item_1.Items.Add(combo.ElementAt(i));
                }
            }
        }

        private void Item_1_KeyDown(object sender, KeyEventArgs e)
        {
            In_Click();
        }

        private void Category_1_KeyDown(object sender, KeyEventArgs e)
        {
            In_Click();
        }

        private void Old_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                click1();
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                click1();
        }

        private void Confirm_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                click1();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Mowared_number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void Dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));
        }
        public void Save_Microtik_Login_()
        {
            if (Host.Text != "" && User.Text != "")
            {
                bool tt = internet_carts.write_connection(Host.Text, User.Text, Pass.Text);

                if (tt)
                    MessageBox.Show("Saved Successfully");
            }
            else
                MessageBox.Show("Fill all fields First");
        }

        private void Save_Microtik_Login(object sender, RoutedEventArgs e)
        {
            Save_Microtik_Login_();
        }

        private void Save_Microtik_Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Save_Microtik_Login_();
        }

        private void Hotspot(object sender, RoutedEventArgs e)
        {
            if (internet_carts.test_login())
            {
                Hotspot h = new Hotspot();
                h.ResizeMode = ResizeMode.NoResize;
                h.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                h.ShowDialog();
            }
        }
        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        private void order_id_change_Click(object sender, RoutedEventArgs e)
        {
            InputDialog input_dialog = new InputDialog("ادخل الكود الجديد", "");
            if (input_dialog.ShowDialog() == true)
            {
                if (IsDigitsOnly(input_dialog.Answer))
                {
                    if (input_dialog.Answer.Length < 8)
                    {
                        Properties.Settings.Default.order_id = input_dialog.Answer;
                        Properties.Settings.Default.Save();
                        Properties.Settings.Default.Reload();
                        MessageBox.Show("تم بنجاح");
                    }
                    else
                        MessageBox.Show("الحد الاقصى للكود هو 7 خانات");
                }
                else
                    MessageBox.Show("يرجى ادخال ارقام فقط");

            }

        }
    }
}
