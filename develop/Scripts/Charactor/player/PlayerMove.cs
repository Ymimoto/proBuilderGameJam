/// <summary>
/// 3Dオブジェクトで2Dアクションを作ることを想定したモジュール(プレイヤー側)
/// TODO: あとでオブジェクト移動用モジュールとプレイヤーモジュールに分割すること
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove:MonoBehaviour {

    // プレイヤー動作の可/不可切り替え
    public bool moveable;

    // 水平方向入力方向
    public float horizontalInput = 0.0f;
    // 水平移動速度
    private float horizontalSpeed = 5.0f;

    // ジャンプ入力
    public bool jumpInput;
    // ジャンプの強さ
    private float jumpForce = 500.0f;
    // 重力調整値(この値でjumpForceを割った値を上から下へ加えることで、落下速度を調整する)
    // TODO: この値をいじるとアニメーションがずれる場合があるので、別途animatorで調整すること
    private float gravityRate = 150;
    // カーブ補正
    private float useCurvesHeight = 0.5f;
    // ジャンプ中コライダー補正
    public float correctionCollHeight;

    // 状態フラグは外部からの取得に対しては読み取りオンリーにする
    // 接地状態
    [HideInInspector] public bool isGround { get; private set; }
    // ジャンプ状態
    [HideInInspector] public bool isJump { get; private set; }
    // 落下状態
    [HideInInspector] public bool isFall { get; private set; }
    // 旋回中(使うかどうかは別として)
    [HideInInspector] public bool isRotate { get; private set; }

    // Z方向固定用変数
    private float positionZ;
    // 現在設定されている進行方向(setVelocity)か向きに応じたY軸回転角度設定値
    private float rotateYFromSetVelocity {
        get {
            if(setVelocity.x != 0.0f) {
                return setVelocity.x > 0.0f ? 90.0f : -90.0f;
            } else {
                return transform.rotation.y > 0.0f ? 90.0f : -90.0f;
            }
        }
    }
    // 1フレームごとに旋回させる角度
    public float rotateRate = 15.0f;

    // 水平方向設定速度
    public Vector3 setVelocity;

    // 接地判定用Rayのキャスト距離
    private float groundDistance = 0.35f;
    // 接地判定用Rayの開始位置Y軸補正
    private float startRayY = 0.3f;
    // 地面の法線ベクトル
    private Vector3 groundNormal;

    // コライダーのデフォルトの高さ
    private float originColHeight;
    // コライダーのデフォルトのセンター
    private Vector3 originColCenter;

    // 左端の座標(画面外判定用)
    private Vector3 leftEnd;
    // 右端の座標(画面外判定用)
    private Vector3 rightEnd;

    // rigidbody
    private Rigidbody playerRigidbody;
    // collider
    private CapsuleCollider playerCollider;
    // collider
    private Animator playerAnimator;

    // アニメーションの状態のIDを取得する
    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");

    // 現在のアニメーションステート状態を格納する
    private AnimatorStateInfo currentBaseState;

    // 入力関連のクラス
    private CharactorInput playerInput;

    /// <summary>
    /// 初期化
    /// </summary>
    void Awake() {
        // なぜアタッチしなくても呼べるのだろう
        playerInput = new CharactorInput();
        playerInput.Setup();
        playerInput.isInput = false;

        // 各種コンポーネントを取得
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerAnimator = GetComponent<Animator>();

        // Z位置を格納する
        positionZ = transform.position.z;

        // デフォルトでは接地状態、非ジャンプ状態、非落下状態
        isGround = true;
        isJump = false;
        isFall = false;

        // 移動不可状態にする
        moveable = false;
        playerInput.isInput = false;
    }

    /// <summary>
    /// コンポーネント有効化時処理(コンポーネントが動く状態になったとき)
    /// </summary>
    void OnEnable() {
        // 移動可能状態にする
        moveable = true;
        playerInput.isInput = true;
    }

    /// <summary>
    /// コンポーネント無効化時処理(コンポーネントが動かない状態になったとき)
    /// </summary>
    void OnDisable() {
        // 移動不可状態にする
        moveable = false;
        playerInput.isInput = false;
    }

    /// <summary>
    /// Update処理前初期化
    /// </summary>
    void Start() {
        // コライダーの高さとセンターを格納しておく
        originColHeight = playerCollider.height;
        originColCenter = playerCollider.center;

        // 画面外判定用に左下と右上の位置をビューポート→ワールド座標変換
        // 今回はxの位置さえわかればいいので、最初に現在の位置から作成するのみでいい
        // また、ある程度のマージン(コライダーの半径)ももたせておく。
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        leftEnd = camera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, camera.nearClipPlane));
        rightEnd = camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, camera.nearClipPlane));
        leftEnd.x += playerCollider.radius;
        rightEnd.x -= playerCollider.radius;
    }

    /// <summary>
    /// Update処理ループ
    /// </summary>
    void Update() {
        // 現在の接地・ジャンプ・落下ステータス更新
        UpdateState();

        // 入力情報を取る
        horizontalInput = playerInput.getHorizontalInput();
        jumpInput = playerInput.getJumpInput();

        // 速度を作成する
        setVelocity = new Vector3(horizontalSpeed * horizontalInput, playerRigidbody.velocity.y, positionZ);
    }

    /// <summary>
    /// FixUpdate処理ループ
    /// </summary>
    void FixedUpdate() {
        // アニメーションの現在のステート情報を取得する
        currentBaseState = playerAnimator.GetCurrentAnimatorStateInfo(0);

        // animatorに速度をセット
        playerAnimator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // 一定の角度を向くようにする
        // オブジェクトのY軸の回転角度を-90 ~ 90度の間で変換する
        float rotateY = Mathf.DeltaAngle(transform.eulerAngles.y, 180.0f);
        // 現在向くべき角度でなければ旋回する
        if(rotateY != rotateYFromSetVelocity) {
            if(rotateYFromSetVelocity == 90.0f) {
                transform.Rotate(new Vector3(0.0f, rotateRate, 0.0f));
            } else {
                transform.Rotate(new Vector3(0.0f, -rotateRate, 0.0f));
            }
        }

        playerRigidbody.velocity = setVelocity;
        if(jumpInput && !isJump && isGround && !playerAnimator.IsInTransition(0)) {
            // 上方向に加速度を加える
            playerRigidbody.AddForce(jumpForce * Vector3.up);

            // アニメーション処理。
            // ジャンプフラグをtrueにする
            playerAnimator.SetBool("Jump", true);

            // ジャンプ状態、非接地状態、非落下状態にする
            isJump = true;
            isGround = false;
            isFall = false;
        } else if(isJump || isFall) {
            // ジャンプ中か落下中ならば下方向に加速度を加える(挙動調整)
            playerRigidbody.AddForce(jumpForce / gravityRate * Vector3.down);

            // ジャンプ中であるなら地上との距離を取り、コライダーのサイズを調整する
            if(isJump) {
                Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                RaycastHit hitInfo = new RaycastHit();

                float jumpHeight = playerAnimator.GetFloat("JumpHeight");

                if(Physics.Raycast(ray, out hitInfo)) {
                    if(hitInfo.distance > useCurvesHeight) {
                        playerCollider.height = originColHeight - (jumpHeight * correctionCollHeight);
                        playerCollider.center = new Vector3(0, originColCenter.y + jumpHeight, 0);
                    } else {
                        playerCollider.height = originColHeight;
                        playerCollider.center = originColCenter;
                    }
                }
            }
        } else {
            // 上の条件以外 = 接地状態であればアニメーションのジャンプフラグをfalseにする
            playerAnimator.SetBool("Jump", false);
            if(playerCollider.height != originColHeight) {
                // もし戻っていなければ、コライダーサイズを戻す
                playerCollider.height = originColHeight;
                playerCollider.center = originColCenter;
            }
        }

        // 画面端であれば画面外に出ないように調整する
        Vector3 playerPosition = playerRigidbody.position;
        playerPosition.x = Mathf.Clamp(playerPosition.x, leftEnd.x, rightEnd.x);
        playerRigidbody.position = playerPosition;
    }

    /// <summary>
    /// 接地状態、ジャンプ状態、落下状態、真下の地面に対しての法線ベクトルを更新
    /// </summary>
    public void UpdateState() {

        // Rayを使って、接地チェック
        RaycastHit hitInfo;
        float radius = playerCollider.radius;
        Vector3 startPos = transform.position;
        startPos.y = transform.position.y + radius + startRayY;
        bool checkRay = Physics.SphereCast(startPos, radius, Vector3.down, out hitInfo, groundDistance);
        if(checkRay) {
            // 地面と接地状態
            isGround = true;
            isJump = false;
            isFall = false;

            //地面の法線ベクトル
            groundNormal = hitInfo.normal;
        } else {
            // 地面と非接地状態
            isGround = false;

            //地面の法線ベクトルは上方向で固定させる
            groundNormal = Vector3.up;

            // ジャンプの状態になるには入力等の外的要因が主のため、ここでのジャンプ中判定の必要はない
            // 落下中判定は、ジャンプ中ではないかつ接地状態ではない場合は落下中判定。
            // TODO:ジャンプの下降時は落下判定にするのかどうか、必要に応じて考える。
            if(isJump) {
                isFall = false;
            } else {
                isFall = true;
            }
        }
    }

    /// <summary>
    /// キー入力を受け付けるかどうかを設定する
    /// </summary>
    /// <param name="setStatus">trueにするとキー入力を受け付ける</param>
    public void setKeyInput(bool setStatus) {
        playerInput.isInput = setStatus;
    }

    /// <summary>
    /// 指定したパラメータでプレイヤーを初期化する
    /// </summary>
    public void resetPlayer(Vector3 position) {
        // 位置情報および速度関連を初期化する
        playerRigidbody.velocity = Vector3.zero;
        transform.position = position;
        playerRigidbody.AddForce(Vector3.zero);

        //float rotateY = Mathf.DeltaAngle(transform.eulerAngles.y, 180.0f);
        //transform.Rotate(new Vector3(0.0f, 90.0f - rotateY, 0.0f));

        transform.LookAt(Vector3.right);

        // アニメーションの初期化は、パラメータに初期値を設定することで実装する
        playerAnimator.SetFloat("Speed", 0.0f);
        playerAnimator.SetFloat("Direction", 0.0f);
        playerAnimator.SetFloat("JumpHeight", 0.0f);
        playerAnimator.SetFloat("GravityControl", 0.0f);
        playerAnimator.SetBool("Jump", false);
        playerAnimator.SetBool("Rest", false);

        // コライダーサイズを戻す
        playerCollider.height = originColHeight;
        playerCollider.center = originColCenter;

    }

    /*
void OnGUI() {
GUI.Box(new Rect(Screen.width - 260, 10, 250, 150), "Interaction");
GUI.Label(new Rect(Screen.width - 245, 30, 250, 30), "transform.position.y:" + transform.position.y);
GUI.Label(new Rect(Screen.width - 245, 50, 250, 30), "rotateYFromSetVelocity:" + rotateYFromSetVelocity);
GUI.Label(new Rect(Screen.width - 245, 70, 250, 30), "isJump:" + isJump);
GUI.Label(new Rect(Screen.width - 245, 90, 250, 30), "isGround:" + isGround);
GUI.Label(new Rect(Screen.width - 245, 110, 250, 30), "isFall:" + isFall);
}

void OnDrawGizmos() {

RaycastHit hitInfo;
float radius = GetComponent<CapsuleCollider>().radius;
Vector3 startPos = transform.position;
startPos.y = transform.position.y + radius + startRayY;
bool checkRay = Physics.SphereCast(startPos, radius, Vector3.down, out hitInfo, groundDistance);

if(checkRay) {
Gizmos.DrawRay(startPos, Vector3.down * hitInfo.distance);
Gizmos.DrawWireSphere(startPos + Vector3.down * (hitInfo.distance), radius);
} else {
Gizmos.DrawRay(transform.position, Vector3.down * 10);
}
}
*/

}

