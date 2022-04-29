using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour
{
	public CanvasGroup group;

	public float fadeInTime = 1f;
	public float fadeOutTime = 1f;
    private bool fading = false;

	public void FadeOut()
	{
		StartCoroutine(FadeRoutine(false));
	}

	public void FadeOutDirectly()
	{
		group.alpha = 0;
		gameObject.SetActive(false);
	}

	public void FadeIn()
	{
		gameObject.SetActive(true);
		StartCoroutine(FadeRoutine(true));
	}

	public void FadeInDirectly()
	{
		gameObject.SetActive(true);
		group.alpha = 1;
	}

	private IEnumerator FadeRoutine(bool fadeIn)
	{
		float from = fadeIn ? 0 : 1;
		float to = fadeIn ? 1 : 0;
		float fadeFactor = fadeIn ? fadeInTime : fadeOutTime;
		float t = fading ? Mathf.InverseLerp(from, to, group.alpha) : 0;

		fading = true;
		while (t < 1)
		{
			t += Time.deltaTime / fadeFactor;
			group.alpha = Mathf.Lerp(from, to, t);
			yield return null;
		}
		group.alpha = to;
		fading = false;

		if (!fadeIn) gameObject.SetActive(false);
	}
}
