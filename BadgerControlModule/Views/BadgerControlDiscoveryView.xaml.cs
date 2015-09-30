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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;

namespace BadgerControlModule.Views
{
    /// <summary>
    /// Interaction logic for BadgerControlDiscoveryView.xaml
    /// </summary>
    public partial class BadgerControlDiscoveryView : UserControl
    {
        private static BadgerControlDiscoveryViewModel viewModel;

        public BadgerControlDiscoveryView()
        {
            InitializeComponent();
            viewModel = new BadgerControlDiscoveryViewModel(ApplicationService.Instance.EventAggregator);
            DataContext = viewModel;
        }
    }
}
