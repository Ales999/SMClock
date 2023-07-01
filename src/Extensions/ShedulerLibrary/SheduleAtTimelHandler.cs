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
using System.Linq;
using System.Threading.Tasks;
using CommonLib.Interfaces;
using ShedulerLibrary.Jobs;
using ShedulerLibrary.PlayFile;
using Trigger.NET.Cron;

namespace ShedulerLibrary
{
    using Caliburn.Micro;
    using System.Threading;
    using Trigger.NET;
    using Trigger.NET.FluentAPI;

    public class SheduleAtTimelHandler : IHandle<IAtTimePlayDataMsg>, IDisposable
    {

        #region Fileds

        private readonly IEventAggregator _eventAggregator;
        private readonly CommonLib.Interfaces.ILogger _logger;
        private Scheduler _scheduler;
        private PlayWavFile _playerFile;
        private string _fileNameToPlay;
        private List<string> _tsLists;


        /// <summary>
        /// Признак что нужно пересозать Job-ы
        /// </summary>
        private static bool _neededChangeJob;

        /// <summary>
        /// Список выполняемых задач
        /// </summary>
        private Dictionary<Guid, string> jobsDictionary;

        /// <summary>
        /// Имя файла для проигрывания в заданное время (wav)
        /// </summary>
        public string FileNameToPlay
        {
            get
            {
                return _fileNameToPlay;
            }
            set
            {   // TODO: If change - remove all jobs and recreate
                if (value.Equals(_fileNameToPlay)) return;
                _fileNameToPlay = value;
            }
        }

        #endregion

        #region ctor

        /// <summary>
        /// ctor handler for AT play time
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="logger"></param>
        public SheduleAtTimelHandler(IEventAggregator eventAggregator, CommonLib.Interfaces.ILogger logger)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
            _logger = logger;
            //_scheduler = new Scheduler();
            jobsDictionary = new Dictionary<Guid, string>();
#if DEBUG
            _logger?.Information("--- Enter Ctor SheduleAtTimelHandler ---");
#endif
        }

        #endregion

        #region Handle

        // Срабатывает когда происходят изменения на закладке запуска в определенное время.
        public async Task HandleAsync(IAtTimePlayDataMsg message, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                this.Handle(message);
                return Task.CompletedTask;
            });
        }


        public void Handle(IAtTimePlayDataMsg notification)
        {
            FileNameToPlay = notification.Message.FileNameToPlay;
            _tsLists = notification.Message.TsLists;
#if DEBUG
            _logger?.Information("Create jobs for play file {0}", FileNameToPlay);
#endif
            ConfigureSheduler();
        }

        #region ConfigureSheduler

        private void ConfigureSheduler()
        {
            // Если у нас уже изменеения
            if (_neededChangeJob)
            {

                #region Remove existing jobs

                if (_scheduler != null)
                {
                    // Удалим все задания
                    foreach (var guid in jobsDictionary)
                    {
                        _scheduler.RemoveJob(guid.Key);
                        _logger.Information($"Remove job for {guid.Value} with guid: {guid.Key}");
                    }

                    _scheduler.Dispose();
                    _scheduler = null;
                    // Раз задания удалили все - очищаем и хранилку их.
                    jobsDictionary.Clear();
                }

                if (_playerFile != null)
                {
                    _playerFile.Dispose();
                    _playerFile = null;
                }

                #endregion

            }

            // Если количество отметок больше нуля И файл проигрывания указан - добавляем задание.
            if (_tsLists.Count > 0 && !string.IsNullOrEmpty(FileNameToPlay))
            {
                if (_scheduler == null)
                    _scheduler = new Scheduler();

                // Создаем наш объект проигрывания.
                if (_playerFile == null)
                    _playerFile = new PlayWavFile(_logger, FileNameToPlay);

                // Удаляем дубликаты если есть.
                var jobsTimeLists = _tsLists.Distinct().ToList();

                foreach (var timeAt in jobsTimeLists)
                {
                    var newGuid = _scheduler.AddJob<PlayFileAtTimeJob>(cfg => cfg
                        .UseCron(CrontabPatterns.GetConcretePatterns(timeAt)).WithParameter(_playerFile));

                    jobsDictionary.Add(newGuid, timeAt);
                    _logger.Information($"Create Job for {timeAt} by GUID: {newGuid}, with play file: {FileNameToPlay}");
                }
                _neededChangeJob = true;
            }
        }

        #endregion

        #endregion

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

                    //  Удаляем все задания сначала
                    foreach (var guid in jobsDictionary)
                    {
                        _scheduler.RemoveJob(guid.Key);
                    }

                    _scheduler?.Dispose();
                    _playerFile?.Dispose();

                    jobsDictionary.Clear();
                    jobsDictionary = null;
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~SheduleAtTimelHandler() {
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
