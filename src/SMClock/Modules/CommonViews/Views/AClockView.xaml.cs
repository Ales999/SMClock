using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowInTaskbar = false;

        }

        public new void Focus()
        {
            SystemCommands.RestoreWindow(this);
            this.WindowState = WindowState.Normal;

            Dispatcher.BeginInvoke(new Action(delegate
            {
                this.Activate();
            }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            AClockData clock = new AClockData
            {
                BackColor = new SolidColorBrush(Colors.Black),
                TickColor = new SolidColorBrush(Colors.White),
                TickThicknessDivisor = 130,
                NumbersColor = new SolidColorBrush(Colors.Yellow),
                NumbersFontFamily = new FontFamily("Calibri"),
                NumbersSize = 14
            };

            clock.HourHand.HandColor = new SolidColorBrush(Colors.BlueViolet);
            clock.HourHand.LengthMultiplier = 0.48F;
            clock.HourHand.ThicknessDivisor = 100;

            clock.MinuteHand.HandColor = new SolidColorBrush(Colors.GreenYellow);
            clock.MinuteHand.LengthMultiplier = 0.58F;
            clock.MinuteHand.ThicknessDivisor = 150;

            clock.SecondsHand.HandColor = new SolidColorBrush(Colors.Red);
            clock.SecondsHand.LengthMultiplier = 0.68F;
            clock.SecondsHand.ThicknessDivisor = 200;

            ClockControl.StartClock(clock);

        }
    }
}
