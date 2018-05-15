using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour {

    // モザイク表示フラグ
    public bool isMosaic;

    // テンプレートテキスト
    // 時間テキスト
    private string timeString = "Time: {XXX}";
    // 獲得アイテム数テキスト
    private string getItemString = ": {XXX}";
    // 目標アイテム数テキスト
    private string targetItemStrinig = "Target: {XXX}";

    // 各テキスト表示領域
    public Text TimeLabel;
    public Text OnigiriCountLabel;
    public Text TargetCountLabel;

    public void setup() {

    }

    public void drowScore<T>(T number) {
        // テンプレートテキストから表示文字列を作成
        string str = getItemString.Replace("{XXX}", number.ToString());

        // UIのテキストに適用
        OnigiriCountLabel.text = str;

    }

    public void drawTime(int number) {
        // テンプレートテキストから表示文字列を作成
        string str = timeString.Replace("{XXX}", number.ToString());

        // UIのテキストに適用
        TimeLabel.text = str;
    }

    public void drawTarget<T>(T number) {
        // テンプレートテキストから表示文字列を作成
        string str = targetItemStrinig.Replace("{XXX}", number.ToString());

        // UIのテキストに適用
        TargetCountLabel.text = str;
    }

    void setMosaic() {

    }
}
