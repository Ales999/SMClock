# SMClock
Модульное приложение - Часы-Будильник.

Написано в процессе изучения C# и общей идеии создания модульного приложения.
Часть модулей подгружаются по необходимости, включая необходимые им компоненты.
Поэтому окно настроек запускается не мгновенно. 

Время выставляется путем нажатия на соответствующие мини-кнопки рассположенные справа от поля.

Присутствует создание инсталятора (при сборке Release), - на выходе получается MSI файл
В данный момент - установка Per-User не требующая админских прав.
Примеры звуков, которые не входят в инсталятор, можно забрать из папочки [ExampleSounds](ExampleSounds)


![ScreenShot1](ExampleScreenshot/Scr1.png) ![ScreenShot2](ExampleScreenshot/Scr2.png)


Так выглядят часы:
![ScreenShot3](ExampleScreenshot/Scr3.png)

Как забрать для самостоятельной сборки:
```
git.exe clone -v --progress https://github.com/Ales999/SMClock.git
cd SMClock
git.exe submodule update --init SharpConfig
```

В проекте используются стронние компоненты :

* [Caliburn.Micro](http://caliburnmicro.com) - MVVM Framework.
* [StrucureMap](http://structuremap.github.io) - DI контейнер.
* [Sharp-Config](https://github.com/ruarai/SharpConfig) - Сохранения настроек приложения (с моими правками - пока как SubModule (Версия 1.0.0 c библиотеки VS их пока не имеет!) ).
* [Extended.Wpf.Toolkit](https://github.com/xceedsoftware/wpftoolkit) (используется только UserControl, для выбора времени срабатывания ).
* [Trigger.Net](https://github.com/Novakov/trigger.net) - Планировщик выполнения задач.
* [AnalogClock](http://www.sabrinacosolo.com/multiclock-visualizzare-un-orologio-analogico/) (визуальные часы)
* [WixSharp](https://github.com/oleg-shilo/wixsharp) - создание инсталятора
