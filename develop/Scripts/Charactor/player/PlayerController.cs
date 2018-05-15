using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class PlayerController {

    // プレイヤー自体のプレハブ
    public GameObject playerObject;

    // プレイヤーの初期位置
    private Vector3 setPosition;

    // プレイヤーの移動コンポーネント
    private PlayerMove playermove;

    /// <summary>
    /// プレイヤーに関する各コンポーネント等の初期化
    /// <summary>
    public void setup() {
        // プレイヤーに関するコンポーネントを取得
        playermove = playerObject.GetComponent<PlayerMove>();

        // 現在の位置を初期位置として格納する
        setPosition = playerObject.transform.position;

        // プレイヤーの移動を無効化にする
        // isNotMoveable();
    }

    /// <summary>
    /// プレイヤー関連のオブジェクト・コンポーネント無効化
    /// </summary>
    public void DisableControl() {
        // 移動のコンポーネントを無効化
        playermove.enabled = false;
    }

    /// <summary>
    /// プレイヤー関連のオブジェクト・コンポーネント有効化
    /// </summary>
    public void EnableControl() {
        // 移動のコンポーネントを有効化
        playermove.enabled = true;

        // プレイヤー位置等を初期化
        playerObject.transform.position = setPosition;

    }

    /// <summary>
    /// プレイヤーの位置等を初期の位置に戻す
    /// </summary>
    public void ResetPlayer() {
        playermove.resetPlayer(setPosition);
    }

    /// <summary>
    /// キー入力によるキャラクターの移動を受け付けるようにする
    /// </summary>
    public void isMoveable() {
        playermove.setKeyInput(true);
    }
    /// <summary>
    /// キー入力によるキャラクターの移動を受け付けないようにする
    /// </summary>
    public void isNotMoveable() {
        playermove.setKeyInput(false);
    }




}
