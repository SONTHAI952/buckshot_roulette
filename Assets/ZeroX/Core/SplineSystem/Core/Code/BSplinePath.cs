using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroX.LogSystem;

namespace ZeroX.SplineSystem
{

    public class BSplinePath
    {
        private struct ArcSample
        {
            public float U;
            public float Distance;

            public ArcSample(float u, float distance)
            {
                U = u;
                Distance = distance;
            }
        }
        
        public class SamplePoint
        {
            public Vector3 position;
            public Vector3 direction;
        }

        private readonly List<Vector3> points = new();
        private readonly List<float> knots = new();
        private readonly List<ArcSample> arcSamples = new();

        private int degree;
        private float totalLength;
        private bool initialized;

        /// <summary>
        /// Càng cao thì uniform càng chính xác, nhưng Init sẽ tốn hơn.
        /// </summary>
        public int SamplePerSegment { get; set; } = 40;

        public float TotalLength => totalLength;

        public void Init(List<Vector3> listPoint)
        {
            if (listPoint == null || listPoint.Count == 0)
                throw new ArgumentException("BSplinePath cần ít nhất 1 point.");

            points.Clear();
            points.AddRange(listPoint);

            degree = Mathf.Min(3, points.Count - 1);

            BuildKnots();
            BuildArcLengthTable();

            initialized = true;
        }

        /// <summary>
        /// delta từ 0 tới 1.
        /// Pos trả về đã được uniform theo độ dài path.
        /// </summary>
        public Vector3 CalculatePos(float delta)
        {
            if (!initialized)
                throw new InvalidOperationException("Cần gọi Init trước khi CalculatePos.");

            if (points.Count == 1 || totalLength <= Mathf.Epsilon)
                return points[0];

            delta = Mathf.Clamp01(delta);

            float targetDistance = delta * totalLength;
            float u = DistanceToU(targetDistance);

            return Evaluate(u);
        }

        public Vector3 CalculateDirection(float delta)
        {
            if (!initialized)
                throw new InvalidOperationException("Cần gọi Init trước khi CalculateDirection.");

            if (points.Count == 1 || totalLength <= Mathf.Epsilon)
                return Vector3.zero;

            delta = Mathf.Clamp01(delta);

            float targetDistance = delta * totalLength;
            return CalculateDirectionAtDistance(targetDistance);
        }

        public SamplePoint CalculateSamplePoint(float delta)
        {
            if (!initialized)
                throw new InvalidOperationException("Cần gọi Init trước khi CalculateSamplePoint.");

            SamplePoint samplePoint = new SamplePoint();

            if (points.Count == 1 || totalLength <= Mathf.Epsilon)
            {
                samplePoint.position = points[0];
                samplePoint.direction = Vector3.zero;
                return samplePoint;
            }

            delta = Mathf.Clamp01(delta);

            float targetDistance = delta * totalLength;
            float u = DistanceToU(targetDistance);

            samplePoint.position = Evaluate(u);
            samplePoint.direction = CalculateDirectionAtDistance(targetDistance);

            return samplePoint;
        }

        private void BuildKnots()
        {
            knots.Clear();

            int pointCount = points.Count;
            int knotCount = pointCount + degree + 1;

            for (int i = 0; i < knotCount; i++)
            {
                if (i <= degree)
                {
                    knots.Add(0f);
                }
                else if (i >= pointCount)
                {
                    knots.Add(1f);
                }
                else
                {
                    float value = (float)(i - degree) / (pointCount - degree);
                    knots.Add(value);
                }
            }
        }

        private void BuildArcLengthTable()
        {
            arcSamples.Clear();

            if (points.Count == 1)
            {
                totalLength = 0f;
                arcSamples.Add(new ArcSample(0f, 0f));
                return;
            }

            int segmentCount = Mathf.Max(1, points.Count - degree);
            int sampleCount = Mathf.Max(2, segmentCount * SamplePerSegment);

            totalLength = 0f;

            Vector3 prevPos = Evaluate(0f);
            arcSamples.Add(new ArcSample(0f, 0f));

            for (int i = 1; i <= sampleCount; i++)
            {
                float u = (float)i / sampleCount;
                Vector3 pos = Evaluate(u);

                totalLength += Vector3.Distance(prevPos, pos);
                arcSamples.Add(new ArcSample(u, totalLength));

                prevPos = pos;
            }
        }

