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
using Caliburn.Micro;
using CommonLib;
using CommonLib.Interfaces;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StructureMap;
using StructureMap.Pipeline;

namespace SMClock.Modules.SheduleConfig.ViewModels
{


    internal class SheduleConfigViewModel : Conductor<object>.Collection.OneActive, ISheduleConfig, IDisposable
    {

        public sealed override string DisplayName { get; set; }


#if DEBUG
        public const string AppName = "SMClock";
#else
        public static readonly string AppName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
#endif


        #region Private fields

        private readonly IContainer _container;

        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private readonly IAppConfig _appConfig;

        /// <summary>
        /// Признак что файл конфигурации существует
        /// </summary>
        private readonly bool _useSavesConfig;

        /// <summary>
        /// Признак что приложение в процессе загрузки
        /// </summary>
        private readonly bool _useNormalWork;

        private bool _neededSendMsgForSetAutoStartApp;

        #endregion

        #region Binding Fuleds

        // ------------ Binding fiels----------------------------------
        // Общиая кнопка на все вкладки типа
        private bool _enableSaveButton;
        // Первая вкладка
        private string _playFileEveryTime;
        private KeyValuePair<string, int> _selectedPeriodTime;

        // Вторая вкладка
        private bool _usingAtTime;
        private string _playFileAtTime = string.Empty;
        private bool _at1Box;
        private bool _at2Box;
        private bool _at3Box;
        private bool _at4Box;

        private string _oneDt; // = "12:00";
        private string _twoDt; // = "12:00";
        private string _threeDt; // = "12:00";
        private string _fourDt; // = "12:00";

        /// <summary>
        /// Список в виде "hh:mm" который и будем передавать для установки задания в определенное время
        /// </summary>
        private readonly List<string> tsList;

        /// <summary>
        /// Пустой список, для удаления всех отметок, если они ранее были добавлены. 
        /// </summary>
        private readonly List<string> _clrarTsList = new List<string>();

        private bool _autoStartApp;

#pragma warning disable IDE0060 // Удалите неиспользуемый параметр
        public void OnClose(EventArgs ea)
#pragma warning restore IDE0060 // Удалите неиспользуемый параметр
        {
            Dispose();
        }

        /// <summary>
        /// Признак что пользователь изменил настройки
        /// и можно разблокировать кнопку записи конфигурации
        /// </summary>
        private bool EnableSaveButton
        {
            get { return _enableSaveButton; }
            set
            {
                _enableSaveButton = value;
                NotifyOfPropertyChange(() => CanSaveConfig);
            }
        }

        // ------------------------------------------------------------

        #endregion

        #region Ctor and read config

        public SheduleConfigViewModel(IContainer container, IEventAggregator eventAggregator, ILogger logger, IAppConfig appConfig)
        {
            this.DisplayName = "Настройка SM Clock";

            _container = container;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _appConfig = appConfig;
            _useSavesConfig = appConfig.IsConfigFileExists;
            tsList = new List<string>();

            this.RestoreFromConfig();

            // Last - disable SaveButton
            EnableSaveButton = false;
            _useNormalWork = true;

            if (UsingAtTime && tsList.Count > 0)
                SendUpdateAtTimes(tsList);

            //MyTimeSpan = DateTime.Now.TimeOfDay;
        }

        #region TimesConver

        /// <summary>
        /// Конвертируем локальное значение времение вида "HH:mm" в UTC вида "HH:mm"
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        private string ConvLocalToUtc(string localTime)
        {
            string retval = "00:00";

            try
            {
                var utcTime = DateTime.ParseExact(localTime, "H:mm", CultureInfo.InvariantCulture).ToUniversalTime();
                // Если минут меньше 10 то в выходной строке добавлеям ноль в префикс для Минут
                retval = utcTime.Minute < 10 ? $"{utcTime.Hour}:0{utcTime.Minute}" : $"{utcTime.Hour}:{utcTime.Minute}";
                
            }
            catch (FormatException fe)
            {
                _logger.Error(fe, $"Не удалось преобразовать {localTime} в DateTime");
            }

            return retval;
        }

        /// <summary>
        /// Конвертируем Utc значение времение вида "HH:mm" в местное вида "HH:mm"
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        private string ConvUtcToULocal(string utcTime)
        {
            string retval = "00:00";

            try
            {
                var localTime = DateTime.ParseExact(utcTime, "H:mm", CultureInfo.InvariantCulture).ToLocalTime();
                // Если минут меньше 10 то в выходной строке добавлеям ноль в префикс для Минут
                retval = localTime.Minute < 10 ? $"{localTime.Hour}:0{localTime.Minute}" : $"{localTime.Hour}:{localTime.Minute}";
            }
            catch (FormatException fe)
            {
                _logger.Error(fe, $"Не удалось преобразовать {utcTime} в DateTime");
            }


            return retval;
        }

