using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// カメラ移動の可/不可切り替え
	public bool moveable;

    // カメラY軸初期位置
    private float defaultY;

    // 追従対象
    public GameObject targetObject;

	/*
	 * Update()実行前処理
	 */
	void Start () {
        // 現在のy位置を取得
        defaultY = transform.position.y;
    }

	void Update () {
		// 動かすのが主なので、今のところはないです
	}

    /// <summary>
    /// コンポーネント有効化時処理(コンポーネントが動く状態になったとき)
    /// </summary>
    void OnEnable() {
        // 移動可能状態にする
        moveable = true;
    }

    /// <summary>
    /// コンポーネント無効化時処理(コンポーネントが動かない状態になったとき)
    /// </summary>
    void OnDisable() {
        // 移動不可状態にする
        moveable = false;
    }

    void FixedUpdate(){
		// カメラを移動させる
		MoveCamera();
	}

	void MoveCamera(){
        // カメラが移動可能かチェック
        if(moveable) {
            // 追従対象の位置を取得する
            Vector3 targetPosition = targetObject.transform.position;

            float setY;
            if(targetPosition.y < defaultY) {
                // 追従対象のY位置がカメラのデフォルト位置より低ければ、カメラのデフォルトY位置を移動位置とする
                setY = defaultY;
            } else {
                // それ以外は追従対象のY位置を移動位置とする
                setY = targetPosition.y;
            }

            // カメラの移動位置を設定する
            transform.position = new Vector3(transform.position.x, setY, transform.position.z);
        }
    }

    public void ResetCamera() {
        // カメラの位置を初期化する
        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
    }

}
