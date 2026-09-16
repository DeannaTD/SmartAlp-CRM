using NewGenBridge.UoW;

namespace NewGenBridge.Services
{
    public class SimpleLoggerService
    {
        IWebHostEnvironment _webHost;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static readonly string _logFilePath = "all-logs.txt";
        public SimpleLoggerService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        public async Task Log(string message)
        {
            string path = Path.Combine(_webHost.WebRootPath, "Logs");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string logFilePath = Path.Combine(path, _logFilePath);

            await _semaphore.WaitAsync();
            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, append: true))
                {
                    await writer.WriteLineAsync($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm")}: {message}");
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}

/*
 ublic class Logger
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private static readonly string _logFilePath = "logs.txt";

    public static async Task LogAsync(string message)
    {
        await _semaphore.WaitAsync();
        try
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))
            {
                await writer.WriteLineAsync($"{DateTime.Now}: {message}");
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

using (StreamWriter sw = new StreamWriter(Path.Combine(path, fileName), true))
{
    sw.WriteLineAsync($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm")} - {message}");
}
            using (StreamWriter sw = new StreamWriter(Path.Combine(path, fileName), true))
{
    sw.WriteLineAsync($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm")} - {message}");
}
            
 */