        #endregion

        /// <summary>
        /// Восстановить настройки программы из файла
        /// TODO: В полночь надо опять их восстановить на случай изменения локальнй TimeZone или смещения (переход на летнее время например)
        /// </summary>
        private void RestoreFromConfig()
        {
            // Если есть файл конфигурации - пробеум читать из него
            if (_useSavesConfig)
            {
                object tmpValue;

                if (_appConfig.TryGetValue(nameof(PlayFileEveryTime), out tmpValue))
                    PlayFileEveryTime = tmpValue as string;
                // PlayFileAtTime
                if (_appConfig.TryGetValue(nameof(PlayFileAtTime), out tmpValue))
                    PlayFileAtTime = tmpValue as string;


                #region Первая вкладка

                if (_appConfig.TryGetValue(nameof(SelectedPeriodicTimeList), out tmpValue))
                {

                    var jobj = (JObject)tmpValue;

                    //var values = period.ToObject<KeyValuePair<string, object>>();
                    //var values = period.Cast<JProperty>().ToDictionary( x => x.Name, x => (string)x.Value );
                    //var values = period.Cast<JProperty>().Values(); // return JToken
                    /*
                    foreach (JProperty x in (JToken)period)
                    {
                        string name = x.Name;
                        JToken value = x.Value;

                    }
                    */


                    var i1 = System.Convert.ChangeType(jobj["Key"].ToString(), typeof(string));
                    var i2 = JsonConvert.DeserializeObject(jobj["Value"].ToString(), typeof(int));
                    SelectedPeriodicTimeList = new KeyValuePair<string, int>((string)i1, (int)i2);
                }

                if (_appConfig.TryGetValue(nameof(AutoStartApp), out tmpValue))
                    AutoStartApp = (bool)tmpValue;

                #endregion

                #region Вторая вкладка
                // ---=== Вторая вкладка ===---
                if (_appConfig.TryGetValue(nameof(UsingAtTime), out tmpValue))
                    UsingAtTime = (bool)tmpValue;

                if (_appConfig.TryGetValue(nameof(At1Box), out tmpValue))
                    At1Box = (bool)tmpValue;
                if (_appConfig.TryGetValue(nameof(At2Box), out tmpValue))
                    At2Box = (bool)tmpValue;
                if (_appConfig.TryGetValue(nameof(At3Box), out tmpValue))
                    At3Box = (bool)tmpValue;
                if (_appConfig.TryGetValue(nameof(At4Box), out tmpValue))
                    At4Box = (bool)tmpValue;

                // ---

                if (_appConfig.TryGetValue(nameof(OneDT), out tmpValue))
                    OneDT = ConvUtcToULocal(tmpValue as string);

                if (_appConfig.TryGetValue(nameof(TwoDT), out tmpValue))
                    TwoDT = ConvUtcToULocal(tmpValue as string);

                if (_appConfig.TryGetValue(nameof(ThreeDT), out tmpValue))
                    ThreeDT = ConvUtcToULocal(tmpValue as string);

                if (_appConfig.TryGetValue(nameof(FourDT), out tmpValue))
                    FourDT = ConvUtcToULocal(tmpValue as string);
                // ---=== End ===---
                #endregion

            }
            else
            { // Иначе - устанавливаем дефолтные значения и записываем их
                PlayFileEveryTime = string.Empty;
                var defKey = PeriodicTimeList.Keys.ToList()[0];
                var defValue = PeriodicTimeList.Values.ToList()[0];

                SelectedPeriodicTimeList = new KeyValuePair<string, int>(defKey, defValue);


                _appConfig.SaveConfig();
            }

        }

        #endregion


        #region Methods to Binding

        #region SaveConfig

        public void SaveConfig()
        {
            // Сохраним статус автозапуска
            if (_neededSendMsgForSetAutoStartApp)
            {
                SendMsgForSetAutoStartApp();
                _neededSendMsgForSetAutoStartApp = false;
            }

            _appConfig.SaveConfig();
            EnableSaveButton = false;
            GC.Collect();
        }

        /// <summary>
        /// Если false - блокируеи кнопку с именем SaveConfig
        /// </summary>
        /// <returns></returns>
        public bool CanSaveConfig => EnableSaveButton;
        #endregion

        #region TextBox for path and file name for play every time

