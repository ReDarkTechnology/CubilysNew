using UnityEngine;
using UnityEngine.Events;

namespace Cubilys.World
{
    public class CharacterMovement : MonoBehaviour
    {
        #region Variables
        public static CharacterMovement self;

        [Header("Components")]
        public BoxCollider col;
        public AudioSource source;
        public Rigidbody rigid;
        public GameObject groundDetector;
        private CharacterGroundDetector cgd;

        [Header("Parameters")]
        public float moveSpeed = 15;
        public float rotateSpeed = 45;
        public float boostRate = 1.5f;
        public float jumpPower = 200;

        [Header("Controls")]
        public KeyCode[] forwardKeys = { KeyCode.UpArrow, KeyCode.W };
        public KeyCode[] leftKeys = { KeyCode.LeftArrow, KeyCode.A };
        public KeyCode[] backKeys = { KeyCode.DownArrow, KeyCode.S };
        public KeyCode[] rightKeys = { KeyCode.RightArrow, KeyCode.D };
        public KeyCode[] jumpKeys = { KeyCode.Space };
        public KeyCode actionKey = KeyCode.LeftShift;

        [Header("States")]
        public bool isAlive = true;
        public bool isGrounded = true;

        [Header("Events")]
        public UnityEvent onPlayerTransforms;
        #endregion

        #region Unity Functions
        private void Start()
        {
            source = source ?? GetComponent<AudioSource>();
            rigid = rigid ?? GetComponent<Rigidbody>();
            cgd = groundDetector.AddComponent<CharacterGroundDetector>();
            self = this;
        }

        private void Update()
        {
            isGrounded = NowGrounded();
            Vector3 direction = Vector3.zero;
            float multiplier = 1;
            if (AreKeysPressed(forwardKeys))
            {
                direction += transform.forward;
            }
            if (AreKeysPressed(leftKeys))
            {
                if (Input.GetKey(actionKey))
                    direction -= transform.right;
                else
                    Rotate(new Vector3(0, -rotateSpeed * Time.deltaTime));
            }
            if (AreKeysPressed(backKeys))
            {
                direction -= transform.forward;
            }
            if (AreKeysPressed(rightKeys))
            {
                if (Input.GetKey(actionKey))
                    direction += transform.right;
                else
                    Rotate(new Vector3(0, rotateSpeed * Time.deltaTime));
            }

            if (isGrounded)
            {
                if (AreKeysPressedDown(jumpKeys))
                {
                    rigid.AddForce(transform.up * jumpPower);
                }
            }

            if (Input.GetKey(actionKey))
                multiplier = boostRate;
            Move(direction.normalized * moveSpeed * Time.deltaTime * multiplier);
        }

        public bool NowGrounded()
        {
            return cgd.isColliding;
        }

        private void OnDestroy()
        {
            self = null;
        }
        #endregion

        #region External Functions
        public void Move(Vector3 direction)
        {
            rigid.MovePosition(transform.position + direction);
            onPlayerTransforms.Invoke();
        }

        public void Rotate(Vector3 euler)
        {
            transform.eulerAngles += euler;
            onPlayerTransforms.Invoke();
        }

        public static bool AreKeysPressed(KeyCode[] codes)
        {
            foreach (var c in codes)
            {
                if (Input.GetKey(c)) return true;
            }
            return false;
        }

        public static bool AreKeysPressedDown(KeyCode[] codes)
        {
            foreach (var c in codes)
            {
                if (Input.GetKeyDown(c)) return true;
            }
            return false;
        }
        #endregion
    }

    public class CharacterGroundDetector : MonoBehaviour
    {
        public Collider info;
        public bool isColliding;

        private void OnTriggerStay(Collider other)
        {
            info = other;
            isColliding = !other.isTrigger;
        }

        private void OnTriggerExit(Collider other)
        {
            info = null;
            isColliding = false;
        }
    }
}