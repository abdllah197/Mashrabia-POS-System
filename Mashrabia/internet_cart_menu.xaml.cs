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
    public partial class internet_cart_menu : Window
    {
        private Database database = new Database();
        private databaseOrderSys database_OrderSys = new databaseOrderSys();
        private Internet_Carts internet_carts = new Internet_Carts();
        public internet_cart_menu()
        {
            InitializeComponent();
            CartOP.Items.Add("طبع كروت");
            CartOP.Items.Add("مسح كارت");
            CartOP.Items.Add("تصفير الاجمالى");
            Profiles.ItemsSource = internet_carts.read_Generate_Profiles_Name();
            if(Profiles.ItemsSource!=null)
                Profiles.SelectedIndex = 0;
            CartOP.SelectedIndex = 0;
            numbersCounter.Content = internet_carts.read_counter()+"EGP";
        }
        
        private void setings_in_Click_1(object sender, RoutedEventArgs e)
        {
            if(CartOP.SelectedIndex==0)
            {
                if (Profiles.SelectedItem != null)
                {
                    if (CardsNUMIN.Text != "0")
                    {
                        if (CardsNUMIN.Text == "")
                            CardsNUMIN.Text = "1";
                        internet_carts.insert_user(Profiles.SelectedItem.ToString(), CardsNUMIN.Text);
                        numbersCounter.Content = internet_carts.read_counter() + "EGP";
                        CardsNUMIN.Text = "";
                    }
                    else
                        MessageBox.Show("Card Number Must Be Bigger Than 0");
                }
                else
                    MessageBox.Show("Fill All Fields");
            }
            else if(CartOP.SelectedIndex==1)
            {
                if(CardNUM.Text!="")
                {
                    if (internet_carts.delete_user_name(CardNUM.Text))
                    {
                        numbersCounter.Content = internet_carts.read_counter() + "EGP";
                        CardNUM.Text = "";
                    }
                }
                else
                    MessageBox.Show("Fill All Fields");
            }
            else
            {
                pin_dialog dialoge = new pin_dialog("Enter PIN","");
                if (dialoge.ShowDialog() == true)
                {
                    String[] pin=internet_carts.read_connection();
                    
                    if (pin[2]==dialoge.Answer)
                    {
                        internet_carts.write_counter("0");
                        numbersCounter.Content = "0" + "EGP";
                    }
                    else
                        MessageBox.Show("Wrong PIN");
                }
                

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CartOP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CartOP.SelectedIndex == 0)
            {
                cardCreateGrid.Visibility = Visibility.Visible;
                cardDeleteGrid.Visibility = Visibility.Hidden;
                setings_in.Content = "طبع الكروت";
                CardsNUMIN.Text = "";
            }
            else if (CartOP.SelectedIndex == 1)
            {
                cardDeleteGrid.Visibility = Visibility.Visible;
                cardCreateGrid.Visibility = Visibility.Hidden;
                setings_in.Content = "مسح الكارت";
                CardNUM.Text = "";
            }
            else
            {
                cardCreateGrid.Visibility = Visibility.Hidden;
                cardDeleteGrid.Visibility = Visibility.Hidden;
                setings_in.Content = "تصفير الاجمالى";
            }
        }

        private void CardsNUMIN_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));

        }
    }
}
