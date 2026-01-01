using System;
using System.Collections.Generic;
using UnityEngine;

namespace FXnRXn
{
    /// <summary>
    /// Debug levels for filtering messages
    /// </summary>
    public enum DebugLevel
    {
        Info,
        Warning,
        Error,
        Success,
        Custom
    }

    /// <summary>
    /// Abstract interface for debug message handlers
    /// </summary>
    public interface IDebugHandler
    {
        void Log(string message, DebugLevel level, Color color);
        void Clear();
    }


    /// <summary>
    /// Advanced debug system with customized console and on-screen printing
    /// </summary>
    public static class DebugSystem
    {
        // ------------------------------------------ Properties -------------------------------------------------------

        private static bool _isEnabled = true;
        private static DebugLevel _minLevel = DebugLevel.Info;
        private static readonly List<IDebugHandler> _handlers = new List<IDebugHandler>();

        // Color presets
        public static readonly Color InfoColor = Color.cyan;
        public static readonly Color WarningColor = Color.yellow;
        public static readonly Color ErrorColor = Color.red;
        public static readonly Color SuccessColor = Color.green;

        // ---------------------------------------- Public Methods --------------------------------------------------

        /// <summary>
        /// Initialize the debug system
        /// </summary>
        public static void Initialize(bool enableScreenDebug = true)
        {
            _handlers.Clear();
            _handlers.Add(new ConsoleDebugHandler());

            if (enableScreenDebug)
            {
                var screenHandler = new ScreenDebugHandler();
                _handlers.Add(screenHandler);
            }
        }

        /// <summary>
        /// Register a custom debug handler
        /// </summary>
        public static void RegisterHandler(IDebugHandler handler)
        {
            if (handler != null && !_handlers.Contains(handler))
            {
                _handlers.Add(handler);
            }
        }

        /// <summary>
        /// Unregister a debug handler
        /// </summary>
        public static void UnregisterHandler(IDebugHandler handler)
        {
            _handlers.Remove(handler);
        }

