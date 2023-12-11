using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using WorkManagerSample.Services;

namespace WorkManagerSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureWorkManagerToDataSync()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddScoped<IMeuServico, MeuServico>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        public static MauiAppBuilder ConfigureWorkManagerToDataSync(this MauiAppBuilder mauiAppBuilder)
        {
            mauiAppBuilder.ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android => android
                   .OnCreate((activity, bundle) =>
                   {
                       //Preferences.Remove(Platforms.Android.WorkManager.SampleSyncWorker.TAG);
                       //WorkManager.GetInstance(activity).CancelAllWorkByTag(Platforms.Android.WorkManager.SampleSyncWorker.TAG);

                       //ref: https://developer.android.com/topic/libraries/architecture/workmanager/how-to/define-work?hl=pt-br#java

                       const int REPLAY_TIME = 30;

                       using var builder = new AndroidX.Work.Constraints.Builder();
                       builder.SetRequiredNetworkType(AndroidX.Work.NetworkType.Connected!);
                       var constraints = builder.Build();

                       var request = AndroidX.Work.PeriodicWorkRequest.Builder.From<Platforms.Android.WorkManager.SampleSyncWorker>(TimeSpan.FromMinutes(REPLAY_TIME))
                           .SetConstraints(constraints)
                           .AddTag(Platforms.Android.WorkManager.SampleSyncWorker.TAG)
                           .Build();

                       AndroidX.Work.WorkManager.GetInstance(activity)
                            .EnqueueUniquePeriodicWork(nameof(Platforms.Android.WorkManager.SampleSyncWorker), AndroidX.Work.ExistingPeriodicWorkPolicy.Replace!, request);

                   }));
            });

            return mauiAppBuilder;
        }
    }


}
