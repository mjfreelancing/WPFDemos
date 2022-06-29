using AllOverIt.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThrottleInput.ViewModels
{
    public class MainWindowViewModel : ActivatableViewModel
    {
        [Reactive]
        public bool AllowUserQuery { get; set; }

        [ObservableAsProperty]
        public bool IsBusy { get; }

        public ReactiveCommand<string, IReadOnlyCollection<string>> UserQueryCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> LongAsyncQueryCommand { get; private set; }

        protected override void OnActivated(CompositeDisposable disposables)
        {
            AllowUserQuery = false;

            var canExecute = this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy);

            UserQueryCommand = ReactiveCommand
                .CreateFromTask<string, IReadOnlyCollection<string>>(async (query, cancellationToken) =>
                {
                    return await GetUserInputQueryAsync(query, cancellationToken).ConfigureAwait(false);
                }, canExecute);

            // The associated button on the UI will auto-disable when executing
            LongAsyncQueryCommand = ReactiveCommand
                .CreateFromTask(async cancellationToken =>
                {
                    await Task.Delay(10000, cancellationToken).ConfigureAwait(false);
                }, canExecute);


            Observable
                .CombineLatest(
                    UserQueryCommand.IsExecuting,
                    LongAsyncQueryCommand.IsExecuting,
                    (userQuery, longQuery) => 
                    {
                        return userQuery || longQuery;
                    })
                .ToPropertyEx(this, vm => vm.IsBusy);
        }

        private static async Task<IReadOnlyCollection<string>> GetUserInputQueryAsync(string input, CancellationToken cancellationToken)
        {
            await Task.Delay(500, cancellationToken).ConfigureAwait(false);        // emulate a query delay

            return new List<string>
            { 
                "Result 1",
                "Result 2",
                "Result 3",
                "Result 4",
                "Result 5",
            }.AsReadOnlyCollection();
        }
    }
}