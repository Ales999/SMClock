/*

    This file is part of SmClock.

    SmClock is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SmClock is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.

  (Этот файл — часть SmClock.

   SmClock - свободная программа: вы можете перераспространять ее и/или
   изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
   в каком она была опубликована Фондом свободного программного обеспечения;
   либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
   версии.

   SmClock распространяется в надежде, что она будет полезной,
   но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
   или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
   общественной лицензии GNU.

   Вы должны были получить копию Стандартной общественной лицензии GNU
   вместе с этой программой. Если это не так, см.
   <https://www.gnu.org/licenses/>.)

*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AClockLibrary.ViewModels
{
    using CommonLib.Interfaces;
    using Caliburn.Micro;
    using Screen = Caliburn.Micro.Screen;

    public class AClockControlViewModel : Screen, IAClockControlViewModel
    {
        //private readonly ILogger _logger;

        #region Public Fields

        /// <summary>
        /// Clock
        /// </summary>
        public const string FLD_Clock = "Clock";

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// The pi constant
        /// </summary>
        private const double PI = Math.PI; // 3.141592654F;

        /// <summary>
        /// Clock
        /// </summary>
        private AClockData mClock;

        private DispatcherTimer mClockTimer;
        private Canvas _tickCanvas;
        private double _actualWidth;
        private double _actualHeight;

        #endregion Private Fields

        #region Public Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        //public new event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Constructors
        /*
        public AClockControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }
        */

        public AClockControlViewModel()
        {
            //this.ActualWidth = 120.0;
            //this.ActualHeight = 120.0;
            TickCanvas = new Canvas();
            
        }



        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Clock
        /// </summary>
        public AClockData Clock
        {
            get
            {
                return mClock;
            }
            set
            {
                mClock = value;
                NotifyOfPropertyChange(() => Clock);
                //NotifyOfPropertyChange(() => FLD_Clock);
                //OnPropertyChanged(FLD_Clock);
            }
        }
        
        public Canvas TickCanvas
        {
            get { return _tickCanvas; }
            private set
            {
                if (Equals(value, _tickCanvas)) return;
                _tickCanvas = value;
                NotifyOfPropertyChange(() => TickCanvas);
            }
        }
        
        //public ObservableCollection<Canvas> TickCanvas;
       // public  Canvas TickCanvas { get; }

        #endregion Public Properties

        #region Public Methods
        
        public void EvntChangeSize(double width, double height)
        {
            ActualWidth = width;
            ActualHeight = height;
        }
        

        public void StartClock(AClockData clockData)
        {
            Clock = clockData;

            CheckClockDimension();

            this.mClockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            this.mClockTimer.Tick += ClockTimer_Tick;
            this.mClockTimer.Start();
        }

        #endregion Public Methods

        #region Protected Methods
        /*
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        */

        #endregion Protected Methods

        #region Private Methods

        private void CheckClockDimension()
        {
            double dimension = ActualHeight < ActualWidth ? ActualHeight : ActualWidth;

            //If the clock control dimension is different due to a resize of the container
            //changes the clock dimension and redraws the ticks.
            if (dimension != Clock.ClockDimension)
            {
                Clock.ClockDimension = dimension;
                DrawTicks();
            }
        }

        public double ActualWidth
        {
            get { return _actualWidth; }
            set
            {
                if (value.Equals(_actualWidth)) return;
                _actualWidth = value;
                NotifyOfPropertyChange(() => ActualWidth);
                if(Clock != null)
                    UpdateClock();
            }
        }

        public double ActualHeight
        {
            get { return _actualHeight; }
            set
            {
                if (value.Equals(_actualHeight)) return;
                _actualHeight = value;
                NotifyOfPropertyChange(() => ActualHeight);
                if (Clock != null)
                    UpdateClock();
            }
        }

        /// <summary>
        /// Clocks the timer tick.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ClockTimer_Tick(object sender, object e)
        {
            UpdateClock();
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="color">The color.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="destinationCanvas">The destination canvas.</param>
        private Line DrawLine(double x1, double y1, double x2, double y2,
            SolidColorBrush color, double thickness, Canvas destinationCanvas)
        {
            Line line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                StrokeThickness = thickness,
                Stroke = color
            };

            destinationCanvas.Children.Add(line);
            return line;
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="color">The color.</param>
        /// <param name="destinationCanvas">The destination canvas.</param>
        private void DrawText(string text, double x1, double y1, double x2, double y2,
            SolidColorBrush color, Canvas destinationCanvas)
        {
            TextBlock txt = new TextBlock()
            {
                Foreground = Clock.NumbersColor,
                FontSize = Clock.NumbersSize,
                FontFamily = Clock.NumbersFontFamily,
                Text = text
            };

            Size sz = MeasureString(text, txt);
            txt.Margin = new System.Windows.Thickness(x1 - sz.Width / 2, y1 - sz.Height / 2, x2, y2);

            destinationCanvas.Children.Add(txt);
        }

        /// <summary>
        /// Draws the clock ticks.
        /// </summary>
        private void DrawTicks()
        {
            string[] hours = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            int hourIdx = 0;
            TickCanvas.Children.Clear();
            for (int i = 0; i < 60; i++)
            {
                if (i % 5 == 0)
                {
                    DrawLine(
                        Clock.CenterX + (Clock.ClockRadius / 1.10 * System.Math.Sin(i * 6 * PI / 180)),
                        Clock.CenterY - (Clock.ClockRadius / 1.10 * System.Math.Cos(i * 6 * PI / 180)),
                        Clock.CenterX + (Clock.ClockRadius / 1.35 * System.Math.Sin(i * 6 * PI / 180)),
                        Clock.CenterY - (Clock.ClockRadius / 1.35 * System.Math.Cos(i * 6 * PI / 180)),
                        Clock.TickColor, Clock.TickThickness, TickCanvas);
                    DrawText(hours[hourIdx],
                        Clock.CenterX + (Clock.ClockRadius / 1.50 * System.Math.Sin(i * 6 * PI / 180)),
                        Clock.CenterY - (Clock.ClockRadius / 1.50 * System.Math.Cos(i * 6 * PI / 180)),
                        Clock.CenterX + (Clock.ClockRadius / 1.55 * System.Math.Sin(i * 6 * PI / 180)),
                        Clock.CenterY - (Clock.ClockRadius / 1.55 * System.Math.Cos(i * 6 * PI / 180)),
                        Clock.TickColor, TickCanvas);
                    hourIdx++;
                }
                else
                {
                    DrawLine(
                        Clock.CenterX + (Clock.ClockRadius / 1.10 * System.Math.Sin(i * 6 * PI / 180)),
                        Clock.CenterY - (Clock.ClockRadius / 1.10 * System.Math.Cos(i * 6 * PI / 180)),
                        Clock.CenterX + (Clock.ClockRadius / 1.15 * System.Math.Sin(i * 6 * PI / 180)),
                        Clock.CenterY - (Clock.ClockRadius / 1.15 * System.Math.Cos(i * 6 * PI / 180)),
                        Clock.TickColor, Clock.TickThickness, TickCanvas);
                }
            }
        }

        /// <summary>
        /// Measures the string.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="txt">The text.</param>
        /// <returns></returns>
        private Size MeasureString(string hour, TextBlock txt)
        {
            //PresentationSource ps = PresentationSource.FromVisual(txt);
            var formattedText = new FormattedText(
                hour,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(txt.FontFamily, txt.FontStyle, txt.FontWeight, txt.FontStretch),
                txt.FontSize, Brushes.Black, VisualTreeHelper.GetDpi(txt).PixelsPerDip);

            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// Draws the clock.
        /// </summary>
        private void UpdateClock()
        {
            CheckClockDimension();

            Clock.DateAndTime = DateTime.Now;
        }

        #endregion Private Methods

    }
}
