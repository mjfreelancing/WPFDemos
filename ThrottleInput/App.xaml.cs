using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ThrottleInput.ViewModels;
using ThrottleInput.Views;

namespace ThrottleInput
{
    public partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        protected override void OnStartup(StartupEventArgs eventArgs)
        {
            base.OnStartup(eventArgs);

            var services = new ServiceCollection();

            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            //builder.AddUserSecrets<AuthDemoSettings>();
            //var configuration = builder.Build();

            //services.Configure<AuthDemoSettings>(configuration);
            //services.AddSingleton(configuration);

            //services.AddCognito(
            //   provider =>
            //   {
            //       var options = provider.GetRequiredService<IOptions<AuthDemoSettings>>().Value;

            //       return new CognitoClientConfig
            //       {
            //           ClientId = options.Auth.ClientId,
            //           AuthUri = options.Auth.AuthUri,
            //           RedirectUri = options.Auth.RedirectUri,
            //           CognitoActionUri = options.Auth.CognitoActionUri,
            //           Serializer = provider.GetRequiredService<IJsonSerializer>()
            //       };
            //   });

            //services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
            //services.AddSingleton<IUserStore, UserStore>();
            //services.AddScoped<IUserAuthenticator, UserAuthenticator>();
            //services.AddSingleton<IViewFactory, ViewFactory>();

            //services.AddTransient<IViewFor<LoginWindowViewModel>, LoginWindow>();
            //services.AddTransient<LoginWindowViewModel>();
            //services.AddTransient<LoginWindow>();

            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<MainWindow>();

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<MainWindow>().Show();
            //StartApplication(serviceProvider);
        }

        //private static void StartApplication(ServiceProvider serviceProvider)
        //{
        //    var authenticator = serviceProvider.GetService<IUserAuthenticator>();
        //    var userAuthContext = authenticator!.Login();

        //    if (userAuthContext.Status == AuthenticationStatus.Authenticated)
        //    {
        //        serviceProvider.GetService<MainWindow>().Show();
        //    }
        //    else
        //    {
        //        Current.Shutdown();
        //    }
        //}
    }
}
