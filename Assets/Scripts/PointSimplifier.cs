/*
 * The MIT License (MIT)
 * Copyright (c) 2013 Danilo Gaby Andersen Trindade
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
 * and to permit persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class PointSimplifier
{
	/// <summary>
	/// Runs the Ramer-Douglas-Peucker algorithm to straighten the lines in the point list.
	/// </summary>
	/// <returns>
	/// An ArrayList with the straightened lines.
	/// </returns>
	/// <param name='pointList'>
	/// List of points to straighten.
	/// </param>
	/// <param name='precision'>
	/// Precision in pixels of points to consider in the same line.
	/// </param>
	/// <param name='startPosition'>
	/// Start of the pointList to consider. Default to 0;
	/// </param>
	/// <param name='endPosition'>
	/// End of the pointList to consider. Negative numbers will be considered total size of the list. Default to -1.
	/// </param>
	public static List<Vector2> RamerDouglasPeucker(List<Vector2> pointList, float precision, int startPosition = 0, int endPosition = -1) {
		float dmax = 0.0f;
		int index = -1;
		
		List<Vector2> resultList = new List<Vector2>();
		
		if (endPosition < 0) {
			endPosition = pointList.Count-1;
		}
		
		for (int i = startPosition; i < endPosition; i++) {
			float d = PerpendicularDistance(pointList[i], pointList[startPosition], pointList[endPosition]);
			if (d > dmax) {
				index = i;
				dmax = d;
			}
		}
		
		if (dmax >= precision) {
			List<Vector2> result2 = RamerDouglasPeucker(pointList, precision, index, endPosition);
			resultList = RamerDouglasPeucker(pointList, precision, startPosition, index);
			
			result2.RemoveAt(0);
			resultList.AddRange(result2);
		} else {
			resultList.Add(pointList[startPosition]);
			resultList.Add(pointList[endPosition]);
		}
		
		return resultList;
	}
	
	/// <summary>
	/// Calculates the distance between two points.
	/// </summary>
	/// <returns>
	/// The distance.
	/// </returns>
	/// <param name='point1'>
	/// First point.
	/// </param>
	/// <param name='point2'>
	/// Second point.
	/// </param>
	private static float PointDistance(Vector2 point1, Vector2 point2) {
		float dx = point1.x - point2.x;
		float dy = point1.y - point2.y;
		return (float)Math.Sqrt( dx*dx + dy*dy );
	}
	
	/// <summary>
	/// Calculates Angular Coefficient of a line between two different points.
	/// </summary>
	/// <returns>
	/// The coefficient.
	/// </returns>
	/// <param name='point1'>
	/// First point.
	/// </param>
	/// <param name='point2'>
	/// Second point.
	/// </param>
	private static float AngularCoefficient(Vector2 point1, Vector2 point2) {
		float yDiff = (point1.y - point2.y);
		float xDiff = (point1.x - point2.x);
		if (xDiff == 0.0f) xDiff = 0.0001f;
		
		return (yDiff/xDiff);
	}
	
	/// <summary>
	/// Calculates Y-Intercept from the angular coeficient of a line
	/// between two different points and one of these points.
	/// </summary>
	/// <returns>
	/// The Y-intercept.
	/// </returns>
	/// <param name='point'>
	/// Point.
	/// </param>
	/// <param name='angularCoef'>
	/// Angular coeficient.
	/// </param>
	private static float YIntercept(Vector2 point, float angularCoef) {
		return (point.y - (angularCoef*point.x));
	}
	
	/// <summary>
	/// Calculates the distance from a point to a line.
	/// </summary>
	/// <returns>
	/// The distance.
	/// </returns>
	/// <param name='point'>
	/// Point.
	/// </param>
	/// <param name='lineStartPoint'>
	/// Line start point.
	/// </param>
	/// <param name='lineEndPoint'>
	/// Line end point.
	/// </param>
	private static float PerpendicularDistance(Vector2 point, Vector2 lineStartPoint, Vector2 lineEndPoint) {
		
		float d = 0.0f;
		float denom = 0.0001f;
		
		if (lineStartPoint.x == lineEndPoint.x) {
			return (Math.Abs(point.x - lineStartPoint.x));
		} else {
			float angCoef = AngularCoefficient(lineStartPoint, lineEndPoint);
			float yInter = YIntercept(lineStartPoint, angCoef);
			
			d = Math.Abs((angCoef*point.x)-point.y+yInter);
			denom = (float)Math.Sqrt((angCoef*angCoef)+1);
		}
		
		return (d/denom);
	}
	
}

