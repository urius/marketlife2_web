using System;
using Data;

namespace Model.Popups
{
    public abstract class PopupViewModelBase : IDisposable
    {
        public abstract PopupKey PopupKey { get; }
        
        public abstract void Dispose();
    }
}