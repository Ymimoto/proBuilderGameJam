using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResult : MonoBehaviour {

    // テンプレートテキスト
    // ターゲットメッセージ
    private string targetItemString = "目標おにぎり数：{XXX}個";
    // 獲得アイテムメッセージ
    private string getItemString = "獲得おにぎり数：{XXX}個";
    // 獲得アイテムメッセージ(獲得 < ターゲット)
    private string resultLessString = "ううぬ\nちょっとたりない……";
    // 獲得アイテムメッセージ(獲得 > ターゲット)
    private string resultGreaterString = "くっ！\n多かったね……";
    // 獲得アイテムメッセージ(獲得 == ターゲット)
    private string resultSuccessString = "ぴったり！\nおめでとう！！";

    // 各テキスト表示領域
    public Text GetLabel;
    public Text TargetLabel;
    public Text ResultLabel;

    // 各テキスト表示領域
    // public Text InfomationLabel;

    public void setup() {

    }

    public void drawTarget(int number) {
        // テンプレートテキストから表示文字列を作成
        string str = targetItemString.Replace("{XXX}", number.ToString());

        // UIのテキストに適用
        TargetLabel.text = str;
    }

    public void drawGet(int number) {
        // テンプレートテキストから表示文字列を作成
        string str = getItemString.Replace("{XXX}", number.ToString());

        // UIのテキストに適用
        GetLabel.text = str;
    }

    public void drawResult(int getCount, int targetCount) {
        if(getCount < targetCount) {
            ResultLabel.text = resultLessString;
        } else if(getCount > targetCount) {
            ResultLabel.text = resultGreaterString;
        } else {
            ResultLabel.text = resultSuccessString;
        }

    }
}
