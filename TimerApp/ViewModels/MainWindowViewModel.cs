using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TimerApp.ViewModels
{
    public class MainWindowViewModel : ActivatableViewModel
    {
        private IDisposable _timerSubscription;
        
        [Reactive]
        public double ElapsedSeconds { get; private set; }

        [Reactive]
        public bool IsRunning { get; set; }

        public ReactiveCommand<Unit, Unit> ResumeTimerCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> PauseTimerCommand { get; private set; }

        protected override void OnActivated(CompositeDisposable disposables)
        {
            var canRun = this
                .WhenAnyValue(vm => vm.IsRunning)
                .Select(isRunning => !isRunning);

            var canPause = this
                .WhenAnyValue(vm => vm.IsRunning)
                .Select(isRunning => isRunning);

            ResumeTimerCommand = ReactiveCommand
                .Create(() =>
                {
                    var currentElapsed = ElapsedSeconds;

                    _timerSubscription = Observable
                        .Interval(TimeSpan.FromSeconds(1))
                        .Select(interval => currentElapsed + interval)
                        .Subscribe(elapsed => ElapsedSeconds = elapsed);

                    IsRunning = true;
                }, canRun)
                .DisposeWith(disposables);

            PauseTimerCommand = ReactiveCommand
                .Create(() =>
                {
                    _timerSubscription.Dispose();
                    _timerSubscription = null;

                    IsRunning = false;
                }, canPause)
                .DisposeWith(disposables);
        }
    }
}