using Elder.Framework.Core.Definitions;
using Elder.Framework.Core.UI.App;
using Elder.Framework.Core.UI.Interfaces;
using Elder.Framework.Log.Helper;
using Elder.Framework.Log.Interfaces;
using System;

namespace Elder.Framework.Core.UI.Infra
{
    public class UIViewFinder : IUIViewFinder
    {
        private ILoggerEx _logger = LogFacade.GetLogger<UIViewFinder>();
        public Type ClassType => typeof(UIViewFinder);
        public int Scope => GlobalScope.Id;

        public bool TryFindUIView<T>(out T view) where T : UIViewBase
        {
            view = UnityEngine.Object.FindAnyObjectByType<T>();
            if (view == null)¡¡_logger.Error($"Failed to find UIView: {typeof(T).Name}."); // [HEAP]
            return view != null;
        }
    }
}