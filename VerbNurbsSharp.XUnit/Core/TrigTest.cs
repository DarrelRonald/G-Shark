﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using VerbNurbsSharp.Core;
using VerbNurbsSharp.Geometry;
using Xunit;
using Xunit.Abstractions;

namespace VerbNurbsSharp.XUnit.Core
{
    [Trait("Category", "Trig")]
    public class TrigTest
    {
        private readonly ITestOutputHelper _testOutput;

        public TrigTest(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void ThreePointsAreFlat_ReturnTrue()
        {
            Vector3 p1 = new Vector3() { 0.0, 0.0, 0.0 };
            Vector3 p2 = new Vector3() { 10.0, 0.0, 0.0 };
            Vector3 p3 = new Vector3() { 5.0, 5.0, 0.0 };
            Vector3 p4 = new Vector3() { -5.0, -15.0, 0.0 };
            List<Vector3> points = new List<Vector3>(){p1,p2,p3,p4};

            Trig.ArePointsCoplanar(points).Should().BeTrue();
        }

        [Fact]
        public void RayClosestPoint_ReturnTheProjectPoint()
        {
            Ray ray = new Ray(new Vector3(){0,0,0},new Vector3(){30,45,0});
            Vector3 pt = new Vector3(){10,20,0};
            Vector3 expectedPt = new Vector3(){ 12.30769230769231, 18.461538461538463, 0 };

            Vector3 closestPt = Trig.RayClosestPoint(pt, ray);

            closestPt.Should().BeEquivalentTo(closestPt);
        }

        [Fact]
        public void DistanceToRay_ReturnTheDistance_Between_APointAndTheRay()
        {
            Ray ray = new Ray(new Vector3() { 0, 0, 0 }, new Vector3() { 30, 45, 0 });
            Vector3 pt = new Vector3() { 10, 20, 0 };
            double distanceExpected = 2.7735009811261464;

            double distance = Trig.DistanceToRay(pt, ray);

            _testOutput.WriteLine(distance.ToString());
            distance.Should().Be(distanceExpected);
        }

        [Fact]
        public void isPointOnPlane_ReturnTrue_IfThePointLiesOnPlane()
        {
            Plane plane = new Plane(new Vector3() { 30, 45, 0 }, new Vector3() { 30, 45, 0 });
            Vector3 pt = new Vector3() { 26.565905, 47.289396, 0.0 };

            Trig.isPointOnPlane(pt, plane, 0.1).Should().BeTrue();
        }
    }
}
