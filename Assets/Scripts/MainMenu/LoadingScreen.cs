using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{

	[SerializeField]
	private GameObject _loadingObj, _textObj;
	[SerializeField]
	private ChangeLanguage _changeLanguage;


	void Start()
	{
		_changeLanguage.Load();
		StartCoroutine(LoadScene());
		StartPicChange();
	}

	IEnumerator LoadScene()
	{
		yield return new WaitForSeconds(1);
		AsyncOperation op = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("SceneToLoad"));
		op.allowSceneActivation = false;

		while (op.progress < 0.9f)
		{
			yield return null;
		}

		_loadingObj.SetActive(false);
		_textObj.SetActive(true);

		while (op.progress >= 0.9f)
		{
			if (Input.anyKeyDown)
			{
				op.allowSceneActivation = true;
			}
			yield return null;
		}
	}

	[SerializeField]
	private Image _image;
	[SerializeField]
	private Sprite[] _sprites;
	private int _spriteIndex;


	private bool _isChanging;

	private void StartPicChange()
	{
		_image.color = Color.white; 
		_spriteIndex = UnityEngine.Random.Range(0, _sprites.Length);
		_image.sprite = _sprites[_spriteIndex];
		StartCoroutine(ChangePictures());
	}
	private IEnumerator ChangePictures()
	{
		while (true)
		{
			yield return new WaitForSeconds(3);

			_isChanging = true;
			ChangePics();

			while (_isChanging)
				yield return null;

			yield return null;
		}
	}
	private void ChangePics()
	{
		if (_image.color == Color.white)
			_image.DOColor(Color.black, 1).OnComplete(ColorChangeOutComplete);
		else
			_image.DOColor(Color.white, 1).OnComplete(ColorChangeInComplete);
	}
	private void ColorChangeOutComplete()
	{
		_spriteIndex = (_spriteIndex + 1) % _sprites.Length;
		_image.sprite = _sprites[_spriteIndex];
		_image.color = Color.black;
		ChangePics();
	}
	private void ColorChangeInComplete()
	{
		_image.color = Color.white; 
		_isChanging = false;
	}
}
