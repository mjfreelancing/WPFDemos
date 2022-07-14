using ReactiveUI;
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using ThrottleInput.ViewModels;

namespace ThrottleInput.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = ViewModel;
            ViewModel = viewModel;

            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // enables / disables view.UserInput when AllowUserQuery toggles state
                this.Bind(ViewModel, vm => vm.AllowUserQuery, view => view.UserInput.IsEnabled)
                    .DisposeWith(disposables);


                Observable
                    .CombineLatest(
                        ViewModel.WhenAnyValue(vm => vm.AllowUserQuery),
                        ViewModel.WhenAnyValue(vm => vm.IsBusy),
                        (allowUserQuery, isBusy) => new
                        {
                            AllowUserQuery = allowUserQuery,
                            IsBusy = isBusy
                        })
                    .Subscribe(state =>
                    {
                        if (!state.IsBusy)
                        {
                            // Update the button text
                            AllowUserQuery.Content = state.AllowUserQuery
                                ? "Disable User Query"
                                : "Allow User Query";
                        }

                        AllowUserQuery.IsEnabled = !state.IsBusy;
                    })
                    .DisposeWith(disposables);



                // Bind the long query button / command
                this.BindCommand(ViewModel, vm => vm.LongAsyncQueryCommand, view => view.LongQuery)
                    .DisposeWith(disposables);



                ViewModel
                    .WhenAnyObservable(vm => vm.LongAsyncQueryCommand.IsExecuting)
                    .Subscribe(isExecuting =>
                    {
                        UserInput.IsEnabled = !isExecuting;
                    })
                    .DisposeWith(disposables);



                // Run a new query when the user input text changes - two approaches are shown

                // Option 1 - executing a command within a subscription
                //Observable
                //    .FromEventPattern<TextChangedEventArgs>(UserInput, nameof(UserInput.TextChanged))
                //    .Throttle(TimeSpan.FromMilliseconds(300))
                //    .ObserveOn(RxApp.MainThreadScheduler)
                //    .Subscribe(eventPattern =>
                //    {
                //        // Not using the event args, just get the current text
                //        // var changes = eventPattern.EventArgs.Changes;

                //        // Execute() is observable - can Subscribe() or await (in an async method) to wait for the results.
                //        ViewModel.UserQueryCommand
                //            .Execute(UserInput.Text)
                //            .ObserveOn(RxApp.MainThreadScheduler)
                //            .Subscribe(results =>
                //            {
                //                foreach (var result in results)
                //                {
                //                    QueryOutput.AppendText($"User Input Query Result: {result}");
                //                    QueryOutput.AppendText(Environment.NewLine);
                //                }
                //            });
                //    })
                //    .DisposeWith(disposables);

                // Option 2: Executing a command (which is an observable) as an async operation using SelectMany()
                Observable
                    .FromEventPattern<TextChangedEventArgs>(UserInput, nameof(UserInput.TextChanged))
                    .Throttle(TimeSpan.FromMilliseconds(300))           // prevent querying on every keystroke
                    .ObserveOn(RxApp.MainThreadScheduler)               // required when access the UI (UserInput.Text on the next line)
                    .SelectMany(eventPattern => ViewModel.UserQueryCommand.Execute(UserInput.Text))
                    .ObserveOn(RxApp.MainThreadScheduler)               // required when updating the UI (QueryOutput below)
                    .Subscribe(results =>
                    {
                        foreach (var result in results)
                        {
                            QueryOutput.AppendText($"User Input Query Result: {result}");
                            QueryOutput.AppendText(Environment.NewLine);
                        }
                    })
                    .DisposeWith(disposables);
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (ViewModel.IsBusy)
            {
                e.Cancel = true;
                MessageBox.Show("The application is still busy...", "Cannot Close App");
            }
        }

        private void OnAllowUserQuery(object sender, RoutedEventArgs e)
        {
            ViewModel.AllowUserQuery = !ViewModel.AllowUserQuery;
        }
    }
}
