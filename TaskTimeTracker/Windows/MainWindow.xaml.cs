using Microsoft.Win32;
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
using TaskTimeTracker.ViewModels;

namespace TaskTimeTracker.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel();

            DataContext = _vm;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "Choose task file to create";
            saveFileDialog.Filter = "Task Time Tracker file|*.ttt";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var tt = TaskTimeViewModel.CreateNewFile(saveFileDialog.FileName);
                    _vm.ActiveTaskTime = tt;
                    _vm.ActiveTaskTime.TriggerNotifyAll();
                }
                catch
                { }
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Open task file";
            openFileDialog.Filter = "Task Time Tracker file|*.ttt";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var tt = TaskTimeViewModel.FromFile(openFileDialog.FileName);
                    _vm.ActiveTaskTime = tt;
                    _vm.ActiveTaskTime.TriggerNotifyAll();
                }
                catch
                { }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _vm.StartSession();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _vm.EndSession();
        }
    }
}
