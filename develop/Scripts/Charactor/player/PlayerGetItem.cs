using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテム獲得に関する処理を行うモジュール
/// </summary>
public class PlayerGetItem : MonoBehaviour {

    private GameManager gameManager;

    private void Start() {
        // ゲームマネージャへ処理を送るため、あらかじめ取得しておく
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// トリガーを持つオブジェクトに触れた場合の処理
    /// </summary>
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "item") {
            // 接触したのがアイテムだった場合、ゲームマネージャでアイテム獲得処理を行わせる
            gameManager.setItemCount(other.gameObject);
        }
    }
}
