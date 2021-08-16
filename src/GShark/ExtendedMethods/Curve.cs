﻿using GShark.Geometry;
using GShark.Geometry.Interfaces;
using GShark.Operation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GShark.ExtendedMethods
{
    public static class Curve
    {
        /// <summary>
        /// Divides a curve for a given number of time, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="numberOfSegments">The number of parts to split the curve into.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        public static (List<Point3> Points, List<double> Parameters) Divide(this ICurve curve, int numberOfSegments)
        {
            if (numberOfSegments < 2)
            {
                throw new ArgumentException("Number of segments must be greater than 1.", nameof(numberOfSegments));
            }

            if (curve == null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            var divideResult = Operation.Divide.CurveByCount(curve, numberOfSegments);
            var points = divideResult.Select(curve.PointAt).ToList();
            return (points, divideResult);
        }

        /// <summary>
        /// Divides a curve for a given max segment length, including the end points.<br/>
        /// The result is not split curves but a collection of t values and lengths that can be used for splitting.<br/>
        /// As with all arc length methods, the result is an approximation.
        /// </summary>
        /// <param name="curve">The curve object to divide.</param>
        /// <param name="maxSegmentLength">The maximum length the segments have to be split in.</param>
        /// <param name="equalSegmentLengths">Force to have all the segments of the same lengths.</param>
        /// <returns>A tuple define the t values where the curve is divided and the lengths between each division.</returns>
        public static (List<Point3> Points, List<double> Parameters) Divide(this ICurve curve, double maxSegmentLength, bool equalSegmentLengths = false)
        {
            if (maxSegmentLength <= 0)
            {
                throw new ArgumentException("Segment length must be greater than 0.", nameof(maxSegmentLength));
            }

            if (curve == null)
            {
                throw new ArgumentNullException(nameof(curve));
            }

            var len = maxSegmentLength;
            if (equalSegmentLengths)
            {
                List<ICurve> curves = Modify.DecomposeCurveIntoBeziers(curve);
                List<double> curveLengths = curves.Select(nurbsCurve => Analyze.BezierCurveLength(nurbsCurve)).ToList();
                double totalLength = curveLengths.Sum();

                len = totalLength / Math.Ceiling(totalLength / maxSegmentLength);
            }

            var (tValues, lengths) = Operation.Divide.CurveByLength(curve, len);
            var points = tValues.Select(curve.PointAt).ToList();

            return (points, tValues);
        }

        /// <summary>
        /// Creates rotation minimized perpendicular frames (RMF) at given t parameters along the curve.
        /// <para>//Double reflection method taken from Wang, W., J¨uttler, B., Zheng, D., and Liu, Y. 2008. "Computation of rotation minimizing frame."  https://www.microsoft.com/en-us/research/wp-content/uploads/2016/12/Computation-of-rotation-minimizing-frames.pdf </para>
        /// </summary>
        ///<param name="curve">The input curve.</param>
        /// ///<param name="uValues">The curve parameter values to locate perpendicular curve frames</param>
        public static List<Plane> PerpendicularFrames(this ICurve curve, List<double> uValues)
        {
            var pointsOnCurve = uValues.Select(curve.PointAt).ToList(); //get points at t values
            var pointsOnCurveTan = uValues.Select(t => Evaluation.RationalCurveTangent(curve, t)).ToList(); //get tangents at t values
            var firstParameter = uValues[0]; //get first t value

            //Create initial frame at first parameter
            var origin = curve.PointAt(firstParameter);
            var crvTan = Evaluation.RationalCurveTangent(curve, firstParameter);
            var crvNormal = Vector3.PerpendicularTo(crvTan);
            var yAxis = Vector3.CrossProduct(crvTan, crvNormal);
            var xAxis = Vector3.CrossProduct(yAxis, crvTan);

            //Set initial frame
            Plane[] perpFrames = new Plane[pointsOnCurve.Count];
            perpFrames[0] = new Plane(origin, xAxis, yAxis);

            //Given boundary data(x0, t0; x1, t1) and an initial right-handed
            //orthonormal frame U0 = (r0, s0, t0) at x0, the next frame U1 = (r1, s1, t1)
            //at x1 for RMF approximation is computed by the double reflection method in
            //the following two steps.
            //
            //Step 1.Let R1 denote the reflection in the bisecting plane of the points x0
            //and x1(see Figure 4).Use R1 to map U0 to a left - handed orthonormal frame
            //UL0 = (rL0, sL0, tL0).
            //
            //Step 2.Let R2 denote the reflection in the bisecting plane of the points x1 + tL
            //0 and x1 +t1. Use R2 to map UL0 to a right - handed orthonormal frame U1 = (r1, s1, t1).
            //Output U1.

            for (int i = 0; i < pointsOnCurve.Count - 1; i++)
            {
                Vector3 v1 = pointsOnCurve[i + 1] - pointsOnCurve[i]; //compute reflection vector of R1
                double c1 = v1 * v1;
                Vector3 rLi = perpFrames[i].XAxis - (2 / c1) * (v1 * perpFrames[i].XAxis) * v1; //compute reflected rL vector by R1
                Vector3 tLi = pointsOnCurveTan[i] - (2 / c1) * (v1 * pointsOnCurveTan[i]) * v1; //compute reflected tL vector by R1
                Vector3 v2 = pointsOnCurveTan[i + 1] - tLi; //compute reflection vector of R2
                double c2 = v2 * v2;
                Vector3 rNext = rLi - (2 / c2) * (v2 * rLi) * v2; //compute reflected r vector by R2
                var sNext = Vector3.CrossProduct(pointsOnCurveTan[i + 1], rNext); //compute vector s[i+1] of next frame

                //create output frame
                var frameNext = new Plane { Origin = pointsOnCurve[i + 1], XAxis = rNext, YAxis = sNext };
                perpFrames[i + 1] = frameNext; //output frame
            }

            return perpFrames.ToList();
        }
    }
}
