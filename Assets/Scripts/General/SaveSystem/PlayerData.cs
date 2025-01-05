using UnityEngine;

[System.Serializable]
public class PlayerData: GameData
{
	public Vector3 Position;
	public Quaternion Rotation;
	public int SelectedWeapon;
	public float Health;
	public float Stamina;
}