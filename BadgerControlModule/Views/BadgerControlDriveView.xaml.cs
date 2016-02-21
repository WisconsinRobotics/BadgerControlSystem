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
using BadgerControlModule.ViewModels;
using BadgerControlModule.Services;

namespace BadgerControlModule.Views
{
    /// <summary>
    /// Interaction logic for BadgerControlDrive.xaml
    /// </summary>
    public partial class BadgerControlDriveView : UserControl
    {
        public BadgerControlDriveView()
        {
            InitializeComponent();
            DriveModeComboBox.SelectedIndex = 0;
            BadgerControlDriveViewModel viewModel = new BadgerControlDriveViewModel(ApplicationService.Instance.EventAggregator);
            DataContext = viewModel;
        }
    }
}