        /// <summary>
        /// Файл который будет проигрываться каджые N минут.
        /// </summary>
        public string PlayFileEveryTime
        {
            get
            {
                return _playFileEveryTime;
            }
            set
            {
                if (value.Equals(_playFileEveryTime)) return;

                _playFileEveryTime = value;
                _appConfig[nameof(PlayFileEveryTime)] = value;
                EnableSaveButton = true;
                this.SendMsgPeriodicTimeList(SelectedPeriodicTimeList);
                NotifyOfPropertyChange(() => PlayFileEveryTime);

            }
        }

        public void SetEveryTimeFileBtn()
        {
            var openFileDialog =
                new OpenFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Filter = @"WAV file (*.wav)|*.wav;*.WAV"
                };

            if (openFileDialog.ShowDialog() == true)
            {
                PlayFileEveryTime = openFileDialog.FileName;
            }
        }

        #endregion

        #region ComboBox for list time with eve time play file

        /// <summary>
        /// Сам сомбик, bind by caliburn, c подготовленными данными
        /// </summary>
        public Dictionary<string, int> PeriodicTimeList => new Dictionary<string, int>
        {
            {"Отключено", 0 },
            {"1 минута", 1},
            {"5 минут", 5},
            {"10 минут", 10 },
            {"30 минут", 30},
            {"60 минут", 60 }

        };

        /// <summary>
        /// ???
        /// </summary>
        //public BindableCollection<KeyValuePair<string, int>> PeriodicTimeLists => new BindableCollection<KeyValuePair<string, int>>(this.PeriodicTimeList);

        /// <summary>
        /// Что выбрано в комбо-боксе (Selected... -> magic caliburn)
        /// </summary>
        public KeyValuePair<string, int> SelectedPeriodicTimeList
        {
            get { return _selectedPeriodTime; }
            set
            {
                if (value.Equals(_selectedPeriodTime)) return;

                _selectedPeriodTime = value;
                _appConfig[nameof(SelectedPeriodicTimeList)] = value;
                this.SendMsgPeriodicTimeList(value);
                EnableSaveButton = true;
                NotifyOfPropertyChange(() => SelectedPeriodicTimeList);
            }
        }



        #endregion

        #region AutoStart App

        /// <summary>
        /// Автозапуск приложения при запуске рабочего стола
        /// </summary>
        public bool AutoStartApp
        {
            get { return _autoStartApp; }
            set
            {
                if (value.Equals(_autoStartApp)) return;

                _autoStartApp = value;
                _appConfig[nameof(AutoStartApp)] = value;
                EnableSaveButton = true;
                // Установим признак что при записи конфигурации необходимо сохранит статус автозапуска в реестор
                _neededSendMsgForSetAutoStartApp = true;
                NotifyOfPropertyChange(() => AutoStartApp);
            }
        }

        private void SendMsgForSetAutoStartApp()
        {
            // Если вызов идет во время инициалзации - выходим из метода
            if (!_useNormalWork) return;

            var sendArgs = new ExplicitArguments();
            sendArgs.SetArg("usingAutoStart", _autoStartApp);
            sendArgs.SetArg("appName", AppName);

            var autoHandler = _container.TryGetInstance<IHandle<IAppAutoStartMsg>>();

            var setAutoStart = _container.TryGetInstance<IAppAutoStartMsg>(sendArgs);

            if (setAutoStart != null && autoHandler != null)
            {
                //await mediator.Publish(setAutoStart);
                //_eventAggregator.PublishOnBackgroundThread(setAutoStart); // Old Caliburn
                _eventAggregator.PublishOnBackgroundThreadAsync(setAutoStart);
            }
        }

        #endregion

        #region TextBox for path and file name for play Concrete time

        /// <summary>
        /// Имя файла дла проигрывания в конкретное время
        /// </summary>
        public string PlayFileAtTime
        {
            get { return _playFileAtTime; }
            set
            {
                if (value.Equals(_playFileAtTime)) return;

                _playFileAtTime = value;
                _appConfig[nameof(PlayFileAtTime)] = value;
                EnableSaveButton = true;
                NotifyOfPropertyChange(() => PlayFileAtTime);
            }
        }

        public void SetAtTimeFileBtn()
        {
            var openFileDialog =
                new OpenFileDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Filter = @"WAV file (*.wav)|*.wav;*.WAV"
                };

