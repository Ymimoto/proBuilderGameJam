using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICountdown : MonoBehaviour {

    // 各テキスト表示領域
    public Text CountNumberLabel;

    public void setup() {

    }

    public void drawMessage(int number) {

        // 加工せずにそのままUIに適用
        CountNumberLabel.text = number.ToString();
    }
}
