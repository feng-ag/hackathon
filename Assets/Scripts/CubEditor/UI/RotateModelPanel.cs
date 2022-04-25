using UnityEngine;
using UnityEngine.EventSystems;
namespace ArcadeGalaxyKit
{
    public class RotateModelPanel : MonoBehaviour
         , IPointerDownHandler
         , IDragHandler
         , IPointerEnterHandler
         , IPointerExitHandler
         , IPointerUpHandler
    {
        bool isPointerEnter = false;
        bool isPointerExit = false;
        bool isPointerDragging = false;
        Vector2 clickPosition;
        Quaternion lastRotation;
        public float smoothRotate = 0.2f;
        [Header("UI需選轉的場景物件")]
        public GameObject objToRotate;

        /// <summary>
        /// Rotate cub model by x degree
        /// </summary>
        public void PreviewRotateModelY(float xOffset)
        {
            if (objToRotate)
            {
                Vector3 temp = objToRotate.transform.localRotation.eulerAngles;
                Quaternion quaternion = objToRotate.transform.localRotation;
                temp.y = lastRotation.eulerAngles.y+xOffset;
                quaternion.eulerAngles = temp;
                objToRotate.transform.localRotation = quaternion;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (isPointerEnter && isPointerDragging && !isPointerExit)
            {
                var offsetRotate = (clickPosition.x - eventData.position.x) * smoothRotate;
                PreviewRotateModelY(offsetRotate);
                isPointerDragging = true;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerEnter = true;
            isPointerExit = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isPointerDragging) {
                var offsetRotate = (clickPosition.x - eventData.position.x) * smoothRotate;
                PreviewRotateModelY(offsetRotate);
            }
            isPointerEnter = false;
            isPointerExit = true;
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPointerDragging) {
                if (isPointerDragging) {
                    var offsetRotate = (clickPosition.x - eventData.position.x) * smoothRotate;
                    PreviewRotateModelY(offsetRotate);
                }
                isPointerDragging = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDragging = true;
            clickPosition = eventData.position;
            lastRotation = objToRotate.transform.localRotation;
        }
    }
}