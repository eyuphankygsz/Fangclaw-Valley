using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LockKey))]
public class LockKeyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		SerializedProperty lockedProp = property.FindPropertyRelative("_locked");
		SerializedProperty keyNameProp = property.FindPropertyRelative("_keyName");

		// �izim yaparken kullanaca��n�z pozisyonlar� ayarlay�n
		Rect lockedRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		Rect keyNameRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

		// _locked alan�n� �izdirin
		EditorGUI.PropertyField(lockedRect, lockedProp);

		// _locked true ise _keyName alan�n� �izdirin
		if (lockedProp.boolValue)
		{
			EditorGUI.PropertyField(keyNameRect, keyNameProp);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		// �ki alan�n y�ksekli�ini d�nd�r
		if (property.FindPropertyRelative("_locked").boolValue)
		{
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
		return EditorGUIUtility.singleLineHeight;
	}
}
