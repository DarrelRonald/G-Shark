﻿using FluentAssertions;
using GShark.Core;
using GShark.Geometry;
using GShark.Geometry.Enum;
using GShark.Operation;
using GShark.Test.XUnit.Data;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace GShark.Test.XUnit.Geometry
{
    public class NurbsSurfaceTests
    {
        private readonly ITestOutputHelper _testOutput;
        public NurbsSurfaceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void It_Returns_A_NURBS_Surface_By_Four_Points()
        {
            // Arrange
            Point3 p1 = new Point3(0.0, 0.0, 0.0);
            Point3 p2 = new Point3(10.0, 0.0, 0.0);
            Point3 p3 = new Point3(10.0, 10.0, 2.0);
            Point3 p4 = new Point3(0.0, 10.0, 4.0);

            Point3 expectedPt = new Point3(5.0, 5.0, 1.5);

            // Act
            NurbsSurface surface = NurbsSurface.CreateFromCorners(p1, p2, p3, p4);
            Point3 evalPt = Evaluation.SurfacePointAt(surface, 0.5, 0.5);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(2);
            surface.LocationPoints[0].Count.Should().Be(2);
            surface.LocationPoints[0][1].Equals(p4).Should().BeTrue();
            evalPt.EpsilonEquals(expectedPt, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Theory]
        [InlineData(0.1, 0.1, new double[] { -0.020397, -0.392974, 0.919323 })]
        [InlineData(0.5, 0.5, new double[] { 0.091372, -0.395944, 0.913717 })]
        [InlineData(1.0, 1.0, new double[] { 0.507093, -0.169031, 0.845154 })]
        public void It_Returns_The_Surface_Normal_At_A_Given_U_And_V_Parameter(double u, double v, double[] pt)
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedNormal = new Vector3(pt[0], pt[1], pt[2]);

            // Act
            Vector3 normal = surface.EvaluateAt(u, v, SurfaceDirection.Normal);

            // Assert
            normal.EpsilonEquals(expectedNormal, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_The_Evaluated_Surface_At_A_Given_U_And_V_Parameter()
        {
            // Assert
            NurbsSurface surface = NurbsSurfaceCollection.SurfaceFromPoints();
            Vector3 expectedUDirection = new Vector3(0.985802, 0.152837, 0.069541);
            Vector3 expectedVDirection = new Vector3(0.053937, 0.911792, 0.407096);

            // Act
            Vector3 uDirection = surface.EvaluateAt(0.3, 0.5, SurfaceDirection.U);
            Vector3 vDirection = surface.EvaluateAt(0.3, 0.5, SurfaceDirection.V);

            // Assert
            uDirection.EpsilonEquals(expectedUDirection, GeoSharkMath.MinTolerance).Should().BeTrue();
            vDirection.EpsilonEquals(expectedVDirection, GeoSharkMath.MinTolerance).Should().BeTrue();
        }

        [Fact]
        public void It_Returns_A_NURBS_Lofted_Surface()
        {
            // Arrange
            List<Point3> pts1 = new List<Point3> { new Point3(-20.0, 0.0, 0.0),
                                                   new Point3(0.0, 0.0, 10.0),
                                                   new Point3(10.0, 0.0, 0.0) };

            List<Point3> pts2 = new List<Point3> { new Point3(-15.0, 2.0, 0.0),
                                                   new Point3(0.0, 2.0, 5.0),
                                                   new Point3(20.0, 2.0, 1.0) };

            List<Point3> pts3 = new List<Point3> { new Point3(-5.0, 7.0, 0.0),
                                                   new Point3(0.0, 7.0, 20.0),
                                                   new Point3(10.0, 7.0, 0.0) };

            List<Point3> pts4 = new List<Point3> { new Point3(-5.0, 10.0, -2.0),
                                                   new Point3(0.0, 10.0, 20.0),
                                                   new Point3(5.0, 10.0, 0.0) };

            NurbsCurve c1 = new NurbsCurve(pts1, 2);
            NurbsCurve c2 = new NurbsCurve(pts2, 2);
            NurbsCurve c3 = new NurbsCurve(pts3, 2);
            NurbsCurve c4 = new NurbsCurve(pts4, 2);

            // Act
            NurbsSurface surface = NurbsSurface.CreateLoftedSurface(new List<NurbsCurve> { c1, c2, c3, c4}, 2);

            // Assert
            surface.Should().NotBeNull();
            surface.LocationPoints.Count.Should().Be(4);
            surface.LocationPoints[0].Count.Should().Be(3);
        }
    }
}
