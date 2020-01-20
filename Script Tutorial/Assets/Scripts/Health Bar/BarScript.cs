using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
  private float fillAmount;

  [SerializeField] private Image content;

  [SerializeField] private Text valueText;

  [SerializeField] private float lerpSpeed;

  public float MaxValue { get; set; }

  public float Value {
    set {
      string[] tmp = valueText.text.Split('/');
      valueText.text = value + "/" + tmp[1];
      fillAmount = Map(value, MaxValue, 0, 1, 0);
    }
  }

  void Start()
  {
        
  }

  void Update()
  {
    ManageBar();
  }

  private void ManageBar() {
    if (content.fillAmount != fillAmount) {
      content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
    }
  }

  private float Map(float val, float inMax, float inMin, float outMax, float outMin) {
    return (val - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
  }
}
