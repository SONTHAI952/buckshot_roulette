using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace ZeroX.Extensions
{
    public static class DOTween_TransformExtension
    {
        public static Sequence DOJumpX(
            this Transform target,
            Vector3 endValue,
            float jumpPower,
            int numJumps,
            float duration,
            bool snapping = false)
        {
            if (numJumps < 1)
                numJumps = 1;
            
            float startPosX = target.position.x;
            float offsetX = -1f;
            bool offsetXSet = false;
            Sequence s = DOTween.Sequence();
            Tween xTween = (Tween) DOTween.To(() => target.position, v => target.position = v, new Vector3(jumpPower, 0.0f, 0.0f), duration / (float) (numJumps * 2)).SetOptions(AxisConstraint.X, snapping).SetEase<Tweener>(Ease.OutQuad).SetRelative<Tweener>().SetLoops<Tweener>(numJumps * 2, LoopType.Yoyo).OnStart<Tweener>((TweenCallback) (() => startPosX = target.position.x));
            s.Append((Tween) DOTween.To(() => target.position, v => target.position = v, new Vector3(0.0f, endValue.y, 0.0f), duration).SetOptions(AxisConstraint.Y, snapping).SetEase<Tweener>(Ease.Linear))
                .Join((Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (v => target.position = v), new Vector3(0.0f, 0.0f, endValue.z), duration).SetOptions(AxisConstraint.Z, snapping).SetEase<Tweener>(Ease.Linear)).Join(xTween).SetTarget<Sequence>((object) target).SetEase<Sequence>(DOTween.defaultEaseType);
            xTween.OnUpdate<Tween>((TweenCallback) (() =>
            {
                if (!offsetXSet)
                {
                    offsetXSet = true;
                    offsetX = s.isRelative ? endValue.x : endValue.x - startPosX;
                }
                Vector3 position = target.position;
                position.x += DOVirtual.EasedValue(0.0f, offsetX, xTween.ElapsedPercentage(), Ease.OutQuad);
                target.position = position;
            }));
            return s;
        }

        public static Sequence DOJumpZ(
            this Transform target,
            Vector3 endValue,
            float jumpPower,
            int numJumps,
            float duration,
            bool snapping = false)
        {
            if (numJumps < 1)
                numJumps = 1;
            float startPosZ = target.position.y;
            float offsetZ = -1f;
            bool offsetZSet = false;
            Sequence s = DOTween.Sequence();
            Tween yTween = (Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (v => target.position = v), new Vector3(0.0f, 0.0f, jumpPower), duration / (float) (numJumps * 2)).SetOptions(AxisConstraint.Z, snapping).SetEase<Tweener>(Ease.OutQuad).SetRelative<Tweener>().SetLoops<Tweener>(numJumps * 2, LoopType.Yoyo).OnStart<Tweener>((TweenCallback) (() => startPosZ = target.position.z));
            s.Append((Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (v => target.position = v), new Vector3(endValue.x, 0.0f, 0.0f), duration).SetOptions(AxisConstraint.X, snapping).SetEase<Tweener>(Ease.Linear))
                .Join((Tween) DOTween.To((DOGetter<Vector3>) (() => target.position), (DOSetter<Vector3>) (v => target.position = v), new Vector3(0.0f, endValue.y, 0), duration).SetOptions(AxisConstraint.Y, snapping).SetEase<Tweener>(Ease.Linear)).Join(yTween).SetTarget<Sequence>((object) target).SetEase<Sequence>(DOTween.defaultEaseType);
            yTween.OnUpdate<Tween>((TweenCallback) (() =>
            {
                if (!offsetZSet)
                {
                    offsetZSet = true;
                    offsetZ = s.isRelative ? endValue.z : endValue.z - startPosZ;
                }
                Vector3 position = target.position;
                position.z += DOVirtual.EasedValue(0.0f, offsetZ, yTween.ElapsedPercentage(), Ease.OutQuad);
                target.position = position;
            }));
            return s;
        }



        #region Slime
        
        public static Tween DOScaleSlimeBounceUp(this Transform transform, Vector3 endValue, float duration, Vector3 startValue, Vector3 additionValue, Vector3 subtractionValue)
        {
            Sequence sequence = DOTween.Sequence(transform);

            var tweenScale1 = transform.DOScale(new Vector3(additionValue.x, subtractionValue.y, endValue.z), duration * 0.25f).From(startValue).SetEase(Ease.Linear);
            var tweenScale2 = transform.DOScale(new Vector3(endValue.x, additionValue.y, endValue.z), duration * 0.25f).SetEase(Ease.Linear);
            var tweenScale3 = transform.DOScale(new Vector3(subtractionValue.x, subtractionValue.y, endValue.z), duration * 0.25f).SetEase(Ease.Linear);
            var tweenScale4 = transform.DOScale(endValue, duration * 0.25f).SetEase(Ease.Linear);

            sequence.Append(tweenScale1);
            sequence.Append(tweenScale2);
            sequence.Append(tweenScale3);
            sequence.Append(tweenScale4);
            return sequence;
        }

        public static Tween DOScaleSlimeBounceUp(this Transform transform, Vector3 endValue, float duration, Vector3 startValue)
        {
            return DOScaleSlimeBounceUp(transform, endValue, duration, startValue, endValue * 1.2f, endValue * 0.85f);
        }
        
        public static Tween DOScaleSlimeBounceUp(this Transform transform, Vector3 endValue, float duration)
        {
            return DOScaleSlimeBounceUp(transform, endValue, duration, endValue * 0.5f, endValue * 1.15f, endValue * 0.85f);
        }
        
        #endregion


        
        #region Scale

        public static Tweener DoLossyScale(this Transform transform, Vector3 endValue, float duration)
        {
            return DOVirtual.Vector3(transform.lossyScale, endValue, duration, transform.SetLossyScale);
        }
        
        /// <summary>
        /// Thông thường chỉ có thể DoLocalScale trong parent của nó. Hàm này giúp DoScale trong một không gian của một transform bất kỳ
        /// </summary>
        public static Tweener DoScaleInTransformSpace(this Transform transform, Transform transformSpace,
            Vector3 endValue, float duration)
        {
            return DOVirtual.Vector3(transform.GetScaleInTransformSpace(transformSpace), endValue, duration,
                v => { transformSpace.SetScaleInTransformSpace(transformSpace, v); });
        }

        #endregion



        #region Rotate Around

        public static TweenerCore<float, float, FloatOptions> DoRotateAround(this Transform transform, Vector3 axis, float addAngle, float duration)
        {
            Quaternion startRotation = transform.rotation;
            return DOTween.To(() => 0, v =>
            {
                transform.rotation = Quaternion.AngleAxis(v, axis) * startRotation;
            }, addAngle, duration);
        }
        
        public static TweenerCore<float, float, FloatOptions> DoLocalRotateAround(this Transform transform, Vector3 localAxis, float addAngle, float duration)
        {
            Quaternion startRotation = transform.localRotation;
            return DOTween.To(() => 0, v =>
            {
                transform.localRotation = Quaternion.AngleAxis(v, localAxis) * startRotation;
            }, addAngle, duration);
        }

        #endregion



        #region Shake Rotation

        /// <summary>
        /// MinStrength và MaxStrength giúp biết được nên random trong khoảng bao nhiêu. Hàm này đã tự đảo âm dương rồi, minStrength, maxStrength không phải dùng để đảo âm dương. Vì vậy hãy khai báo 2 con số dương.
        /// </summary>
        public static Tween DOLocalShakeRotationPro(this Transform transform, float duration, Vector3 minStrength, Vector3 maxStrength, int vibrato = 15, float fadeOutDuration = 0f)
        {
            float timeStep = duration / vibrato;
            Vector3 startEuler = transform.localEulerAngles;
            int signX = Random.Range(0, 2) == 0 ? -1 : 1;
            int signY = Random.Range(0, 2) == 0 ? -1 : 1;
            int signZ = Random.Range(0, 2) == 0 ? -1 : 1;

            Sequence sequence = DOTween.Sequence(transform);
            for (int i = 0; i < vibrato; i++)
            {
                float time = i * timeStep;

                if (i < vibrato - 1)
                {
                    Vector3 offset = new Vector3();
                    offset.x = Random.Range(minStrength.x, maxStrength.x) * signX;
                    offset.y = Random.Range(minStrength.y, maxStrength.y) * signY;
                    offset.z = Random.Range(minStrength.z, maxStrength.z) * signZ;

                    float fadeDelta = (duration - time) / fadeOutDuration; //Càng gần cuối thì càng về 0
                    offset *= Mathf.Lerp(0, 1, fadeDelta);

                    sequence.Append(transform.DOLocalRotate(startEuler + offset, timeStep).SetEase(Ease.Linear));
                    
                    signX = Random.Range(0, 2) == 0 ? -1 : 1;
                    signY = Random.Range(0, 2) == 0 ? -1 : 1;
                    signZ = Random.Range(0, 2) == 0 ? -1 : 1;
                }
                else
                {
                    sequence.Append(transform.DOLocalRotate(startEuler, timeStep));
                }
            }

            sequence.SetEase(Ease.Linear);
            return sequence;
        }

        /// <summary>
        /// MinStrength và MaxStrength giúp biết được nên random trong khoảng bao nhiêu. Hàm này đã tự đảo âm dương rồi, minStrength, maxStrength không phải dùng để đảo âm dương. Vì vậy hãy khai báo 2 con số dương.
        /// </summary>
        public static Tween DOLocalShakeRotationProZ(this Transform transform, float duration, float minStrength, float maxStrength, int vibrato = 15, float fadeOutDuration = 0f)
        {
            return DOLocalShakeRotationPro(transform, duration, new Vector3(0, 0, minStrength), new Vector3(0, 0, maxStrength), vibrato, fadeOutDuration);
        }

        /// <summary>
        /// Hàm này dùng để thêm khoảng nghỉ giữa mỗi lần shake nếu bật loop
        /// <para>MinStrength và MaxStrength giúp biết được nên random trong khoảng bao nhiêu. Hàm này đã tự đảo âm dương rồi, minStrength, maxStrength không phải dùng để đảo âm dương. Vì vậy hãy khai báo 2 con số dương.</para>
        /// </summary>
        public static Tween DOLocalShakeRotationPro_Intermittent(this Transform transform, float duration, float intermittentTime, Vector3 minStrength, Vector3 maxStrength, int vibrato = 15, float fadeOutDuration = 0f)
        {
            Sequence sequence = DOTween.Sequence(transform);
            sequence.Append(DOLocalShakeRotationPro(transform, duration, minStrength, maxStrength, vibrato, fadeOutDuration));
            sequence.AppendInterval(intermittentTime);
            return sequence;
        }
        
        /// <summary>
        /// Hàm này dùng để thêm khoảng nghỉ giữa mỗi lần shake nếu bật loop
        /// <para>MinStrength và MaxStrength giúp biết được nên random trong khoảng bao nhiêu. Hàm này đã tự đảo âm dương rồi, minStrength, maxStrength không phải dùng để đảo âm dương. Vì vậy hãy khai báo 2 con số dương.</para>
        /// </summary>
        public static Tween DOLocalShakeRotationProZ_Intermittent(this Transform transform, float duration, float intermittentTime, float minStrength, float maxStrength, int vibrato = 15, float fadeOutDuration = 0f)
        {
            Sequence sequence = DOTween.Sequence(transform);
            sequence.Append(DOLocalShakeRotationProZ(transform, duration, minStrength, maxStrength, vibrato, fadeOutDuration));
            sequence.AppendInterval(intermittentTime);
            return sequence;
        }
        
        #endregion


        #region Shake Position

        /// <summary>
        /// MinStrength và MaxStrength giúp biết được nên random trong khoảng bao nhiêu. Hàm này đã tự đảo âm dương rồi, minStrength, maxStrength không phải dùng để đảo âm dương. Vì vậy hãy khai báo 2 con số dương.
        /// </summary>
        public static Tween DOLocalShakePositionPro(this Transform transform, float duration, Vector3 minStrength, Vector3 maxStrength, int vibrato = 15, float fadeOutDuration = 0f)
        {
            float timeStep = duration / vibrato;
            Vector3 startLocalPos = transform.localPosition;
            int signX = Random.Range(0, 2) == 0 ? -1 : 1;
            int signY = Random.Range(0, 2) == 0 ? -1 : 1;
            int signZ = Random.Range(0, 2) == 0 ? -1 : 1;

            Sequence sequence = DOTween.Sequence(transform);
            for (int i = 0; i < vibrato; i++)
            {
                float time = i * timeStep;

                if (i < vibrato - 1)
                {
                    Vector3 offset = new Vector3();
                    offset.x = Random.Range(minStrength.x, maxStrength.x) * signX;
                    offset.y = Random.Range(minStrength.y, maxStrength.y) * signY;
                    offset.z = Random.Range(minStrength.z, maxStrength.z) * signZ;

                    float fadeDelta = (duration - time) / fadeOutDuration; //Càng gần cuối thì càng về 0
                    offset *= Mathf.Lerp(0, 1, fadeDelta);

                    sequence.Append(transform.DOLocalMove(startLocalPos + offset, timeStep).SetEase(Ease.Linear));
                    
                    signX = Random.Range(0, 2) == 0 ? -1 : 1;
                    signY = Random.Range(0, 2) == 0 ? -1 : 1;
                    signZ = Random.Range(0, 2) == 0 ? -1 : 1;
                }
                else
                {
                    sequence.Append(transform.DOLocalMove(startLocalPos, timeStep));
                }
            }

            sequence.SetEase(Ease.Linear);
            return sequence;
        }
        

        /// <summary>
        /// Hàm này dùng để thêm khoảng nghỉ giữa mỗi lần shake nếu bật loop
        /// <para>MinStrength và MaxStrength giúp biết được nên random trong khoảng bao nhiêu. Hàm này đã tự đảo âm dương rồi, minStrength, maxStrength không phải dùng để đảo âm dương. Vì vậy hãy khai báo 2 con số dương.</para>
        /// </summary>
        public static Tween DOLocalShakePositionPro_Intermittent(this Transform transform, float duration, float intermittentTime, Vector3 minStrength, Vector3 maxStrength, int vibrato = 15, float fadeOutDuration = 0f)
        {
            Sequence sequence = DOTween.Sequence(transform);
            sequence.Append(DOLocalShakePositionPro(transform, duration, minStrength, maxStrength, vibrato, fadeOutDuration));
            sequence.AppendInterval(intermittentTime);
            return sequence;
        }
        
        
        #endregion



        #region Move Curve
        
        public static Vector3[] CreateMoveCurvePath(Vector3 startPos, Vector3 endPos, float height, Vector3 rotationAxis, AnimationCurve curve)
        {
            //Tạo path cong
            Vector3[] path = new Vector3[12];
            Vector3 direction = (endPos - startPos).normalized;
            Vector3 direction90 = Quaternion.AngleAxis(90, rotationAxis) * direction;
            
            for (int i = 0; i < path.Length; i++)
            {
                float delta = i / (float)(path.Length - 1);
                Vector3 point = Vector3.Lerp(startPos, endPos, delta);
            
                delta = curve.Evaluate(delta);
                point += direction90 * height * delta;
                path[i] = point;
            }
            
            return path;
        }
        
        public static Vector3[] CreateMoveCurvePath(Vector3 startPos, Vector3 endPos, float height, Vector3 rotationAxis)
        {
            return CreateMoveCurvePath(startPos, endPos, height, rotationAxis, CreateDefaultMoveCurveEase());
        }

        public static AnimationCurve CreateDefaultMoveCurveEase()
        {
            AnimationCurve curve = new AnimationCurve();

            
            Keyframe keyframe0 = new Keyframe(0, 0, 4.698442f, 4.698442f, 0, 0.0594965f);
            keyframe0.weightedMode = WeightedMode.None;
            
            Keyframe keyframe1 = new Keyframe(0.5f, 1, 0, 0, 0.3333333f, 0.3333333f);
            keyframe1.weightedMode = WeightedMode.None;
            
            Keyframe keyframe2 = new Keyframe(1, 0, -4.698442f, -4.698442f, 0.0594965f, 0);
            keyframe2.weightedMode = WeightedMode.None;
            
            
            curve.AddKey(keyframe0);
            curve.AddKey(keyframe1);
            curve.AddKey(keyframe2);
            
            
            return curve;
        }
        
        public static Tween DOMoveCurve(this Transform transform, Vector3 endPos, float duration, float height, Vector3 rotationAxis, AnimationCurve curve)
        {
            var path = CreateMoveCurvePath(transform.position, endPos, height, rotationAxis, curve);
            return transform.DOPath(path, duration, PathType.CatmullRom);
        }
        
        public static Tween DOMoveCurve2D(this Transform transform, Vector3 endPos, float duration, float height)
        {
            var path = CreateMoveCurvePath(transform.position, endPos, height, Vector3.forward, CreateDefaultMoveCurveEase());
            return transform.DOPath(path, duration, PathType.CatmullRom);
        }
        
        public static Tween DOMoveCurve2D(this Transform transform, Vector3 endPos, float duration)
        {
            float height = Vector3.Distance(transform.position, endPos) * 0.33f;
            var path = CreateMoveCurvePath(transform.position, endPos, height, Vector3.forward, CreateDefaultMoveCurveEase());
            return transform.DOPath(path, duration, PathType.CatmullRom);
        }
        
        
        
        
        
        public static Tween DOLocalMoveCurve(this Transform transform, Vector3 endPos, float duration, float height, Vector3 rotationAxis, AnimationCurve curve)
        {
            var path = CreateMoveCurvePath(transform.localPosition, endPos, height, rotationAxis, curve);
            return transform.DOLocalPath(path, duration, PathType.CatmullRom);
        }
        
        public static Tween DOLocalMoveCurve(this Transform transform, Vector3 endPos, float duration, float height, Vector3 rotationAxis)
        {
            var path = CreateMoveCurvePath(transform.localPosition, endPos, height, rotationAxis, CreateDefaultMoveCurveEase());
            return transform.DOLocalPath(path, duration, PathType.CatmullRom);
        }
        
        public static Tween DOLocalMoveCurve2D(this Transform transform, Vector3 endPos, float duration, float height)
        {
            var path = CreateMoveCurvePath(transform.localPosition, endPos, height, Vector3.forward, CreateDefaultMoveCurveEase());
            return transform.DOLocalPath(path, duration, PathType.CatmullRom);
        }
        
        public static Tween DOLocalMoveCurve2D(this Transform transform, Vector3 endPos, float duration)
        {
            float height = Vector3.Distance(transform.localPosition, endPos) * 0.33f;
            var path = CreateMoveCurvePath(transform.localPosition, endPos, height, Vector3.forward, CreateDefaultMoveCurveEase());
            return transform.DOLocalPath(path, duration, PathType.CatmullRom);
        }

        #endregion
    }
}