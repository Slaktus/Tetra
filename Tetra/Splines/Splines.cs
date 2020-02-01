using Unity.Mathematics;

namespace Tetra.Splines
{
    public sealed class Spline
    {
        /// <summary>
        /// Calculates total length, segment start locations and segment distances
        /// </summary>
        public void BuildPath() => _solver.BuildPath();

        /// <summary>
        /// Directly evaluates the point on the curve without referring to lookup table for constant speed correction
        /// </summary>
        public float3 GetPoint(float t) => _solver.GetPoint(t);

        /// <summary>
        /// Gets point on curve, clamps t to ensure accuracy and uses lookup for constant speed
        /// </summary>
        public float3 GetPointOnPath(float t) => _solver.GetPointOnPath(math.clamp(t, 0, 1));

        public int GetTotalPointsBetweenPoints(float t, float t2) => _solver.GetTotalPointsBetweenPoints(t, t2);

        /// <summary>
        /// Generates an arc from start to end with axis perpendicular to start and end
        /// </summary>
        /// <returns>The arc.</returns>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="curvature">The distance the arc extends from the line between start and end</param>
        public static Spline GenerateArc(float3 start, float3 end, float curvature) => GenerateArc(start, end, curvature, math.cross(start, end));

        /// <summary>
        /// Generates an arc from start to end along the given axis
        /// </summary>
        /// <returns>The arc</returns>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="curvature">The distance the arc extends from the line between start and end</param>
        /// <param name="curvatureAxis">The axis the arc extends along</param>
        public static Spline GenerateArc(float3 start, float3 end, float curvature, float3 curvatureAxis)
        {
            curvatureAxis = math.normalizesafe(curvatureAxis);
            return GenerateArc(start, end, curvature, curvatureAxis, curvatureAxis);
        }

        /// <summary>
        /// Generates an arc from start to end with a separate axis for start and end points
        /// </summary>
        /// <returns>The arc.</returns>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="curvature">The distance the arc extends from the line between start and end</param>
        /// <param name="startCurvatureAxis">The axis extended from start</param>
        /// <param name="endCurvatureAxis">The axis extended from end</param>
        public static Spline GenerateArc(float3 start, float3 end, float curvature, float3 startCurvatureAxis, float3 endCurvatureAxis) => new Spline(
            new float3[]
            {
                start,
                start + math.normalize(startCurvatureAxis) * curvature,
                end + math.normalize(endCurvatureAxis) * curvature,
                end
            });

        public float length => _solver.length;
        public Type type { get; }

        private SplineSolver _solver { get; }

        /// <summary>
        /// Default constructor. Creates and initializes a spline from an array of points
        /// </summary>
        /// <param name="points">Points</param>
        /// <param name="useBezier">Use bezier if set to <c>true</c></param>
        /// <param name="useStraightLines">Use straight lines if set to <c>true</c></param>
        public Spline(float3[] points, bool useBezier = false, bool useStraightLines = false)
        {
            //Determine spline type and solver based on number of points
            if (useStraightLines || points.Length == 2)
            {
                type = Type.StraightLine;
                _solver = new StraightLineSplineSolver(points);
            }
            else if (points.Length == 3)
            {
                type = Type.QuadraticBezier;
                _solver = new QuadraticBezierSplineSolver(points);
            }
            else if (points.Length == 4)
            {
                type = Type.CubicBezier;
                _solver = new CubicBezierSplineSolver(points);
            }
            else if (useBezier)
            {
                type = Type.Bezier;
                _solver = new BezierSplineSolver(points);
            }
            else
            {
                type = Type.CatmullRom;
                _solver = new CatmullRomSplineSolver(points);
            }
        }

        public enum Type
        {
            StraightLine, // 2 points
            QuadraticBezier, // 3 points
            CubicBezier, // 4 points
            CatmullRom, // 5+ points
            Bezier // 5+ points
        }
    }
}