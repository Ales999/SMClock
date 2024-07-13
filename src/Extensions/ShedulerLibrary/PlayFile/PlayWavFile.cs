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
//using System.Collections.Generic;
using System.IO;
using System.Media;
//using System.Text;
using System.Threading.Tasks;
using CommonLib.Interfaces;

namespace ShedulerLibrary.PlayFile
{
    internal class PlayWavFile : IPlayWavFile
    {
        private SoundPlayer _soundPlayer;
        private readonly ILogger _logger;
        private string FileName { get; set; }

        /// <summary>
        /// Признак что файл успешно загружен
        /// </summary>
        private bool GoodLoadFile { get; set; }

        public PlayWavFile(ILogger logger, string fileName)
        {
            this._logger = logger;
            this.FileName = fileName;
            // ---------------------
            GoodLoadFile = false;

            if (File.Exists(fileName))
            {
                try
                {
                    _soundPlayer = new SoundPlayer { SoundLocation = FileName };
                    _soundPlayer.Load();
                }
                catch (TimeoutException te)
                {
                    GoodLoadFile = false;
                    _logger?.Error(te, $"File {fileName} long load");

                }
                catch (FileNotFoundException fe)
                {
                    GoodLoadFile = false;
                    _logger?.Error(fe, $"File {fileName} not found");
                }
                catch (Exception e)
                {
                    GoodLoadFile = false;
                    _logger?.Error(e, "Unknown exception PlayWavFile");
                }
                finally
                {
                    _logger?.Information($"File {fileName} successful loaded");
                    GoodLoadFile = true;
                }
            }
        }

        public void SetNewPlayFile(string newFileName)
        {
            _soundPlayer.SoundLocation = newFileName;
            _soundPlayer.LoadAsync();
        }

        private void SoundPlayer_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            this.GoodLoadFile = true;
        }

        public void Play()
        {
            if (_soundPlayer != null && GoodLoadFile)
                _soundPlayer.Play();
        }

        /// <summary>
        /// Проиграть файл
        /// </summary>
        /// <returns></returns>
        public Task PlayAsync()
        {
            return Task.Run(() => PlayFileAction());
        }

        private void PlayFileAction()
        {
            if (_soundPlayer != null && GoodLoadFile)
            {
                _soundPlayer.Play();
            }

        }

        public void Dispose()
        {
            if (_soundPlayer == null) return;

            _soundPlayer.Stop();
            _soundPlayer.Dispose();
            _soundPlayer = null;
        }
    }

}
