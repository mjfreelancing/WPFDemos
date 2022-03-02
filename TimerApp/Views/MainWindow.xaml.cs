using System;
using System.ComponentModel;
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
                    });

                this.BindCommand(ViewModel, vm => vm.ResumeTimerCommand, view => view.StartButton);
                this.BindCommand(ViewModel, vm => vm.PauseTimerCommand, view => view.StopButton);
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
