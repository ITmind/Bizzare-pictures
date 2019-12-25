using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Шаблон пустого приложения задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234227

namespace Bizarre_pictures
{
    /// <summary>
    /// Обеспечивает зависящее от конкретного приложения поведение, дополняющее класс Application по умолчанию.
    /// </summary>
    sealed partial class App : Application
    {
        public static new App Current
        {
            get { return Application.Current as App; }
        }

        public SettingsData Settings { get; set; }

        readonly public Random RandomGenerator;

        /// <summary>
        /// Инициализирует одноэлементный объект приложения.  Это первая выполняемая строка разрабатываемого
        /// кода; поэтому она является логическим эквивалентом main() или WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += (s, e) => MarkedUp.AnalyticClient.LogLastChanceException(e);
            RandomGenerator = new Random((int)DateTime.UtcNow.Ticks);
        }

        /// <summary>
        /// Вызывается при обычном запуске приложения пользователем.  Будут использоваться другие точки входа,
        /// если приложение запускается для открытия конкретного файла, отображения
        /// результатов поиска и т. д.
        /// </summary>
        /// <param name="args">Сведения о запросе и обработке запуска.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Не повторяйте инициализацию приложения, если в окне уже имеется содержимое,
            // только обеспечьте активность окна
            if (rootFrame == null)
            {
                // Создание фрейма, который станет контекстом навигации, и переход к первой странице
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Загрузить состояние за ранее приостановленного приложения
                }

                // Размещение фрейма в текущем окне
                Window.Current.Content = rootFrame;
            }

            await LoadSettings();
            MarkedUp.AnalyticClient.Initialize("32d95a78-b50a-4fa2-9ba9-e7be891458ae");

            if (rootFrame.Content == null)
            {
                // Если стек навигации не восстанавливается для перехода к первой странице,
                // настройка новой страницы путем передачи необходимой информации в качестве параметра
                // навигации
                if (!rootFrame.Navigate(typeof(Game), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Обеспечение активности текущего окна
            Window.Current.Activate();
        }

        /// <summary>
        /// Вызывается при приостановке выполнения приложения. Состояние приложения сохраняется
        /// без учета информации о том, будет ли оно завершено или возобновлено с неизменным
        /// содержимым памяти.
        /// </summary>
        /// <param name="sender">Источник запроса приостановки.</param>
        /// <param name="e">Сведения о запросе приостановки.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Сохранить состояние приложения и остановить все фоновые операции
            SaveSettings();
            deferral.Complete();
        }

        private async Task<bool> LoadSettings()
        {
            Settings = await SettingsData.Load() ?? new SettingsData();
            return true;
        }

        private void SaveSettings()
        {
            SettingsData.Save(Settings);
        }
    }
}
