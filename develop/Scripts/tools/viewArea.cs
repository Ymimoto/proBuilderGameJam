using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// rectAreaで左下・右上を設定する際の確認用
/// 何かしらのGameObjectにアタッチして使おう
/// </summary>
public class viewArea:MonoBehaviour {

    [SerializeField]
    public rectArea[] areas;

    void OnDrawGizmos() {
        Vector3 lb, rt, lt, rb;

        Gizmos.color = Color.red;

        for(int i = 0; i < areas.Length; i++) {
            lb = new Vector3(areas[i].leftBottom.x, areas[i].leftBottom.y, 0.0f);
            rt = new Vector3(areas[i].rightTop.x, areas[i].rightTop.y, 0.0f);
            lt = new Vector3(areas[i].leftBottom.x, areas[i].rightTop.y, 0.0f);
            rb = new Vector3(areas[i].rightTop.x, areas[i].leftBottom.y, 0.0f);

            Gizmos.DrawLine(lb, lt); // 左下から左上
            Gizmos.DrawLine(lt, rt); // 左上から右上
            Gizmos.DrawLine(rt, rb); // 右上から右下
            Gizmos.DrawLine(rb, lb); // 右下から左下
        }
    }
}
