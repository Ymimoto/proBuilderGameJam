/// <summary>
/// ゲーム全体の進行を操作するマネージャ
/// 主に時間管理とアイテム数集計まわり
/// 各オブジェクトの有効化等は別途コントローラでやりましょう
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager:MonoBehaviour {

    // 開始画面の表示時間
    private float startTime;
    // 開始画面の最大表示時間
    public float startMaxTime;

    // ゲーム開始カウントダウン用
    // カウントダウン (秒)
    private int countDownTimeSeconds;
    // countDownTimeSecondsを計算するためにTime.deltaTimeを減算していくパラメータ
    public float countDownTime;
    // カウントダウンの開始時間(4秒に設定すれば、3～0で表示される)
    public float countDownMaxTime;

    // 現在のゲームプレイ時間 (秒)
    private int gameTimeSeconds;
    // gameTimeSecondsを計算するためにTime.deltaTimeを加算していくパラメータ
    private float gameTime;
    // ゲームプレイ可能最大時間 (秒)
    public int gameMaxTime;
    // UIマスク開始時間(秒)
    public int UIMaskTime;

    // 結果画面の表示時間
    public float resultTime;
    // 結果画面の最大表示時間
    //public float resultMaxTime;
    // リスタートする時間
    public float restartTime;

    // 取得目標アイテム数
    public int targetItemCount;
    // 現在取得アイテム数
    private int getItemCount;
    // 目標超過時減算アイテム数
    public int reduceItemRate;
    // 取得目標アイテム数下限
    public int targetItemCountMin;
    // 取得目標アイテム数上限
    public int targetItemCountMax;

    //UNIXエポック時刻
    private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // カメラコントローラへのアクセス
    public CameraController cameraController;

    // プレイヤーコントローラのアクセス
    public PlayerController playerController;

    // アイテムコントローラへのアクセス
    public ItemController itemController;

    // UIコントローラへのアクセス
    public UIController uiController;

    // 現在の表示ステート
    private ModeState nowState;
    /// <summary>
    /// 画面遷移ステート管理用
    /// タイトル / 目標アイテム数表示 / カウントダウン / ゲームプレイ / 結果表示
    /// </summary>
    private enum ModeState {
        TITLE, START, COUNTDOWN, PLAYING, RESULT
    }

    // Use this for initialization
    void Start() {

        // 各種コントローラへ初期値を設定していく
        setPlayerController(); // プレイヤーコントローラ
        setCameraController(); // カメラコントローラ
        setItemController(); // アイテムコントローラ
        setUiController(); // UIコントローラ

        // 各種値の初期化
        setup();

        //現在のステートをゲーム開始前に設定
        nowState = ModeState.TITLE;

        // UI表示をタイトルのものにする
        uiController.setUiTitleActive();
    }

    /// <summary>
    /// 値の初期化等を行う
    /// </summary>
    void setup() {
        // 取得目標アイテム数の決定
        setTargetItemCount();

        // 時間系パラメータやアイテム数の初期化
        startTime = 0.0f;
        countDownTimeSeconds = 0;
        countDownTime = 0.0f;
        gameTime = 0.0f;
        resultTime = 0;
        gameTimeSeconds = 0;
        getItemCount = 0;
    }

    /// <summary>
    /// プレイヤーコントローラの設定
    /// </summary>
    void setPlayerController() {
        // プレイヤーコントローラの初期設定メソッドを呼び出す
        playerController.setup();
        // 移動のコンポーネントを無効化する
        playerController.DisableControl();
        playerController.isNotMoveable();
    }

    /// <summary>
    /// カメラコントローラの設定
    /// </summary>
    void setCameraController() {
        // カメラコントローラの初期設定メソッドを呼び出す(とはいえ、今のところはないので直接指定する)
        // cameraController.setup();

        // カメラの移動を無効化させる
        cameraController.moveable = false;
    }

    /// <summary>
    /// アイテムコントローラの設定
    /// </summary>
    void setItemController() {
        // アイテムコントローラの初期設定メソッドを呼び出す
        itemController.setup();
    }

    /// <summary>
    /// UIコントローラの設定
    /// </summary>
    void setUiController() {
        uiController.setup();
    }

    /// <summary>
    /// 目標アイテム数の設定を行う
    /// </summary>
    void setTargetItemCount() {
        // 現在のunix時間をシードとして特定範囲の乱数作成
        System.Random ran = new System.Random(getUnixTimeNow());
        targetItemCount = ran.Next(targetItemCountMin, targetItemCountMax + 1);
    }

    /// <summary>
    /// 現在のUnix時間を返す(乱数作成シード用)
    /// </summary>
    /// <returns>Unix時間</returns>
    private int getUnixTimeNow() {
        return (int)(DateTime.UtcNow.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
    }

    // Update is called once per frame
    void Update() {
        // TODO: 今回は単純だからswitchによる管理のみでいいけど、いつかきちんとステートマシンで実装したいです
        // 状態遷移チェック(条件が一致すれば画面遷移ステートを変更し、新しい画面に対応した初期化や初期処理を行う)
        switch(nowState) {
            case ModeState.TITLE:
                if(Input.GetKeyDown(KeyCode.Space)) {

                    //スペースキーでスタート
                    // STARTステートに変更
                    nowState = ModeState.START;

                    // プレイヤーコントローラにより移動のコンポーネントを有効化し、
                    // プレイヤーの移動を無効化にする
                    //playerController.EnableControl();
                    //playerController.isNotMoveable();

                    // カメラコントローラによりカメラの移動を無効化する
                    cameraController.moveable = false;

                    // アイテムコントローラから、アイテムの初期配置を行う
                    itemController.setupItems();

                    // UI表示をスタートステート用UIに切り替える
                    uiController.setUiStartActive();

                    // 取得目標アイテム数をUIに設定
                    uiController.updateUiStart(targetItemCount);

                    //Debug.Log("STARTステートに変更");
                }
                break;
            case ModeState.START:
                if((int)startTime >= startMaxTime) {
                    // COUNTDOWNステートに変更
                    nowState = ModeState.COUNTDOWN;

                    // カウントダウンの開始値を開始時間-1して設定
                    countDownTime = (float)countDownMaxTime - 1;

                    // UI表示をカウントダウンステート用UIに切り替える
                    uiController.setUiCountdownActive();

                    //Debug.Log("COUNTDOWNステートに変更");
                }
                break;
            case ModeState.COUNTDOWN:
                if(countDownTime < -0.0f) {
                    // PLAYINGステートに変更
                    nowState = ModeState.PLAYING;

                    // プレイヤーコントローラにより移動のコンポーネントを有効化し、
                    // プレイヤーの移動を有効化にする
                    playerController.EnableControl();
                    playerController.isMoveable();

                    // カメラコントローラによりカメラの移動を有効化する
                    cameraController.moveable = true;

                    // UI表示をゲームステート用UIに切り替える
                    uiController.setUiScoreActive();

                    //Debug.Log("PLAYINGステートに変更");
                }
                break;
            case ModeState.PLAYING:
                if(gameTime >= gameMaxTime) {
                    // RESULTステートに変更
                    nowState = ModeState.RESULT;

                    // プレイヤーコントローラによりプレイヤーの移動等を無効化する
                    playerController.DisableControl();
                    //playerController.isNotMoveable();

                    // アイテムを無効化する
                    itemController.stopGetItems();

                    // カメラコントローラによりカメラの移動を無効化する
                    cameraController.moveable = false;

                    // UI表示を結果表示ステート用UIに切り替える
                    uiController.setUiResultActive();

                    // 結果計算処理を行う(オミット)

                    // 集計結果をUIに設定
                    uiController.updateUiResult(getItemCount, targetItemCount);

                    //Debug.Log("RESULTステートに変更");
                }
                break;
            case ModeState.RESULT:
                if(restartTime < resultTime) {

                    // 時間系パラメータやアイテム数の初期化
                    setup();

                    //playerController.EnableControl();
                    playerController.isNotMoveable();

                    // カメラ位置およびプレイヤー位置の初期化
                    playerController.ResetPlayer();
                    cameraController.ResetCamera();

                    // TITLEステートに変更
                    nowState = ModeState.TITLE;

                    // UI表示をタイトルのものにする
                    uiController.setUiTitleActive();
                }

                break;
        }

        // 画面遷移ステートごとの処理分岐
        switch(nowState) {
            case ModeState.START:
                // 表示時間を加算
                setStartTime();

                //Debug.Log("STARTステートです");
                break;
            case ModeState.COUNTDOWN:
                // 現在のカウントダウンの数値をUIに設定
                // TODO: 前の数値を格納しておき、数値が変わったタイミングでUI更新を行う
                uiController.updateUiCountdown(countDownTimeSeconds);

                // カウントダウンを減算
                setCountDownTime();

                //Debug.Log("COUNTDOWNステートです");
                break;
            case ModeState.PLAYING:
                // 現在の各種値をUIに設定
                // TODO: 前の数値を格納しておき、数値が変わったタイミングでUI更新を行う

                if(UIMaskTime <= gameTimeSeconds) {
                    // TODO: 実際はUI側でモザイク処理を行う
                    uiController.updateUiScore<string>(gameTimeSeconds, "***", "***");

                } else {
                    uiController.updateUiScore<int>(gameTimeSeconds, getItemCount, targetItemCount);
                }

                // プレイ時間を加算
                setGameTime();

                // アイテム補てんを行う
                itemController.updateItems();

                //Debug.Log("PLAYINGステートです");
                //Debug.Log("gameTimeSeconds = " + gameTimeSeconds);
                break;
            case ModeState.RESULT:
                // 結果画面を加算
                setResultTime();

                //Debug.Log("RESULTステートです");
                break;
        }

    }

    /// <summary>
    /// アイテム獲得処理
    /// </summary>
    public void setItemCount(GameObject itemObject) {
        // アイテム加算処理を行う
        getItemCount++;

        // 引数のオブジェクトを非アクティブにする
        itemController.deleteItem(itemObject);
        // Debug.Log("getItemCount:" + getItemCount);
    }

    /// <summary>
    /// 開始画面の表示時間を加算する
    /// </summary>
    void setStartTime() {
        // Time.deltaTimeを加算
        startTime += Time.deltaTime;
    }

    /// <summary>
    /// プレイ画面の表示時間を加算する
    /// </summary>
    void setGameTime() {
        // Time.deltaTimeを加算
        gameTime += Time.deltaTime;

        // floatであるgameTimeを秒に変換
        gameTimeSeconds = (int)gameTime;
    }

    /// <summary>
    /// カウントダウン画面の表示時間を減算する
    /// </summary>
    void setCountDownTime() {
        // Time.deltaTimeで減算
        countDownTime -= Time.deltaTime;

        // floatであるcountDownTimeを秒に変換
        countDownTimeSeconds = (int)countDownTime;
    }

    /// <summary>
    /// 結果画面の表示時間を加算する
    /// </summary>
    void setResultTime() {
        // Time.deltaTimeを加算
        resultTime += Time.deltaTime;
    }
}
