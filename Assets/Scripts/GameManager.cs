using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [SerializeField] Camera mainCamera;
    public static Camera MainCamera { get { return instance.mainCamera; } }

    [SerializeField] RectTransform fieldRectTransform;

    void Awake() {
        instance = this;
    }

    public static Vector2 ScreenToFieldLocal(Vector2 screenPosition) {
        Vector2 rectPosition = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(instance.fieldRectTransform, screenPosition, MainCamera, out rectPosition);
        return rectPosition;
    }

}
