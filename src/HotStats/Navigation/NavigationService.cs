using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotStats.Navigation
{
    public interface INavigationService : GalaSoft.MvvmLight.Views.INavigationService
    {
        void AddPage(NavigationFrame navigationFrame);
        void NavigateTo(NavigationFrame navigationFrame);
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Uri> pagesByKey;

        public NavigationService()
        {
            pagesByKey = new Dictionary<string, Uri>();
        }

        public object Parameter { get; private set; }

        //If ever needed, implement as observable property
        public string CurrentPageKey { get; set; }

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(NavigationFrame navigationFrame)
        {
            NavigateTo(navigationFrame.Name, null);
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public virtual void NavigateTo(string pageKey, object parameter)
        {
            lock (pagesByKey)
            {
                if (!pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException($"No such page: {pageKey} ");
                }

                var frame = GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;

                if (frame != null)
                {
                    frame.Source = pagesByKey[pageKey];
                }
                Parameter = parameter;
                CurrentPageKey = pageKey;
            }
        }

        public void AddPage(NavigationFrame navigationFrame)
        {
            AddPage(navigationFrame.Name, navigationFrame.Path);
        }

        public void AddPage(string key, Uri pageType)
        {
            lock (pagesByKey)
            {
                if (pagesByKey.ContainsKey(key))
                {
                    pagesByKey[key] = pageType;
                }
                else
                {
                    pagesByKey.Add(key, pageType);
                }
            }
        }

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (var i = 0; i < count; i++)
            {
                var frameworkElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (frameworkElement == null) continue;
                if (frameworkElement.Name == name)
                {
                    return frameworkElement;
                }

                frameworkElement = GetDescendantFromName(frameworkElement, name);
                if (frameworkElement != null)
                {
                    return frameworkElement;
                }
            }
            return null;
        }
    }
}