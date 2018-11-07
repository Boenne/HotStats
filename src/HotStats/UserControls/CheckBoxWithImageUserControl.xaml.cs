using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;

namespace HotStats.UserControls
{
    /// <summary>
    ///     Interaction logic for CheckBoxWithImageUserControl.xaml
    /// </summary>
    public partial class CheckBoxWithImageUserControl : UserControl
    {
        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register("MouseDownCommand", typeof(Func<string, Task>),
                typeof(CheckBoxWithImageUserControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(CheckBoxWithImageUserControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(string), typeof(CheckBoxWithImageUserControl),
                new PropertyMetadata(null));

        public CheckBoxWithImageUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Func<string, Task> MouseDownCommand
        {
            get => (Func<string, Task>) GetValue(MouseDownCommandProperty);
            set => SetValue(MouseDownCommandProperty, value);
        }

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public string ImagePath
        {
            get => (string) GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public RelayCommand MouseDownMaster => new RelayCommand(async () => MouseDown());

        private async Task MouseDown()
        {
            Check.IsChecked = !Check.IsChecked;
            await MouseDownCommand(Label);
        }
    }
}