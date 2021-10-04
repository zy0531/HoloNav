using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public sealed class GPSEncoder_Double
{

	/////////////////////////////////////////////////
	//////-------------Public API--------------//////
	/////////////////////////////////////////////////

	/// <summary>
	/// Convert UCS (X,Y,Z) coordinates to GPS (Lat, Lon) coordinates
	/// </summary>
	/// <returns>
	/// Returns Vector2 containing Latitude and Longitude
	/// </returns>
	/// <param name='position'>
	/// (X,Y,Z) Position Parameter
	/// </param>
	public static Vector2 USCToGPS_Double(Vector3 position)
	{
		return GetInstance().ConvertUCStoGPS_Double(position);
	}


	/// <summary>
	/// Convert GPS (Lat, Lon) coordinates to UCS (X,Y,Z) coordinates _ double
	/// </summary>
	/// <returns>
	/// Returns a Vector3 containing (X, Y, Z)
	/// </returns>

	public static Vector3 GPSToUCS_Double(double latitude, double longitude)
	{
		return GetInstance().ConvertGPStoUCS_Double(latitude, longitude);
	}

	/// <summary>
	/// Change the relative GPS offset (Lat, Lon), Default (0,0), 
	/// used to bring a local area to (0,0,0) in UCS coordinate system
	/// </summary>
	/// <param name='localOrigin'>
	/// Referance point.
	/// </param>
	public static void SetLocalOrigin(double _LatOrigin_Double, double _LonOrigin_Double)
	{
		GetInstance()._LatOrigin_Double = _LatOrigin_Double;
		GetInstance()._LonOrigin_Double = _LonOrigin_Double;
	}

	/////////////////////////////////////////////////
	//////---------Instance Members------------//////
	/////////////////////////////////////////////////

	#region Singleton
	private static GPSEncoder_Double _singleton;

	private GPSEncoder_Double()
	{

	}

	private static GPSEncoder_Double GetInstance()
	{
		if (_singleton == null)
		{
			_singleton = new GPSEncoder_Double();
		}
		return _singleton;
	}
	#endregion

	#region Instance Variables
	//private Vector2 _localOrigin = Vector2.zero;

	//private double _LatOrigin_Double { get { return _localOrigin.x; } }
	//private double _LonOrigin_Double { get { return _localOrigin.y; } }

	private double _LatOrigin_Double = 0;
	private double _LonOrigin_Double = 0;

	private double metersPerLat_Double;
	private double metersPerLon_Double;
	#endregion

	#region Instance Functions

	private void FindMetersPerLat_Double(double lat) // Compute lengths of degrees
	{
		// Set up "Constants"
		double m1 = 111132.92;    // latitude calculation term 1
		double m2 = -559.82;        // latitude calculation term 2
		double m3 = 1.175;      // latitude calculation term 3
		double m4 = -0.0023;        // latitude calculation term 4
		double p1 = 111412.84;    // longitude calculation term 1
		double p2 = -93.5;      // longitude calculation term 2
		double p3 = 0.118;      // longitude calculation term 3

		//lat = lat * Math.Deg2Rad;
		lat = ConvertToRadians(lat);

		// Calculate the length of a degree of latitude and longitude in meters
		metersPerLat_Double = m1 + (m2 * Math.Cos(2 * lat)) + (m3 * Math.Cos(4 * lat)) + (m4 * Math.Cos(6 * lat));
		metersPerLon_Double = (p1 * Math.Cos(lat)) + (p2 * Math.Cos(3 * lat)) + (p3 * Math.Cos(5 * lat));
	}

	public double ConvertToRadians(double angle)
	{
		return (Math.PI / 180.0) * angle;
	}


	public Vector3 ConvertGPStoUCS_Double(double latitude, double longitude)
    {
		FindMetersPerLat_Double(_LatOrigin_Double);
		double zPosition = metersPerLat_Double * (latitude - _LatOrigin_Double); //Calc current lat
		double xPosition = metersPerLon_Double * (longitude - _LonOrigin_Double); //Calc current lat
		return new Vector3((float)xPosition, 0, (float)zPosition);
	}

	private Vector2 ConvertUCStoGPS_Double(Vector3 position)
	{
		FindMetersPerLat_Double(_LatOrigin_Double);
		Vector2 geoLocation = new Vector2(0, 0);
		geoLocation.x = (float)(_LatOrigin_Double + (position.z) / metersPerLat_Double); //Calc current lat
		geoLocation.y = (float)(_LonOrigin_Double + (position.x) / metersPerLon_Double); //Calc current lon
		return geoLocation;
	}
	#endregion
}
