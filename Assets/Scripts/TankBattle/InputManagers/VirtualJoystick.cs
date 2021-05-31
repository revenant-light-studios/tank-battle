using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TankBattle.InputManagers
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, AxisState.IInputAxisProvider
    {

        public float GetAxisValue(int axis)
        {
            if (axis == 0)
            {
                return InputDirection.x;
            } 
            
            if (axis == 1)
            {
                return InputDirection.y;
            }

            return InputDirection.z;
        }
        
        private Image jsContainer;
        private Image joystick;
        public Vector3 InputDirection;


        void Start()
        {
            jsContainer = GetComponent<Image>();
            joystick = transform.GetChild(0).GetComponent<Image>(); //this command is used because there is only one child in hierarchy
            InputDirection = Vector3.zero;
        }

        public void OnDrag(PointerEventData ped)
        {
            Vector2 position = Vector2.zero;

            //To get InputDirection
            RectTransformUtility.ScreenPointToLocalPointInRectangle
                (jsContainer.rectTransform,
                ped.position,
                ped.pressEventCamera,
                out position);



            position.x = (position.x / jsContainer.rectTransform.sizeDelta.x);
            position.y = (position.y / jsContainer.rectTransform.sizeDelta.y);

            InputDirection = new Vector3(position.x, position.y, 0);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

            //to define the area in which joystick can move around
            joystick.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (jsContainer.rectTransform.sizeDelta.x / 3)
                , InputDirection.y * (jsContainer.rectTransform.sizeDelta.y) / 3);

        }

        internal object FirstOrDefault(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }

        public void OnPointerDown(PointerEventData ped)
        {
            OnDrag(ped);
        }

        public void OnPointerUp(PointerEventData ped)
        {
            InputDirection = Vector3.zero;
            joystick.rectTransform.anchoredPosition = Vector3.zero;
        }
    }
}