using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using BurnSystems.Logging;

namespace BurnSystems.WPF
{
    /// <summary>
    /// This class connects the logging framework
    /// to a WPF textblock, so every logging event is also shown in the WPF textblock
    /// </summary>
    public class TextBlockLogProvider : ILogProvider
    {
        private readonly TextBlock _textBlock;

        public TextBlockLogProvider(TextBlock textBlock)
        {
            _textBlock = textBlock ?? throw new ArgumentNullException(nameof(textBlock));
        }

        public void LogMessage(LogMessage logMessage)
        {
            Debug.Assert(_textBlock.Dispatcher != null, "_textBlock.Dispatcher != null");

            _textBlock.Dispatcher.Invoke(() =>
            {
                _textBlock.Text =
                    $"[{logMessage.LogLevel}]: {logMessage.Message}";
            });
        }
    }
}