        /// <summary>
        /// Enable or disable debug system
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        /// <summary>
        /// Set minimum debug level to display
        /// </summary>
        public static void SetMinLevel(DebugLevel level)
        {
            _minLevel = level;
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        public static void Info(string message, UnityEngine.Object context = null)
        {
            Log(message, DebugLevel.Info, InfoColor, context);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void Warning(string message, UnityEngine.Object context = null)
        {
            Log(message, DebugLevel.Warning, WarningColor, context);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void Error(string message, UnityEngine.Object context = null)
        {
            Log(message, DebugLevel.Error, ErrorColor, context);
        }

        /// <summary>
        /// Log a success message
        /// </summary>
        public static void Success(string message, UnityEngine.Object context = null)
        {
            Log(message, DebugLevel.Success, SuccessColor, context);
        }

        /// <summary>
        /// Log a custom message with specified color
        /// </summary>
        public static void Custom(string message, Color color, UnityEngine.Object context = null)
        {
            Log(message, DebugLevel.Custom, color, context);
        }

        /// <summary>
        /// Log a formatted message with timestamp and caller info
        /// </summary>
        public static void LogFormatted(string message, DebugLevel level, Color color, string callerName = "", UnityEngine.Object context = null)
        {
            if (!_isEnabled || level < _minLevel) return;

            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string levelStr = level.ToString().ToUpper();
            string formattedMessage = $"[{timestamp}] [{levelStr}]";

            if (!string.IsNullOrEmpty(callerName))
            {
                formattedMessage += $" [{callerName}]";
            }

            formattedMessage += $" {message}";

            foreach (var handler in _handlers)
            {
                handler?.Log(formattedMessage, level, color);
            }

            // Also log to Unity console with context
            LogToUnityConsole(formattedMessage, level, color, context);
        }

        /// <summary>
        /// Clear all debug messages
        /// </summary>
        public static void Clear()
        {
            foreach (var handler in _handlers)
            {
                handler?.Clear();
            }
        }

        // ---------------------------------------- Private Methods -------------------------------------------------

        private static void Log(string message, DebugLevel level, Color color, UnityEngine.Object context = null)
        {
            if (!_isEnabled || level < _minLevel) return;

            foreach (var handler in _handlers)
            {
                handler?.Log(message, level, color);
            }

            LogToUnityConsole(message, level, color, context);
        }

        private static void LogToUnityConsole(string message, DebugLevel level, Color color, UnityEngine.Object context)
        {
            string coloredMessage = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";

            switch (level)
            {
                case DebugLevel.Error:
                    UnityEngine.Debug.LogError(coloredMessage, context);
                    break;
                case DebugLevel.Warning:
                    UnityEngine.Debug.LogWarning(coloredMessage, context);
                    break;
                default:
                    UnityEngine.Debug.Log(coloredMessage, context);
                    break;
            }
        }
    }

    /// <summary>
    /// Console debug handler with color support
    /// </summary>
    public class ConsoleDebugHandler : IDebugHandler
    {
        public void Log(string message, DebugLevel level, Color color)
        {
            // Unity console already handles this via DebugSystem.LogToUnityConsole
            // This can be extended for custom console implementations
        }

        public void Clear()
        {
            // Unity doesn't provide API to clear console programmatically
        }
    }

    /// <summary>
    /// On-screen debug message handler
    /// </summary>
    public class ScreenDebugHandler : IDebugHandler
    {
        private readonly List<DebugMessage> _messages = new List<DebugMessage>();
        private readonly int _maxMessages = 10;
        private readonly float _messageDuration = 5f;

        public class DebugMessage
        {
            public string Text;
            public Color Color;
            public float TimeRemaining;
        }

        public void Log(string message, DebugLevel level, Color color)
        {
            _messages.Add(new DebugMessage
            {
                Text = message,
                Color = color,
                TimeRemaining = _messageDuration
            });

            // Remove oldest messages if exceeding max
            while (_messages.Count > _maxMessages)
            {
                _messages.RemoveAt(0);
            }
        }

        public void Clear()
        {
            _messages.Clear();
        }

        public void Update(float deltaTime)
        {
            for (int i = _messages.Count - 1; i >= 0; i--)
            {
                _messages[i].TimeRemaining -= deltaTime;
                if (_messages[i].TimeRemaining <= 0)
                {
                    _messages.RemoveAt(i);
                }
            }
        }

        public void OnGUI()
        {
            if (_messages.Count == 0) return;

            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true
            };

            float yOffset = 10f;
            float padding = 5f;

            foreach (var msg in _messages)
            {
                // Calculate alpha based on remaining time
                float alpha = Mathf.Clamp01(msg.TimeRemaining / _messageDuration);
                Color colorWithAlpha = msg.Color;
                colorWithAlpha.a = alpha;
                style.normal.textColor = colorWithAlpha;

                GUIContent content = new GUIContent(msg.Text);
                Vector2 size = style.CalcSize(content);

                GUI.Label(new Rect(10, yOffset, Screen.width - 20, size.y), msg.Text, style);
                yOffset += size.y + padding;
            }
        }

        public List<DebugMessage> GetMessages() => _messages;
    }

    /// <summary>
    /// MonoBehaviour component to handle on-screen debug rendering
    /// </summary>
    public class DebugScreenDisplay : MonoBehaviour
    {
        private ScreenDebugHandler _screenHandler;

        private void Awake()
        {
            // Find the screen handler
            DontDestroyOnLoad(gameObject);
        }

        public void SetHandler(ScreenDebugHandler handler)
        {
            _screenHandler = handler;
        }

        private void Update()
        {
            _screenHandler?.Update(Time.deltaTime);
        }

        private void OnGUI()
        {
            _screenHandler?.OnGUI();
        }
    }

    /// <summary>
    /// Legacy Debug class wrapper for backward compatibility
    /// </summary>
    public class LittleAdventureDebug : MonoBehaviour
    {
	    // ------------------------------------------ Properties -------------------------------------------------------

        [SerializeField] private bool enableOnScreenDebug = true;
        [SerializeField] private DebugLevel minDebugLevel = DebugLevel.Info;

  	    // ---------------------------------------- Unity Callback -----------------------------------------------------

        private void Awake()
        {
            DebugSystem.Initialize(enableOnScreenDebug);
            DebugSystem.SetMinLevel(minDebugLevel);

            if (enableOnScreenDebug)
            {
                // Create screen display component
                var displayObj = new GameObject("DebugScreenDisplay");
                var display = displayObj.AddComponent<DebugScreenDisplay>();

                // Find and set the screen handler
                var handlers = typeof(DebugSystem)
                    .GetField("_handlers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    ?.GetValue(null) as List<IDebugHandler>;

                if (handlers != null)
                {
                    foreach (var handler in handlers)
                    {
                        if (handler is ScreenDebugHandler screenHandler)
                        {
                            display.SetHandler(screenHandler);
                            break;
                        }
                    }
                }
            }

            // Example usage
            DebugSystem.Info("Debug System Initialized!");
            DebugSystem.Success("All systems operational!");
        }

    	// ---------------------------------------- Public Properties --------------------------------------------------

        /// <summary>
        /// Enable or disable debug system at runtime
        /// </summary>
        public void SetDebugEnabled(bool enabled)
        {
            DebugSystem.SetEnabled(enabled);
        }

        /// <summary>
        /// Clear all on-screen debug messages
        /// </summary>
        public void ClearDebugMessages()
        {
            DebugSystem.Clear();
        }

    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
    
    
    // *****************************************************************************************************************
    // // Info message (cyan color)
   // DebugSystem.Info("This is an info message");

    // // Warning message (yellow color)
   // DebugSystem.Warning("This is a warning message");

    // // Error message (red color)
    // DebugSystem.Error("This is an error message");

    // // Success message (green color)
    // DebugSystem.Success("Operation completed successfully!");

    // // Custom colored message
    // DebugSystem.Custom("This is a custom purple message", new Color(0.5f, 0f, 0.5f));
    
    // // Formatted message with timestamp and caller info
    // DebugSystem.LogFormatted(
    //     "Player health updated",
    //     DebugLevel.Info,
    //     Color.cyan,
    //     "HealthSystem"
    // );
    //
    // // Log with context (clickable in Unity console)
    // DebugSystem.Info("Click this message to highlight the GameObject", this);
    //
    // // Control debug level filtering
    // DebugSystem.SetMinLevel(DebugLevel.Warning); // Only show warnings and errors
    // DebugSystem.Info("This won't be displayed");
    // DebugSystem.Warning("This will be displayed");
    //
    // // Re-enable all levels
    // DebugSystem.SetMinLevel(DebugLevel.Info);
    //
    // // Enable/disable debug system
    // DebugSystem.SetEnabled(false);
    // DebugSystem.Info("This won't be displayed");
    // DebugSystem.SetEnabled(true);
    // DebugSystem.Info("Debug system re-enabled!");
    
    // // Create and register a custom debug handler
    // var customHandler = new FileDebugHandler("debug_log.txt");
    // DebugSystem.RegisterHandler(customHandler);
    //
    // DebugSystem.Info("This message will also be written to file!");
    //
    // // Unregister when done
    // DebugSystem.UnregisterHandler(customHandler);
    
    // // Example: Log player position every second
    // if (Time.frameCount % 60 == 0)
    // {
    //     DebugSystem.Custom(
    //         $"Player Position: {transform.position}",
    //         Color.magenta
    //     );
    // }
    
    // // Clear all on-screen messages
    // DebugSystem.Clear();
    
    // /// <summary>
    // /// Example custom debug handler that writes to a file
    // /// </summary>
    // public class FileDebugHandler : IDebugHandler
    // {
    //     private readonly string _filePath;
    //
    //     public FileDebugHandler(string filePath)
    //     {
    //         _filePath = Application.persistentDataPath + "/" + filePath;
    //     }
    //
    //     public void Log(string message, DebugLevel level, Color color)
    //     {
    //         try
    //         {
    //             string logEntry = $"[{System.DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
    //             System.IO.File.AppendAllText(_filePath, logEntry);
    //         }
    //         catch (System.Exception e)
    //         {
    //             UnityEngine.Debug.LogError($"Failed to write to debug file: {e.Message}");
    //         }
    //     }
    //
    //     public void Clear()
    //     {
    //         try
    //         {
    //             if (System.IO.File.Exists(_filePath))
    //             {
    //                 System.IO.File.Delete(_filePath);
    //             }
    //         }
    //         catch (System.Exception e)
    //         {
    //             UnityEngine.Debug.LogError($"Failed to clear debug file: {e.Message}");
    //         }
    //     }
    // }
}