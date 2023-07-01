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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using CommonLib.Interfaces;

namespace SMClock.Modules.TrayIcon.ViewModels
{
    public class SystemTrayIconViewModel : PropertyChangedBase, ISystemTrayIcon
    {
        private readonly IWindowManager _windowManager;
        //private readonly IContainer _container;
        private readonly ILogger _logger;
        private readonly ISheduleConfig _sheduleConfig;
        private readonly IAClock _aClock;

        //private readonly IEventAggregator _eventAggregator;
        public SystemTrayIconViewModel(IWindowManager windowManager, /*IContainer container,*/ ILogger logger,
            ISheduleConfig sheduleConfig ,  IAClock aClock /* , IEventAggregator eventAggregator */ )
        {
            _windowManager = windowManager;
            //_container = container;
            _logger = logger;
            _sheduleConfig = sheduleConfig;
            _aClock = aClock;
            //_aClock = (IAClock)aScreen;
            // _eventAggregator = eventAggregator;
        }

        /// <summary>
        /// Вернуть иконку для главного приложения которая будет висеть в трее.
        /// Сама иконка находится как ресурс в библиотеке 'CommonViews'
        /// </summary>
        public ImageSource TrayIconSource => BitmapFrame.Create(new Uri("pack://application:,,,/CommonViews;component/Icons/clock.ico"));

        /// <summary>
        /// Показать окно с настройками
        /// </summary>
        public void ShowSheduleConfWindow()
        {
            _windowManager.FocusOrShowWindow(_sheduleConfig);
        }

        /// <summary>
        /// Показать аналоговые часы в отдельном окне.
        /// </summary>
        public void ShowClockWindow()
        {
            _windowManager.FocusOrShowWindow(_aClock);
        }

        /// <summary>
        /// Using from View: &lt;MenuItem cal:Message.Attach="ExitApplication" Header="Exit" /&gt;
        /// </summary>
        public /*async*/ void ExitApplication()
        {
           // await _eventAggregator.PublishOnUIThreadAsync("ExitApp");

            Application.Current.Shutdown();
        }

       

    }
}
