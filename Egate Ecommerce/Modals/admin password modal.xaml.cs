using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using bolt5.ModalWpf;

namespace Egate_Ecommerce.Modals
{
    /// <summary>
    /// Interaction logic for admin_password_modal.xaml
    /// </summary>
    public partial class admin_password_modal : UserControl, IModalClosing
    {
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof(Message), typeof(string), typeof(admin_password_modal));
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public admin_password_modal()
        {
            InitializeComponent();
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Yes)
            {
                bool flag = passwordTxt.Text == Helpers.GetAdminPassword();
                if (!flag)
                {
                    e.Cancel = true;
                    MessageBox.Show("Password is NOT correct", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
