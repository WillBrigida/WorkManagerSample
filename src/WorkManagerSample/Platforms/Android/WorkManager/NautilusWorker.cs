using Android.Content;
using AndroidX.Concurrent.Futures;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using WorkManagerSample.Services;

namespace WorkManagerSample.Platforms.Android.WorkManager
{
    public class SampleListenableWorker : ListenableWorker, CallbackToFutureAdapter.IResolver //Assincrono
    {
        public const string TAG = "SampleListenableWorker";

        public SampleListenableWorker(Context context, WorkerParameters workerParams) : base(context, workerParams) { }

        public override IListenableFuture StartWork() => CallbackToFutureAdapter.GetFuture(this);

        Java.Lang.Object? CallbackToFutureAdapter.IResolver.AttachCompleter(CallbackToFutureAdapter.Completer p0)
        {
            Task.Run(async () =>
            {
                SyncUtil.WorkerTracker("\nINICIO");

                IMeuServico _meuServico = MauiApplication.Current.Services.GetRequiredService<IMeuServico>();
                await _meuServico.ChamarServico();

                SyncUtil.WorkerTracker("FIM");

                return p0.Set(Result.InvokeSuccess());
            });

            return TAG;
        }
    }

    public class SampleSyncWorker : Worker // Sincrono
    {
        public const string TAG = "SampleSyncWorker";

        public SampleSyncWorker(Context context, WorkerParameters workerParams) : base(context, workerParams) { }

        public override Result DoWork()
        {
            Task.Run(async () =>
            {
                SyncUtil.WorkerTracker("\nINICIO");

                IMeuServico _meuServico = MauiApplication.Current.Services.GetRequiredService<IMeuServico>();
                await _meuServico.ChamarServico();

                SyncUtil.WorkerTracker("FIM");
            });

            return Result.InvokeSuccess();
        }
    }

    public class SyncUtil
    {
        public static void WorkerTracker(string log) //Monitorar as chamadas para avaliar o funcionamento
        {
#if DEBUG
            var value = Preferences.Get(SampleSyncWorker.TAG, $"INICIANDO MONITORAMENTO {SampleSyncWorker.TAG} - {DateTime.Now}\n");
            Preferences.Set(SampleSyncWorker.TAG, value += $"\n{log} - {DateTime.Now}");
#endif
        }
    }
}
