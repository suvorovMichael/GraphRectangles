using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Класс Node представляет узел графа.
/// Содержит все ребра, соединяющие этот узел с другими.
/// </summary>
public class Node : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler {

    Transform cachedTransform;

    float lastClickTime = 0;
    bool selected = false;
    float selectEffectTime = 0;
    Image image;
    Color mainColor;
    bool pointerDown = false;
    System.Func<Vector2, Node, bool> validPosition;

    // Все ребра, связывающие данный узел
    HashSet<Edge> edges = new HashSet<Edge>(); 

    public UnityEvent onDestroy = new UnityEvent();
    public UnityEvent onSelect = new UnityEvent();
    public UnityEvent onDeselect = new UnityEvent();

    /// <summary>
    /// Инициализирует узел.
    /// </summary>
    /// <param name="position">Позиция узла</param>
    /// <param name="rectangleValidPosition">Функция проверки того, может ли узел находиться в данной позиции</param>
    public void Initialize(Vector2 position, System.Func<Vector2, Node, bool> rectangleValidPosition) {
        name = "Node";
        cachedTransform = transform;
        cachedTransform.localPosition = position;
        cachedTransform.localScale = Vector2.one;

        image = gameObject.GetComponent<Image>();
        mainColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        image.color = mainColor;

        validPosition = rectangleValidPosition;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 position = GameManager.ScreenToFieldLocal(eventData.position);
        if (!validPosition(position, this))
            return;

        cachedTransform.localPosition = position;
        foreach (Edge edge in edges)
            edge.UpdateTransform();

        pointerDown = false;
        if (selected)
            Deselect();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (!pointerDown)
            return;

        if (Time.time - lastClickTime > .5f) {
            if (selected)
                Deselect();
            else
                Select();
        }
        else
            Delete();
    }

    public void OnPointerDown(PointerEventData eventData) {
        pointerDown = true;
    }

    /// <summary>
    /// Выделение узла.
    /// </summary>
    void Select() {
        lastClickTime = Time.time;
        selected = true;
        onSelect.Invoke();
    }

    /// <summary>
    /// Отменяет выделение узла.
    /// </summary>
    public void Deselect() {
        selected = false;
        image.color = mainColor;
        selectEffectTime = 0;
        onDeselect.Invoke();
    }

    void Delete() {
        List<Edge> edgeList = new List<Edge>();
        foreach (Edge edge in edges) {
            edgeList.Add(edge);
            if (edge.NodeFirst == this)
                edge.NodeSecond.RemoveEdge(edge);
            else
                edge.NodeFirst.RemoveEdge(edge);
        }

        foreach (Edge edge in edgeList)
            edge.Delete();

        onDestroy.Invoke();
        Destroy(gameObject);
    }

    public void AddEdge(Edge edge) {
        edges.Add(edge);
    }

    public void RemoveEdge(Edge edge) {
        if (edges.Contains(edge))
            edges.Remove(edge);
    }

    public bool HasNeighbour(Node other) {
        foreach (Edge edge in edges)
            if (edge.NodeFirst == other || edge.NodeSecond == other)
                return true;
        return false;
    }

    void Update() {
        if (selected) {
            selectEffectTime += Time.deltaTime;
            image.color = Color.Lerp(mainColor, new Color(1 - mainColor.r, 1 - mainColor.g, 1 - mainColor.b, 1), Mathf.PingPong(selectEffectTime, .3f));
        }
    }
}
