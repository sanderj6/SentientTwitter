using Microsoft.AspNetCore.Components;

namespace SentientTwitter.Services
{
    public class ModalService
    {
        // Actions for Show/Close Events
        public event Action<string, string, bool, RenderFragment> OnShow;
        public event Action OnHide;
        public event Action<string> ChangeTitle;

        public void SetTitle(string title)
        {
            ChangeTitle?.Invoke(title);
        }

        public void Show<T>(string title, string classSize, bool dismissable, Type contentType, Action<T> callback)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (!typeof(ComponentBase).IsAssignableFrom(contentType))
                throw new ArgumentException($"{contentType.FullName} must be a Component");

            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, contentType);
                x.AddAttribute(3, "Callback", callback);
                x.CloseComponent();
            });

            OnShow?.Invoke(title, classSize, dismissable, content);
        }

        public void Show<T, TU>(string title, string classSize, bool dismissable, Type contentType, T model, Action<TU> callback)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            if (!typeof(ComponentBase).IsAssignableFrom(contentType))
                throw new ArgumentException($"{contentType.FullName} must be a Component");

            var content = new RenderFragment(x =>
            {
                x.OpenComponent(1, contentType);
                x.AddAttribute(3, "Model", model);
                x.AddAttribute(4, "Callback", callback);
                x.CloseComponent();
            });

            OnShow?.Invoke(title, classSize, dismissable, content);
        }

        public void Hide()
        {
            OnHide?.Invoke();
        }
    }

    public class ModalConfig
    {
        public string Title { get; set; }
        public bool Dismissable { get; set; }
        public bool OverlayDismissable { get; set; }
        public Type Content { get; set; }
    }
}