using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour {

    // テンプレートテキスト
    // ターゲットメッセージ
    private string infomationString = "おにぎりを！{XXX}個！\nあつめろ！！！！";

    // 各テキスト表示領域
    public Text InfomationLabel;

    public void setup() {

    }

    public void drawMessage(int number) {
        // テンプレートテキストから表示文字列を作成
        string str = infomationString.Replace("{XXX}", number.ToString());

        // UIのテキストに適用
        InfomationLabel.text = str;
    }
}
