using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cubilys.Easings;
using Cubilys.Utilities;

namespace Cubilys 
{
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMovement : MonoBehaviour 
	{
		[Header("Components")]
		public MeshRenderer rend;
		public BoxCollider collid;
		public AudioSource source;
		public Rigidbody rigid;

        [Header("Parameters")]
        public GameObject playerStand;
		public Mesh tailMesh;
		public Transform tailParent;
		[AllowSavingState]
		public float speed = 15;
		// Arrays
		[AllowSavingState]
		public Vector3[] directions = {
			new Vector3(0, 90, 0),
			new Vector3(0, 0, 0)
		};
        public Vector3 finishDirection = new Vector3(0, 90, 0);
		public KeyCode[] keys = { 
			KeyCode.Mouse0,
			KeyCode.Mouse1,
			KeyCode.Space,
			KeyCode.UpArrow
		};
        public LayerMask groundMask;
        public bool avoidOverlapping = true;
        [AllowSavingState]
        public float audioVolume
        {
            get => source.volume;
            set => source.volume = value;
        }

		[Header("States")]
		[AllowSavingState]
		public int currentDirection;
		[AllowSavingState]
		public bool isAlive = true;
		[AllowSavingState]
		public bool isControllable = true;
		[AllowSavingState]
		public bool isMoving = true;
		[AllowSavingState]
		public bool isStarted;
        [AllowSavingState]
        public bool wasGrounded = true;
        [AllowSavingState]
        public bool standCollapsing;
        [AllowSavingState]
        public bool standCollapsed;
        [AllowSavingState]
        public bool isFinished;
        [AllowSavingState]
        private bool dieFloating;
        [AllowSavingState]
        private Vector3 lineStart;

        bool isGrounded;

        [Header("Events")]
        public UnityEvent onStandCollapsing;
        public UnityEvent onStandCollapsed;
        public UnityEvent onLineStart;
		public UnityEvent onLineTap;
        public UnityEvent onLineTurned;
		public UnityEvent onLineKilled;
		public UnityEvent onLineFinished;

		// [ Hidden Fields ]
		[HideInInspector] public GameObject currentTail;
		[HideInInspector] public Material temporaryMaterial;
		[HideInInspector] public List<GameObject> tails = new List<GameObject>();
        [HideInInspector] public bool preventStarting;

		public void Start() {
			temporaryMaterial = new Material (rend.sharedMaterial);
		}

		void Update () {
            isGrounded = IsGrounded();
			UpdatePlayer ();
			UpdateTranslations ();
		}

		// Separated Functions
		public void UpdatePlayer()
		{
			if (IsInputPressed () && isControllable) {
				if (isStarted)
                {
                    onLineTap.Invoke();
                    TurnLine ();
				}
                else
                {
                    Monetization.CubeSystem.instance.TryStarting();
                    if (preventStarting)
                        return;

                    if (playerStand != null && !standCollapsed)
                    {
                        if (!standCollapsing) CollapseStand();
                    }
                    else
                    {
                        onLineStart.Invoke();
                        if (!preventStarting) StartLine();
                    }
				}
			}
		}
		
		public void UpdateTranslations()
		{
			if (!isStarted || !isMoving )
				return;
			
			var translation = transform.forward * Time.deltaTime * speed;
			transform.position += translation;
			if (currentTail != null) {
                if (isGrounded)
                {
                    if (!wasGrounded)
                    {
                        CreateTail();
                        wasGrounded = true;
                    }
                    else
                    {
                        currentTail.transform.position += translation / 2;
                        currentTail.transform.localScale += new Vector3(0, 0, Time.deltaTime * speed);
                    }
                }
                else
                {
                    if(wasGrounded)
                    {
                        currentTail.transform.position += (transform.forward * currentTail.transform.localScale.x) / 2;
                        currentTail.transform.localScale += new Vector3(0, 0, currentTail.transform.localScale.x);
                        wasGrounded = false;
                    }
                }
			}
		}

		// Functions
		public bool IsInputPressed(bool checkGround = true)
		{
			if (IsInputAboveUI ())
				return false;
			
			foreach (var key in keys) {
				if (Input.GetKeyDown (key)) {
					if (checkGround) {
						return isGrounded;
					} else {
						return true;
					}
				}
			}
			return false;
		}

        public void AdjustLine(Vector3 to)
        {
            transform.position = to;
            if (currentTail == null)
                return;

            Transform stretchTransform = currentTail.transform;
            stretchTransform.eulerAngles = Vector3.zero;
            stretchTransform.localScale = Vector3.one;

            Vector3[] result = Vector3Util.GetLine(lineStart, to, stretchTransform.localScale);
            stretchTransform.position = result[0];
            stretchTransform.localScale = result[1];
            CreateTail(!avoidOverlapping);
        }

        public bool IsInputAboveUI()
		{
			foreach (var touch in Input.touches) {
				if (EventSystem.current.IsPointerOverGameObject (touch.fingerId))
					return true;
			}
			return EventSystem.current.IsPointerOverGameObject ();
		}

		public bool IsGrounded()
		{
			RaycastHit hit = default(RaycastHit);
            if (Physics.Raycast(transform.position, -transform.up, out hit, (collid.size.y / 2) + 0.03f, groundMask))
                return true;
			return false;
		}

        public void CollapseStand()
        {
            onLineStart.Invoke();
            onStandCollapsing.Invoke();
            preventStarting = true;
            TweenTool.TweenFloat(transform.eulerAngles.y + 90, transform.eulerAngles.y, 1).SetEase(TweenType.OutCubic).SetOnUpdate(val => {
                var o = (float)val;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, o, transform.eulerAngles.z);
            });
            TweenTool.TweenFloat(playerStand.transform.position.y, playerStand.transform.position.y - 2, 1).SetEase(TweenType.OutCubic).SetOnUpdate(val => {
                var v = (float)val;
                playerStand.transform.position = new Vector3(playerStand.transform.position.x, v, playerStand.transform.position.z);
            });
            TweenTool.TweenFloat(transform.position.y, transform.position.y - 2, 1).SetEase(TweenType.OutCubic).SetOnUpdate(val => {
                var v = (float)val;
                transform.position = new Vector3(transform.position.x, v, transform.position.z);
            });
            standCollapsing = true;
            CallDelay.Call(1, () => {
                preventStarting = false;
                onStandCollapsed.Invoke();
                standCollapsing = false;
                standCollapsed = true;
            });
        }

