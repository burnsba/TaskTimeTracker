using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using BurnsBac.WindowsAppToolkit;
using BurnsBac.WindowsAppToolkit.Mvvm;

namespace TaskTimeTracker.ViewModels
{
    public class TaskTimeViewModel : ViewModelBase
    {
        private TaskTime _model;

        private SolidColorBrush _windowBackgroundColor;
        private Thickness _textMargin;
        private Thickness _timerMargin;
        private SolidColorBrush _textColor;
        private SolidColorBrush _timerColor;
        private FontWeight _textFontWeight;
        private FontWeight _timerFontWeight;

        /// <summary>
        /// Gets or sets a value indicating whether the timer for this task is currently active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _model.IsActive;
            }

            set
            {
                _model.IsActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }

        public string WindowBackgroundColorName
        {
            get
            {
                return _model.WindowBackgroundColorName;
            }

            set
            {
                _model.WindowBackgroundColorName = value;
                _windowBackgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                OnPropertyChanged(nameof(WindowBackgroundColorName));
                OnPropertyChanged(nameof(WindowBackgroundColor));
            }
        }

        public SolidColorBrush WindowBackgroundColor
        {
            get
            {
                return _windowBackgroundColor;
            }

            set
            {
                _windowBackgroundColor = value;
                OnPropertyChanged(nameof(WindowBackgroundColor));
            }
        }

        public string TaskName
        {
            get
            {
                return _model.TaskName;
            }

            set
            {
                _model.TaskName = value;
                OnPropertyChanged(nameof(TaskName));
            }
        }

        public string TaskTextDisplay
        {
            get
            {
                return _model.TaskTextDisplay;
            }
        }

        public string TextFontName
        {
            get
            {
                return _model.TextFontName;
            }
        }

        public int TextFontSize
        {
            get
            {
                return _model.TextFontSize;
            }
        }

        public bool TextFontIsBold
        {
            get
            {
                return _model.TextFontIsBold;
            }

            set
            {
                _model.TextFontIsBold = value;
                if (value)
                {
                    _textFontWeight = FontWeights.Bold;
                }
                else
                {
                    _textFontWeight = FontWeights.Normal;
                }

                OnPropertyChanged(nameof(TextFontIsBold));
                OnPropertyChanged(nameof(TextFontWeight));
            }
        }

        public FontWeight TextFontWeight
        {
            get
            {
                return _textFontWeight;
            }
        }

        public string TextColorName
        {
            get
            {
                return _model.TextColorName;
            }

            set
            {
                _model.TextColorName = value;
                _textColor = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                OnPropertyChanged(nameof(TextColorName));
                OnPropertyChanged(nameof(TextColor));
            }
        }

        public SolidColorBrush TextColor
        {
            get
            {
                return _textColor;
            }
        }

        public int TextWidth
        {
            get
            {
                return _model.TextWidth;
            }
        }

        public int TextHeight
        {
            get
            {
                return _model.TextHeight;
            }
        }

        public Thickness TextMargin
        {
            get
            {
                return _textMargin;
            }
        }

        public string TimerFontName
        {
            get
            {
                return _model.TimerFontName;
            }
        }

        public int TimerFontSize
        {
            get
            {
                return _model.TimerFontSize;
            }
        }

        public bool TimerFontIsBold
        {
            get
            {
                return _model.TimerFontIsBold;
            }
        }

        public FontWeight TimerFontWeight
        {
            get
            {
                return _timerFontWeight;
            }
        }

        public SolidColorBrush TimerColor
        {
            get
            {
                return _timerColor;
            }
        }

        public string TimerColorName
        {
            get
            {
                return _model.TimerColorName;
            }
        }

        public int TimerWidth
        {
            get
            {
                return _model.TimerWidth;
            }
        }

        public int TimerHeight
        {
            get
            {
                return _model.TimerHeight;
            }
        }

        public Thickness TimerMargin
        {
            get
            {
                return _timerMargin;
            }
        }

        public bool CanStartTask
        {
            get
            {
                return !IsActive;
            }
        }

        public bool CanStopTask
        {
            get
            {
                return IsActive;
            }
        }

