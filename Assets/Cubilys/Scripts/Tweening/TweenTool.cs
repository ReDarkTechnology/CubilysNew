using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cubilys.Easings
{
	public class TweenTool : MonoBehaviour
	{
		public static TweenTool self;
		public static List<Tweenable> tweenables = new List<Tweenable>();
		public static List<Tweenable> onDestroy = new List<Tweenable>();

		public static void CheckInstance()
		{
			if (self == null) {
				var obj = new GameObject ();
				self = obj.AddComponent<TweenTool> ();
				DontDestroyOnLoad (obj);
			}
		}

		public void Start()
		{
			if (self == null) {
				self = this;
				DontDestroyOnLoad (gameObject);
			} else {
				if (self != this) {
					Destroy (gameObject);
				}
			}
		}

		public void Update()
		{
			for (int i = 0; i < tweenables.Count; i++)
			{
				var e = tweenables[i];
				e.Ease();
				if(e.finished) onDestroy.Add(e);
			}
			for (int i = 0; i < onDestroy.Count; i++)
			{
				tweenables.Remove(onDestroy[i]);
			}
		}

		public static IntTweenable TweenInt(int start, int end, float time)
		{
			CheckInstance ();
			var t = new IntTweenable {
				start = start,
				end = end,
				length = time
			};
			tweenables.Add(t);
			return t;
		}

		public static FloatTweenable TweenFloat(float start, float end, float time)
		{
			CheckInstance ();
			var t = new FloatTweenable {
				start = start,
				end = end,
				length = time
			};
			tweenables.Add(t);
			return t;
        }

        public static FloatTweenable TweenFloat(GameObject host, float start, float end, float time)
        {
            CheckInstance();
            var t = new FloatTweenable
            {
                start = start,
                end = end,
                length = time,
                host = host
            };
            tweenables.Add(t);
            return t;
        }

        public static Vector2Tweenable TweenVector2(Vector2 start, Vector2 end, float time)
		{
			CheckInstance ();
			var t = new Vector2Tweenable {
				start = start,
				end = end,
				length = time
			};
			tweenables.Add(t);
			return t;
		}

		public static Vector3Tweenable TweenVector3(Vector3 start, Vector3 end, float time)
		{
			CheckInstance ();
			var t = new Vector3Tweenable {
				start = start,
				end = end,
				length = time
			};
			tweenables.Add(t);
			return t;
		}

		public static ColorTweenable TweenColor(Color start, Color end, float time)
		{
			CheckInstance ();
			var t = new ColorTweenable {
				start = start,
				end = end,
				length = time
			};
			tweenables.Add(t);
			return t;
		}

        public static void Cancel(GameObject host)
        {
            var l = tweenables.FindAll(val => val.host == host);
            foreach(var t in l)
            {
                t.finished = true;
                tweenables.Remove(t);
            }
        }
	}

	public class Tweenable
	{
		public TweenType type = TweenType.Linear;
        public GameObject host;
		public object start;
		public object end;
		public object result;
		public bool finished;
		public float current;
        public float delay;
		public float length;
		public Action<object> OnUpdate;

		public Tweenable SetEase(TweenType type)
		{
			this.type = type;
			return this;
		}

		public Tweenable SetOnUpdate(Action<object> obj)
		{
			OnUpdate = obj;
			return this;
		}

        public Tweenable SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }

		public virtual void Ease()
		{
			var n = (float)current + Time.deltaTime;
			current = n > length ? GetFinal() : n;
		}

		public virtual object GetValueAtTime(object start, object end, float time, float duration)
		{
			return null;
		}

		float GetFinal()
		{
			finished = true;
			return length;
		}

		public virtual float ThrowValue(float t)
		{
			return type == TweenType.Linear ? TweenFunctions.Linear(t) :
				type == TweenType.InQuad ? TweenFunctions.InQuad(t) :
				type == TweenType.OutQuad ? TweenFunctions.OutQuad(t) :
				type == TweenType.InOutQuad ? TweenFunctions.InOutQuad(t) :
				type == TweenType.InCubic ? TweenFunctions.InCubic(t) :
				type == TweenType.OutCubic ? TweenFunctions.OutCubic(t) :
				type == TweenType.InOutCubic ? TweenFunctions.InOutCubic(t) : 
				type == TweenType.InQuart ? TweenFunctions.InQuart(t) :
				type == TweenType.OutQuart ? TweenFunctions.OutQuart(t) : 
				type == TweenType.InOutQuart ? TweenFunctions.InOutQuart(t) : 
				type == TweenType.InQuint ? TweenFunctions.InQuint(t) :
				type == TweenType.OutQuint ? TweenFunctions.OutQuint(t) :
				type == TweenType.InOutQuint ? TweenFunctions.InOutQuint(t) :
				type == TweenType.InSine ? TweenFunctions.InSine(t) :
				type == TweenType.OutSine ? TweenFunctions.OutSine(t) : 
				type == TweenType.InOutSine ? TweenFunctions.InOutSine(t) : 
				type == TweenType.InExpo ? TweenFunctions.InExpo(t) : 
				type == TweenType.OutExpo ? TweenFunctions.OutExpo(t) :
				type == TweenType.InOutExpo ? TweenFunctions.InOutExpo(t) :
				type == TweenType.InCirc ? TweenFunctions.InCirc(t) :
				type == TweenType.OutCirc ? TweenFunctions.OutCirc(t) :
				type == TweenType.InOutCirc ? TweenFunctions.InOutCirc(t) : 
				type == TweenType.InElastic ? TweenFunctions.InElastic(t) :
				type == TweenType.OutElastic ? TweenFunctions.OutElastic(t) :
				type == TweenType.InOutElastic ? TweenFunctions.InOutElastic(t) : 
				type == TweenType.InBack ? TweenFunctions.InBack(t) :
				type == TweenType.OutBack ? TweenFunctions.OutBack(t) :
				type == TweenType.InOutBack ? TweenFunctions.InOutBack(t) : 
				type == TweenType.InBounce ? TweenFunctions.InBounce(t) :
				type == TweenType.OutBounce ? TweenFunctions.OutBounce(t) : TweenFunctions.InOutBounce(t);
		}
	}

	public enum TweenType
	{
		Linear,
		InQuad,
		OutQuad,
		InOutQuad,
		InCubic,
		OutCubic,
		InOutCubic,
		InQuart,
		OutQuart,
		InOutQuart,
		InQuint,
		OutQuint,
		InOutQuint,
		InSine,
		OutSine,
		InOutSine,
		InExpo,
		OutExpo,
		InOutExpo,
		InCirc,
		OutCirc,
		InOutCirc,
		InElastic,
		OutElastic,
		InOutElastic,
		InBack,
		OutBack,
		InOutBack,
		InBounce,
		OutBounce,
		InOutBounce
	}

	// Easeable
	public class Vector3Tweenable : Tweenable
	{
		public override void Ease()
		{
            if(delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }

			var a = (Vector3)start;
			var b = (Vector3)end;
			base.Ease();
			var r = ThrowValue(current / length);
			var m = b - a;
			result = a + (m * r);
			if (OnUpdate != null) OnUpdate.Invoke(result);
		}

		public override object GetValueAtTime(object start, object end, float time, float duration)
		{
			var a = (Vector3)start;
			var b = (Vector3)end;
			var r = ThrowValue(time / duration);
			var m = b - a;
			return a + (m * r);
		}
	}

	public class Vector2Tweenable : Tweenable
	{
		public override void Ease()
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }

            var a = (Vector2)start;
			var b = (Vector2)end;
			base.Ease();
			var r = ThrowValue(current / length);
			var m = b - a;
			result = a + (m * r);
			if(OnUpdate != null) OnUpdate.Invoke(result);
		}

		public override object GetValueAtTime(object start, object end, float time, float duration)
		{
			var a = (Vector2)start;
			var b = (Vector2)end;
			var r = ThrowValue(time / duration);
			var m = b - a;
			return a + (m * r);
		}
	}

	public class FloatTweenable : Tweenable
	{
		public override void Ease()
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }

            var a = (float)start;
			var b = (float)end;
			base.Ease();
			var r = ThrowValue(current / length);
			var m = b - a;
			result = a + (m * r);
			if(OnUpdate != null) OnUpdate.Invoke(result);
		}

		public override object GetValueAtTime(object start, object end, float time, float duration)
		{
			var a = (float)start;
			var b = (float)end;
			var r = ThrowValue(time / duration);
			var m = b - a;
			return a + (m * r);
		}
	}

	public class IntTweenable : Tweenable
	{
		public override void Ease()
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }

            var a = (int)start;
			var b = (int)end;
			base.Ease();
			var r = ThrowValue(current / length);
			var m = b - a;
			result = a + (m * r);
			if (OnUpdate != null) OnUpdate.Invoke(result);
		}

		public override object GetValueAtTime(object start, object end, float time, float duration)
		{
			var a = (int)start;
			var b = (int)end;
			var r = ThrowValue(time / duration);
			var m = b - a;
			return a + (m * r);
		}
	}

	public class ColorTweenable : Tweenable
	{
		public override void Ease()
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }

            var a = (Color)start;
			var b = (Color)end;
			base.Ease();
			var r = ThrowValue(current / length);
			var m = b - a;
			result = a + (m * r);
			if(OnUpdate != null) OnUpdate.Invoke(result);
		}

		public override object GetValueAtTime(object start, object end, float time, float duration)
		{
			var a = (Color)start;
			var b = (Color)end;
			var r = ThrowValue(time / duration);
			var m = b - a;
			return a + (m * r);
		}
	}
}
