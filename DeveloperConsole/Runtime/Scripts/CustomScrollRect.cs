using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Anarkila.DeveloperConsole {

    /// <summary>
    /// Unity ScrollRect component but without click drag function
    /// </summary>
    public class CustomScrollRect : ScrollRect {
        public override void OnBeginDrag(PointerEventData eventData) { }
        public override void OnDrag(PointerEventData eventData) { }
        public override void OnEndDrag(PointerEventData eventData) { }
    }
}