        public string NowTotalElapsedFormatted
        {
            get
            {
                return _model.NowTotalElapsedFormatted;
            }
        }

        public TaskTimeViewModel()
        {
            _windowBackgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom("White");
            _textColor = (SolidColorBrush)new BrushConverter().ConvertFrom("Black");
            _textMargin = new Thickness(0, 0, 0, 0);
            _textFontWeight = FontWeights.Normal;
        }

        public bool StartSession()
        {
            var b =_model.StartSession();
            OnPropertyChanged(nameof(IsActive));

            return b;
        }

        public bool EndSession()
        {
            var b =_model.EndSession();
            OnPropertyChanged(nameof(IsActive));

            return b;
        }

        public void TriggerNotifyAll()
        {
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(WindowBackgroundColor));
            OnPropertyChanged(nameof(TaskName));
            OnPropertyChanged(nameof(TaskTextDisplay));
            OnPropertyChanged(nameof(NowTotalElapsedFormatted));

            OnPropertyChanged(nameof(TextFontName));
            OnPropertyChanged(nameof(TextFontSize));
            OnPropertyChanged(nameof(TextFontWeight));
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(TextWidth));
            OnPropertyChanged(nameof(TextHeight));
            OnPropertyChanged(nameof(TextMargin));

            OnPropertyChanged(nameof(TimerFontName));
            OnPropertyChanged(nameof(TimerFontSize));
            OnPropertyChanged(nameof(TimerFontWeight));
            OnPropertyChanged(nameof(TimerColor));
            OnPropertyChanged(nameof(TimerWidth));
            OnPropertyChanged(nameof(TimerHeight));
            OnPropertyChanged(nameof(TimerMargin));
        }

        public static TaskTimeViewModel CreateNewFile(string path)
        {
            var ttvm = new TaskTimeViewModel();

            ttvm._model = TaskTime.CreateNewFile(path);

            ttvm.RebuildWindowBackground();
            ttvm.RebuildTextMargin();
            ttvm.RebuildTextForeground();
            ttvm.RebuildTextFontWeight();
            ttvm.RebuildTimerMargin();
            ttvm.RebuildTimerForeground();
            ttvm.RebuildTimerFontWeight();

            return ttvm;
        }

        public static TaskTimeViewModel FromFile(string path)
        {
            var ttvm = new TaskTimeViewModel();

            ttvm._model = TaskTime.FromFile(path);

            ttvm.RebuildWindowBackground();
            ttvm.RebuildTextMargin();
            ttvm.RebuildTextForeground();
            ttvm.RebuildTextFontWeight();
            ttvm.RebuildTimerMargin();
            ttvm.RebuildTimerForeground();
            ttvm.RebuildTimerFontWeight();

            return ttvm;
        }

        private void RebuildWindowBackground()
        {
            _windowBackgroundColor = (SolidColorBrush) new BrushConverter().ConvertFrom(WindowBackgroundColorName);
        }

        private void RebuildTextForeground()
        {
            _textColor = (SolidColorBrush)new BrushConverter().ConvertFrom(TextColorName);
        }

        private void RebuildTextFontWeight()
        {
            if (_model.TextFontIsBold)
            {
                _textFontWeight = FontWeights.Bold;
            }
            else
            {
                _textFontWeight = FontWeights.Normal;
            }
        }

        private void RebuildTextMargin()
        {
            _textMargin = new Thickness(_model.TextMarginLeft, _model.TextMarginTop, _model.TextMarginRight, _model.TextMarginBottom);
        }

        private void RebuildTimerForeground()
        {
            _timerColor = (SolidColorBrush)new BrushConverter().ConvertFrom(TimerColorName);
        }

        private void RebuildTimerFontWeight()
        {
            if (_model.TimerFontIsBold)
            {
                _timerFontWeight = FontWeights.Bold;
            }
            else
            {
                _timerFontWeight = FontWeights.Normal;
            }
        }

        private void RebuildTimerMargin()
        {
            _timerMargin = new Thickness(_model.TimerMarginLeft, _model.TimerMarginTop, _model.TimerMarginRight, _model.TimerMarginBottom);
        }
    }
}