        private float DistanceToU(float targetDistance)
        {
            if (targetDistance <= 0f)
                return 0f;

            if (targetDistance >= totalLength)
                return 1f;

            int left = 0;
            int right = arcSamples.Count - 1;

            while (left < right)
            {
                int mid = (left + right) / 2;

                if (arcSamples[mid].Distance < targetDistance)
                    left = mid + 1;
                else
                    right = mid;
            }

            int nextIndex = left;
            int prevIndex = Mathf.Max(0, nextIndex - 1);

            ArcSample prev = arcSamples[prevIndex];
            ArcSample next = arcSamples[nextIndex];

            float distanceRange = next.Distance - prev.Distance;

            if (distanceRange <= Mathf.Epsilon)
                return next.U;

            float t = (targetDistance - prev.Distance) / distanceRange;
            return Mathf.Lerp(prev.U, next.U, t);
        }

        private Vector3 CalculateDirectionAtDistance(float targetDistance)
        {
            float offsetDistance = Mathf.Min(totalLength, Mathf.Max(totalLength * 0.001f, 0.0001f));
            float prevDistance = Mathf.Max(0f, targetDistance - offsetDistance);
            float nextDistance = Mathf.Min(totalLength, targetDistance + offsetDistance);

            if (nextDistance - prevDistance <= Mathf.Epsilon)
                return Vector3.zero;

            Vector3 prevPos = Evaluate(DistanceToU(prevDistance));
            Vector3 nextPos = Evaluate(DistanceToU(nextDistance));
            Vector3 direction = nextPos - prevPos;

            if (direction.sqrMagnitude <= Mathf.Epsilon)
                return Vector3.zero;

            return direction.normalized;
        }

        private Vector3 Evaluate(float u)
        {
            if (points.Count == 1)
                return points[0];

            u = Mathf.Clamp01(u);

            if (u <= 0f)
                return points[0];

            if (u >= 1f)
                return points[^1];

            int span = FindSpan(u);
            return DeBoor(span, u);
        }

        private int FindSpan(float u)
        {
            int n = points.Count - 1;

            if (u >= knots[n + 1])
                return n;

            if (u <= knots[degree])
                return degree;

            int low = degree;
            int high = n + 1;
            int mid = (low + high) / 2;

            while (u < knots[mid] || u >= knots[mid + 1])
            {
                if (u < knots[mid])
                    high = mid;
                else
                    low = mid;

                mid = (low + high) / 2;
            }

            return mid;
        }

        private Vector3 DeBoor(int span, float u)
        {
            Vector3[] d = new Vector3[degree + 1];

            for (int j = 0; j <= degree; j++)
            {
                d[j] = points[span - degree + j];
            }

            for (int r = 1; r <= degree; r++)
            {
                for (int j = degree; j >= r; j--)
                {
                    int i = span - degree + j;

                    float denominator = knots[i + degree + 1 - r] - knots[i];
                    float alpha = denominator <= Mathf.Epsilon
                        ? 0f
                        : (u - knots[i]) / denominator;

                    d[j] = Vector3.LerpUnclamped(d[j - 1], d[j], alpha);
                }
            }

            return d[degree];
        }

        public void DebugDrawListPoint(Color color, float pointSize, float duration = 0, bool depthTest = true)
        {
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                ZLogger.DrawPoint(point, pointSize, color, duration);
            }
        }
        
        public void DebugDrawPath(Color color, float duration = 0f, int segmentCount = 80, bool depthTest = true)
        {
            if (!initialized)
                throw new InvalidOperationException("Cần gọi Init trước khi DebugDrawPath.");

            if (points.Count < 2 || totalLength <= Mathf.Epsilon)
                return;

            segmentCount = Mathf.Max(1, segmentCount);

            Vector3 prevPos = CalculatePos(0f);

            for (int i = 1; i <= segmentCount; i++)
            {
                float t = (float)i / segmentCount;
                Vector3 currentPos = CalculatePos(t);

                Debug.DrawLine(prevPos, currentPos, color, duration, depthTest);

                prevPos = currentPos;
            }
        }
    }

}
