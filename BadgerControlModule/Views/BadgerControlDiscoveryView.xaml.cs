
using System.Windows.Controls;

using BadgerControlModule.ViewModels;
using BadgerControlModule.Services;

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