		public void StartLine()
		{
			isStarted = true;
            Debug.Log("Source playing!");
			source.Play ();
			CreateTail (!avoidOverlapping);
		}

		public void TurnLine()
		{
			currentDirection++;

			if (currentDirection >= directions.Length)
				currentDirection = 0;
			transform.localEulerAngles = directions [currentDirection];
            onLineTurned?.Invoke();
            if (IsGrounded())
            {
                CreateTail(!avoidOverlapping);
                CreateCenterTail();
            }
		}

        public void CreateCenterTail()
        {
            var instance = new GameObject();
            instance.name = "CenterTail";
            instance.transform.position = transform.position;
            instance.transform.eulerAngles = transform.eulerAngles;
            instance.transform.localScale = transform.localScale;
            if (tailParent != null)
                instance.transform.SetParent(tailParent);
            instance.AddComponent<MeshFilter>().sharedMesh = tailMesh;
            instance.AddComponent<MeshRenderer>().sharedMaterial = temporaryMaterial;
            tails.Add(instance);
        }

        public void CreateTail(bool normal = true)
		{
			var instance = new GameObject ();
			instance.name = "Tail";
			instance.transform.position = normal ? transform.position : transform.position - (transform.forward * (transform.localScale.z / 2));
			instance.transform.eulerAngles = transform.eulerAngles;
			instance.transform.localScale = normal ? transform.localScale : new Vector3(transform.localScale.x, transform.localScale.y, 0);
            if (tailParent != null)
				instance.transform.SetParent (tailParent);
			instance.AddComponent<MeshFilter> ().sharedMesh = tailMesh;
			instance.AddComponent<MeshRenderer> ().sharedMaterial = temporaryMaterial;
			tails.Add (instance);

            lineStart = transform.position;
			currentTail = instance;
		}

		public void DestroyTails()
		{
			foreach (var tail in tails)
			{
				Destroy(tail);
			}
			tails.Clear();
		}

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Obstacle")
            {
                if (!isAlive || isFinished) return;

                isAlive = false;
                isControllable = false;
                Easings.TweenTool.TweenFloat(1, 0, 2).SetEase(Easings.TweenType.Linear).SetOnUpdate(val => {
                    float e = (float)val;
                    audioVolume = e;
                });
                onLineKilled.Invoke();
                dieFloating = true;
            }

            if(other.tag == "Finish" && !isFinished)
            {
                transform.localEulerAngles = finishDirection;
                if (IsGrounded()) CreateTail(!avoidOverlapping);
                isControllable = false;
                onLineFinished.Invoke();
                isFinished = true;
            }

            if (other.gameObject.name.Contains("Turn1"))
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                if (IsGrounded()) CreateTail(!avoidOverlapping);
            }
            if (other.gameObject.name.Contains("Turn2"))
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
                if (IsGrounded()) CreateTail(!avoidOverlapping);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "Obstacle")
            {
                if (dieFloating)
                {
                    CreateExplosion(3);
                    isMoving = false;
                    dieFloating = false;
                }
                if (!isAlive || isFinished) return;
                CreateExplosion(3);
                isMoving = false;
                Easings.TweenTool.TweenFloat(1, 0, 2).SetEase(Easings.TweenType.Linear).SetOnUpdate(val =>
                {
                    float e = (float)val;
                    audioVolume = e;
                });
                isAlive = false;
                isControllable = false;
                onLineKilled.Invoke();
            }
        }

        public void CreateExplosion(int num)
        {
            for (int i = 0; i < num; i++)
            {
                var instance = new GameObject();
                instance.name = "ExplosionInstance";
                instance.transform.position = transform.position;
                instance.transform.eulerAngles = transform.eulerAngles;
                instance.transform.localScale = transform.localScale;
                instance.AddComponent<BoxCollider>();
                instance.AddComponent<Rigidbody>().velocity = Random.onUnitSphere * 1f;
                if (tailParent != null)
                    instance.transform.SetParent(tailParent);
                instance.AddComponent<MeshFilter>().sharedMesh = tailMesh;
                instance.AddComponent<MeshRenderer>().sharedMaterial = temporaryMaterial;
                tails.Add(instance);
            }
        }
    }
}