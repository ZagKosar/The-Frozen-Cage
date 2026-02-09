using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Scripts.WindowSwitcher
{
    public class WindowSwitcher : MonoBehaviour
    {
        [SerializeField] private List<Window> _windows;
        [SerializeField] private Transform _container;

        private static WindowSwitcher s_instance;
        public static WindowSwitcher Instance
        {
            get
            {
                if (!s_instance)
                {
                    s_instance = FindFirstObjectByType<WindowSwitcher>();
                }
                return s_instance;
            }
        }

        private Dictionary<string, WindowPanel> _windowsPrefabsDictionary;
        private Dictionary<string, WindowPanel> _windowsDictionary;

        private Stack<WindowPanel> _windowsStack;

        private void Awake()
        {
            s_instance = this;

            DontDestroyOnLoad(s_instance);
        }

        public void Initialize()
        {
            _windowsPrefabsDictionary = new Dictionary<string, WindowPanel>();
            _windowsDictionary = new Dictionary<string, WindowPanel>();

            foreach (var window in _windows)
                _windowsPrefabsDictionary[window.Name] = window.Panel;
        }

        public void ShowWindow(string name, bool closePrevious = false)
        {

            if (!_windowsDictionary.TryGetValue(name, out var window))
            {
                window = Instantiate(_windowsPrefabsDictionary[name], _container);
                _windowsDictionary[name] = window;
            }

            if (closePrevious && _windowsStack.Count > 0)
            {
                var lastWindow = _windowsStack.Pop();
                lastWindow.Close();
            }

            _windowsStack.Push(window);
            window.Open();
        }
    }

    [Serializable]
    public class Window
    {
        public string Name;
        public WindowPanel Panel;
    }
}
