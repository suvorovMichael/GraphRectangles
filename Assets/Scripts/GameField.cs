using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Класс Игрового поля. Отображает узлы и ребра.
/// </summary>
public class GameField : MonoBehaviour, IPointerClickHandler {

    [SerializeField] Transform nodesRoot;
    [SerializeField] Transform edgesRoot;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject edgePrefab;

    float nodeWidth;
    float nodeHeight;

    HashSet<Node> nodes = new HashSet<Node>();

    Node selectedNode;

    Rect validRect;

    void Start() {
        nodeWidth = nodePrefab.GetComponent<RectTransform>().sizeDelta.x;
        nodeHeight = nodePrefab.GetComponent<RectTransform>().sizeDelta.y;

        DefineValidRect();
    }

    /// <summary>
    /// Определение области, в которой можно создать прямоугольник.
    /// </summary>
    void DefineValidRect() {
        RectTransform rectTransform = GetComponent<RectTransform>();
        validRect = new Rect(-rectTransform.rect.width / 2 + nodeWidth / 2, -rectTransform.rect.height / 2 + nodeHeight / 2, rectTransform.rect.width - nodeWidth, rectTransform.rect.height - nodeHeight);
    }

    public void OnPointerClick(PointerEventData eventData) {
        Vector2 position = GameManager.ScreenToFieldLocal(eventData.position);
        if (IsValidPosition(position))
            CreateNode(position);
    }

    /// <summary>
    /// Проверяет, может ли узел находиться в заданной позиции.
    /// Узел не должен пересекаться с другими узлами и не выходить за границы экрана.
    /// </summary>
    /// <param name="position">Заданная позиция</param>
    /// <param name="rectangle">Узел</param>
    bool IsValidPosition(Vector2 position, Node rectangle = null) {
        if (!validRect.Contains(position))
            return false;

        foreach (Node r in nodes)
            if (rectangle != r && Mathf.Abs(position.x - r.transform.localPosition.x) < nodeWidth && Mathf.Abs(position.y - r.transform.localPosition.y) < nodeHeight)
                return false;

        return true;
    }

    void CreateNode(Vector2 position) {
        Node newNode = Instantiate(nodePrefab, nodesRoot).GetComponent<Node>();
        newNode.Initialize(position, IsValidPosition);
        newNode.onDestroy.AddListener(() => OnNodeDelete(newNode));
        newNode.onSelect.AddListener(() => OnSelectNode(newNode));
        newNode.onDeselect.AddListener(OnDeselectNode);
        nodes.Add(newNode);

        if (selectedNode != null)
            selectedNode.Deselect();
    }

    void OnNodeDelete(Node node) {
        nodes.Remove(node);
    }

    void OnSelectNode(Node newSelectedNode) {
        if (selectedNode != null) {
            CreateEdge(selectedNode, newSelectedNode);
            selectedNode.Deselect();
            newSelectedNode.Deselect();
        }
        else
            selectedNode = newSelectedNode;
    }

    void OnDeselectNode() {
        selectedNode = null;
    }

    void CreateEdge(Node first, Node second) {
        if (!first.HasNeighbour(second) && !second.HasNeighbour(first)) {
            Edge newEdge = Instantiate(edgePrefab, edgesRoot).GetComponent<Edge>();
            newEdge.Initialize(first, second);
        }
    }
}
