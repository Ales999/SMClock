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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Interfaces;
using StructureMap;

namespace AClockLibrary.ViewModels
{
    using Caliburn.Micro;

    public sealed class AClockViewModel : Conductor<object>.Collection.OneActive, IHandle<ISchedulerDataMsg>, IAClock
    {
        
        //public new string DisplayName = "Аналоговые Часы";
        public override string DisplayName { get; set; }

        #region Singlton
        /*
        private static readonly Lazy<AClockViewModel> lazy = new Lazy<AClockViewModel>(() => new AClockViewModel());
        private static AClockViewModel Instance => lazy.Value;

        private AClockViewModel() { }
        */
        #endregion

        //private IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;


        public AClockViewModel(IEventAggregator eventAggregator, ILogger logger)
        {
            //TestVM = theTest;
            //this.MainWidth = 180;
            //this.MainHeight = 180;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _logger = logger;
            this.DisplayName = "Часы";

            _logger.Information("Ctor ---=== AClockViewModel ===--- ");
        }

        public void OnClose(EventArgs ea)
        {
            Dispose();
        }

        /*

        private double _mainWidth;
        private double _mainHeight;


        //public TestViewModel TestVM { get; set; }
        public AClockControlViewModel AClockControlModel { get; private set; }

        public void EvntChangeSize(double width, double height)
        {
           AClockControlModel.ActualWidth = width;
           this.MainWidth = width;
           AClockControlModel.ActualHeight = height;
           this.MainHeight = height;
        }

        public void LoadedAction(double width, double height)
        {
            AClockControlModel.ActualWidth = width;
            AClockControlModel.ActualHeight = height;
        }

        public double MainWidth
        {
            get { return _mainWidth; }
            set
            {
                if (value.Equals(_mainWidth)) return;
                _mainWidth = value;
                NotifyOfPropertyChange(() => MainWidth);
            }
        }

        public double MainHeight
        {
            get { return _mainHeight; }
            set
            {
                if (value.Equals(_mainHeight)) return;
                _mainHeight = value;
                NotifyOfPropertyChange(() => MainHeight);
            }
        }

        public AClockViewModel(IEventAggregator eventAggregator, ILogger logger, AClockControlViewModel aclockControlModel)
        {
            //TestVM = theTest;
            this.MainWidth = 180;
            this.MainHeight = 180;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _logger = logger;

            AClockControlModel = aclockControlModel;
            aclockControlModel.ActualHeight = this.MainHeight;
            aclockControlModel.ActualWidth  = this.MainWidth;

            _atFile = "AtFile this";
            _prFile = "PrFile this";

            _logger.Information("Ctor ---=== AClockViewModel ===--- ");

            AClockData clockData = new AClockData
            {
                BackColor = new SolidColorBrush(Colors.Black),
                TickColor = new SolidColorBrush(Colors.White),
                TickThicknessDivisor = 130,
                NumbersColor = new SolidColorBrush(Colors.Yellow),
                NumbersFontFamily = new FontFamily("Calibri"),
                NumbersSize = 14,
                HourHand =
                {
                    HandColor = new SolidColorBrush(Colors.LightGray),
                    LengthMultiplier = 0.48F,
                    ThicknessDivisor = 100
                },
                MinuteHand =
                {
                    HandColor = new SolidColorBrush(Colors.DarkGray),
                    LengthMultiplier = 0.58F,
                    ThicknessDivisor = 150
                },
                SecondsHand =
                {
                    HandColor = new SolidColorBrush(Colors.Red),
                    LengthMultiplier = 0.68F,
                    ThicknessDivisor = 200
                }
            };

            AClockControlModel.StartClock(clockData);

        }

        //public AClockControlViewModel AClockControl { get; set; }

*/
        private string _atFile;
        private string _prFile;

        public string AtFile
        {
            get
            {
                return _atFile;
            }
            set
            {
                if (value.Equals(_atFile)) return;
                _atFile = value;
                NotifyOfPropertyChange(() => AtFile);
            }
        }

        public string PrFile
        {
            get { return _prFile; }
            set
            {
                if (value.Equals(_prFile)) return;

                _prFile = value;
                NotifyOfPropertyChange(() => PrFile);
            }
        }
        

        /*
        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        */
        public void Handle(ISchedulerDataMsg message)
        {
            _logger?.Information($"---=== AClockViewModel ===--- Handle: {nameof(AClockViewModel)}");

            if (message.AtMessage != null)
                AtFile = message.AtMessage.FileNameToPlay;

            if (message.PrMessage != null)
                PrFile = message.PrMessage.FileName;

        }


        

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов
        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    _eventAggregator.Unsubscribe(this);
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~AClockViewModel() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
