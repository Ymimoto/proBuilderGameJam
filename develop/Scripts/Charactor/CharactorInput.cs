/// <summary>
/// キャラクターの入力に関する共通クラス
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorInput
{
    // 入力自体が不可フラグ
    public bool isInput;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Setup(){
        // 入力受付不可状態にする
        isInput = false;
    }

    /// <summary>
    /// 水平方向への入力を取得する
    /// </summary>
    /// <returns>現在の水平方向入力に対する仮想軸の値(-1 ~ 1)</returns>
    public float getHorizontalInput(){
        // 入力が不可な状態であれば、そのフレームでの入力状態を初期化する
        if(! isInput) {
            Input.ResetInputAxes();
        }

        // 現在の入力を格納する
        // -1 ~ 1の間でとりたいため、Input.GetAxisを使用する
        //return Input.GetAxisRaw("Horizontal");
        return Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// ジャンプの入力を取得する
    /// </summary>
    /// <returns>現在のジャンプホタンのbool値</returns>
    public bool getJumpInput()
    {
        // 入力が不可な状態であれば、入力状態を初期化する
        if(!isInput) {
            Input.ResetInputAxes();
        }

        // 現在の入力を格納する
        // 押した瞬間のみを取得したいため、GetButtonDownを使用する
        return Input.GetButtonDown("Jump");
    }

    // デバッグ表示用
    public Dictionary<string, string> setGUIData() {
        Dictionary<string, string> param = new Dictionary<string, string>() {
            {"horizontalInput", Input.GetAxis("Horizontal").ToString()},
            {"jumpInput", Input.GetButtonDown("Jump").ToString()},
        };
        return param;
    }
}