            if (openFileDialog.ShowDialog() == true)
            {
                PlayFileAtTime = openFileDialog.FileName;
                if (tsList.Count > 0)
                    SendUpdateAtTimes(tsList);
            }
        }

        #endregion

        #region UsingAtTime

        public bool UsingAtTime
        {
            get { return _usingAtTime; }
            set
            {
                if (value.Equals(_usingAtTime)) return;

                _usingAtTime = value;
                _appConfig[nameof(UsingAtTime)] = value;
                EnableSaveButton = true;
                //
                SendUpdateAtTimes(value ? tsList : _clrarTsList);

                NotifyOfPropertyChange(() => UsingAtTime);
            }
        }


        #endregion

        #region At.Boxses

        #region At1Box

        /// <summary>
        /// Разрешить первое уведомление
        /// </summary>
        public bool At1Box
        {
            get { return _at1Box; }
            set
            {
                if (value.Equals(_at1Box)) return;

                _at1Box = value;
                _appConfig[nameof(At1Box)] = value;
                EnableSaveButton = true;
                // If Checked
                if (!string.IsNullOrEmpty(_oneDt))
                {
                    if (value)
                    {
                        if (!tsList.Contains(_oneDt))
                        {
                            tsList.Add(_oneDt);
                        }
                    }
                    else
                    {
                        if (tsList.Contains(_oneDt))
                        {
                            tsList.Remove(_oneDt);
                        }
                    }
                    SendUpdateAtTimes(tsList);
                }

                NotifyOfPropertyChange(() => At1Box);
                NotifyOfPropertyChange(() => OneDT);
            }
        }

        #endregion

        #region At2Box

        /// <summary>
        /// Разрешить второе уведомление
        /// </summary>
        public bool At2Box
        {
            get { return _at2Box; }
            set
            {
                if (value.Equals(_at2Box)) return;

                _at2Box = value;
                _appConfig[nameof(At2Box)] = value;
                EnableSaveButton = true;

                if (!string.IsNullOrEmpty(_twoDt))
                {
                    if (value)
                    {
                        if (!tsList.Contains(_twoDt))
                        {
                            tsList.Add(_twoDt);
                        }
                    }
                    else
                    {
                        if (tsList.Contains(_twoDt))
                        {
                            tsList.Remove(_twoDt);
                        }
                    }
                    SendUpdateAtTimes(tsList);
                }

                NotifyOfPropertyChange(() => At2Box);
                NotifyOfPropertyChange(() => TwoDT);
            }
        }

        #endregion

        #region At3Box

        /// <summary>
        /// Разрешить третье уведомление
        /// </summary>
        public bool At3Box
        {
            get { return _at3Box; }
            set
            {
                if (value.Equals(_at3Box)) return;

                _at3Box = value;
                _appConfig[nameof(At3Box)] = value;
                EnableSaveButton = true;

                if (!string.IsNullOrEmpty(_threeDt))
                {
                    if (value)
                    {
                        if (!tsList.Contains(_threeDt))
                        {
                            tsList.Add(_threeDt);
                        }
                    }
                    else
                    {
                        if (tsList.Contains(_threeDt))
                        {
                            tsList.Remove(_threeDt);
                        }
                    }
                    SendUpdateAtTimes(tsList);
                }
                NotifyOfPropertyChange(() => At3Box);
                NotifyOfPropertyChange(() => ThreeDT);
            }
        }

        #endregion

        #region At4Box

        /// <summary>
        /// Разрешить четвертое уведомление
        /// </summary>
        public bool At4Box
        {
            get { return _at4Box; }
            set
            {
                if (value.Equals(_at4Box)) return;

                _at4Box = value;
                _appConfig[nameof(At4Box)] = value;
                EnableSaveButton = true;

                if (!string.IsNullOrEmpty(_fourDt))
                {
                    if (value)
                    {
                        if (!tsList.Contains(_fourDt))
                        {
                            tsList.Add(_fourDt);
                        }
                    }
                    else
                    {
                        if (tsList.Contains(_fourDt))
                        {
                            tsList.Remove(_fourDt);
                        }
                    }
                    SendUpdateAtTimes(tsList);
                }

                NotifyOfPropertyChange(() => At4Box);
                NotifyOfPropertyChange(() => FourDT);
            }
        }

        #endregion

        #endregion

        #region TDs

        #region OneDT

        public string OneDT
        {
            get { return _oneDt; }
            set
            {
                if (value.Equals(_oneDt)) return;

                if (tsList.Contains(_oneDt))
                {
                    tsList.Remove(_oneDt);
                }
                if (At1Box)
                    tsList.Add(value);

                _oneDt = value;
                _appConfig[nameof(OneDT)] = ConvLocalToUtc(value);
                EnableSaveButton = true;
                if (UsingAtTime)
                    this.SendUpdateAtTimes(tsList);
                NotifyOfPropertyChange(() => OneDT);

            }
        }

        #endregion

        #region TwoDT

        public string TwoDT
        {
            get { return _twoDt; }
            set
            {
                if (value.Equals(_twoDt)) return;

                if (tsList.Contains(_twoDt))
                {
                    tsList.Remove(_twoDt);
                }
                if (At2Box)
                    tsList.Add(value);

                _twoDt = value;
                _appConfig[nameof(TwoDT)] = ConvLocalToUtc(value);
                EnableSaveButton = true;

                if (UsingAtTime)
                    this.SendUpdateAtTimes(tsList);

                NotifyOfPropertyChange(() => TwoDT);

            }
        }

        #endregion

        #region ThreeDT

        public string ThreeDT
        {
            get { return _threeDt; }
            set
            {
                if (value.Equals(_threeDt)) return;

                if (tsList.Contains(_threeDt))
                {
                    tsList.Remove(_threeDt);
                }
                if (At3Box)
                    tsList.Add(value);

                _threeDt = value;
                _appConfig[nameof(ThreeDT)] = ConvLocalToUtc(value);
                EnableSaveButton = true;

                if (UsingAtTime)
                    this.SendUpdateAtTimes(tsList);

                NotifyOfPropertyChange(() => ThreeDT);

            }
        }

        #endregion

        #region FourDT

        public string FourDT
        {
            get { return _fourDt; }
            set
            {
                if (value.Equals(_fourDt)) return;

                if (tsList.Contains(_fourDt))
                {
                    tsList.Remove(_fourDt);
                }
                if (At4Box)
                    tsList.Add(value);

                _fourDt = value;
                _appConfig[nameof(FourDT)] = ConvLocalToUtc(value);
                EnableSaveButton = true;

                if (UsingAtTime)
                    this.SendUpdateAtTimes(tsList);

                NotifyOfPropertyChange(() => FourDT);

            }
        }

        #endregion

        #region Send MsgData

        /// <summary>
        /// Отправим сообщение посредством MediatR что нужно установить такую периодичность.
        /// </summary>
        private void SendMsgPeriodicTimeList(KeyValuePair<string, int> selectedValuePair)
        {
            // Если имя файла не определно или переодичность как 'выключено' - не отправлять сообщение для создания задачи
            if (string.IsNullOrEmpty(PlayFileEveryTime) || selectedValuePair.Value == 0) return;

            IPeriodicPlayData sendData = new PeriodicPlayData(selectedValuePair.Value, PlayFileEveryTime);
            IPeriodicPlayDataMsg senDataMsg = new PeriodicPlayDataMsg(sendData);

            SendToAclockMessage(sendData, null);

            //mediator.Publish(senDataMsg);
            var perHandle = _container.TryGetInstance<IHandle<IPeriodicPlayDataMsg>>();
            if (perHandle != null)
                //_eventAggregator.PublishOnBackgroundThread(senDataMsg); // Old Caliburn
                _eventAggregator.PublishOnBackgroundThreadAsync(senDataMsg);
        }

        /// <summary>
        /// Отправляем сообщение посредством EventAggregator что нам нужно установить
        /// такой набор временных отметок в отпределенное время. 
        /// </summary>
        /// <param name="timesList"></param>
        private async void SendUpdateAtTimes(List<string> timesList)
        {
            // Если вызов идет во время инициалзации - выходим из метода
            if (!_useNormalWork) return;

            var args = new ExplicitArguments();
            args.SetArg("fileName", _playFileAtTime);
            args.SetArg("tsLists", timesList);
            IAtTimePlayData atData = _container.GetInstance<IAtTimePlayData>(args);

            var argMsg = new ExplicitArguments();
            argMsg.SetArg("msgData", atData);
            var atMessage = _container.GetInstance<IAtTimePlayDataMsg>(argMsg);

            SendToAclockMessage(null, atData);

            //mediator.Publish(atMessage);
            // Пробуем запросить нашего работника, в если удачно - пересылаему ему задачу
            var atHandle = _container.TryGetInstance<IHandle<IAtTimePlayDataMsg>>();
            if (atHandle != null)
                await _eventAggregator.PublishOnUIThreadAsync(atMessage);

        }

        protected virtual void SendToAclockMessage(IPeriodicPlayData prMsg, IAtTimePlayData atMsg)
        {
            var clockMsg = new SchedulerDataMsg { PrMessage = prMsg, AtMessage = atMsg };
            //mediator.Publish(clockMsg);
            //_eventAggregator.PublishOnUIThread(clockMsg); // Old Caliburn
            _eventAggregator.PublishOnUIThreadAsync(clockMsg);
        }
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
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~SheduleConfigViewModel() {
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


        #endregion

        #endregion

    }
}
