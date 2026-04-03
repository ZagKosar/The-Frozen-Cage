using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.WindowSwitcher
{
    public class WindowSwitcher : MonoBehaviour
    {
        [SerializeField] private List<Window> _windows;
        [SerializeField] private Transform _container;
        [SerializeField] private EventSystem _eventSystem;

        private Dictionary<string, WindowPanel> _windowsPrefabsDictionary;
        private Dictionary<string, WindowPanel> _windowsDictionary;

        private Stack<WindowPanel> _windowsStack;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(_container);
            DontDestroyOnLoad(_eventSystem);
        }

        public void Initialize()
        {
            _windowsPrefabsDictionary = new Dictionary<string, WindowPanel>();
            _windowsDictionary = new Dictionary<string, WindowPanel>();
            _windowsStack = new Stack<WindowPanel>();

            foreach (var window in _windows)
                _windowsPrefabsDictionary[window.Name] = window.Panel;
        }

        public void ShowWindow(string name, bool closePrevious = false)
        {

            if (!_windowsDictionary.TryGetValue(name, out var window))
            {
                window = Instantiate(_windowsPrefabsDictionary[name], _container);
                window.Load();

                _windowsDictionary[name] = window;
            }

            if (_windowsStack.Count > 0 && _windowsStack.Peek() == window)
                return;

            if (closePrevious && _windowsStack.Count > 0)
            {
                var lastWindow = _windowsStack.Pop();
                lastWindow.Close();
            }

            _windowsStack.Push(window);
            window.Open();
        }

        public void CloseWindow(string name)
        {
            if(!_windowsDictionary.TryGetValue(name, out var window))
                return;

            window.Close();

            if (_windowsStack.Count == 0)
                return;

            var filtered = _windowsStack.Where(w => w != window).Reverse().ToList();

            _windowsStack.Clear();

            foreach (var w in filtered)
                _windowsStack.Push(w);
        }

        public void CloseLast()
        {
            if (_windowsStack.Count <= 1)
                return;

            var lastWindow = _windowsStack.Pop();
            
            lastWindow.Close();
        }
    }

    [Serializable]
    public class Window
    {
        public string Name;
        public WindowPanel Panel;
    }
}
