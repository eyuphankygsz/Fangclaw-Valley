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

		// Çizim yaparken kullanacaðýnýz pozisyonlarý ayarlayýn
		Rect lockedRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		Rect keyNameRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);

		// _locked alanýný çizdirin
		EditorGUI.PropertyField(lockedRect, lockedProp);

		// _locked true ise _keyName alanýný çizdirin
		if (lockedProp.boolValue)
		{
			EditorGUI.PropertyField(keyNameRect, keyNameProp);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		// Ýki alanýn yüksekliðini döndür
		if (property.FindPropertyRelative("_locked").boolValue)
		{
			return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
		}
		return EditorGUIUtility.singleLineHeight;
	}
}
