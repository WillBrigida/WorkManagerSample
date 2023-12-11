using WorkManagerSample.Platforms.Android.WorkManager;

namespace WorkManagerSample.Services
{
    internal interface IMeuServico
    {
        Task ChamarServico();
    }

    internal class MeuServico : IMeuServico
    {
        public async Task ChamarServico()
        {
            SyncUtil.WorkerTracker($"Entrou no método ChamarServico");
        }
    }
}
