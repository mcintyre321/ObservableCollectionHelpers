using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ObservableCollectionHelpers
{
    public static class ObservableCollectionHelper
    {
        public static ObservableCollection<T> OnInitOrAdd<T>(this ObservableCollection<T> collection, Action<T> onInitOrAdd)
        {
            return collection.OnInit(onInitOrAdd).OnAdd(onInitOrAdd);
        }

        public static ObservableCollection<T> OnInit<T>(this ObservableCollection<T> collection, Action<T> onInit)
        {
            foreach (var item in collection)
            {
                onInit(item);
            }
            return collection;
        }
        public static ObservableCollection<T> OnAdd<T>(this ObservableCollection<T> collection, Action<T> onAdd)
        {
            collection.CollectionChanged += (o, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems.Cast<object>().OfType<T>())
                    {
                        onAdd(item);
                    }
                }
            };
            return collection;
        }

        public static ObservableCollection<T> OnRemove<T>(this ObservableCollection<T> collection, Action<T> onRemove)
        {
            collection.CollectionChanged += (o, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.NewItems.Cast<object>().OfType<T>())
                    {
                        onRemove(item);
                    }
                }
            };
            return collection;
        }

        public static ObservableCollection<T> OnUpdate<T>(this ObservableCollection<T> collection, Action<T> onUpdate)
        {
            return OnUpdate(collection, (t, args) => onUpdate(t));
        }
        public static ObservableCollection<T> OnUpdate<T>(this ObservableCollection<T> collection, Action<T, PropertyChangedEventArgs> onUpdate)
        {
            PropertyChangedEventHandler handler = (sender, args) => onUpdate((T) sender, args);
            collection.CollectionChanged += (o, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                    {
                        item.PropertyChanged += handler;
                    }
                }
                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                    {
                        item.PropertyChanged -= handler;
                    }
                }
            };
            return collection;
        }
    }
}
