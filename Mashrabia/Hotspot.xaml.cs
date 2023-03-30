using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using tik4net;
using tik4net.Objects;
using tik4net.Objects.Ip.Hotspot;

namespace Mashrabia
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Hotspot : Window
    {
        private Internet_Carts internet_carts = new Internet_Carts();

        public Hotspot()
        {
            InitializeComponent();
            View.Items.Add("Users");
            View.Items.Add("Active Users");
            
            Op.Items.Add("Create Profile");
            Op.Items.Add("Create Generator");
            Op.Items.Add("Manage Users");            
            Op.SelectedIndex = 0;
            
            Profiles.ItemsSource = internet_carts.profiles();
            if (Profiles.ItemsSource != null)
                Profiles.SelectedIndex = 0;
            uap.IsChecked = false;
            uep.IsChecked = false;
            uep.IsEnabled = false;
            User.IsEnabled = false;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Op_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Op.SelectedIndex == 0)
            {
                profie_create.Visibility = Visibility.Visible;
                Generate_prof.Visibility = Visibility.Hidden;
                View_grid.Visibility = Visibility.Hidden;
                OK.Visibility = Visibility.Visible;
                dg.ItemsSource = internet_carts.Profile_Table().DefaultView;
                Profile.Text = "";
                Users.Text = "";
                up.Text = "";
                Down.Text = "";
                Days.Text = "";
                Hours.Text = "";
                Minutes.Text = "";
            }
            else if (Op.SelectedIndex == 1)
            {
                profie_create.Visibility = Visibility.Hidden;
                Generate_prof.Visibility = Visibility.Visible;
                View_grid.Visibility = Visibility.Hidden;
                OK.Visibility = Visibility.Visible;

                Profiles.ItemsSource = internet_carts.profiles();
                if (Profiles.ItemsSource != null)
                    Profiles.SelectedIndex = 0;

                if (internet_carts.Generate_Profiles_Table() != null)
                    dg.ItemsSource = internet_carts.Generate_Profiles_Table().DefaultView;
                else
                    dg.ItemsSource=null;
                
                Letter.Text = "";
                Length.Text = "";
                Price.Text = "";
                uap.IsChecked = false;
                uep.IsChecked = false;
                uep.IsEnabled = false;
            }
            else
            {
                profie_create.Visibility = Visibility.Hidden;
                Generate_prof.Visibility = Visibility.Hidden;
                View_grid.Visibility = Visibility.Visible;
                OK.Visibility = Visibility.Hidden;
                View.SelectedIndex = 0;
                dg.ItemsSource = internet_carts.Users_Table().DefaultView;

            }
        }

        private void Users_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));

        }

        private void limit_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));

        }

        public void button()
        {
            if (Op.SelectedIndex == 0)
            {
                String Min="";
                if (Days.Text != "" && Hours.Text != "" && Minutes.Text != "")
                {
                    if (Convert.ToInt32(Days.Text) == 0 && Convert.ToInt32(Hours.Text) == 0 && Convert.ToInt32(Minutes.Text) == 0)
                        Min = "error";
                    else
                        Min = Minutes.Text;
                }
                if (Days.Text == "")
                    Days.Text = "0";
                if (Profile.Text != "" && Users.Text != "" && up.Text != "" && Down.Text != "" && Hours.Text != "" && Minutes.Text != "")
                {
                    if (Convert.ToInt32(Users.Text) != 0)
                    {                        
                        if (internet_carts.Insert_profile(Profile.Text, Users.Text, up.Text, Down.Text, Days.Text + "d " + Hours.Text + ":" + Min + ":00"))
                        {
                            Profile.Text = "";
                            Users.Text = "";
                            up.Text = "";
                            Down.Text = "";
                            Days.Text = "";
                            Hours.Text = "";
                            Minutes.Text = "";
                            dg.ItemsSource = internet_carts.Profile_Table().DefaultView;
                            MessageBox.Show("Done");
                        }
                    }
                    else
                        MessageBox.Show("Users Must Be Bigger Than 1");
                }
                else
                    MessageBox.Show("Fill All Fields");
            }
            else if (Op.SelectedIndex == 1)
            {
                String l;
                if (Letter.Text == "")
                    l = "no_letter";
                else
                    l = Letter.Text;

                if (Profiles.SelectedItem != null && Length.Text != "" && Price.Text!="" && LimitMB.Text!="")
                {
                    if (Convert.ToInt32(Length.Text) <= 12)
                    {
                        if (uap.IsChecked == true && uep.IsChecked == true)
                        {


                            if (internet_carts.Generate_Profiles(Profiles.SelectedItem.ToString(), "u=p", Profiles.SelectedItem.ToString(), l, Length.Text, true, true,Price.Text,LimitMB.Text))
                            {
                                User.Text = "";
                                Letter.Text = "";
                                Length.Text = "";
                                Price.Text = "";
                                LimitMB.Text = "";
                                uap.IsChecked = false;
                                uep.IsChecked = false;
                                uep.IsEnabled = false;
                                dg.ItemsSource = internet_carts.Generate_Profiles_Table().DefaultView;
                                MessageBox.Show("Done");
                            }
                            else
                                MessageBox.Show("Error Try Again");
                        }
                        else if (uap.IsChecked == true && uep.IsChecked == false)
                        {
                            if (internet_carts.Generate_Profiles(Profiles.SelectedItem.ToString(),User.Text, Profiles.SelectedItem.ToString(), l, Length.Text, true, false,Price.Text, LimitMB.Text))
                            {
                                User.Text = "";
                                Letter.Text = "";
                                Length.Text = "";
                                Price.Text = "";
                                LimitMB.Text = "";
                                uap.IsChecked = false;
                                uep.IsChecked = false;
                                uep.IsEnabled = false;
                                dg.ItemsSource = internet_carts.Generate_Profiles_Table().DefaultView;
                                MessageBox.Show("Done");
                            }
                            else
                                MessageBox.Show("Error Try Again");
                        }
                        else if (uap.IsChecked == false)
                        {
                            if (internet_carts.Generate_Profiles(Profiles.SelectedItem.ToString(), "u=p", Profiles.SelectedItem.ToString(), l, Length.Text, false, false, Price.Text, LimitMB.Text))
                            {
                                User.Text = "";
                                Letter.Text = "";
                                Length.Text = "";
                                LimitMB.Text = "";
                                uap.IsChecked = false;
                                uep.IsChecked = false;
                                uep.IsEnabled = false;
                                Price.Text = "";
                                dg.ItemsSource = internet_carts.Generate_Profiles_Table().DefaultView;
                                MessageBox.Show("Done");
                            }
                            else
                                MessageBox.Show("Error Try Again");
                        }
                    }
                    else
                        MessageBox.Show("Length Must Be 12 Or Less");

                }
                else
                    MessageBox.Show("Fill All Fields");
            }
            else
            {

            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            button();
        }

        private void dg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                object i = dg.SelectedItem;
                string Name;
                if (Op.SelectedIndex == 0)
                {
                    if (dg.SelectedCells.Count != 0)
                    {
                        Name = (dg.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                        if (MessageBox.Show("Are You Sure To Delete " + Name, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            if (internet_carts.delete_profile(Name))
                            {
                                while (dg.SelectedItems.Count == 1)
                                {
                                    DataRowView drv = (DataRowView)dg.SelectedItem;
                                    drv.Row.Delete();
                                }
                                MessageBox.Show("Done");
                            }
                        }
                    }
                }
                else if (Op.SelectedIndex == 1)
                {
                    if (dg.SelectedCells.Count != 0)
                    {
                        Name = (dg.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                        if (MessageBox.Show("Are You Sure To Delete " + Name, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            if (internet_carts.delete_Generate_Profiles(Name))
                            {
                                while (dg.SelectedItems.Count == 1)
                                {
                                    DataRowView drv = (DataRowView)dg.SelectedItem;
                                    drv.Row.Delete();
                                }
                                MessageBox.Show("Done");
                            }
                        }
                    }
                }
                else
                {
                    if(View.SelectedIndex==0)
                    {
                        if (dg.SelectedCells.Count != 0)
                        {
                            Name = (dg.SelectedCells[0].Column.GetCellContent(i) as TextBlock).Text;
                            if (MessageBox.Show("Are You Sure To Delete " + Name, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                            {
                                if (internet_carts.delete_user_name_admin(Name))
                                {
                                    while (dg.SelectedItems.Count == 1)
                                    {
                                        DataRowView drv = (DataRowView)dg.SelectedItem;
                                        drv.Row.Delete();
                                    }
                                    MessageBox.Show("Done");
                                }
                            }
                        }
                    }
                }
            }
        }
        
        private void dg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.CellStyle = (Style)(TryFindResource("DataGridCellCentered"));
            e.Column.HeaderStyle = (Style)(TryFindResource("DataGridColumnHeader"));

        }

        private void Generate_Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button();
            }
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            if (View.SelectedIndex == 0)
            {
                dg.ItemsSource = internet_carts.Users_Table().DefaultView;
            }
            else
            {
                dg.ItemsSource = internet_carts.Active_Users_Table().DefaultView;
            }
        }

        private void View_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(View.SelectedIndex==0)
            {
                dg.ItemsSource = internet_carts.Users_Table().DefaultView;
            }
            else
            {
                dg.ItemsSource = internet_carts.Active_Users_Table().DefaultView;
            }
        }

        private void uep_Checked(object sender, RoutedEventArgs e)
        {
            if (uep.IsChecked == true)
                User.IsEnabled = false;
        }

        private void uep_Unchecked(object sender, RoutedEventArgs e)
        {
            if (uep.IsChecked == false)
                User.IsEnabled = true;
        }

        private void uap_Unchecked(object sender, RoutedEventArgs e)
        {

            if (uap.IsChecked == false)
            {
                User.IsEnabled = false;
                uep.IsEnabled = false;
            }
        }

        private void uap_Checked(object sender, RoutedEventArgs e)
        {

            if (uap.IsChecked == true)
            {
                User.IsEnabled = true;
                uep.IsEnabled = true;
            }
        }
    }
}
