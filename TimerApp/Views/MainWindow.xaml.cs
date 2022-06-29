using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using TimerApp.ViewModels;

namespace TimerApp.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;

            this.WhenActivated(disposables =>
            {
                ViewModel
                    .WhenAnyValue(vm => vm.ElapsedSeconds)
                    .Select(seconds => $"{TimeSpan.FromSeconds(seconds):hh\\:mm\\:ss}")
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(formatted =>
                    {
                        ElapsedTime.Text = formatted;
                    })
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.ResumeTimerCommand, view => view.StartButton)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.PauseTimerCommand, view => view.StopButton)
                    .DisposeWith(disposables);
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel!.IsRunning)
            {
                MessageBox.Show("Stop the timer first");
            }

            e.Cancel = ViewModel.IsRunning;
        }
    }
}
