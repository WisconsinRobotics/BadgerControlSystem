using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using BadgerControlModule.Views;

namespace BadgerControlSystem
{
    /// <summary>
    /// The Shell class is the overarching class to display external GUI modules.
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void ShellClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
