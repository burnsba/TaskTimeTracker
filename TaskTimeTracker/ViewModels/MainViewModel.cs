using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using BurnsBac.WindowsAppToolkit;
using BurnsBac.WindowsAppToolkit.Mvvm;
using TaskTimeTracker.Windows;

namespace TaskTimeTracker.ViewModels
{
    /// <summary>
    /// Main window view model.
    /// </summary>
    public class MainViewModel : WindowViewModelBase
    {
        private Timer _elapsedUiRefreshTimer;
        private int _elapsedUiRefreshIntervalMs = 1000;

        private TaskTimeViewModel _activeTaskTime = null;
        private SolidColorBrush _backgroundColor = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
        private string _backgroundColorString = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            ReadConfig();
        }

        /// <summary>
        /// Gets or sets currently active task.
        /// </summary>
        public TaskTimeViewModel ActiveTaskTime
        {
            get
            {
                return _activeTaskTime;
            }

            set
            {
                _activeTaskTime = value;
                OnPropertyChanged(nameof(ActiveTaskTime));
                OnPropertyChanged(nameof(BackgroundColor));
                OnPropertyChanged(nameof(ActiveTaskTitle));
                //OnPropertyChanged(nameof(TaskTextDisplay));
                OnPropertyChanged(nameof(CanStartTask));
                OnPropertyChanged(nameof(CanStopTask));
                OnPropertyChanged(nameof(NowTotalElapsedFormatted));
            }
        }

        /// <summary>
        /// Gets or sets the main window background color.
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (!object.ReferenceEquals(null, _activeTaskTime))
                {
                    return _activeTaskTime.WindowBackgroundColor;
                }

                return _backgroundColor;
            }

            set
            {
                _backgroundColor = value;
                _backgroundColorString = $"#{value.Color.R.ToString("X2")}{value.Color.G.ToString("X2")}{value.Color.B.ToString("X2")}";
                OnPropertyChanged(nameof(BackgroundColor));
                OnPropertyChanged(nameof(BackgroundColorString));
            }
        }

        /// <summary>
        /// Gets or sets the main window background color, as a hex string.
        /// </summary>
        public string BackgroundColorString
        {
            get
            {
                return _backgroundColorString;
            }

            set
            {
                _backgroundColorString = value;
                _backgroundColor = (SolidColorBrush)new BrushConverter().ConvertFrom(value);
                OnPropertyChanged(nameof(BackgroundColorString));
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public bool CanStartTask
        {
            get
            {
                return !object.ReferenceEquals(null, _activeTaskTime) && !_activeTaskTime.IsActive;
            }
        }

        public bool CanStopTask
        {
            get
            {
                return !object.ReferenceEquals(null, _activeTaskTime) && _activeTaskTime.IsActive;
            }
        }

        public bool CanCreateTask
        {
            get
            {
                return object.ReferenceEquals(null, _activeTaskTime) || !_activeTaskTime.IsActive;
            }
        }

        public bool CanOpenTask
        {
            get
            {
                return object.ReferenceEquals(null, _activeTaskTime) || !_activeTaskTime.IsActive;
            }
        }

        public string ActiveTaskTitle
        {
            get
            {
                if (!object.ReferenceEquals(null, _activeTaskTime))
                {
                    return _activeTaskTime.TaskName;
                }

                return string.Empty;
            }
        }

        public string NowTotalElapsedFormatted
        {
            get
            {
                if (!object.ReferenceEquals(null, _activeTaskTime))
                {
                    return _activeTaskTime.NowTotalElapsedFormatted;
                }

                return TimeSpan.Zero.ToString();
            }
        }

        public void StartSession()
        {
            bool started = false;

            if (!object.ReferenceEquals(null, _activeTaskTime))
            {
                started = _activeTaskTime.StartSession();
                OnPropertyChanged(nameof(CanStartTask));
                OnPropertyChanged(nameof(CanStopTask));
                OnPropertyChanged(nameof(CanCreateTask));
                OnPropertyChanged(nameof(CanOpenTask));
            }

            if (started)
            {
                if (!object.ReferenceEquals(null, _elapsedUiRefreshTimer))
                {
                    throw new InvalidOperationException("ui timer already active?");
                }

                _elapsedUiRefreshTimer = new Timer();
                _elapsedUiRefreshTimer.AutoReset = true;
                _elapsedUiRefreshTimer.Interval = _elapsedUiRefreshIntervalMs;
                _elapsedUiRefreshTimer.Elapsed += _elapsedUiRefreshTimer_Elapsed;
                _elapsedUiRefreshTimer.Start();

                // It's possible the user could start the timer then stop it before
                // the first update interval. Go ahead and update the ui at least
                // once to keep things in sync.
                OnPropertyChanged(nameof(NowTotalElapsedFormatted));
            }
        }

        private void _elapsedUiRefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(NowTotalElapsedFormatted));
        }

        public void EndSession()
        {
            bool ended = false;

            if (!object.ReferenceEquals(null, _activeTaskTime))
            {
                ended = _activeTaskTime.EndSession();
                OnPropertyChanged(nameof(CanStartTask));
                OnPropertyChanged(nameof(CanStopTask));
                OnPropertyChanged(nameof(CanCreateTask));
                OnPropertyChanged(nameof(CanOpenTask));
            }

            if (ended)
            {
                _elapsedUiRefreshTimer.Stop();
                _elapsedUiRefreshTimer.Close();
                _elapsedUiRefreshTimer.Dispose();
                _elapsedUiRefreshTimer = null;
            }
        }

        /// <summary>
        /// Reads app.config and sets global settings.
        /// </summary>
        private void ReadConfig()
        {
            var s = ConfigurationManager.AppSettings["BackgroundColor"];
            if (string.IsNullOrEmpty(s))
            {
                s = "White";
            }

            BackgroundColorString = s;
        }
    }
}