using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mashrabia
{
    public partial class Password : Window
    {
        public Password(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            lblQuestion.Content = question;
            password.Password = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.password.Focus();
        }

        public string Answer
        {
            get { return password.Password; }
        }
    }
}
