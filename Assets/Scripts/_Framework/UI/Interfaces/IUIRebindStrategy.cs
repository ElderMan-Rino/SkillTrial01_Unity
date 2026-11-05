namespace Elder.Framework.UI.Interfaces
{
    // Wake 시 View를 어떤 방식으로 재바인딩할지 캡슐화. UIEntryHandle이 구체 바인딩 방식(VisualElement 등)을 알 필요가 없도록 분리.
    public interface IUIRebindStrategy
    {
        public void Rebind(IUIView view);
    }
}
