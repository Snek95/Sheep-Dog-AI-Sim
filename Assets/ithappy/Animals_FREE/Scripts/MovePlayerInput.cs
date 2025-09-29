using UnityEngine;
using UnityEngine.InputSystem;

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CreatureMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField]
        private InputActionReference moveAction;
        [SerializeField]
        private InputActionReference jumpAction;
        [SerializeField]
        private InputActionReference runAction;

        [Header("Camera")]
        [SerializeField]
        private PlayerCamera m_Camera;
        [SerializeField]
        private InputActionReference lookAction;
        [SerializeField]
        private InputActionReference scrollAction;

        private CreatureMover m_Mover;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;

        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            jumpAction.action.Enable();
            runAction.action.Enable();
            lookAction.action.Enable();
            scrollAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            jumpAction.action.Disable();
            runAction.action.Disable();
            lookAction.action.Disable();
            scrollAction.action.Disable();
        }

        private void Update()
        {
            GatherInput();
            SetInput();
        }

        public void GatherInput()
        {
            m_Axis = moveAction.action.ReadValue<Vector2>();
            m_IsRun = runAction.action.IsPressed();
            m_IsJump = jumpAction.action.IsPressed();

            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;
            m_MouseDelta = lookAction.action.ReadValue<Vector2>();
            m_Scroll = scrollAction.action.ReadValue<float>();
        }

        public void BindMover(CreatureMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
            }

            if (m_Camera != null)
            {
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
            }
        }
    }
}