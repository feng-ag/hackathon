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

        [Header("UI需選轉的場景物件")]
        public GameObject objToRotate;

        /// <summary>
        /// Rotate cub model by x degree
        /// </summary>
        public void SetRotateModelY(float xOffset)
        {
            if (objToRotate)
            {
                Vector3 temp = objToRotate.transform.localRotation.eulerAngles;
                Quaternion quaternion = objToRotate.transform.localRotation;
                temp.y = xOffset;
                quaternion.eulerAngles = temp;
                objToRotate.transform.localRotation = quaternion;
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (isPointerEnter && isPointerDragging && !isPointerExit)
            {
                SetRotateModelY(clickPosition.x-eventData.position.x);
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
            if (isPointerDragging) SetRotateModelY(clickPosition.x - eventData.position.x);
            isPointerEnter = false;
            isPointerExit = true;
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isPointerDragging) {
                if (isPointerDragging) SetRotateModelY(clickPosition.x - eventData.position.x);
                isPointerDragging = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDragging = true;
            clickPosition = eventData.position;
        }
    }
}