using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class UIController {

    // 各ステートで表示されるUI
    public UITitle uiTitle;
    public UIStart uiStart;
    public UICountdown uiCountdown;
    public UIResult uiResult;

    // ゲーム中のUI
    public UIScore uiScore;

    public void setup() {
        // 各UIを初期化(予定はない)


        // いったんすべてのUIを非アクティブにする
        uiTitle.gameObject.SetActive(false);
        uiStart.gameObject.SetActive(false);
        uiCountdown.gameObject.SetActive(false);
        uiResult.gameObject.SetActive(false);
        uiScore.gameObject.SetActive(false);
    }

    /// <summary>
    /// タイトルステート用UIのみをアクティブにする
    /// </summary>
    public void setUiTitleActive() {
        uiTitle.gameObject.SetActive(true);

        uiStart.gameObject.SetActive(false);
        uiCountdown.gameObject.SetActive(false);
        uiResult.gameObject.SetActive(false);
        uiScore.gameObject.SetActive(false);
    }

    /// <summary>
    /// スタートステート用UIのみをアクティブにする
    /// </summary>
    public void setUiStartActive() {
        uiStart.gameObject.SetActive(true);

        uiTitle.gameObject.SetActive(false);
        uiCountdown.gameObject.SetActive(false);
        uiResult.gameObject.SetActive(false);
        uiScore.gameObject.SetActive(false);
    }

    /// <summary>
    /// カウントダウンステート用UIのみをアクティブにする
    /// </summary>
    public void setUiCountdownActive() {
        uiCountdown.gameObject.SetActive(true);

        uiStart.gameObject.SetActive(false);
        uiStart.gameObject.SetActive(false);
        uiResult.gameObject.SetActive(false);
        uiScore.gameObject.SetActive(false);
    }

    /// <summary>
    /// ゲームステート用UIのみをアクティブにする
    /// </summary>
    public void setUiScoreActive() {
        uiScore.gameObject.SetActive(true);

        uiStart.gameObject.SetActive(false);
        uiStart.gameObject.SetActive(false);
        uiCountdown.gameObject.SetActive(false);
        uiResult.gameObject.SetActive(false);
    }

    /// <summary>
    /// 結果表示ステート用UIのみをアクティブにする
    /// </summary>
    public void setUiResultActive() {
        uiResult.gameObject.SetActive(true);

        uiStart.gameObject.SetActive(false);
        uiStart.gameObject.SetActive(false);
        uiCountdown.gameObject.SetActive(false);
        uiScore.gameObject.SetActive(false);
    }

    /// <summary>
    /// スタートステート用UIを更新する
    /// </summary>
    public void updateUiStart(int targetCount) {
        if(uiStart.gameObject.activeSelf == true) {
            uiStart.drawMessage(targetCount);
        }
    }

    /// <summary>
    /// カウントダウンステート用UIを更新する
    /// </summary>
    public void updateUiCountdown(int countDown) {
        if(uiCountdown.gameObject.activeSelf == true) {
            uiCountdown.drawMessage(countDown);
        }
    }

    /// <summary>
    /// ゲームステート用UIを更新する
    /// </summary>
    public void updateUiScore<T>(int time, T getCount, T targetCount) {
        if(uiScore.gameObject.activeSelf == true) {
            uiScore.drawTime(time);
            uiScore.drowScore(getCount);
            uiScore.drawTarget(targetCount);
        }
    }

    /// <summary>
    /// 結果表示ステート用UIを更新する
    /// </summary>
    public void updateUiResult(int getCount, int targetCount) {
        if(uiResult.gameObject.activeSelf == true) {
            uiResult.drawTarget(targetCount);
            uiResult.drawGet(getCount);
            uiResult.drawResult(getCount, targetCount);
        }
    }

}

