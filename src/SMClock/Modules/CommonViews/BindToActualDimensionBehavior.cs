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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace CommonViews
{
    // Get from:
    // http://blogs.microsoft.co.il/iblogger/2014/12/22/simple-behavior-binding-actualwidthactualheight/


    public class BindToActualDimensionBehavior : Behavior<FrameworkElement>
    {
        public FrameworkElement SourceControl
        {
            get { return (FrameworkElement)GetValue(SourceControlProperty);
            }
            set { SetValue(SourceControlProperty, value);
            }
        }
 
        // Using a DependencyProperty as the backing store for SourceControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceControlProperty =
            DependencyProperty.Register("SourceControl", typeof(FrameworkElement), typeof(BindToActualDimensionBehavior), new PropertyMetadata(null, OnSourceControlChanged));

        private static void OnSourceControlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {

            ((BindToActualDimensionBehavior)dependencyObject).OnSourceControlChanged((FrameworkElement)dependencyPropertyChangedEventArgs.OldValue, (FrameworkElement)dependencyPropertyChangedEventArgs.NewValue);
        }

        private void OnSourceControlChanged(FrameworkElement oldControl, FrameworkElement newControl)
        {
            if (oldControl != null)
            {
                oldControl.LayoutUpdated -= OnSourceLayoutUpdated;
            }
            if (newControl != null)
            {
                newControl.LayoutUpdated += OnSourceLayoutUpdated;
            }
        }
        public double ActualWidth
        {
            get { return (double)GetValue(ActualWidthProperty); }
            set { SetValue(ActualWidthProperty, value); }
        }

// Using a DependencyProperty as the backing store for ActualWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualWidthProperty =
            DependencyProperty.Register("ActualWidth", typeof(double), typeof(BindToActualDimensionBehavior), new PropertyMetadata(0.0));



        public double ActualHeight
        {
            get { return (double)GetValue(ActualHeightProperty); }
            set { SetValue(ActualHeightProperty, value); }
        }



// Using a DependencyProperty as the backing store for ActualHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualHeightProperty =
            DependencyProperty.Register("ActualHeight", typeof(double), typeof(BindToActualDimensionBehavior), new PropertyMetadata(0.0));



        private void UpdateDimensions()
        {
            ActualHeight = SourceControl.ActualHeight;
            ActualWidth = SourceControl.ActualWidth;
        }



        void OnSourceLayoutUpdated(object sender, object e)
        {
            UpdateDimensions();
        }
    }
}
