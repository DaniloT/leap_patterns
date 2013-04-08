using UnityEngine;
using System.Collections;
using Leap;

public class GestureData : Object {
	
	public float maximumX = -1000000000.0f;
	public float maximumY = -1000000000.0f;
	public float maximumZ = -1000000000.0f;
	public float minimumX = 1000000000.0f;
	public float minimumY = 1000000000.0f;
	public float minimumZ = 1000000000.0f;
	
	public ArrayList frames = null;

	public GestureData() {
		frames = new ArrayList();
	}
	
	public void appendFrame(GestureFrame frame) {
		if (frame.position.x < this.minimumX) {
			this.minimumX = frame.position.x;
		}
		
		if (frame.position.y < this.minimumY) {
			this.minimumY = frame.position.y;
		}
		
		if (frame.position.z < this.minimumZ) {
			this.minimumZ = frame.position.z;
		}
		
		if (frame.position.x > this.maximumX) {
			this.maximumX = frame.position.x;
		}
		
		if (frame.position.y > this.maximumY) {
			this.maximumY = frame.position.y;
		}
		
		if (frame.position.z > this.maximumZ) {
			this.maximumZ = frame.position.z;
		}
		
		this.frames.Add(frame);
	}
	
	/// <summary>
	/// Gets the normalized gesture data.
	/// </summary>
	/// <returns>
	/// The normalized gesture data.
	/// </returns>
	/// <param name='resolution'>
	/// Resolution to normalize to.
	/// </param>
	public GestureData getNormalizedGestureData(float resolution) {
		var dx = (this.maximumX - this.minimumX) * 1.0f;
		var dy = (this.maximumY - this.minimumY) * 1.0f;
				
		if (dx > dy)
			dy = dx;
		else
			dx = dy;
		
		GestureData normalizedData = new GestureData();
		
		for (var i = 0; i < this.frames.Count; i++) {
			GestureFrame frame = (GestureFrame)this.frames[i];
			
			float x = (frame.position.x * 1.0f - this.minimumX) / dx * resolution;
			float y = resolution - (frame.position.y * 1.0f - this.minimumY) / dy * resolution;
			
			GestureFrame normalizedFrame = new GestureFrame(new Vector(x, y, 0.0f), frame.velocity, frame.timestamp);
			normalizedData.appendFrame(normalizedFrame);
		}
		
		return normalizedData;
	}
}
