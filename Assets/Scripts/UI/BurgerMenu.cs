using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerMenu : MonoBehaviour
{
    public GameObject UI_Menu;

	private bool runningAnim;

	private void Awake()
	{
		runningAnim = false;
	}

	public void buttonClick()
	{
		if (UI_Menu.GetComponent<CanvasGroup>().interactable && runningAnim == false)
		{
			UI_Menu.GetComponent<Animator>().Play("close_UI_Menu");
			gameObject.GetComponent<Animator>().Play("goUp");
			StartCoroutine(waitAnim());
		}
		else if(!UI_Menu.GetComponent<CanvasGroup>().interactable && runningAnim == false)
		{
			gameObject.GetComponent<Animator>().Play("goDown");
			UI_Menu.GetComponent<Animator>().Play("open_UI_Menu");
			StartCoroutine(waitAnim());
		}


	}

	private IEnumerator waitAnim()
	{
		runningAnim = true;
		yield return new WaitForSeconds(1.5f);
		runningAnim = false;
	}

}
