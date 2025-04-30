using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Pixelatte.UI.Behaviors
{
    internal class LocalServerBehavior : Behavior<ToggleButton>
    {
        public static readonly DependencyProperty OutputTextBoxProperty =
            DependencyProperty.Register(
                nameof(OutputTextBox),
                typeof(TextBox),
                typeof(LocalServerBehavior),
                new PropertyMetadata(null));

        public TextBox OutputTextBox
        {
            get { return (TextBox)GetValue(OutputTextBoxProperty); }
            set { SetValue(OutputTextBoxProperty, value); }
        }

        public static readonly DependencyProperty IsLocalServerRunningProperty =
            DependencyProperty.Register(
                nameof(IsLocalServerRunning),
                typeof(bool),
                typeof(LocalServerBehavior),
                new PropertyMetadata(false));

        public bool IsLocalServerRunning
        {
            get { return (bool)GetValue(IsLocalServerRunningProperty); }
            set { SetValue(IsLocalServerRunningProperty, value); }
        }

        public static readonly DependencyProperty IsLocalServerLoadingProperty =
            DependencyProperty.Register(
                nameof(IsLocalServerLoading),
                typeof(bool),
                typeof(LocalServerBehavior),
                new PropertyMetadata(false));

        public bool IsLocalServerLoading
        {
            get { return (bool)GetValue(IsLocalServerLoadingProperty); }
            set { SetValue(IsLocalServerLoadingProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Click += AssociatedObject_Click;
        }

        private async void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            await RunLocalServer();
        }

        private async void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            App.m_window.Closed += M_window_Closed;
            await RunLocalServer();
        }

        private void M_window_Closed(object sender, WindowEventArgs args)
        {
            StopLocalServer();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Click -= AssociatedObject_Click;
            App.m_window.Closed -= M_window_Closed;
        }

        private async Task RunLocalServer()
        {
            this.IsLocalServerLoading = true;

            if (this.IsLocalServerRunning)
            {
                StopLocalServer();
            }
            else
            {
                await StartLocalServer();
            }
        }

        public static async Task<bool> FolderExistsAsync(StorageFolder parentFolder, string folderName)
        {
            try
            {
                var item = await parentFolder.TryGetItemAsync(folderName);
                return item != null && item.IsOfType(StorageItemTypes.Folder);
            }
            catch
            {
                return false;
            }
        }

        private async Task StartLocalServer()
        {
            try
            {
                await CreateBackend();
            }
            catch (Exception ex)
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    OutputTextBox.Text += $"Error: {ex.Message}" + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                });
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    if (e.Data.ToLower().Contains("http://0.0.0.0:8000 (Press CTRL+C to quit)", StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.IsLocalServerRunning = true;
                        this.IsLocalServerLoading = false;
                    }
                    OutputTextBox.Text += e.Data + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                });
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    if (e.Data.ToLower().Contains("http://0.0.0.0:8000 (Press CTRL+C to quit)", StringComparison.CurrentCultureIgnoreCase))
                    {
                        this.IsLocalServerRunning = true;
                        this.IsLocalServerLoading = false;
                    }
                    OutputTextBox.Text += e.Data + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                });
            }
        }

        private async Task CreateBackend()
        {
            StorageFolder installedLocationFolder = Package.Current.InstalledLocation;
            StorageFolder backendFolder = await installedLocationFolder.GetFolderAsync("Backend");
            IReadOnlyList<StorageFile> backendFiles = await backendFolder.GetFilesAsync();

            var localFolder = ApplicationData.Current.LocalFolder;

            var localBackendFolder = await CreateFolderAsync(localFolder, backendFolder.Name);
            if (localBackendFolder == null) return;

            foreach (var file in backendFiles)
            {
                try
                {
                    await file.CopyAsync(localBackendFolder, file.Name, NameCollisionOption.FailIfExists);
                    OutputTextBox.Text += $"File {file.Name} created." + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                }
                catch (Exception)
                {
                    OutputTextBox.Text += $"File {file.Name} already exists." + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                }
            }

            string virtualEnvDirName = ".venv";
            string virtualEnvDirPath = Path.Combine(localBackendFolder.Path, virtualEnvDirName);
            string virtualEnvActivateFilePath = $"echo Activating virtual environment... && {Path.Combine(virtualEnvDirPath, "Scripts", "activate.bat")}";
            string dependenciesFilePath = Path.Combine(localBackendFolder.Path, "requirements.txt");
            string apiFilePath = Path.Combine(localBackendFolder.Path, "api.py");

            string fastApiCommand = $"echo Running local server... && fastapi run {apiFilePath}";
            string installDependenciesCommand = $"echo Installing dependencies... && pip install -r {dependenciesFilePath}";
            string createVirtualEnvCommand = $"echo Creating virtual environment... && python -m venv {virtualEnvDirPath}";

            if (!await FolderExistsAsync(localBackendFolder, virtualEnvDirName))
            {
                await ExecuteCommand(filename: "cmd.exe", workdir: localBackendFolder.Path, command: $"/c {createVirtualEnvCommand} && {virtualEnvActivateFilePath} && {installDependenciesCommand} && {fastApiCommand}");
            }
            else
            {
                await ExecuteCommand(filename: "cmd.exe", workdir: localBackendFolder.Path, command: $"/c {virtualEnvActivateFilePath} && {fastApiCommand}");
            }
        }

        private async Task<StorageFolder?> CreateFolderAsync(StorageFolder parentFolder, string folderName)
        {
            try
            {
                if (!await FolderExistsAsync(parentFolder, folderName))
                {
                    StorageFolder newFolder = await parentFolder.CreateFolderAsync(folderName);
                    OutputTextBox.Text += $"Folder '{folderName}' created." + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                    return newFolder;
                }
                else
                {
                    OutputTextBox.Text += $"Folder '{folderName}' already exists." + Environment.NewLine;
                    OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
                    return await parentFolder.GetFolderAsync(folderName);
                }
            }
            catch (Exception ex)
            {
                OutputTextBox.Text += $"Error creating folder: {ex.Message}" + Environment.NewLine;
                OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
            }

            return null;
        }

        private Process _serverProcess = null;
        private async Task ExecuteCommand(string filename, string workdir, string command = null)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = command,
                WorkingDirectory = workdir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            processStartInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8";

            _serverProcess = new Process();
            _serverProcess.StartInfo = processStartInfo;
            _serverProcess.OutputDataReceived += Process_OutputDataReceived;
            _serverProcess.ErrorDataReceived += Process_ErrorDataReceived;
            _serverProcess.Start();
            _serverProcess.BeginOutputReadLine();
            _serverProcess.BeginErrorReadLine();
            await _serverProcess.WaitForExitAsync();
            this.AssociatedObject.IsChecked = false;
            this.IsLocalServerRunning = false;
            this.IsLocalServerLoading = false;
            _serverProcess.OutputDataReceived -= Process_OutputDataReceived;
            _serverProcess.ErrorDataReceived -= Process_ErrorDataReceived;
            _serverProcess.Dispose();

            OutputTextBox.Text += "Server stopped." + Environment.NewLine;
            OutputTextBox.SelectionStart = OutputTextBox.Text.Length;
        }

        private void StopLocalServer()
        {
            this.IsLocalServerLoading = true;
            _serverProcess.CancelOutputRead();
            _serverProcess.CancelErrorRead();
            _serverProcess.Kill(true);
            _serverProcess.Close();
        }
    }
}
