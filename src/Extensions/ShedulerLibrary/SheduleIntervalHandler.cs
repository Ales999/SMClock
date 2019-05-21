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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Interfaces;
using ShedulerLibrary.Jobs;
using ShedulerLibrary.PlayFile;
using Trigger.NET.Cron;

namespace ShedulerLibrary
{
    using Caliburn.Micro;

    using Trigger;
    using Trigger.NET;
    using Trigger.NET.FluentAPI;

    public class SheduleIntervalHandler : IHandle<IPeriodicPlayDataMsg>, IDisposable
    {

        #region Fields

        private readonly CommonLib.Interfaces.ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private Guid jobId;

        private readonly Scheduler _scheduler;
        private PlayWavFile _playerFile;
        // Признак что нужно пересозать Job
        private static bool _neededChangeJob;

        internal int Period { get; set; }

        internal string FileNamePeriodPlay { get; set; } = string.Empty;

        #endregion


        public SheduleIntervalHandler(IEventAggregator eventAggregator, CommonLib.Interfaces.ILogger logger)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _logger = logger;
            _scheduler = new Scheduler();

            _neededChangeJob = false;

            _logger.Information("--- Enter Ctor SheduleHandler ---");
        }
        
        /// <summary>
        /// Получатель сообщений от медиатра
        /// </summary>
        /// <param name="notification"></param>
        public void Handle(IPeriodicPlayDataMsg notification)
        {
            
            Period = notification.Message.EveryMinut;
            FileNamePeriodPlay = notification.Message.FileName;

            _logger.Information($"Handle SheduleHandler, period: {Period}, file to play: {FileNamePeriodPlay}");
            // Получены новые данные - запускаем/перезапускаем наш шедулер
            ConfigureSheduler(notification.Message.FileName, notification.Message.EveryMinut);

        }

        /// <summary>
        /// Запускаем/Перезапускаем Cron
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="period"></param>
        private void ConfigureSheduler(string fileName, int period)
        {

            // Если не дефолт - значит Job уже назначен - удаляем его
            if ( _neededChangeJob )
            {
                _scheduler.RemoveJob(jobId);

                if (_playerFile != null)
                {
                    _playerFile.Dispose();
                    _playerFile = null;
                }

            }

            // Если период стал 'выключено' или имя файла пустое - на выход, тут делать больше нечего.
            if (period == 0 || fileName == string.Empty) return;

            // Создаем наш объект проигрывания.
            _playerFile = new PlayWavFile(_logger, fileName);

            // Создаем новое задание с новыми параметрами
            jobId = _scheduler.AddJob<PlayEveryTimeJob>(cfg => cfg.UseCron( CrontabPatterns.GetIntervalPattern(period) ).WithParameter(_playerFile) );
            
            // Установим признак что задача уже есть запущенная
            _neededChangeJob = true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    _eventAggregator.Unsubscribe(this);
                    _scheduler.RemoveJob(jobId);

                    _scheduler.Dispose();
                    _playerFile.Dispose();

                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~SheduleIntervalHandler() {
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
