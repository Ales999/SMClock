using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AClockLibrary.Views
{
    using Dnw.AnalogClock.Entities;

    /// <summary>
    /// Логика взаимодействия для AClockView.xaml
    /// </summary>
    public partial class AClockView : Window
    {
        public AClockView()
        {
            InitializeComponent();
            //this.DataContext = this;
            try
            {
                var tryIcon = BitmapFrame.Create(new Uri("pack://application:,,,/CommonViews;component/Icons/btn884.ico"));
                if (tryIcon != null)
                        Icon = tryIcon;

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                if(e.InnerException != null)
                    Console.Error.WriteLine(e.InnerException.Message);
            }
            //this.Icon = BitmapFrame.Create(new Uri("pack://application:,,,/btn884.ico", UriKind.RelativeOrAbsolute));
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //
            this.ShowInTaskbar = false;

        }

        public new void Focus()
        {
            SystemCommands.RestoreWindow(this);
            this.WindowState = WindowState.Normal;
            //this.Activate();
            //base.Focus();

            Dispatcher.BeginInvoke(new Action(delegate
            {
                this.Activate();
            }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);

        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {

            AClockData clock = new AClockData();

            clock.BackColor = new SolidColorBrush(Colors.Black);
            clock.TickColor = new SolidColorBrush(Colors.White);
            clock.TickThicknessDivisor = 130;
            clock.NumbersColor = new SolidColorBrush(Colors.Yellow);
            clock.NumbersFontFamily = new FontFamily("Calibri");
            clock.NumbersSize = 14;

            clock.HourHand.HandColor = new SolidColorBrush(Colors.LightGray);
            clock.HourHand.LengthMultiplier = 0.48F;
            clock.HourHand.ThicknessDivisor = 100;

            clock.MinuteHand.HandColor = new SolidColorBrush(Colors.DarkGray);
            clock.MinuteHand.LengthMultiplier = 0.58F;
            clock.MinuteHand.ThicknessDivisor = 150;

            clock.SecondsHand.HandColor = new SolidColorBrush(Colors.Red);
            clock.SecondsHand.LengthMultiplier = 0.68F;
            clock.SecondsHand.ThicknessDivisor = 200;

            ClockControl.StartClock(clock);

        }
    }
}
