using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 範囲情報
/// ex) アイテム配置可能範囲
/// TODO: あとで別途ファイルに分離して、プレイヤーの移動可能範囲としても使用できるようにする
/// </summary>

[System.Serializable]
public struct rectArea {
    public Vector3 leftBottom;
    public Vector3 rightTop;
    public rectArea(Vector3 lb, Vector3 rt) {
        leftBottom = lb;
        rightTop = rt;
    }
}

/// <summary>
/// アイテム管理コントローラ
/// </summary>
public class ItemController:MonoBehaviour {

    // アイテム配置範囲
    private rectArea itemArea;
    // アイテム配置不可範囲
    private rectArea[] itemExclusionAreas;

    // 最大アイテム配置数
    public int setItemCount;

    // 設定したいアイテムのプレハブ
    public GameObject itemObject;

    // 配置アイテムリスト
    private List<GameObject> itemObjects = new List<GameObject>();

    // 乱数インスタンス
    private System.Random ran;

    // UNIXエポック時刻
    private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// アイテムの初期化・設定
    /// </summary>
    public void setup() {
        // TODO:別ファイルから読み込んで設定したい

        // アイテム配置範囲を設定
        itemArea = new rectArea(new Vector3(-9.0f, 0.5f, 0.0f), new Vector3(9.0f, 17.0f, 0.0f));

        // アイテム配置不可範囲を設定
        itemExclusionAreas = new rectArea[] {
            new rectArea(new Vector3(-9.0f, 2.0f, 0), new Vector3(-5.0f, 4.0f, 0)),
            new rectArea(new Vector3(-3.5f, 3.7f, 0), new Vector3(3.5f, 5.6f, 0)),
            new rectArea(new Vector3(3.5f, 6.0f, 0), new Vector3(9.0f, 9.0f, 0)),
            new rectArea(new Vector3(-9.0f, 8.5f, 0), new Vector3(0.6f, 11.0f, 0)),
            new rectArea(new Vector3(0.8f, 13.0f, 0), new Vector3(7.5f, 17.0f, 0)),
            new rectArea(new Vector3(-9.0f, 13.5f, 0), new Vector3(0.0f, 17.0f, 0)),
        };

        // 乱数のシード値を設定
        ran = new System.Random(getUnixTimeNow());
    }

    /// <summary>
    /// アイテムの初期配置を行う
    /// </summary>
    public void setupItems() {
        if(itemObjects.Count > 1) {
            // すでにリストに値があるなら配置などの初期化を行う
            resetItems();
        } else {
            // 最大数までアイテムの配置を行う
            for(int i = 0; i < setItemCount; i++) {
                setItemField();
            }
        }
	}

    /// <summary>
    /// アイテムの補てんを行う
    /// </summary>
    public void updateItems () {
        // 配置アイテムリスト中に非アクティブのアイテムが存在するなら、アクティブにし位置の再配置を行う
        for(int i = 0; i < setItemCount; i++) {
            if(itemObjects[i].activeSelf == false) {
                itemObjects[i].transform.position = setPosition();
                itemObjects[i].SetActive(true);
            }
        }
	}

    /// <summary>
    /// アイテムの初期化(再配置/コライダー有効化)を行う
    /// </summary>
    public void resetItems() {
        for(int i = 0; i < setItemCount; i++) {
            itemObjects[i].transform.position = setPosition();
            itemObjects[i].GetComponent<BoxCollider>().enabled = true;
            itemObjects[i].SetActive(true);
        }
    }

    /// <summary>
    /// アイテムの獲得をできないようにする
    /// </summary>
    public void stopGetItems() {
        // アイテムに設定されているコライダーをすべて無効化する
        // TODO: Linqでかけないかなこれ
        for(int i = 0; i < itemObjects.Count; i++) {
            itemObjects[i].GetComponent<BoxCollider>().enabled = false;
        }
    }

    /// <summary>
    /// アイテムを無効化する
    /// </summary>
    public void deleteItem(GameObject itemObject) {
        // 削除だと、リスト管理が大変になるのとリスト化した利点がなくなるため、非アクティブにする
        itemObject.SetActive(false);
    }

    void setItemField() {
        // ランダムで配置可能な位置を取得する
        // TODO: 地形は判断できても、アイテム同士は判断できていないため、対応を行う
        Vector3 itemPosition = setPosition();

        // 取得した位置にアイテムをItemsオブジェクトの子として配置する
        // TODO: 今後のことを考えてオブジェクトのキャッシュを考える
        GameObject item = (GameObject)Instantiate(itemObject, itemPosition, Quaternion.identity);
        item.transform.parent = transform;
        itemObjects.Add(item);
    }

    /// <summary>
    /// アイテムを配置する位置をランダムで作成する
    /// </summary>
    Vector3 setPosition() {
        // 位置X,Y
        float posX, posY;

        //  C#固有のランダム生成はfloat対応していないため、位置情報を10倍しておく
        int leftBottomX = (int)itemArea.leftBottom.x * 10;
        int leftBottomY = (int)itemArea.leftBottom.y * 10;
        int rightTopX = (int)itemArea.rightTop.x * 10;
        int rightTopY = (int)itemArea.rightTop.y * 10;

        // X座標を作成する (位置情報を10倍したものを使用して乱数を取った後に10で割る)
        posX = (float)ran.Next(leftBottomX, rightTopX) / 10;
        do {
            // 作成座標が作成不可の範囲外になるまでY座標を作成する
            posY = (float)ran.Next(leftBottomY, rightTopY) / 10;
        } while(! checkDeployableAreas(posX, posY));
        // Debug.Log("確定位置:" + posX + ", " + posY);
        return new Vector3(posX, posY, 0);
    }

    bool checkDeployableAreas(float posX, float posY) {
        // アイテム配置不可範囲の範囲に作成した位置が含まれてるかチェックする
        //Debug.Log("posX, posY:" + posX + ", " + posY);
        for(int i = 0; i < itemExclusionAreas.Length; i++) {
            //Debug.Log(itemExclusionAreas[i].leftBottom+":"+ itemExclusionAreas[i].rightTop);
            if(posX >= itemExclusionAreas[i].leftBottom.x && posX <= itemExclusionAreas[i].rightTop.x
                && posY >= itemExclusionAreas[i].leftBottom.y && posY <= itemExclusionAreas[i].rightTop.y) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 現在のUnix時間を返す(乱数作成シード用)
    /// TODO: GameManagerのコピペなんで後でUtilで新規作成する
    /// </summary>
    /// <returns>Unix時間</returns>
    private int getUnixTimeNow() {
        return (int)(DateTime.UtcNow.ToUniversalTime() - UNIX_EPOCH).TotalSeconds;
    }
}
